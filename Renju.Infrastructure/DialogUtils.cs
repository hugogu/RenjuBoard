namespace Renju.Infrastructure
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    public static class DialogUtils
    {
        public static Window CreateViewModelDialog<T>(this T viewModel, string title, ResizeMode resizeMode = ResizeMode.CanMinimize)
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
                ResizeMode = resizeMode,
                SizeToContent = resizeMode == ResizeMode.CanMinimize ? SizeToContent.WidthAndHeight : SizeToContent.Manual,
                UseLayoutRounding = true,
                SnapsToDevicePixels = true,
            };
            TextOptions.SetTextFormattingMode(window, TextFormattingMode.Display);

            return window;
        }

        public static Window WithSize(this Window window, double width, double height)
        {
            window.Width = width;
            window.Height = height;

            return window;
        }

        public static Window WithMinSize(this Window window, double minWidth, double minHeight)
        {
            window.MinHeight = minHeight;
            window.MinWidth = minWidth;

            return window;
        }

        public static Window OnOKDo(this Window window, Action okAction)
        {
            window.Closed += (sender, e) =>
            {
                if (window.DialogResult == true && okAction != null)
                {
                    okAction();
                }
            };

            return window;
        }
    }
}
