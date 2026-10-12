using System;

namespace _3D_Rotation_Presenter;

/// <summary>
/// 三维三分量容器. 既可表示"点"(位置), 也可表示"向量"(方向 / 法向),
/// 具体语义由派生类型区分: <see cref="Point"/> 与 <see cref="Vector3D"/>.
/// </summary>
public class Doubles_3
{
    internal double X { get; set; }

    internal double Y { get; set; }

    internal double Z { get; set; }
}

public class Point : Doubles_3 { }

public class Vector3D : Doubles_3
{
    /// <summary>物体所在平面 (z = 0) 的正向法向量, 即始终朝向摄像机的那一面.</summary>
    internal static Vector3D ScreenNormal => new() { X = 0, Y = 0, Z = 1 };
}

public static class Extensions
{
    /// <summary>直线与平面视为平行时的分母阈值.</summary>
    private const double ParallelEpsilon = 1e-5;

    public static double Half(this double num) => num / 2;

    public static double ToCenter(this double n, double relate, bool isY = false)
        =>
        isY
        ?
        (
            n > 0
            ?
            relate.Half() - n
            :
            relate.Half() + Math.Abs(n)
        )
        :
        (
            n > 0
            ?
            relate.Half() + n
            :
            relate.Half() - Math.Abs(n)
        );

    internal static double Dot(this Doubles_3 a, Doubles_3 b)
        => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    public static Vector3D GetDirection(this Point p1, Point p2) => new()
    {
        X = p2.X - p1.X,
        Y = p2.Y - p1.Y,
        Z = p2.Z - p1.Z
    };

    /// <summary>
    /// 求直线与平面交点 (本项目的透视投影就是它: 从被摄点朝摄像机连线, 与 z = 0 平面求交)
    /// </summary>
    /// <param name="p">已知直线上一点</param>
    /// <param name="camera">已知直线上另一点 (摄像机视角)</param>
    /// <param name="n">平面上一点, 默认为 (0, 0, 0)</param>
    /// <param name="plane">平面法向量, 默认为 (0, 0, 1)</param>
    /// <returns>若存在交点则返回交点, 否则返回 null</returns>
    public static Point? GetCrossPoint(this Point p, Point camera, Point? n, Vector3D? plane)
    {
        /*
         *  如果直线不与平面平行, 将存在交点
         *  已知直线 L 过点 p (p1, p2, p3), 且方向向量为 VL (v1, v2, v3)
         *  平面 P 过点 n (n1, n2, n3), 且法线方向向量为 VP (vp1, vp2, vp3)
         *  求得直线与平面的交点 O 的坐标 (x, y, z): 
         *  将直线方程写成参数方程形式, 即有: 
         *  
         *      x = p1 + v1 * t
         *      y = p2 + v2 * t                                                 (1) 式
         *      z = p3 + v3 * t
         *  
         *  将平面方程写成点法式方程形式, 即有: 
         *  
         *      vp1 * (x - n1) + vp2 * (y - n2) + vp3 * (z - n3) = 0            (2) 式
         *  
         *  则直线与平面的交点一定满足 (1) 式 和 (2) 式, 联立两式, 求得: 
         *  
         *          (n1 - p1) * vp1 + (n2 - p2) * vp2 + (n3 - p3) * vp3
         *      t = ---------------------------------------------------         (3) 式
         *                    vp1 * v1 + vp2 * v2 + vp3 * v3
         *  
         *  如果 (3) 式中分母 (vp1 * v1 + vp2 * v2 + vp3 * v3) 为 0, 则表示直线与平面平行, 即直线与平面没有交点.
         *  求解出 t 后, 然后将 t 代入式 (1) 即可求得交点 O 的坐标 (x, y, z)
         */

        var VL = p.GetDirection(camera);
        var VP = plane ?? new Vector3D() //  一个垂直于绘图平面的法向量, P 表示绘图平面, 过点 (0, 0, 0)
        {
            X = 0,
            Y = 0,
            Z = 1
        };

        double vp1 = VP.X, vp2 = VP.Y, vp3 = VP.Z;
        double v1 = VL.X, v2 = VL.Y, v3 = VL.Z;

        var denominator = VP.Dot(VL);

        //  分母为 0 表示直线与平面平行, 无交点;
        //  分母为负表示可以求得交点, 但交点在摄像机的另一侧 (被摄点已经越过了摄像机所在平面), 同样无法成像.
        //  两种情况都丢弃该点, 由调用方决定线段的取舍.
        if (denominator <= ParallelEpsilon) return null;

        n ??= new()
        {
            X = 0,
            Y = 0,
            Z = 0
        };

        double n1 = n.X, n2 = n.Y, n3 = n.Z;
        double p1 = p.X, p2 = p.Y, p3 = p.Z;

        var numerator = (n1 - p1) * vp1 + (n2 - p2) * vp2 + (n3 - p3) * vp3;
        var t = numerator / denominator;

        return new()
        {
            X = p1 + v1 * t,
            Y = p2 + v2 * t,
            Z = p3 + v3 * t
        };
    }
}
