namespace Renju.Infrastructure
{
    using System.Windows;
    using System.Windows.Media;

    public static class DialogUtils
    {
        public static Window CreateViewModelDialog<T>(this T viewModel, string title)
            where T : ModelBase
        {
            var window = new Window()
            {
                Title = title,
                DataContext = viewModel,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false,
                ShowActivated = true,
                ResizeMode = ResizeMode.CanMinimize,
                SizeToContent = SizeToContent.WidthAndHeight,
                UseLayoutRounding = true,
                SnapsToDevicePixels = true,
            };
            TextOptions.SetTextFormattingMode(window, TextFormattingMode.Display);

            return window;
        }
    }
}
