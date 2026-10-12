namespace _3D_Rotation_Presenter.Rotation;

/// <summary>
/// 欧拉角 (Tait-Bryan) 实现: 姿态就是三个角本身, 每次施加旋转时按固定顺序
/// 依次绕 Z (yaw) → X (pitch) → Y (roll) 做三次基元旋转,
/// 等价于矩阵 Ry(roll)·Rx(pitch)·Rz(yaw).
/// </summary>
/// <remarks>
/// 这是本项目最初的算法, 抽象到这里时保持数值行为不变, 只修正了基元旋转的命名
/// (见 <see cref="AxisRotation"/>). 逐轴顺序旋转在 pitch 或 roll 接近 ±90° 时会出现
/// 万向锁式的表示退化; 本演示是"给定角度算一次投影", 不做角度插值, 因此不会显现,
/// 但一旦要加拖拽惯性或角度补间, 就应当换用 <see cref="QuaternionRotationCore"/>.
/// </remarks>
public sealed class EulerAngleRotationCore : IRotationCore
{
    public string DisplayName => "Euler angles (sequential axes)";

    public IRotation Create(double yaw, double pitch, double roll)
        => new EulerAngleRotation(yaw, pitch, roll);

    public string Format(double yaw, double pitch, double roll)
        => $"yaw(Z) = {yaw,8:F2}°\npitch(X) = {pitch,8:F2}°\nroll(Y) = {roll,8:F2}°";

    private sealed class EulerAngleRotation(double yaw, double pitch, double roll) : IRotation
    {
        public T Apply<T>(T value) where T : Doubles_3, new()
            => value.RotateAboutZ(yaw).RotateAboutX(pitch).RotateAboutY(roll);
    }
}
