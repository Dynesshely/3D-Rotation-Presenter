namespace _3D_Rotation_Presenter.Rotation;

/// <summary>
/// 一次已经确定的纯旋转 (无平移、无缩放), 可以反复施加到任意多个点或向量上.
/// </summary>
/// <remarks>
/// 之所以把它与 <see cref="IRotationCore"/> 分开: 把角度换算成旋转的代价与施加次数无关,
/// 一次创建、多次 <see cref="Apply{T}"/> 才能避免逐点重复换算 —— 四元数实现尤其需要这样.
/// </remarks>
public interface IRotation
{
    /// <summary>把物体坐标系下的点 (或向量) 映射到世界坐标系.</summary>
    T Apply<T>(T value) where T : Doubles_3, new();
}

/// <summary>
/// 三维旋转算法核心: 把三个输入角换算成一个 <see cref="IRotation"/>.
/// 每个实现代表一种旋转表示法 (欧拉角 / 四元数 / ...), 对同一组输入角必须给出同一个旋转,
/// 否则切换实现会导致画面跳变.
/// </summary>
public interface IRotationCore
{
    /// <summary>供 UI 显示的实现名称.</summary>
    string DisplayName { get; }

    /// <summary>用给定的 yaw / pitch / roll 创建旋转, 角度单位为度.</summary>
    IRotation Create(double yaw, double pitch, double roll);

    /// <summary>把该实现内部的表示形式格式化成一行文本, 让使用者看见"当前用的是哪种表示法".</summary>
    string Format(double yaw, double pitch, double roll);
}
