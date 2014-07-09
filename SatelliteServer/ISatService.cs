using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Drawing;

namespace SatelliteServer
{
    [ServiceContract]
    public interface ISatService
    {
        [OperationContract]
        void SetStabilization(bool active);

        [OperationContract]
        bool GetStablizationActive();

        [OperationContract]
        void SetServoPos(int channel, int val);

        /**
         * Returns the servo position without side-effect (see GetServoPosIfChanged for 
         * side effect on the modified status of the servo position)
         * Returns -2 if the channel is invalid
         */
        [OperationContract]
        int GetServoPos(int channel);

        [OperationContract]
        double[] GetEulerAngles();

        [OperationContract]
        byte[] Capture(); 

        [OperationContract]
        void SetEulerAngle(int angle_id, double angle_value);

        /** 
         * Returns the servo position (of given channel) and mark the servo value as "not modified"
         * These actions are done only if the value was marked as modified before, otherwise this 
         * method returns -1.
         * Returns -2 if the channel is invalid
         */
        [OperationContract]
        int GetServoPosIfChanged(int channel);
    }
}
