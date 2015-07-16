using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SatelliteServer
{
    class ThreadSafeContainer<T>
    {
        private T _obj;
        private EventWaitHandle _go;

        public ThreadSafeContainer()
        {
            _go = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        public T get()
        {
            _go.WaitOne();
            lock(this) {
                return _obj;
            }
        }

        public void set(T obj)
        {
            lock(this) {
                _obj = obj;
            }
            _go.Set();
        }
    }
}
