namespace Renju.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    public class BoardPiece : Control
    {
        public static readonly DependencyProperty WeightProperty = DependencyProperty.Register("Weight", typeof(int), typeof(BoardPiece), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty SequenceNumberProperty = DependencyProperty.Register("SequenceNumber", typeof(int?), typeof(BoardPiece), new FrameworkPropertyMetadata());

        static BoardPiece()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BoardPiece), new FrameworkPropertyMetadata(typeof(BoardPiece)));
        }

        public int Weight
        {
            get { return (int)GetValue(WeightProperty); }
            set { SetValue(WeightProperty, value); }
        }

        public int? SequenceNumber
        {
            get { return (int?)GetValue(SequenceNumberProperty); }
            set { SetValue(SequenceNumberProperty, value); }
        }
    }
}
