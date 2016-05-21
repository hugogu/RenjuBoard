namespace Renju.Infrastructure
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Disposables;
    using Microsoft.Practices.Unity.Utility;

    public class DisposableModelBase : ModelBase, IDisposable
    {
        [NonSerialized]
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _disposed;

        ~DisposableModelBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Trace.WriteLine("Disposing " + GetType().Name);
                _disposables.Dispose();
            }

            _disposed = true;
        }

        protected void AutoCallOnDisposing(Action action)
        {
            Guard.ArgumentNotNull(action, "atcion");
            _disposables.Add(Disposable.Create(action));
        }

        protected void AutoDispose(IDisposable disposable)
        {
            Guard.ArgumentNotNull(disposable, "disposable");
            CheckDisposableReference(disposable);
            _disposables.Add(disposable);
        }

        [Conditional("DEBUG")]
        private void CheckDisposableReference(IDisposable disposable)
        {
            if (_disposables.Any(d => d == disposable))
            {
                throw new InvalidOperationException(String.Format("IDisposable of Type {0} has already been added.", disposable.GetType().Name));
            }
        }
    }
}
