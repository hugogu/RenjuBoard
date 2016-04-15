﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;

namespace Renju.Infrastructure
{
    public class DisposableModelBase : ModelBase, IDisposable
    {
        private bool _disposed;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DisposableModelBase()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _disposables.Dispose();
            }

            _disposed = true;
        }

        protected void AutoDispose(IDisposable disposable)
        {
            if (disposable == null)
                throw new ArgumentNullException("disposable");
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