using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Setting
{
    internal class Settings : CLDC_DeviceDriver.Setting.SettingBase
    {
        private static Settings setting;

        public Settings()
            : base("Driver_Geny_Standard.ini")
        {
            setting = this;
        }

        public override void InitItemValue()
        {
            base.AddDefault("台体通信_串口号", "1", "台体通信的串口号");

            base.AddDefault("台体通信_串口参数", "19200,n,8,2", "台体通信的通信参数");

            base.AddDefault("485通信_串口号", "2", "与485通信的串口号");

            base.AddDefault("1表位设备号", "1", "设备号：DriverId号码");

            base.AddDefault("脉冲方式_共阴", "0", "0或者2");

            base.AddDefault("标准表名称", "SZ-03A-K6", "标准表的类型名称");

            base.AddDefault("台体操作重试次数", "6", "默认设6");

            base.AddDefault("每次重试时间", "500", "默认设为500");

            base.AddDefault("升源后等待稳定时间", "2", "单位秒");

            base.AddDefault("485通道数", "1", "台体485通道数量");

            base.AddDefault("GPS串口设置", "4800,n,8,1", "格林台体的GPS串口设置");

            base.AddDefault("时基源串口设置", "19200,M,8,1", "时基源串口设置");

            base.AddDefault("时基源串口号", "8", "连接时基源串的设置");

            base.AddDefault("是否直接与时基源连接", "否", "默认 否");

            base.AddDefault("对色标设置", "0", "0 不具有对色标功能,1 具有对色标功能（需要输入输出电流),2 具有对色标功能（由台体自动提供色标电流)");

            Array impluseChannels = Enum.GetValues(typeof(GenyImplusechannels));

            foreach (GenyImplusechannels impluse in impluseChannels)
            {
                this.AddDefault(impluse.ToString(), ((int)(impluse)).ToString(), "请向相关厂家联系，以确定其脉冲通道");
            }
        }

        /// <summary>
        /// 当然台体色标类型
        /// </summary>
        public static GenySeBiaoType SiBiaoType
        {
            get
            {
                try
                {
                    return (GenySeBiaoType)(Enum.Parse(typeof(GenySeBiaoType), setting["对色标设置"], true));
                }
                catch
                {
                    return GenySeBiaoType.NotSupport;
                }
            }
        }


    }
}
