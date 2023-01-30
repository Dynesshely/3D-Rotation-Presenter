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

        private static (double, double) Rotate2D((double, double) a, (double, double) o, double angle)
        {
            angle = -angle;
            var sin = Math.Sin(Math.PI / 180 * angle);
            var cos = Math.Cos(Math.PI / 180 * angle);
            (double, double) b = new()
            {
                Item1 = (a.Item1 - o.Item1) * cos - (a.Item2 - o.Item2) * sin + o.Item1,
                Item2 = (a.Item1 - o.Item1) * sin + (a.Item2 - o.Item2) * cos + o.Item2
            };
            return b;
        }

        private (double, double) Corner_lt
        {
            get
            {
                var source = (-obj_width.Half(), obj_length.Half());

                //  Yaw
                var origin = (0, 0);
                var result = Rotate2D(source, origin, yaw_slider.Value);

                return result;
            }
        }

        private (double, double) Corner_rt
        {
            get
            {
                var source = (obj_width.Half(), obj_length.Half());

                //  Yaw
                var origin = (0, 0);
                var result = Rotate2D(source, origin, yaw_slider.Value);

                return result;
            }
        }

        private (double, double) Corner_lb
        {
            get
            {
                var source = (-obj_width.Half(), -obj_length.Half());

                //  Yaw
                var origin = (0, 0);
                var result = Rotate2D(source, origin, yaw_slider.Value);

                return result;
            }
        }

        private (double, double) Corner_rb
        {
            get
            {
                var source = (obj_width.Half(), -obj_length.Half());

                //  Yaw
                var origin = (0, 0);
                var result = Rotate2D(source, origin, yaw_slider.Value);

                return result;
            }
        }

        private void Draw()
        {
            Connect(Corner_lt, Corner_rt);
            Connect(Corner_lb, Corner_rb);
            Connect(Corner_lt, Corner_lb);
            Connect(Corner_rt, Corner_rb);
        }

        private void Clear() => MainCanvas.Children.Clear();

        private void Connect((double, double) p1, (double, double) p2)
            => Connect(p1.Item1, p1.Item2, p2.Item1, p2.Item2);

        private void Connect(double x1, double y1, double x2, double y2)
        {
            var canvas = MainCanvas;
            var line = new Line
            {
                X1 = x1.ToCenter(canvas_width),
                Y1 = y1.ToCenter(canvas_height, true),
                X2 = x2.ToCenter(canvas_width),
                Y2 = y2.ToCenter(canvas_height, true),
                Stroke = main_brush,
                Fill = main_brush,
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

        private void Yaw_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
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

    public static class Extensions
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
    }
}
