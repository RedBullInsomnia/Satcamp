using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace SatelliteServer
{
    class Logger
    {
        private static Logger logger = null; // singleton instance 
        private TextBoxBase tBox = null;
        private Queue<string> strQueue;
       
        private Logger() 
        {
            strQueue = new Queue<string>();
        }

        /**
         * Return the unique instance of the ConsoleLogger from the program
         */
        public static Logger instance() 
        {
            if(logger == null)
                logger = new Logger();
            return logger;
        }

        public void setTextBox(TextBoxBase box)
        {
            tBox = box;
            lock (this) { flush(); }
        }

        public void log(string str) {
            log(str, false);
        }
  
        private void log(string str, bool error) {
            actualLog(getPrefix() + " " + str + "\n", error);
        }

        private void actualLog(string toPrint, bool error) {
            if(tBox == null) {
                lock (this) { strQueue.Enqueue(toPrint);  }
            } else {
                lock (this) {
                    flush();
                    tBox.AppendText(toPrint);
                }
            }
        }

        private void flush() {
            while (strQueue.Count() > 0)
                tBox.AppendText(strQueue.Dequeue());
        }

        private string getPrefix() {
            return "[" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fffff") + "]";
        }
    }
}
