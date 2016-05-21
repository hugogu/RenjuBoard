namespace Renju.Infrastructure.ViewModel
{
    using System;
    using System.Windows;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    public class VMInfo : DependencyObject
    {
        public static readonly DependencyProperty DataTypeProperty =
            DependencyProperty.RegisterAttached("DataType", typeof(Type), typeof(VMInfo), new FrameworkPropertyMetadata(null, OnVMDataTypePropertyChanged));

        public static Type GetDataType(DependencyObject obj)
        {
            return obj.GetValue(DataTypeProperty) as Type;
        }

        public static void SetDataType(DependencyObject obj, Type value)
        {
            obj.SetValue(DataTypeProperty, value);
        }

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

            if (obj is FrameworkElement)
                (obj as FrameworkElement).DataContext = instance;
            else
                throw new InvalidOperationException("Can't set VMInfo.DataType on object of type " + type.FullName);
        }
    }
}