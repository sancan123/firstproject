using CLDC_DeviceDriver.Drivers.Clou.DllPackage;
using CLDC_DeviceDriver.PortFactory;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver
{
   public class DeviceControl : CLDC_Comm.BaseClass.SingletonBase<DeviceControl>
    {
       //南网其他厂家设备控制
       public Drivers.Clou.DllPackage. DeviceControl[] Dev_DeviceControl = null;

       public void LoadDeviceControl()
       {
           Dev_DeviceControl = DeviceFactory.Instance.GetDeviceControl();
       }
    }
}
