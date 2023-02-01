using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Rotation_Presenter;

public class Doubles_3
{
    private double x, y, z;

    internal double X
    {
        get => x;
        set => x = value;
    }

    internal double Y
    {
        get => y;
        set => y = value;
    }

    internal double Z
    {
        get => z;
        set => z = value;
    }
}

public class Point : Doubles_3 { }

public class Vector3D : Doubles_3 { }

public static partial class Extensions
{

    public static Point Rotate(this Point p, (double, double, double) angles)
        => p.Rotate(angles.Item1, angles.Item2, angles.Item3);

    public static Point Rotate(this Point p, double yaw, double pitch, double roll) => p
        .Z_Rotate(yaw)
        .Y_Rotate(pitch)
        .X_Rotate(roll);

    public static Point X_Rotate(this Point p, double alpha)
    {
        alpha *= Math.PI / 180;
        return new()
        {
            X = Math.Cos(alpha) * p.X + Math.Sin(alpha) * p.Z,
            Y = p.Y,
            Z = -Math.Sin(alpha) * p.X + Math.Cos(alpha) * p.Z
        };
    }

    public static Point Y_Rotate(this Point p, double beta)
    {
        beta *= Math.PI / 180;
        return new()
        {
            X = p.X,
            Y = Math.Cos(beta) * p.Y - Math.Sin(beta) * p.Z,
            Z = Math.Sin(beta) * p.Y + Math.Cos(beta) * p.Z
        };
    }

    public static Point Z_Rotate(this Point p, double gamma)
    {
        gamma *= Math.PI / 180;
        return new()
        {
            X = Math.Cos(gamma) * p.X - Math.Sin(gamma) * p.Y,
            Y = Math.Sin(gamma) * p.X + Math.Cos(gamma) * p.Y,
            Z = p.Z
        };
    }

    public static Vector3D GetDirection(this Point p1, Point p2) => new()
    {
        X = p2.X - p1.X,
        Y = p2.Y - p1.Y,
        Z = p2.Z - p1.Z
    };

    /// <summary>
    /// 求直线与平面交点
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

        var t_base = vp1 * v1 + vp2 * v2 + vp3 * v3;
        if (t_base - 0 <= 0.00001) return null;
        else
        {
            n ??= new()
            {
                X = 0,
                Y = 0,
                Z = 0
            };

            double n1 = n.X, n2 = n.Y, n3 = n.Z;
            double p1 = p.X, p2 = p.Y, p3 = p.Z;

            var t_devide = (n1 - p1) * vp1 + (n2 - p2) * vp2 + (n3 - p3) * vp3;
            var t = t_devide / t_base;

            var x = p1 + v1 * t;
            var y = p2 + v2 * t;
            var z = p3 + v3 * t;

            return new()
            {
                X = x,
                Y = y,
                Z = z
            };
        }
    }
}
