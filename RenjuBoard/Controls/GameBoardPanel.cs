using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RenjuBoard.Controls
{
    public class GameBoardPanel : ItemsControl
    {
        static GameBoardPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GameBoardPanel), new FrameworkPropertyMetadata(typeof(GameBoardPanel)));
        }

        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(int), typeof(GameBoardPanel),
                new FrameworkPropertyMetadata(15, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange));

        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawGrid(drawingContext);
            base.OnRender(drawingContext);
        }

        private void DrawGrid(DrawingContext drawingContext)
        {
            var cellWidth = ActualWidth / Size;
            var cellHeight = ActualHeight / Size;
            var lineWidth = ActualWidth - cellWidth;
            var lineHeight = ActualHeight - cellHeight;

            foreach (var row in Enumerable.Range(0, Size))
            {
                var rowHeight = cellHeight / 2 + row * cellHeight;
                var x1 = new Point(cellWidth / 2, rowHeight);
                var x2 = new Point(cellWidth / 2 + lineWidth, rowHeight);

                drawingContext.DrawLine(new Pen(Foreground, 1), x1, x2);
            }

            foreach(var column in Enumerable.Range(0, Size))
            {
                var columnOffset = cellWidth / 2 + column * cellWidth;
                var y1 = new Point(columnOffset, cellHeight / 2);
                var y2 = new Point(columnOffset, cellHeight / 2 + lineHeight);

                drawingContext.DrawLine(new Pen(Foreground, 1), y1, y2);
            }
        }
    }
}
