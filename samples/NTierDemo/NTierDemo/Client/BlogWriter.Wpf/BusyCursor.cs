using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BlogWriter.Wpf
{
    public sealed class BusyCursor : IDisposable
    {
        private static readonly object SyncLock = new object();
        private static int _count = 0;
        private bool _disposed = false;

        public BusyCursor()
        {
            Start();
        }

        public static void Start()
        {
            lock (SyncLock)
            {
                _count++;
                if (_count == 1)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                }
            }
        }

        public static void Stop()
        {
            lock (SyncLock)
            {
                _count--;
                if (_count == 0)
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }
        
        #region IDisposable

        ~BusyCursor()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true); 
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                Stop();
                _disposed = true;
            }
        }
        
        #endregion IDisposable
    }
}
