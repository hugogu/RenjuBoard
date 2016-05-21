namespace Renju.Infrastructure.Execution
{
    using System.Linq;
    using Model;

    public class SideExecutionReporter : ReportExecutionObject
    {
        private readonly Side _side;

        public SideExecutionReporter(IGameBoard<IReadOnlyBoardPoint> board, Side timingSide)
        {
            _side = timingSide;
            board.PieceDropped += OnBoardPieceDropped;
        }

        private void OnBoardPieceDropped(object sender, PieceDropEventArgs e)
        {
            var board = sender as IGameBoard<IReadOnlyBoardPoint>;
            if (board.DroppedPoints.Last().Status == Sides.Opposite(_side))
            {
                RaiseStartedEvent();
            }
            else if (board.DroppedPoints.Last().Status != null)
            {
                RaiseFinishedEvent();
            }
        }
    }
}
