using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Renju.Core;

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

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(int), typeof(GameBoardPanel),
                new FrameworkPropertyMetadata(15, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange));

        public IEnumerable<PieceLine> Lines
        {
            get { return (IEnumerable<PieceLine>)GetValue(LinesProperty); }
            set { SetValue(LinesProperty, value); }
        }

        public static readonly DependencyProperty LinesProperty =
            DependencyProperty.Register("Lines", typeof(IEnumerable<PieceLine>), typeof(GameBoardPanel), new FrameworkPropertyMetadata(null));


        public IReadBoardState ResolvingBoard
        {
            get { return (IReadBoardState)GetValue(ResolvingBoardProperty); }
            set { SetValue(ResolvingBoardProperty, value); }
        }

        public static readonly DependencyProperty ResolvingBoardProperty =
            DependencyProperty.Register("ResolvingBoard", typeof(IReadBoardState), typeof(GameBoardPanel), new FrameworkPropertyMetadata(null));

        public IEnumerable<string> XLables
        {
            get { return Enumerable.Range(0, Size).Select(i => ((char)(i + 'A')).ToString()); }
        }

        public IEnumerable<string> YLables
        {
            get { return Enumerable.Range(0, Size).Select(i => (Size - i).ToString()); }
        }
    }
}
