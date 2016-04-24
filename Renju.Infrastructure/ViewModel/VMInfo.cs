using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Renju.Infrastructure.ViewModel
{
    public class VMInfo : DependencyObject
    {
        public static Type GetDataType(DependencyObject obj)
        {
            return obj.GetValue(DataTypeProperty) as Type;
        }

        public static void SetDataType(DependencyObject obj, Type value)
        {
            obj.SetValue(DataTypeProperty, value);
        }

        public static readonly DependencyProperty DataTypeProperty =
            DependencyProperty.RegisterAttached("DataType", typeof(Type), typeof(VMInfo), new FrameworkPropertyMetadata(null, OnVMDataTypePropertyChanged));

        private static void OnVMDataTypePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var type = e.NewValue as Type;
            object instance = null;
            if (ServiceLocator.IsLocationProviderSet)
            {
                var container = ServiceLocator.Current.GetInstance<IUnityContainer>();
                instance = container.Resolve(type);
            }
            else
            {
                instance = Activator.CreateInstance(type);
            }
            if (obj is ContentControl)
            {
                (obj as ContentControl).Content = instance;
            }
            else if (obj is FrameworkElement)
            {
                (obj as FrameworkElement).DataContext = instance;
            }
        }
    }
}
