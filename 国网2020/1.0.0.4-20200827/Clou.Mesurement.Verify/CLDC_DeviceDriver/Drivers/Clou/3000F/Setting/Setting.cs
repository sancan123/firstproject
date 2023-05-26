using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Setting
{
    internal class Setting:CLDC_DeviceDriver.Setting.SettingBase
    {
        public Setting()
            : base("Driver_3000F.ini")
        { 
            
        }

        public override void InitItemValue()
        {
            //2018-1服务器IP
            AddDefault("ServerIP", "193.168.18.1", "串口服务器IP");
            AddDefault("ServerPort", "31", "2036通讯通道号");
            //AddDefault("Meter", "30", "标准表端口");
            //AddDefault("MeterCom", "串口", "标准表端口类型,串口:串口通讯，其它为2018通讯");
            //AddDefault("MeterSetting", "9600,n,8,1", "标准表通讯参数");
            //AddDefault("Cl191", "32", "标准时基源端口");
            //AddDefault("Cl191Setting", "2400,n,8,1", "标准时基源通讯参数");
            //AddDefault("Power", "33", "功率源端口");
            //AddDefault("PowerSetting", "9600,n,8,1", "功率源通讯参数");
            //AddDefault("Cl188", "25,26", "误差板端口,如果有多路按顺序用,隔开");
            //AddDefault("Cl188Setting", "19200,n,8,1", "误差板通讯参数");
            AddDefault("Rs485", "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16", "485端口，多路按顺序用,隔开");
        }
    }
}
