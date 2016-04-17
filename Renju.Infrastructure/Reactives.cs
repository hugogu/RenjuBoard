using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace Renju.Infrastructure
{
    public static class Reactives
    {
        public static IObservable<PropertyChangedEventArgs> ObserveAnyProperties(this INotifyPropertyChanged obj)
        {
            return Observable.FromEvent<PropertyChangedEventArgs>(
                       handler => obj.PropertyChanged += (sender, e) => handler(e),
                       handler => obj.PropertyChanged -= (sender, e) => handler(e));
        }

        public static IObservable<PropertyChangedEventArgs> ObserveProperty<T>(this INotifyPropertyChanged obj, params Expression<Func<T>>[] propertyGetters)
        {
            return obj.ObserveAnyProperties()
                      .Where(args => propertyGetters.Select(g => g.GetMemberName())
                                                    .Any(propertyName =>  String.Compare(args.PropertyName, propertyName) == 0));
        }
    }
}
