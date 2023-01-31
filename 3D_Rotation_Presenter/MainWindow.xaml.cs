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

        private Point Corner_lt => new Point()
        {
            X = -obj_width.Half(),
            Y = obj_length.Half(),
            Z = 0,
        }
        .Z_Rotate(yaw_slider.Value)
        .Y_Rotate(pitch_slider.Value)
        .X_Rotate(roll_slider.Value);

        private Point Corner_rt => new Point()
        {
            X = obj_width.Half(),
            Y = obj_length.Half(),
            Z = 0,
        }
        .Z_Rotate(yaw_slider.Value)
        .Y_Rotate(pitch_slider.Value)
        .X_Rotate(roll_slider.Value);

        private Point Corner_lb => new Point()
        {
            X = -obj_width.Half(),
            Y = -obj_length.Half(),
            Z = 0,
        }
        .Z_Rotate(yaw_slider.Value)
        .Y_Rotate(pitch_slider.Value)
        .X_Rotate(roll_slider.Value);

        private Point Corner_rb => new Point()
        {
            X = obj_width.Half(),
            Y = -obj_length.Half(),
            Z = 0,
        }
        .Z_Rotate(yaw_slider.Value)
        .Y_Rotate(pitch_slider.Value)
        .X_Rotate(roll_slider.Value);

        private void Draw()
        {
            var pitchAngle = pitch_slider.Value;
            var rollAngle = roll_slider.Value;
            var isBack = pitchAngle.Abs() > 90 || rollAngle.Abs() > 90;
            var nowBrush = isBack ? secondary_brush : main_brush;
            Connect(Corner_lt, Corner_rt, nowBrush);
            Connect(Corner_lb, Corner_rb, nowBrush);
            Connect(Corner_lt, Corner_lb, nowBrush);
            Connect(Corner_rt, Corner_rb, nowBrush);
        }

        private void Clear() => MainCanvas.Children.Clear();

        private void Connect(Point p1, Point p2, Brush brush) => Connect((p1.X, p1.Y), (p2.X, p2.Y), brush);

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
