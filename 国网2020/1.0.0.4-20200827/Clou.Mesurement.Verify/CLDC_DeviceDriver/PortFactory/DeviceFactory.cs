using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_DeviceDriver.Drivers.Clou;
using CLDC_DeviceDriver.Drivers.Clou.DllPackage;
using System;
using System.Collections.Generic;

namespace CLDC_DeviceDriver.PortFactory
{
    class DeviceFactory
    {
        private static DeviceFactory instance = null;
        public static DeviceFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DeviceFactory();
                }
                return instance;
            }
        }
        //根据设备名区分
        public bool InitialDeviceSetting(string[] arrayDevice)
        {
            try
            {
                listRs485.Clear();
                listRsCard.Clear();
                listDeviceControl.Clear();
                listCardControl.Clear();

                string[] relayNames = { "001", "002", "003", "004", "005", "006", "007", "008" };
                for (int i = 0; i < arrayDevice.Length; i++)
                {
                    
                    #region RS485端口配置,读卡器
                    if (arrayDevice[i].StartsWith("RS485"))
                    {
                        listRs485.AddRange(ConvertMeterPorts(arrayDevice[i]));
                        continue;
                    }
                    if (arrayDevice[i].StartsWith("读卡器"))
                    {
                        listRsCard.AddRange(ConvertMeterPorts(arrayDevice[i]));
                        continue;
                    }
                    #endregion
                    #region 仪表设备
                    DeviceParaUnit deviceParaUnit = new DeviceParaUnit(arrayDevice[i]);
                    if (deviceParaUnit == null)
                    {
                        continue;
                    }
                    switch (deviceParaUnit.Name)
                    {
                        case "南网设备统一接口" :
                            listDeviceControl.Add(deviceParaUnit);
                            break;
                        case"南网读写卡器统一接口":
                            listCardControl.Add(deviceParaUnit);
                            break;
                    }
                    #endregion
                }
                return true;
            }
            catch (Exception e)
            {
                CLDC_DataCore.MessageController.Instance.AddMessage(string.Format("设备初始化参数错误:{0}", e.Message), 7, 2);
                return false;
            }
        }

        #region 设备初始化

        #region 

        private List<DeviceParaUnit> listDeviceControl = new List<DeviceParaUnit>();
        public Drivers.Clou.DllPackage.DeviceControl[] GetDeviceControl()
        {
            if (listDeviceControl == null || listDeviceControl.Count == 0)
            {
                return null;
            }
            Drivers.Clou.DllPackage.DeviceControl[] powers = new Drivers.Clou.DllPackage.DeviceControl[listDeviceControl.Count];
            for (int i = 0; i < listDeviceControl.Count; i++)
            {
                Drivers.Clou.DllPackage.DeviceControl power = new Drivers.Clou.DllPackage.DeviceControl(listDeviceControl[i].DriverName, listDeviceControl[i].ClassName, false);
                GlobalUnit.DeviceDllType = (Cus_SouthDeviceDllType)Enum.Parse(typeof(Cus_SouthDeviceDllType), listDeviceControl[i].DllType);
                powers[i] = power;
            }
            return powers;
        }

        private List<DeviceParaUnit> listCardControl = new List<DeviceParaUnit>();

        public Drivers.Clou.DllPackage.CardReaderControl[] GetCardControl()
        {
            if (listCardControl == null || listCardControl.Count == 0)
            {
                return null;
            }
            Drivers.Clou.DllPackage.CardReaderControl[] powers = new Drivers.Clou.DllPackage.CardReaderControl[listCardControl.Count];
            for (int i = 0; i < listCardControl.Count; i++)
            {
                Drivers.Clou.DllPackage.CardReaderControl power = new Drivers.Clou.DllPackage.CardReaderControl(listCardControl[i].DriverName, listCardControl[i].ClassName, false);
                GlobalUnit.CardDllType = (Cus_SouthCardDllType)Enum.Parse(typeof(Cus_SouthCardDllType), listCardControl[i].DllType); 
                power.InitCard();
                powers[i] = power;
            }
            return powers;
        }


        #endregion


        #region RS485端口配置
        private List<DeviceParaUnit> listRs485 = new List<DeviceParaUnit>();
        private List<DeviceParaUnit> listRsCard = new List<DeviceParaUnit>();

        private List<DeviceParaUnit> ConvertMeterPorts(string meterConfig)
        {
            List<DeviceParaUnit> ports = new List<DeviceParaUnit>();
            #region 解析字符串
            string[] arrayPort = meterConfig.Split('|');
            if (arrayPort.Length <8 )
            {
                return ports;
            }
            int meterCount = 0;
            int startComno = 0;
            int comnoInterval = 0;
            int maxTimePerFrame = 0;
            int maxTimePerByte = 0;
            if (!int.TryParse(arrayPort[1], out meterCount) || !int.TryParse(arrayPort[2], out startComno) || !int.TryParse(arrayPort[3], out comnoInterval) || !int.TryParse(arrayPort[6], out maxTimePerFrame) || !int.TryParse(arrayPort[7], out maxTimePerByte))
            {
                return ports;
            }
            string baudrate = arrayPort[4];
            string[] serverArray = arrayPort[5].Split('_');
            string address = arrayPort[5];
            string dllType = "";
            if (arrayPort.Length > 8)
            {
                dllType = arrayPort[8];
            }
            int startPort = 0;
            int remotePort = 0;
            if (arrayPort[5] != "COM")
            {
                if (serverArray.Length != 3)
                {
                    return ports;
                }
                address = serverArray[0];
                if (!int.TryParse(serverArray[2], out startPort) || !int.TryParse(serverArray[1], out remotePort))
                {
                    return ports;
                }
            }
            #endregion
            #region 构造表端口
            for (int i = 0; i < meterCount; i++)
            {
                ports.Add(new DeviceParaUnit(arrayPort[0], address, startPort, remotePort, startComno + i * comnoInterval, "Rs485", "Rs485", baudrate, maxTimePerFrame, maxTimePerByte,dllType));
            }
            #endregion
            return ports;
        }
        /// 获取表端口信息
        /// <summary>
        /// 获取表端口信息
        /// </summary>
        /// <returns></returns>
        public DeviceParaUnit[] GetRs485Params()
        {
            return listRs485.ToArray();
        }
        /// 获取读卡器端口信息
        /// <summary>
        /// 获取读卡器端口信息
        /// </summary>
        /// <returns></returns>
        public DeviceParaUnit[] GetRsCardParams()
        {
            return listRsCard.ToArray();
        }
        #endregion

        #endregion
    }
}
