using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatelliteClient
{
    class ExponentialAverage
    {
        double _alpha;
        double _currMean;

        public ExponentialAverage(double alpha)
        {
            _alpha = alpha;
            _currMean = 0.0;
        }

        public void push(double val)
        {
            _currMean = _alpha * val + (1 - _alpha) * _currMean;
        }

        public double get()
        {
            return _currMean;
        }
    }
}
