using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace RenjuBoard.Controls
{
    public class LinedGrid : UniformGrid
    {
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            DrawGridLines(dc);
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(LinedGrid), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(LinedGrid), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        private void DrawGridLines(DrawingContext drawingContext)
        {
            var cellWidth = ActualWidth / Columns;
            var cellHeight = ActualHeight / Rows;
            var lineWidth = ActualWidth - cellWidth;
            var lineHeight = ActualHeight - cellHeight;

            foreach (var row in Enumerable.Range(0, Rows))
            {
                var rowHeight = cellHeight / 2 + row * cellHeight;
                var x1 = new Point(cellWidth / 2, rowHeight);
                var x2 = new Point(cellWidth / 2 + lineWidth, rowHeight);

                drawingContext.DrawLine(new Pen(Stroke, StrokeThickness), x1, x2);
            }

            foreach (var column in Enumerable.Range(0, Columns))
            {
                var columnOffset = cellWidth / 2 + column * cellWidth;
                var y1 = new Point(columnOffset, cellHeight / 2);
                var y2 = new Point(columnOffset, cellHeight / 2 + lineHeight);

                drawingContext.DrawLine(new Pen(Stroke, StrokeThickness), y1, y2);
            }
        }
    }
}
