namespace Renju.Infrastructure
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Disposables;

    [Serializable]
    public class DisposableModelBase : ModelBase, IDisposable
    {
        [NonSerialized]
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public bool Disposed { get; protected set; }

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
            if (Disposed)
                return;

            if (disposing)
            {
                Trace.WriteLine("Disposing " + GetType().Name);
                PrintAllDisposingObjects();
                _disposables.Dispose();
            }

            Disposed = true;
        }

        protected void AutoCallOnDisposing(Action action)
        {
            Debug.Assert(action != null);
            _disposables.Add(Disposable.Create(action));
        }

        protected void AutoDispose(IDisposable disposable)
        {
            if (disposable != null)
            {
                CheckDisposableReference(disposable);
                _disposables.Add(disposable);
            }
        }

        [Conditional("DEBUG")]
        private void PrintAllDisposingObjects()
        {
            foreach (var disposable in _disposables)
            {
                Trace.WriteLine("Disposing " + disposable.GetType().Name);
            }
        }

        [Conditional("DEBUG")]
        private void CheckDisposableReference(IDisposable disposable)
        {
            Debug.Assert(!_disposables.Any(d => d == disposable), String.Format("IDisposable of Type {0} has already been added.", disposable.GetType().Name));
        }
    }
}
