using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Runju.Infrastructure
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

        protected internal virtual void SetProperty<T>(ref T field, T newValue, Expression<Func<T>> propertyGetter)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                RaisePropertyChanged(propertyGetter);
            }
        }
    }
}
