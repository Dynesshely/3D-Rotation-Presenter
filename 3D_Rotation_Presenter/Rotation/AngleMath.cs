using System;

namespace _3D_Rotation_Presenter.Rotation;

/// <summary>
/// 角度换算. 统一收口, 避免各处写出"数学等价但舍入不同"的表达式.
/// </summary>
internal static class AngleMath
{
    /// <summary>
    /// 度 -> 弧度.
    /// </summary>
    /// <remarks>
    /// 刻意写成"先算常量 Math.PI / 180, 再乘以角度", 与重构前 `alpha *= Math.PI / 180`
    /// 的浮点舍入逐位一致. 若写成 `degrees * Math.PI / 180`, 求值顺序变成先乘后除,
    /// 结果可能相差 1 ULP —— 在 cos(90°) = 6.1e-17 这种量级上, 1 ULP 就足以翻转符号.
    /// </remarks>
    internal const double DegreesToRadians = Math.PI / 180;

    internal static (double Sin, double Cos) SinCos(double degrees)
    {
        var radians = degrees * DegreesToRadians;
        return (Math.Sin(radians), Math.Cos(radians));
    }
}
