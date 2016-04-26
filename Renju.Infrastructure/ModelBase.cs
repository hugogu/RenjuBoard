using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using Prism.Mvvm;

namespace Renju.Infrastructure
{
    [Serializable]
    public class ModelBase : BindableBase
    {
        public ModelBase()
        {
            OnConstructingNewObject();
        }

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
