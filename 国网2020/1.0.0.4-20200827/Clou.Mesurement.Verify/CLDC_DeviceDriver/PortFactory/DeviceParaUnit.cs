namespace CLDC_DeviceDriver.PortFactory
{
    public class DeviceParaUnit
    {
        public DeviceParaUnit(string stringPara)
        {
            string[] arrayPara = stringPara.Split('|');
            Name = arrayPara[0];
            DriverName = arrayPara[5];
            ClassName = arrayPara[6];
            Address = arrayPara[1];
            Baudrate = arrayPara[7];
            int temp = 0;
            if (int.TryParse(arrayPara[2], out temp))
            {
                StartPort = temp;
            }
            if (int.TryParse(arrayPara[3], out temp))
            {
                RemotePort = temp;
            }
            if (int.TryParse(arrayPara[4], out temp))
            {
                PortNo = temp;
            }
            if (int.TryParse(arrayPara[8], out temp))
            {
                MaxTimePerFrame = temp;
            }
            if (int.TryParse(arrayPara[9], out temp))
            {
                MaxTimePerByte = temp;
            }
            if (arrayPara.Length > 10)
            {
                DllType = arrayPara[10];
            }
        }
        public DeviceParaUnit(string name, string address, int startPort, int remotePort, int portNo, string driverName, string className, string baudrate, int maxtimeperframe, int maxtimeperbyte,string dllType)
        {
            Name = name;
            DriverName = driverName;
            ClassName = className;
            Address = address;
            StartPort = startPort;
            RemotePort = remotePort;
            PortNo = portNo;
            Baudrate = baudrate;
            MaxTimePerFrame = maxtimeperframe;
            MaxTimePerByte = maxtimeperbyte;
            DllType = dllType;
        }
        public string Name { get; private set; }
        public string DriverName { get; private set; }
        public string ClassName { get; private set; }
        public string Address { get; private set; }
        public int StartPort { get; private set; }
        public int RemotePort { get; private set; }
        public int PortNo { get; private set; }
        public string Baudrate { get; private set; }
        public int MaxTimePerFrame { get; private set; }
        public int MaxTimePerByte { get; private set; }
        public string DllType { get; private set; }
    }
}
