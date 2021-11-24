using System;

namespace ITManagerProject.Managers
{
    public class CompleteOfferManager : IDisposable
    {
        public readonly ApplicationManager _applicationManager;
        public readonly OfferManager _offerManager;

        private bool _disposed = false;
        public CompleteOfferManager(ApplicationManager applicationManager, OfferManager offerManager)
        {
            _applicationManager = applicationManager;
            _offerManager = offerManager;
        }
        
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}