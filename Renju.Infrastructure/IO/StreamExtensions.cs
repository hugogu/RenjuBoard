﻿namespace Renju.Infrastructure.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    public static class StreamExtensions
    {
        [SuppressMessage("General", "RCS1047:Non-asynchronous method name should not end with 'Async'.", Justification = "Observable objects are asynchrnous")]
        public static IObservable<string> ReadAllLinesAsync(this StreamReader reader)
        {
            return Observable.Create<string>(async subscriber =>
            {
                try
                {
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync().ConfigureAwait(false);
                        subscriber.OnNext(line);
                    }
                }
                catch (Exception ex)
                {
                    subscriber.OnError(ex);
                }
                subscriber.OnCompleted();
            }).SubscribeOn(ThreadPoolScheduler.Instance);
        }

        public static IObservable<string> ReadAllLinesInTask(this StreamReader reader)
        {
            return Observable.Create<string>(subscriber =>
            {
                return Task.Run(async () =>
                {
                    try
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = await reader.ReadLineAsync().ConfigureAwait(false);
                            subscriber.OnNext(line);
                        }
                    }
                    catch (Exception ex)
                    {
                        subscriber.OnError(ex);
                    }
                    subscriber.OnCompleted();
                });
            });
        }

        public static IObservable<string> ReadAllLines(this StreamReader reader)
        {
            return reader.ReadAllLinesSync().ToObservable().SubscribeOn(ThreadPoolScheduler.Instance);
        }

        public static IEnumerable<string> ReadAllLinesSync(this StreamReader reader)
        {
            while (!reader.EndOfStream)
                yield return reader.ReadLine();
        }
    }
}
