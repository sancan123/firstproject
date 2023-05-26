using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80_2036.Setting
{
    internal class Setting : DeviceDriver.Setting.SettingBase
    {
        public Setting()
            : base("Driver_V80_2036.ini")
        {
        }
        public override void InitItemValue()
        {
            base.AddDefault("2018Port", "1003","");
        }
    }
}
