namespace Renju.Infrastructure
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive.Linq;

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
            var names = propertyGetters.Select(g => g.GetMemberName()).ToList();
            Func<string, bool> matchNames = givenName => names.Any(name => String.Equals(givenName, name));

            return obj.ObserveAnyProperties().Where(args => matchNames(args.PropertyName));
        }

        /// <summary>
        /// Produce the deltas of given input source. Delta means the changes compare with the previous item in the source. 
        /// 
        /// For example: 1, 2, 4, 5, 8 produce delta list of 1, 2, 1, 3
        /// </summary>
        public static IObservable<TResult> Deltas<TSource, TResult>(this IObservable<TSource> source, Func<TSource, TSource, TResult> calcDelta)
        {
            return Observable.Create<TResult>(observer =>
            {
                TSource lastSource = default(TSource);
                var sourceSubscription = source.Select((s, i) => new { Item = s, Index = i }).Subscribe(newSource =>
                {
                    if (newSource.Index != 0)
                    {
                        var delta = calcDelta(lastSource, newSource.Item);
                        observer.OnNext(delta);
                    }
                    lastSource = newSource.Item;
                }, observer.OnError, observer.OnCompleted);

                return sourceSubscription;
            });
        }
    }
}
