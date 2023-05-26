// ArrowLine.cs by Charles Petzold, December 2007
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Mesurement.UiLayer.WPF.Controls
{
    /// <summary>
    /// 箭头控件
    /// </summary>
    public class ArrowLine : ArrowLineBase
    {
        #region 依赖属性
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty X1Property =
            Line.X1Property.AddOwner(typeof(ArrowLine),
                new FrameworkPropertyMetadata(0.0,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty Y1Property =
            Line.Y1Property.AddOwner(typeof(ArrowLine),
                new FrameworkPropertyMetadata(0.0,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty X2Property =
            Line.X2Property.AddOwner(typeof(ArrowLine),
                new FrameworkPropertyMetadata(0.0,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty Y2Property =
            Line.Y2Property.AddOwner(typeof(ArrowLine),
                new FrameworkPropertyMetadata(0.0,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));
        /// <summary>
        /// 开始点的X轴坐标
        /// </summary>
        public double X1
        {
            set
            {
                SetValue(X1Property, value);
            }
            get { return (double)GetValue(X1Property); }
        }
        /// <summary>
        /// 开始点的Y轴坐标
        /// </summary>
        public double Y1
        {
            set
            {
                SetValue(Y1Property, value);
            }
            get { return (double)GetValue(Y1Property); }
        }
        /// <summary>
        /// 终点的X轴坐标
        /// </summary>
        public double X2
        {
            set { 
                SetValue(X2Property, value);
            }
            get { return (double)GetValue(X2Property); }
        }
        /// <summary>
        /// 终点的Y轴坐标
        /// </summary>
        public double Y2
        {
            set
            {
                SetValue(Y2Property, value);
            }
            get { 
                return (double)GetValue(Y2Property); 
            }
        }
        #endregion 依赖属性
        /// <summary>
        /// 重写箭头的几何形状属性
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                // Clear out the PathGeometry.
                pathgeo.Figures.Clear();

                // Define a single PathFigure with the points.
                pathfigLine.StartPoint = new Point(X1, Y1);
                polysegLine.Points.Clear();
                polysegLine.Points.Add(new Point(X2, Y2));
                pathgeo.Figures.Add(pathfigLine);

                // Call the base property to add arrows on the ends.
                return base.DefiningGeometry;
            }
        }
    }
}
