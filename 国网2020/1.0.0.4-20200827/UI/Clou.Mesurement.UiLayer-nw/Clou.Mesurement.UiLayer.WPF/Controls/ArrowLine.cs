// ArrowLine.cs by Charles Petzold, December 2007
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Mesurement.UiLayer.WPF.Controls
{
    /// <summary>
    /// ��ͷ�ؼ�
    /// </summary>
    public class ArrowLine : ArrowLineBase
    {
        #region ��������
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
        /// ��ʼ���X������
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
        /// ��ʼ���Y������
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
        /// �յ��X������
        /// </summary>
        public double X2
        {
            set { 
                SetValue(X2Property, value);
            }
            get { return (double)GetValue(X2Property); }
        }
        /// <summary>
        /// �յ��Y������
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
        #endregion ��������
        /// <summary>
        /// ��д��ͷ�ļ�����״����
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
