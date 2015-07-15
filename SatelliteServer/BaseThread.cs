using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SatelliteServer
{
    /**
     * A class for spawning a thread that must perform a specific task
     */
    public abstract class BaseThread
    {
        private Thread _thread; /** Thread that fetches the data */
        protected bool _go; /** True for the thread to go on */

        public BaseThread()
        {
            _thread = new Thread(new ThreadStart(work));
        }

        public void Start() 
        { 
            _go = true; 
            _thread.Start(); 
        }

        public void Stop() { _go = false; }
        public void Join() { _thread.Join(); }
        public bool IsAlive() { return _thread.IsAlive; }

        /**
         * Work to perform on the thread
         * if the action is done in a loop the loop should be conditionned with _go and IsAlive()
         */
        protected abstract void work();
    }
}
