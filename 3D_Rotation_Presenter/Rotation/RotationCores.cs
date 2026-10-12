using System.Collections.Generic;

namespace _3D_Rotation_Presenter.Rotation;

/// <summary>所有可选的旋转核心实现, 供 UI 列出.</summary>
public static class RotationCores
{
    public static IReadOnlyList<IRotationCore> All { get; } =
    [
        new EulerAngleRotationCore(),
        new QuaternionRotationCore()
    ];
}
