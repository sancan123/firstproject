using CLDC_DeviceDriver.Drivers.Clou.DllPackage;
using CLDC_DeviceDriver.PortFactory;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver
{
    public class CardReaderControl : CLDC_Comm.BaseClass.SingletonBase<CardReaderControl>
    {
        //南网其他厂家设备控制
        public Drivers.Clou.DllPackage.CardReaderControl[] Dev_CardControl = null;

        public void LoadCardControl()
        {
            Dev_CardControl = DeviceFactory.Instance.GetCardControl();
        }
    }
}
