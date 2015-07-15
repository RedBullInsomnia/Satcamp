using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatelliteClient
{
    abstract class MovingAverage<T> where T : struct 
    {
        protected T[] values;
        protected int nextIndex, currIndex, windowsSize;
       
        public abstract T get();
        public abstract void clear();

        public MovingAverage(int windowsSize, T defVal)
        {
            if (windowsSize <= 0)
                throw new ArgumentException("Window for the moving average must be of size greater than 0");
            this.windowsSize = windowsSize;
            this.values = new T[this.windowsSize];
            fill(defVal);
            resetIndex();
        }

        public T getLast() 
        {
            return values[currIndex];
        }

        public void push(T val) {
            values[nextIndex] = val;
            nextIndex = (nextIndex + 1) % windowsSize;
            currIndex = (currIndex + 1) % windowsSize;
        }

        protected void resetIndex()
        {
            nextIndex = 0;
            currIndex = windowsSize - 1;
        }

        protected void fill(T val)
        {
            for (int i = 0; i < windowsSize; ++i)
                values[i] = val;
        }
    }

    class MovingAverageInt : MovingAverage<int>
    {
        public MovingAverageInt(int windowsSize) : base(windowsSize,0)
        {
        }

        public MovingAverageInt(int windowsSize, int defVal) : base(windowsSize,defVal)
        {
        }

        public override int get()
        {
            int average = values[0];
            for (int i = 1; i < windowsSize; ++i) 
                average += values[i];
            return average / windowsSize;
        }

        public override void clear() 
        {
            for (int i = 0; i < windowsSize; ++i)
                values[i] = 0;
            resetIndex();
        }
    }

    class MovingAverageDouble : MovingAverage<double>
    {
        public MovingAverageDouble(int windowsSize) : base(windowsSize,0.0)
        {
        }

        public MovingAverageDouble(int windowsSize, double defVal) : base(windowsSize,defVal)
        {
        }


        public override double get()
        {
            double average = values[0];
            for (int i = 1; i < windowsSize; ++i) 
                average += values[i];
            return average / (double) windowsSize;
        }

        public override void clear() 
        {
            for (int i = 0; i < windowsSize; ++i)
                values[i] = 0.0;
            resetIndex();
        }
    }


}
