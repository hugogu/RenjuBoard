using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace Renju.Infrastructure
{
    [Serializable]
    public class ModelBase : INotifyPropertyChanged
    {
        public ModelBase()
        {
            OnConstructingNewObject();
        }

        [field: NonSerialized]
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Logging purpose only.
        /// </summary>
        protected virtual void OnConstructingNewObject()
        {
            Trace.WriteLine("Initializing " + GetType().Name);
        }

        protected internal virtual void RaisePropertyChangedAsync(string propertyName)
        {
            RunInDispatcher(new Action(() => OnPropertyChanged(propertyName)), DispatcherPriority.Background);
        }

        protected virtual void RunInDispatcher(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (Application.Current == null)
                return;

            Application.Current.Dispatcher.BeginInvoke(action, priority);
        }

        protected internal virtual void SetProperty<T>(ref T field, T newValue, bool asyncNotify = false, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                if (asyncNotify)
                    RaisePropertyChangedAsync(propertyName);
                else
                    OnPropertyChanged(propertyName);
            }
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyGetter)
        {
            OnPropertyChanged(propertyGetter.GetMemberName());
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var temp = PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void RaiseEvent<TEventArgs>(EventHandler<TEventArgs> handler, TEventArgs args)
        {
            var temp = handler;
            if (temp != null)
            {
                temp(this, args);
            }
        }

        protected void RaiseEvent(EventHandler handler)
        {
            var temp = handler;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }
    }
}
