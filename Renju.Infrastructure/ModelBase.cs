﻿using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Threading;

namespace Renju.Infrastructure
{
    [Serializable]
    public class ModelBase : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public virtual event PropertyChangedEventHandler PropertyChanged;

        protected internal virtual void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            var handlers = PropertyChanged;
            if (handlers != null)
            {
                handlers(this, new PropertyChangedEventArgs(expression.GetMemberName()));
            }
        }

        protected internal virtual void RaisePropertyChangedAsync<T>(Expression<Func<T>> expression)
        {
            var handlers = PropertyChanged;
            if (handlers != null)
            {
                RunInDispatcher(new Action(() => handlers(this, new PropertyChangedEventArgs(expression.GetMemberName()))), DispatcherPriority.Background);
            }
        }

        protected virtual void RunInDispatcher(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (Application.Current == null)
                return;

            Application.Current.Dispatcher.BeginInvoke(action, priority);
        }

        protected internal virtual void SetProperty<T>(ref T field, T newValue, Expression<Func<T>> propertyGetter, bool asyncNotify = false)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                if (asyncNotify)
                    RaisePropertyChangedAsync(propertyGetter);
                else
                    RaisePropertyChanged(propertyGetter);
            }
        }
    }
}
