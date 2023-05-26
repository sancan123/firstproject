namespace CLDC_VerifyClient
{
    interface IVerifyMessage
    {
        bool NotifyIsChecking(string checkState);
        bool OutFrame(CLDC_DataCore.Model.LogModel.LogFrameInfo frameInfo);
        bool OutMessage(int messageSourse, int messageType, string message);
        bool OutMonitorInfo(int monitorType, string formatData);
        bool OutOneMeterData(string itemKey, string dataName, int meterIndex, string dataValue);
        bool OutVerifyData(string itemKey, string dataName, string[] dataValue);
        void RequestId();
        bool VerifyFinished();
    }
}
