namespace Renju.Controls
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Infrastructure.Model;

    public class GameBoardPanel : ItemsControl
    {
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(int), typeof(GameBoardPanel), new FrameworkPropertyMetadata(15, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange));

        public static readonly DependencyProperty LinesProperty =
            DependencyProperty.Register("Lines", typeof(IEnumerable<PieceLine>), typeof(GameBoardPanel), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty PreviewLinesProperty =
            DependencyProperty.Register("PreviewLines", typeof(IEnumerable<PieceLine>), typeof(GameBoardPanel), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty ResolvingPointsProperty =
            DependencyProperty.Register("ResolvingPoints", typeof(IEnumerable<IReadOnlyBoardPoint>), typeof(GameBoardPanel), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty ShowCalibrationLabelsProperty =
            DependencyProperty.Register("ShowCalibrationLabels", typeof(bool), typeof(GameBoardPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsParentArrange));

        static GameBoardPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GameBoardPanel), new FrameworkPropertyMetadata(typeof(GameBoardPanel)));
        }

        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public IEnumerable<PieceLine> Lines
        {
            get { return (IEnumerable<PieceLine>)GetValue(LinesProperty); }
            set { SetValue(LinesProperty, value); }
        }

        public IEnumerable<PieceLine> PreviewLines
        {
            get { return (IEnumerable<PieceLine>)GetValue(PreviewLinesProperty); }
            set { SetValue(PreviewLinesProperty, value); }
        }

        public IEnumerable<IReadOnlyBoardPoint> ResolvingPoints
        {
            get { return (IEnumerable<IReadOnlyBoardPoint>)GetValue(ResolvingPointsProperty); }
            set { SetValue(ResolvingPointsProperty, value); }
        }

        public bool ShowCalibrationLabels
        {
            get { return (bool)GetValue(ShowCalibrationLabelsProperty); }
            set { SetValue(ShowCalibrationLabelsProperty, value); }
        }

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
