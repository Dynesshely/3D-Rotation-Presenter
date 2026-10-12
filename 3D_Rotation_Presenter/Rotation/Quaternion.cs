using System;

namespace _3D_Rotation_Presenter.Rotation;

/// <summary>
/// Hamilton 四元数 (w, x, y, z), 单位四元数用来表示旋转.
/// </summary>
/// <remarks>
/// 约定与 <see cref="AxisRotation"/> / 欧拉角实现一致, 即列向量左乘: p' = q ⊗ p ⊗ q⁻¹.
/// 由四元数乘法满足 R(q1 ⊗ q2) = R(q1)·R(q2), 所以 q = qy ⊗ qx ⊗ qz 正好对应
/// 欧拉角实现的 R = Ry(roll)·Rx(pitch)·Rz(yaw), 两种实现在数学上恒等.
///
/// 这里刻意没有使用 System.Numerics.Quaternion: 它是行向量约定 (v' = v·M) 的组织方式,
/// 与本项目"点左乘旋转"的写法相反, 直接混用会引入一次隐式转置; 自己实现 Hamilton 积
/// 也让"四元数到底怎么旋转一个点"这件事在代码里是可见的, 便于与欧拉角实现逐点对照.
/// </remarks>
public readonly struct Quaternion(double w, double x, double y, double z)
{
    public readonly double W = w;

    public readonly double X = x;

    public readonly double Y = y;

    public readonly double Z = z;

    /// <summary>绕单位轴 (ax, ay, az) 旋转 degrees 度.</summary>
    public static Quaternion FromAxisAngle(double degrees, double ax, double ay, double az)
    {
        var (sin, cos) = AngleMath.SinCos(degrees / 2);
        return new Quaternion(cos, ax * sin, ay * sin, az * sin);
    }

    /// <summary>
    /// 由 yaw(Z) / pitch(X) / roll(Y) 构造四元数.
    /// 与欧拉角实现同顺序: Ry(roll)·Rx(pitch)·Rz(yaw) ⇒ qy(roll) ⊗ qx(pitch) ⊗ qz(yaw).
    /// </summary>
    public static Quaternion FromEulerAngles(double yaw, double pitch, double roll)
        => FromAxisAngle(roll, 0, 1, 0)
         * FromAxisAngle(pitch, 1, 0, 0)
         * FromAxisAngle(yaw, 0, 0, 1);

    /// <summary>Hamilton 积. 先做右边的旋转, 再做左边的: R(a ⊗ b) = R(a)·R(b).</summary>
    public static Quaternion operator *(Quaternion a, Quaternion b) => new(
        a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z,
        a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
        a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X,
        a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W);

    /// <summary>共轭. 对单位四元数而言它就是逆.</summary>
    public Quaternion Conjugate() => new(W, -X, -Y, -Z);

    public double Norm() => Math.Sqrt(W * W + X * X + Y * Y + Z * Z);

    /// <summary>用 q ⊗ v ⊗ q⁻¹ 旋转一个点或向量 (要求 q 是单位四元数).</summary>
    public T Rotate<T>(T v) where T : Doubles_3, new()
    {
        /*
         *  v' = v + 2w(q_vec × v) + 2(q_vec × (q_vec × v))
         *  令 t = 2(q_vec × v), 上式即
         *  v' = v + w·t + (q_vec × t)
         *  全程只用叉乘与数乘, 不展开成 3x3 矩阵.
         */

        var tx = 2 * (Y * v.Z - Z * v.Y);
        var ty = 2 * (Z * v.X - X * v.Z);
        var tz = 2 * (X * v.Y - Y * v.X);

        return new T
        {
            X = v.X + W * tx + (Y * tz - Z * ty),
            Y = v.Y + W * ty + (Z * tx - X * tz),
            Z = v.Z + W * tz + (X * ty - Y * tx)
        };
    }

    public override string ToString() => $"{W:F4}, {X:F4}, {Y:F4}, {Z:F4}";
}
