namespace Renju.Controls
{
    using System.Windows.Controls;
    using Microsoft.Practices.Unity;

    public class GenericView<TViewModel> : ContentControl
        where TViewModel : class
    {
        [Dependency]
        public TViewModel ViewModel
        {
            get { return Content as TViewModel; }
            set { Content = value; }
        }
    }
}
