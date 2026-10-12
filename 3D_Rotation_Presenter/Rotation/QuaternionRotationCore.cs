namespace _3D_Rotation_Presenter.Rotation;

/// <summary>
/// 四元数实现: 把三个输入角先合成一个单位四元数, 之后每个点都用 q ⊗ v ⊗ q⁻¹ 变换.
/// </summary>
/// <remarks>
/// 与欧拉角实现的区别不在"结果"(两者数学上恒等), 而在"过程":
/// 姿态一旦合成为四元数就不再保留三个角, 因此不存在某个角在奇异点附近失去意义的表示法问题,
/// 四个分量也可以直接做球面插值 (本演示尚未用到). 代价是每次施加旋转要付一次叉乘级别的运算,
/// 换来的是"创建一次、施加 N 次"的结构优势 —— 这正是 <see cref="IRotation"/> 存在的理由.
/// </remarks>
public sealed class QuaternionRotationCore : IRotationCore
{
    public string DisplayName => "Quaternion (Hamilton product)";

    public IRotation Create(double yaw, double pitch, double roll)
        => new QuaternionRotation(Quaternion.FromEulerAngles(yaw, pitch, roll));

    public string Format(double yaw, double pitch, double roll)
    {
        var q = Quaternion.FromEulerAngles(yaw, pitch, roll);
        return $"q = ({q})\n|q| = {q.Norm():F6}";
    }

    private sealed class QuaternionRotation(Quaternion q) : IRotation
    {
        public T Apply<T>(T value) where T : Doubles_3, new() => q.Rotate(value);
    }
}
