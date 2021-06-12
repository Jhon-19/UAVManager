using System;
using System.Collections.Generic;
using System.Linq;
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

namespace UAV_Info
{
    /// <summary>
    /// Interaction logic for HUD.xaml
    /// </summary>
    public partial class HUD : UserControl
    {
        public HUD()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 航向角
        /// </summary>
        public static readonly DependencyProperty YawAngleProperty =
          DependencyProperty.Register("YawAngle", typeof(double), typeof(HUD), new FrameworkPropertyMetadata((double)0, GestureChangedCallback));
        /// <summary>
        ///翻转角最小值0
        /// </summary>
        public static readonly DependencyProperty RollAngleProperty =
            DependencyProperty.Register("RollAngle", typeof(double), typeof(HUD), new FrameworkPropertyMetadata((double)0, GestureChangedCallback));
        /// <summary>
        /// 翻转角最大值
        /// </summary>
        public static readonly DependencyProperty MaxRollAngleProperty =
            DependencyProperty.Register("MaxRollAngle", typeof(double), typeof(HUD), new FrameworkPropertyMetadata((double)180, GestureChangedCallback));
        /// <summary>
        /// 俯仰角
        /// </summary>
        public static readonly DependencyProperty PitchAngleProperty =
          DependencyProperty.Register("PitchAngle", typeof(double), typeof(HUD), new FrameworkPropertyMetadata((double)0, GestureChangedCallback));

        /// <summary>
        /// 海拔（高度）
        /// </summary>
        public static readonly DependencyProperty AltitudeProperty =
          DependencyProperty.Register("Altitude", typeof(double), typeof(HUD), new FrameworkPropertyMetadata((double)0, GestureChangedCallback));

        /// <summary>
        /// 海拔（高度）
        /// </summary>
        public double Altitude
        {
            get { return (double)GetValue(AltitudeProperty); }
            set { SetValue(AltitudeProperty, value); }
        }

        private static void GestureChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //使布局失效，重新布置
            ((HUD)d).InvalidateVisual();
        }
        /// <summary>
        /// 航向角
        /// </summary>
        public double YawAngle
        {
            get { return (double)GetValue(YawAngleProperty); }
            set { SetValue(YawAngleProperty, value); }
        }
        /// <summary>
        /// 翻转角
        /// </summary>
        public double RollAngle
        {
            get { return (double)GetValue(RollAngleProperty); }
            set { SetValue(RollAngleProperty, value); }
        }
        /// <summary>
        /// 翻转角范围
        /// </summary>
        public double MaxRollAngle
        {
            get { return (double)GetValue(MaxRollAngleProperty); }
            set { SetValue(MaxRollAngleProperty, value); }
        }
        /// <summary>
        /// 俯仰角
        /// </summary>
        public double PitchAngle
        {
            get { return (double)GetValue(PitchAngleProperty); }
            set { SetValue(PitchAngleProperty, value); }
        }
        /// <summary>
        /// 判断俯仰角是否在正常范围内
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool PitchAngleValidataVaflueCallback(object obj)
        {
            bool b = (double)obj >= -90 && (double)obj <= 90;
            if (!b)
                throw new Exception("角度出错");
            return b;
        }
        private int tickcount = 9;
        /// <summary>
        /// 画航向
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="IsRightCanvas"></param>
        private void DrawStaffTicks(Canvas canvas, bool IsRightCanvas)
        {
            //清空canvas面板中的所有子元素
            canvas.Children.Clear();
            //获取面板的实际宽度
            double w1 = canvas.ActualWidth;
            //画航线的一条直线
            Line left_11 = new Line();
            left_11.X1 = 0;
            left_11.Y1 = 30;
            left_11.X2 = w1;
            left_11.Y2 = 30;
            left_11.Stroke = Brushes.White;
            left_11.StrokeThickness = 1;
            canvas.Children.Add(left_11);
            //画刻度
            for (int i = 1; i < 90 / tickcount - 1; i++)
            {
                Line left_t1 = new Line();
                left_t1.X1 = i * w1 / tickcount;// w1 / tickcount 将宽度分为n等份
                left_t1.Y1 = 20;
                left_t1.X2 = left_t1.X1;
                left_t1.Y2 = 30;
                left_t1.Stroke = Brushes.White;
                left_t1.StrokeThickness = 1;
                canvas.Children.Add(left_t1);
                //如果是奇数，则不显示
                if ((IsRightCanvas && i % 2 == 1) || (!IsRightCanvas && i % 2 == 0))
                    continue;
                var ticktext = new BorderTextLabel();
                ticktext.FontWeight = FontWeights.ExtraBold;
                ticktext.Stroke = Brushes.Gray;
                ticktext.FontSize = 14;
                //显示数字
                if (IsRightCanvas)
                    ticktext.Text = (i * 90 / tickcount).ToString();
                else
                    ticktext.Text = (90 - i * 90 / tickcount).ToString();
                ticktext.Foreground = Brushes.White;
                Canvas.SetTop(ticktext, 2);
                Canvas.SetLeft(ticktext, left_t1.X1 - 10);
                canvas.Children.Add(ticktext);
            }
        }
        /// <summary>
        /// 画偏航
        /// </summary>
        /// <param name="yaw"></param>
        private void SetYaw(double yaw)
        {
            if (Grid_YawStaff.ActualWidth == 0)
                return;
            yaw = yaw % 360;
            if (yaw > 180)
                yaw = yaw - 360;
            if (yaw < -180)
                yaw = yaw + 360;
            double left = Grid_YawStaff.ActualWidth / 2 - 15;
            if (yaw > 90 || yaw < -90)
            {
                TextBlock_YawStaff_Left.Text = "E";
                TextBlock_YawStaff_Middle.Text = "S";
                TextBlock_YawStaff_Right.Text = "W";
                if (yaw != 180 && yaw != -180)
                {
                    if (yaw > 0)
                    {
                        if (yaw > 0)
                            left = Grid_YawStaff.ActualWidth / 2 - 30 - (180 - yaw) / 90 * Canvas_YawStaff_Right.ActualWidth;
                        else if (yaw < 0)
                            left = Grid_YawStaff.ActualWidth / 2 + (180 + yaw) / 90 * Canvas_YawStaff_Right.ActualWidth;
                    }
                }
            }
            else
            {
                TextBlock_YawStaff_Left.Text = "W";
                TextBlock_YawStaff_Middle.Text = "N";
                TextBlock_YawStaff_Right.Text = "E";
                if (yaw == 90) left = Grid_YawStaff.ActualWidth - 30;
                else if (yaw == -90) left = 0;
                else if (yaw > 0) left = Grid_YawStaff.ActualWidth / 2 + yaw / 90 * Canvas_YawStaff_Right.ActualWidth;
                else if (yaw < 0) left = Grid_YawStaff.ActualWidth / 2 - 30 + yaw / 90 * Canvas_YawStaff_Right.ActualWidth;
            }
            Canvas_YawStaff_Value.Margin = new Thickness(left, 0, 0, 0);
            Text_YawStaff_Value.Text = Math.Abs(yaw % 90).ToString("0.##");
            Text_YawStaff_Value.ToolTip = "原始值：" + yaw.ToString("0.###");
        }
        /// <summary>
        /// 画弧形的小刻度和数字
        /// </summary>
        /// <param name="cycleR">半径</param>
        /// <param name="angle">弧度</param>
        private void DrawRollTick(double cycleR, double angle)
        {
            //画刻度
            Line line = new Line();
            line.X1 = 0;
            line.X2 = 0;
            line.Y1 = 6;
            line.Y2 = 0;
            line.Stroke = Brushes.White;
            line.StrokeThickness = 2;
            Canvas.SetTop(line, -cycleR);
            line.RenderTransform = new RotateTransform(angle, 0, cycleR);
            Canvas_ViewPortMiddle.Children.Add(line);
            //画数字
            var textblock = new BorderTextLabel();
            textblock.Width = 20;
            textblock.Stroke = Brushes.DimGray;
            textblock.HorizontalContentAlignment = HorizontalAlignment.Center;
            textblock.Text = Math.Abs(angle).ToString("##0");
            textblock.Foreground = Brushes.White;
            textblock.FontSize = 14;
            textblock.FontWeight = FontWeights.Bold;
            Canvas.SetTop(textblock, -cycleR - 20);
            Canvas.SetLeft(textblock, -textblock.Width / 2);
            textblock.RenderTransform = new RotateTransform(angle, textblock.Width / 2, cycleR + 20);
            Canvas_ViewPortMiddle.Children.Add(textblock);
        }
        /// <summary>
        /// 画俯仰角的刻度和数字
        /// </summary>
        /// <param name="pitch"></param>
        /// <param name="offset"></param>
        private void DrawPitchTick(double pitch, double offset)
        {
            //画刻度(大刻度，每五个小刻度为单位)
            Line line = new Line();
            line.X1 = 0;
            line.X2 = 40;
            line.Y1 = 0;
            line.Y2 = 0;
            line.Stroke = Brushes.White;
            line.StrokeThickness = 2;
            Canvas.SetLeft(line, -20);
            Canvas.SetTop(line, offset);
            Canvas_ViewPortMiddle.Children.Add(line);
            //画数字
            var textblock = new BorderTextLabel();
            textblock.Width = 22;
            textblock.Stroke = Brushes.DimGray;
            textblock.HorizontalContentAlignment = HorizontalAlignment.Center;
            textblock.Text = pitch.ToString("##0");
            textblock.Foreground = Brushes.White;
            textblock.FontSize = 16;
            textblock.FontWeight = FontWeights.Bold;
            Canvas.SetTop(textblock, offset - 8);
            Canvas.SetLeft(textblock, -textblock.Width - 26);
            Canvas_ViewPortMiddle.Children.Add(textblock);
        }
        /// <summary>
        /// 画俯仰角的小刻度
        /// </summary>
        /// <param name="pitch"></param>
        /// <param name="offset"></param>
        //
        private void DrawShortPitchTick(double pitch, double offset)
        {
            Line line = new Line();
            line.X1 = 0;
            line.X2 = 20;
            line.Y1 = 0;
            line.Y2 = 0;
            line.Stroke = Brushes.White;
            line.StrokeThickness = 1;
            Canvas.SetLeft(line, -10);
            Canvas.SetTop(line, offset);
            Canvas_ViewPortMiddle.Children.Add(line);
        }
        /// <summary>
        /// 画俯仰角指示值的刻度和数字
        /// </summary>
        /// <param name="pitch"></param>
        /// <param name="offset"></param>
        private void DrawPitchValue(double pitch, double offset)
        {
            //画刻度
            Line line = new Line();
            line.X1 = 0;
            line.X2 = 26;
            line.Y1 = 0;
            line.Y2 = 0;
            line.Stroke = Brushes.Violet;
            line.StrokeThickness = 2;
            Canvas.SetTop(line, offset);
            Canvas_ViewPortMiddle.Children.Add(line);
            //画数字
            TextBlock textblock = new TextBlock();
            textblock.Width = 40;
            textblock.TextAlignment = TextAlignment.Center;
            textblock.Text = pitch.ToString("##0.#");
            textblock.Foreground = Brushes.White;
            textblock.Background = Brushes.Red;
            textblock.FontSize = 12;
            textblock.FontWeight = FontWeights.Bold;
            Canvas.SetTop(textblock, offset - 8);
            Canvas.SetLeft(textblock, 26);
            Canvas_ViewPortMiddle.Children.Add(textblock);
        }
        private void DrawRollPitchCycle()
        {
            Canvas_ViewPortMiddle.Children.Clear();
            bool isLargeArc = MaxRollAngle > 90;
            //double cycleR = Grid_Virwport.ActualWidth / 4;
            double cycleR = 100; //圆弧半径
            //MaxRollAngle和180的比例，求出x，y坐标
            Point startPoint = new Point(-cycleR * Math.Sin(MaxRollAngle * Math.PI / 180), -cycleR * Math.Cos(MaxRollAngle * Math.PI / 180));
            Point endPoint = new Point(cycleR * Math.Sin(MaxRollAngle * Math.PI / 180), -cycleR * Math.Cos(MaxRollAngle * Math.PI / 180));
            if (MaxRollAngle == 180)
            {
                startPoint = new Point(-0.1, cycleR);
                endPoint = new Point(0.1, cycleR);
            }
            //两点间的一条椭弧线
            ArcSegment arcpath = new ArcSegment(endPoint, new Size(cycleR, cycleR), 0, isLargeArc, SweepDirection.Clockwise, true);
            PathGeometry geometry = new PathGeometry(new PathFigure[] { new PathFigure(startPoint, new PathSegment[] { arcpath }, false) });
            Path cyclepath = new Path();
            cyclepath.Data = geometry;
            cyclepath.Stroke = Brushes.Blue;
            cyclepath.StrokeThickness = 2;
            int tickangle = 10;
            DrawRollTick(cycleR, 0);
            for (int angle = tickangle; angle <= MaxRollAngle; angle += tickangle)
            {
                if (angle == 180) break;
                DrawRollTick(cycleR, angle);
                DrawRollTick(cycleR, -angle);
            }
            if (MaxRollAngle == 180)
                DrawRollTick(cycleR, MaxRollAngle);
            Canvas_ViewPortMiddle.Children.Add(cyclepath);
            //设置小刻度位置
            #region 俯仰角
            int pitch = (((int)PitchAngle) / 10) * 10;
            int pitchcount = 3;
            int pitchspace = 5;
            DrawPitchTick(pitch, 0);
            for (int i = 1; i < pitchcount; i++)
            {
                DrawPitchTick(pitch + i * pitchspace, -cycleR * i / pitchcount);
                for (int j = 1; j < pitchspace; j++)
                    DrawShortPitchTick(pitch + (i - 1) * pitchspace + j, -cycleR * ((i - 1) + (float)j / pitchspace) / pitchcount);
                DrawPitchTick(pitch - i * pitchspace, cycleR * i / pitchcount);
                for (int j = 1; j < pitchspace; j++)
                    DrawShortPitchTick(pitch + (i - 1) * pitchspace + j, cycleR * ((i - 1) + (float)j / pitchspace) / pitchcount);
            }
            DrawPitchValue(PitchAngle, -(PitchAngle - pitch) * cycleR / (pitchspace * pitchcount));
            #endregion

            //具有以度为单位顺时针旋转指定的角度。 旋转中心位于原点(0，0)
            Canvas_ViewPortMiddle.RenderTransform = new RotateTransform(-RollAngle);

            Canvas.SetTop(Canvas_RollCursor, -cycleR - Canvas_RollCursor.Height);
            Canvas.SetLeft(Canvas_RollCursor, -Canvas_RollCursor.Width / 2);
            Text_RollStaff_Value.Text = RollAngle.ToString("0.##");
        }

        protected virtual void RedrawRoll()
        {
            if (Grid_Virwport.ActualHeight == 0 || Grid_Virwport.ActualWidth == 0) return;
            var bkbrush = (LinearGradientBrush)Grid_Virwport.Background;
            double roll = (RollAngle % 360) * Math.PI / 180;
            if (roll > Math.PI) roll = Math.PI * 2 - roll;
            if (roll < -Math.PI) roll = Math.PI * 2 + roll;
            double oppositeangle = Math.Atan(Grid_Virwport.ActualWidth / Grid_Virwport.ActualHeight);
            double startx = 0, starty = 0;
            if (roll >= -oppositeangle && roll <= oppositeangle)
            {
                startx = 0.5 * Grid_Virwport.ActualWidth - 0.5 * Grid_Virwport.ActualHeight * Math.Tan(roll);
                starty = 0;
            }
            else if (roll >= Math.PI - oppositeangle || roll <= -Math.PI + oppositeangle)
            {
                if (roll > 0)
                    startx = 0.5 * Grid_Virwport.ActualWidth - 0.5 * Grid_Virwport.ActualHeight * Math.Tan(Math.PI - roll);
                else
                    startx = 0.5 * Grid_Virwport.ActualWidth + 0.5 * Grid_Virwport.ActualHeight * Math.Tan(Math.PI + roll);
                starty = Grid_Virwport.ActualHeight;
            }
            else if (roll > oppositeangle && roll < Math.PI - oppositeangle)
            {
                startx = 0;
                starty = 0.5 * Grid_Virwport.ActualHeight - 0.5 * Grid_Virwport.ActualWidth / Math.Tan(roll);
            }
            else if (roll > -Math.PI + oppositeangle && roll < -oppositeangle)
            {
                startx = Grid_Virwport.ActualWidth;
                starty = 0.5 * Grid_Virwport.ActualHeight + 0.5 * Grid_Virwport.ActualWidth / Math.Tan(roll);
            }
            bkbrush.StartPoint = new Point(startx, starty);
            bkbrush.EndPoint = new Point(Grid_Virwport.ActualWidth - startx, Grid_Virwport.ActualHeight - starty);

            DrawRollPitchCycle();
        }

        protected virtual void RedrawPitch()
        {
            var bkbrush = (LinearGradientBrush)Grid_Virwport.Background;
            double offset = 0.5;
            double pitch = PitchAngle * Math.PI / 180;
            if (pitch > Math.PI / 3) pitch = Math.PI / 3;//设置俯仰视觉为120度
            if (pitch < -Math.PI / 3) pitch = -Math.PI / 3;
            offset = 0.5 * (1 + Math.Tan(pitch) / Math.Tan(Math.PI / 3));
            bkbrush.GradientStops[1].Offset = offset;
            bkbrush.GradientStops[2].Offset = offset;

            DrawRollPitchCycle();
        }

        //绘制右边海拔（高度）矩形
        private void RedrawHeight()
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 50;
            rectangle.Height = 150;
            rectangle.StrokeThickness = 2;
            rectangle.Stroke = Brushes.White;
            Canvas.SetTop(rectangle, -rectangle.Height / 2);
            Canvas.SetRight(rectangle, 0);
            Canvas_ViewPortRight.Children.Add(rectangle);
            double from = Altitude - Altitude % 5 + 15;
            double space = (rectangle.Height - 20) / 5;
            for (int i = 0; i < 6; i++)
            {
                Line li = new Line();
                li.X1 = -rectangle.Width;
                li.Y1 = -rectangle.Height / 2 + 10 + space * i;
                li.X2 = li.X1 + 10;
                li.Y2 = li.Y1;
                li.StrokeThickness = 2;
                li.Stroke = Brushes.White;
                Canvas_ViewPortRight.Children.Add(li);
                BorderTextLabel texti = new BorderTextLabel();
                texti.Width = 22;
                texti.Stroke = Brushes.DimGray;
                texti.HorizontalContentAlignment = HorizontalAlignment.Left;
                texti.Text = (from - i * 5).ToString("##0");
                texti.Foreground = Brushes.White;
                texti.FontSize = 16;
                texti.FontWeight = FontWeights.Bold;
                Canvas.SetLeft(texti, li.X2 + 4);
                Canvas.SetTop(texti, li.Y1 - 10);
                Canvas_ViewPortRight.Children.Add(texti);
            }
            TextBlock textblock = new TextBlock();
            textblock.Width = 20;
            textblock.Padding = new Thickness(0, 0, 2, 0);
            textblock.TextAlignment = TextAlignment.Right;
            textblock.Text = Altitude.ToString("##0.#");
            textblock.Foreground = Brushes.White;
            textblock.Background = Brushes.Red;
            textblock.FontSize = 12;
            textblock.FontWeight = FontWeights.Bold;
            double offset = (from - Altitude) / 25 * (rectangle.Height - 20) - rectangle.Height / 2 + 10;
            Canvas.SetTop(textblock, offset - 8);
            Canvas.SetRight(textblock, 0);
            Canvas_ViewPortRight.Children.Add(textblock);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            RedrawYaw();
            RedrawRoll();
            RedrawPitch();
            Canvas_ViewPortRight.Children.Clear();
            RedrawHeight();
        }
        protected virtual void RedrawYaw()
        {
            DrawStaffTicks(Canvas_YawStaff_Left, false);
            DrawStaffTicks(Canvas_YawStaff_Right, true);
            SetYaw(YawAngle);
        }
    }
}
