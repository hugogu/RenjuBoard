using Runju.Infrastructure;

namespace Renju.Core
{
    public class BoardPoint : ModelBase
    {
        private Side? _status = null;
        public Side? Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value, () => Status); }
        }
    }
}
