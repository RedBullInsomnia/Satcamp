using System.ServiceModel;

namespace SatelliteServer
{
    [ServiceContract]
    public interface ISatService
    {
        [OperationContract]
        CamData GetCamData();

        [OperationContract]
        void SetCamData(CamData camData); 

        [OperationContract]
        bool GetStablizationActive();

        [OperationContract]
        void SetServoPos(int channel, int val);

        [OperationContract]
        int GetServoPos(int channel);

        [OperationContract]
        double getKi();

        [OperationContract]
        double getKp();

        [OperationContract]
        double getFrameRate();

        [OperationContract]
        double getExposureTime();

        [OperationContract]
        string NamedPing(string name);

        [OperationContract]
        bool Ping();

        [OperationContract]
        double[] GetEulerAngles();

        [OperationContract]
        byte[] Capture(); 
    }
}
