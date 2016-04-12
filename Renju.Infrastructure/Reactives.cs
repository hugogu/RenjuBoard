using System;
using System.ComponentModel;
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
    }
}
