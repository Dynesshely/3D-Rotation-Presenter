using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _3D_Rotation_Presenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Draw();
        }

        private static double obj_width = 180;

        private static double obj_length = 320;

        private const double canvas_width = 650;

        private const double canvas_height = 650;

        private static readonly SolidColorBrush main_brush = new(Colors.Red);

        private static readonly SolidColorBrush secondary_brush = new(Colors.Green);

        private (double, double, double) RotationAngles
            => (yaw_slider.Value, pitch_slider.Value, roll_slider.Value);

        private static Point Camera => new()
        {
            X = 0,
            Y = 0,
            Z = 800
        };

        private Point Bottom_Bar_Left => new Point()
        {
            X = -obj_width.Half() + obj_width / 9,
            Y = -obj_length.Half() + obj_length / 16,
            Z = 0
        }
        .Rotate(RotationAngles);

        private Point Bottom_Bar_Right => new Point()
        {
            X = obj_width.Half() - obj_width / 9,
            Y = -obj_length.Half() + obj_length / 16,
            Z = 0
        }
        .Rotate(RotationAngles);

        private Point Corner_lt => new Point()
        {
            X = -obj_width.Half(),
            Y = obj_length.Half(),
            Z = 0,
        }
        .Rotate(RotationAngles);

        private Point Corner_rt => new Point()
        {
            X = obj_width.Half(),
            Y = obj_length.Half(),
            Z = 0,
        }
        .Rotate(RotationAngles);

        private Point Corner_lb => new Point()
        {
            X = -obj_width.Half(),
            Y = -obj_length.Half(),
            Z = 0,
        }
        .Rotate(RotationAngles);

        private Point Corner_rb => new Point()
        {
            X = obj_width.Half(),
            Y = -obj_length.Half(),
            Z = 0,
        }
        .Rotate(RotationAngles);

        private void Draw()
        {
            var pitchAngle = pitch_slider.Value;
            var rollAngle = roll_slider.Value;
            var isBack = pitchAngle.Abs() > 90 || rollAngle.Abs() > 90;
            var nowBrush = isBack ? secondary_brush : main_brush;

            var a = Corner_lt.GetCrossPoint(Camera, null, null);
            var b = Corner_rt.GetCrossPoint(Camera, null, null);
            var c = Corner_rb.GetCrossPoint(Camera, null, null);
            var d = Corner_lb.GetCrossPoint(Camera, null, null);
            var e = Bottom_Bar_Left.GetCrossPoint(Camera, null, null);
            var f = Bottom_Bar_Right.GetCrossPoint(Camera, null, null);

            if (a is not null && b is not null)
                Connect(a, b, nowBrush);
            if (b is not null && c is not null)
                Connect(b, c, nowBrush);
            if (c is not null && d is not null)
                Connect(c, d, nowBrush);
            if (a is not null && d is not null)
                Connect(a, d, nowBrush);
            if (e is not null && f is not null)
                Connect(e, f, nowBrush);

            //Connect(Corner_lt, Corner_rt, nowBrush);
            //Connect(Corner_lb, Corner_rb, nowBrush);
            //Connect(Corner_lt, Corner_lb, nowBrush);
            //Connect(Corner_rt, Corner_rb, nowBrush);
            //Connect(Bottom_Bar_Left, Bottom_Bar_Right, nowBrush);
        }

        private void Clear() => MainCanvas.Children.Clear();

        private void Connect(Point p1, Point p2, Brush brush)
            => Connect((p1.X, p1.Y), (p2.X, p2.Y), brush);

        private void Connect((double, double) p1, (double, double) p2, Brush brush)
            => Connect(p1.Item1, p1.Item2, p2.Item1, p2.Item2, brush);

        private void Connect(double x1, double y1, double x2, double y2, Brush brush)
        {
            var canvas = MainCanvas;
            var line = new Line
            {
                X1 = x1.ToCenter(canvas_width),
                Y1 = y1.ToCenter(canvas_height, true),
                X2 = x2.ToCenter(canvas_width),
                Y2 = y2.ToCenter(canvas_height, true),
                Stroke = brush,
                Fill = brush,
                StrokeThickness = 1
            };
            canvas.Children.Add(line);
        }

        private void SetObjSizeButton_Click(object sender, RoutedEventArgs e)
        {
            obj_width = double.Parse(obj_width_input.Text);
            obj_length = double.Parse(obj_length_input.Text);
        }

        private void ForceDrawButton_Click(object sender, RoutedEventArgs e)
        {
            Draw();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e) => Clear();

        private void Rotation_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Clear();
            Draw();
        }

        private void ResetRotationButton_Click(object sender, RoutedEventArgs e)
        {
            yaw_slider.Value = 0;
            pitch_slider.Value = 0;
            roll_slider.Value = 0;
        }

        private void Camera_input_height_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(camera_input_height.Text, out int z))
                Camera.Z = z;
        }
    }

    public static partial class Extensions
    {
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

        public static double Half(this double num) => num / 2;

        public static double Abs(this double num) => Math.Abs(num);
    }
}
