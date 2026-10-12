namespace _3D_Rotation_Presenter.Rotation;

/// <summary>
/// 右手系下的三个基元旋转, 角度单位为度. 用标量公式直接算, 不构造矩阵.
/// </summary>
/// <remarks>
/// 命名按"点绕哪根轴旋转"来定, 因此 <see cref="RotateAboutX{T}"/> 只改动 Y / Z 分量.
/// 原 RotationEmulator.cs 中的 X_Rotate / Y_Rotate 改名前的行为恰好相反
/// (X_Rotate 改的是 X / Z 分量, 即绕 Y 轴), 这里连同名字一起修正.
/// </remarks>
internal static class AxisRotation
{
    /// <summary>绕 X 轴旋转 degrees 度 (只改 Y / Z).</summary>
    internal static T RotateAboutX<T>(this T p, double degrees) where T : Doubles_3, new()
    {
        var (sin, cos) = AngleMath.SinCos(degrees);
        return new T
        {
            X = p.X,
            Y = cos * p.Y - sin * p.Z,
            Z = sin * p.Y + cos * p.Z
        };
    }

    /// <summary>绕 Y 轴旋转 degrees 度 (只改 X / Z).</summary>
    internal static T RotateAboutY<T>(this T p, double degrees) where T : Doubles_3, new()
    {
        var (sin, cos) = AngleMath.SinCos(degrees);
        return new T
        {
            X = cos * p.X + sin * p.Z,
            Y = p.Y,
            Z = -sin * p.X + cos * p.Z
        };
    }

    /// <summary>绕 Z 轴旋转 degrees 度 (只改 X / Y).</summary>
    internal static T RotateAboutZ<T>(this T p, double degrees) where T : Doubles_3, new()
    {
        var (sin, cos) = AngleMath.SinCos(degrees);
        return new T
        {
            X = cos * p.X - sin * p.Y,
            Y = sin * p.X + cos * p.Y,
            Z = p.Z
        };
    }
}
