using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using _3D_Rotation_Presenter.Rotation;

namespace _3D_Rotation_Presenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static double obj_width = 180;

        private static double obj_length = 320;

        private const double canvas_width = 650;

        private const double canvas_height = 650;

        private const double default_camera_height = 800;

        private static readonly SolidColorBrush main_brush = new(Colors.Red);

        private static readonly SolidColorBrush secondary_brush = new(Colors.Green);

        /// <summary>
        /// 摄像机位置 (0, 0, z). 必须是字段而不能写成表达式体属性:
        /// 写成 `=> new() { ... }` 时每次读取都新建对象, 输入框设定的高度会立刻被丢掉.
        /// </summary>
        private static readonly Point camera = new() { X = 0, Y = 0, Z = default_camera_height };

        /// <summary>当前选中的旋转核心实现, 由 core_selector 决定.</summary>
        private IRotationCore? _core;

        /// <summary>XAML 中控件的 TextChanged 会在 InitializeComponent 期间就触发, 用它挡住首帧之前的重绘.</summary>
        private bool _ready;

        public MainWindow()
        {
            InitializeComponent();

            core_selector.ItemsSource = RotationCores.All;
            core_selector.DisplayMemberPath = nameof(IRotationCore.DisplayName);
            core_selector.SelectedIndex = 0;

            _ready = true;
            Redraw();
        }

        private (double Yaw, double Pitch, double Roll) RotationAngles
            => (yaw_slider.Value, pitch_slider.Value, roll_slider.Value);

        //  以下都是物体坐标系下的点 (未旋转). 具体用哪种算法旋转, 由 Draw() 里选中的 IRotationCore 决定.

        private Point Corner_lt => new Point()
        {
            X = -obj_width.Half(),
            Y = obj_length.Half(),
            Z = 0,
        };

        private Point Corner_rt => new Point()
        {
            X = obj_width.Half(),
            Y = obj_length.Half(),
            Z = 0,
        };

        private Point Corner_rb => new Point()
        {
            X = obj_width.Half(),
            Y = -obj_length.Half(),
            Z = 0,
        };

        private Point Corner_lb => new Point()
        {
            X = -obj_width.Half(),
            Y = -obj_length.Half(),
            Z = 0,
        };

        private Point Bottom_Bar_Left => new Point()
        {
            X = -obj_width.Half() + obj_width / 9,
            Y = -obj_length.Half() + obj_length / 16,
            Z = 0
        };

        private Point Bottom_Bar_Right => new Point()
        {
            X = obj_width.Half() - obj_width / 9,
            Y = -obj_length.Half() + obj_length / 16,
            Z = 0
        };

        private Point[] ObjectPoints =>
        [
            Corner_lt,
            Corner_rt,
            Corner_rb,
            Corner_lb,
            Bottom_Bar_Left,
            Bottom_Bar_Right
        ];

        private void Draw()
        {
            if (!_ready || _core is null) return;

            var (yaw, pitch, roll) = RotationAngles;

            //  旋转只换算一次, 六个点复用同一个 IRotation.
            var rotation = _core.Create(yaw, pitch, roll);

            //  背面判断: 看变换之后的面法向量是否背向摄像机.
            //  物体位于 z = 0 平面且中心在原点, 摄像机在 (0, 0, z) 且 z > 0, 于是
            //  n·(摄像机 - 物体中心) = n.Z * z, 符号与 n.Z 完全一致, 判据即 n.Z < 0.
            //  原先用 |pitch| > 90 || |roll| > 90 近似, 在两个角都越过 ±90° 时会误判.
            var normal = rotation.Apply(Vector3D.ScreenNormal);
            var isBack = normal.Z < 0;
            var nowBrush = isBack ? secondary_brush : main_brush;

            var a = rotation.Apply(Corner_lt).GetCrossPoint(camera, null, null);
            var b = rotation.Apply(Corner_rt).GetCrossPoint(camera, null, null);
            var c = rotation.Apply(Corner_rb).GetCrossPoint(camera, null, null);
            var d = rotation.Apply(Corner_lb).GetCrossPoint(camera, null, null);
            var e = rotation.Apply(Bottom_Bar_Left).GetCrossPoint(camera, null, null);
            var f = rotation.Apply(Bottom_Bar_Right).GetCrossPoint(camera, null, null);

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

            var projected = new[] { a, b, c, d, e, f };
            var visible = 0;
            foreach (var p in projected)
                if (p is not null) visible++;

            core_readout.Text = _core.Format(yaw, pitch, roll);
            state_readout.Text =
                $"n = ({normal.X,7:F3}, {normal.Y,7:F3}, {normal.Z,7:F3})  ->  {(isBack ? "BACK (green)" : "FRONT (red)")}\n"
                + $"camera = (0, 0, {camera.Z:F1}),  projected vertices = {visible}/6";

            //Connect(Corner_lt, Corner_rt, nowBrush);
            //Connect(Corner_lb, Corner_rb, nowBrush);
            //Connect(Corner_lt, Corner_lb, nowBrush);
            //Connect(Corner_rt, Corner_rb, nowBrush);
            //Connect(Bottom_Bar_Left, Bottom_Bar_Right, nowBrush);
        }

        /// <summary>清空后重画. 凡是会改变投影的输入都走这里, 保证画面不叠加.</summary>
        private void Redraw()
        {
            Clear();
            Draw();
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

        private void Core_selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (core_selector.SelectedItem is not IRotationCore core) return;

            _core = core;
            Redraw();
        }

        /// <summary>逐个点比较所有核心实现的输出, 用来确认"换表示法不换结果".</summary>
        private void VerifyCoresButton_Click(object sender, RoutedEventArgs e)
        {
            var (yaw, pitch, roll) = RotationAngles;

            var rotations = new List<IRotation>();
            foreach (var core in RotationCores.All)
                rotations.Add(core.Create(yaw, pitch, roll));

            var max = 0d;
            foreach (var point in ObjectPoints)
            {
                var reference = rotations[0].Apply(point);
                for (var i = 1; i < rotations.Count; i++)
                {
                    var other = rotations[i].Apply(point);
                    max = Math.Max(max, Math.Abs(reference.X - other.X));
                    max = Math.Max(max, Math.Abs(reference.Y - other.Y));
                    max = Math.Max(max, Math.Abs(reference.Z - other.Z));
                }
            }

            verify_readout.Text =
                $"{RotationCores.All.Count} cores x {ObjectPoints.Length} points\n"
                + $"max coordinate deviation = {max:E3}";
        }

        private void SetObjSizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(obj_width_input.Text, out var width) && width > 0)
                obj_width = width;
            if (double.TryParse(obj_length_input.Text, out var length) && length > 0)
                obj_length = length;

            Redraw();
        }

        private void ForceDrawButton_Click(object sender, RoutedEventArgs e)
        {
            Draw();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e) => Clear();

        private void Rotation_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Redraw();
        }

        private void ResetRotationButton_Click(object sender, RoutedEventArgs e)
        {
            yaw_slider.Value = 0;
            pitch_slider.Value = 0;
            roll_slider.Value = 0;
        }

        private void Camera_input_height_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(camera_input_height.Text, out var z) && z > 0)
                camera.Z = z;

            Redraw();
        }
    }
}
