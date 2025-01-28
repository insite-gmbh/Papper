using System;
using System.Threading;

namespace Papper.Internal
{
    internal class ReaderGuard : IDisposable
    {
        private readonly ReaderWriterLockSlim _readerWriterLock;
        public ReaderGuard(ReaderWriterLockSlim readerWriterLock)
        {
            _readerWriterLock = readerWriterLock;
            _readerWriterLock.EnterReadLock();
        }
        public void Dispose() => _readerWriterLock.ExitReadLock();
    }


    internal class WriterGuard : IDisposable
    {
        private ReaderWriterLockSlim? _readerWriterLock;
        private bool IsDisposed => _readerWriterLock == null;
        public WriterGuard(ReaderWriterLockSlim readerWriterLock)
        {
            _readerWriterLock = readerWriterLock;
            _readerWriterLock.EnterWriteLock();
        }
        public void Dispose()
        {
            if (IsDisposed)
            {
                ExceptionThrowHelper.ThrowObjectDisposedException(ToString() ?? string.Empty);
            }

            _readerWriterLock?.ExitWriteLock();
            _readerWriterLock = null;
        }
    }


    internal class UpgradeableGuard : IDisposable
    {

        private class UpgradedGuard : IDisposable
        {
            private readonly UpgradeableGuard? _parentGuard;
            private readonly WriterGuard _writerLock;
            public UpgradedGuard(UpgradeableGuard parentGuard)
            {
                _parentGuard = parentGuard;
                _writerLock = new WriterGuard(_parentGuard._readerWriterLock);
            }
            public void Dispose()
            {
                _writerLock.Dispose();
                if (_parentGuard != null)
                {
                    _parentGuard._upgradedLock = null;
                }
            }
        }

        private readonly ReaderWriterLockSlim _readerWriterLock;
        private UpgradedGuard? _upgradedLock;
        public UpgradeableGuard(ReaderWriterLockSlim readerWriterLock)
        {
            _readerWriterLock = readerWriterLock;
            _readerWriterLock.EnterUpgradeableReadLock();
        }
        public IDisposable UpgradeToWriterLock()
        {
            return _upgradedLock ??= new UpgradedGuard(this);
        }
        public void Dispose()
        {
            if (_upgradedLock != null)
            {
                _upgradedLock.Dispose();
            }
            _readerWriterLock.ExitUpgradeableReadLock();
        }
    }

}
