using System;
using System.Threading;
using System.Threading.Tasks;

namespace Papper.Internal
{
    public sealed class SemaphoreGuard : IDisposable
    {
        private SemaphoreSlim _semaphore;
        private bool IsDisposed => _semaphore == null;
        public SemaphoreGuard(SemaphoreSlim semaphore, bool wait = true)
        {
            _semaphore = semaphore ?? ExceptionThrowHelper.ThrowArgumentNullException<SemaphoreSlim>(nameof(semaphore));
            if (wait)
            {
                _semaphore.Wait();
            }
        }

        public static async ValueTask<SemaphoreGuard> Async(SemaphoreSlim semaphore)
        {
            if (semaphore == null)
            {
                return ExceptionThrowHelper.ThrowArgumentNullException<SemaphoreGuard>(nameof(semaphore));
            }
            var guard = new SemaphoreGuard(semaphore, false);

            // first try the fast path, sync grab.
            if (!semaphore.Wait(0))
            {
                await semaphore.WaitAsync().ConfigureAwait(false);
            }
            return guard;
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                ExceptionThrowHelper.ThrowObjectDisposedException(ToString() ?? string.Empty);
            }
            _semaphore.Release();
            _semaphore = null!;
        }
    }

}
