using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace UAV_Info.arrow
{
    /// <summary>
    /// 带文本和箭头的两点之间连线
    /// </summary>
    public class ArrowQuadraticBezierWithText : ArrowQuadraticBezier
    {
        #region DependencyProperty

        /// <summary>
        /// 文本的依赖属性
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(ArrowQuadraticBezierWithText),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 文本对齐的依赖属性
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
            "TextAlignment",
            typeof(TextAlignment),
            typeof(ArrowQuadraticBezierWithText),
            new FrameworkPropertyMetadata(TextAlignment.Left, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 文本朝上的依赖属性
        /// </summary>
        public static readonly DependencyProperty IsTextUpProperty = DependencyProperty.Register(
            "IsTextUp",
            typeof(bool),
            typeof(ArrowQuadraticBezierWithText),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 是否显示文本的依赖属性
        /// </summary>
        public static readonly DependencyProperty ShowTextProperty = DependencyProperty.Register(
            "ShowText",
            typeof(bool),
            typeof(ArrowQuadraticBezierWithText),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion DependencyProperty

        #region Properties

        /// <summary>
        /// 文本
        /// </summary>
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)this.GetValue(TextAlignmentProperty); }
            set { this.SetValue(TextAlignmentProperty, value); }
        }

        /// <summary>
        /// 文本是否朝上
        /// </summary>
        public bool IsTextUp
        {
            get { return (bool)this.GetValue(IsTextUpProperty); }
            set { this.SetValue(IsTextUpProperty, value); }
        }

        /// <summary>
        /// 是否显示文本
        /// </summary>
        public bool ShowText
        {
            get { return (bool)this.GetValue(ShowTextProperty); }
            set { this.SetValue(ShowTextProperty, value); }
        }

        #endregion Properties

        #region Overrides

        /// <summary>
        /// 重载渲染事件
        /// </summary>
        /// <param name="drawingContext">绘图上下文</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (this.ShowText && (this.Text != null))
            {
                var txt = this.Text.Trim();
                var startPoint = this.ControlPoint;
                if (!string.IsNullOrEmpty(txt))
                {
                    var vec = this.EndPoint - this.StartPoint;
                    var angle = this.GetAngle(this.StartPoint, this.EndPoint);

                    //使用旋转变换,使其与线平行
                   var transform = new RotateTransform(angle)
                   {
                       CenterX = this.ControlPoint.X,
                       CenterY = this.ControlPoint.Y
                   };
                    drawingContext.PushTransform(transform);

                    var defaultTypeface = new Typeface(
                        SystemFonts.StatusFontFamily,
                        SystemFonts.StatusFontStyle,
                        SystemFonts.StatusFontWeight,
                        new FontStretch());
                    var pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
                    var formattedText = new FormattedText(txt,
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        defaultTypeface,
                        SystemFonts.StatusFontSize,
                        Brushes.Black,
                        VisualTreeHelper.GetDpi(this).PixelsPerDip)
                    {
                        // 文本最大宽度为线的宽度
                        MaxTextWidth = vec.Length,

                        // 设置文本对齐方式
                        TextAlignment = this.TextAlignment
                    };

                    var offsetY = this.StrokeThickness;
                    if (this.IsTextUp)
                    {
                        // 计算文本的行数
                        double textLineCount = formattedText.Width / formattedText.MaxTextWidth;
                        if (textLineCount < 1)
                        {
                            // 怎么也得有一行
                            textLineCount = 1;
                        }

                        // 计算朝上的偏移
                        offsetY = (-formattedText.Height * textLineCount) - this.StrokeThickness;
                    }

                    startPoint = startPoint + new Vector(0, offsetY);
                    drawingContext.DrawText(formattedText, startPoint);
                    drawingContext.Pop();
                }
            }
        }

        #endregion Overrides

        #region Private Methods

        /// <summary>
        /// 获取两个点的倾角
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <returns>两个点的倾角</returns>
        private double GetAngle(Point start, Point end)
        {
            var vec = end - start;

            // X轴
            var xaxis = new Vector(1, 0);
            return Vector.AngleBetween(xaxis, vec);
        }

        #endregion Private Methods
    }
}
