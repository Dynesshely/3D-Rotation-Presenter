using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Rotation_Presenter;

public class Point
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

public static partial class Extensions
{
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
}
