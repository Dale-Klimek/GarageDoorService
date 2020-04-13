using System;
using System.Collections.Generic;
using System.Text;

namespace GarageDoorService.Shared
{
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncLock
    {
        private readonly SemaphoreSlim _semaphore;

        private readonly Task<Releaser> _releaser;

        public AsyncLock()
        {
            _semaphore = new SemaphoreSlim(1);
            _releaser = Task.FromResult(new Releaser(this));
        }

        public Task<Releaser> LockAsync()
        {
            var wait = _semaphore.WaitAsync();
            return wait.IsCompleted
                ? _releaser
                : wait.ContinueWith(
                    (task, state) => new Releaser((AsyncLock)state),
                    this,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
        }

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock _toRelease;

            internal Releaser(AsyncLock toRelease)
            {
                _toRelease = toRelease;
            }

            public void Dispose()
            {
                _toRelease?._semaphore.Release();
            }
        }
    }
}
