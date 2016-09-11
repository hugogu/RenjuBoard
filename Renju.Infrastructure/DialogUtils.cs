namespace Renju.Infrastructure
{
    using System.Windows;

    public static class DialogUtils
    {
        public static Window CreateViewModelDialog<T>(this T viewModel, string title)
            where T : ModelBase
        {
            return new Window()
            {
                Title = title,
                DataContext = viewModel,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false,
                ShowActivated = true,
                ResizeMode = ResizeMode.CanMinimize,
                SizeToContent = SizeToContent.WidthAndHeight
            };
        }
    }
}
