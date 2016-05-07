using Renju.Infrastructure;
using Renju.Infrastructure.Model;

namespace Renju.Core.Rules
{
    public abstract class DropValidationRule : ModelBase, IGameRule
    {
        private bool _isEnabled = true;

        public abstract string Name { get; }

        public virtual bool IsOptional { get { return true; } }

        public virtual bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        public virtual bool? CanDropOn(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            return true;
        }

        public Side? NextSide(IReadBoardState<IReadOnlyBoardPoint> board)
        {
            return null;
        }

        public bool? Win(IReadBoardState<IReadOnlyBoardPoint> board, PieceDrop drop)
        {
            return null;
        }
    }
}
