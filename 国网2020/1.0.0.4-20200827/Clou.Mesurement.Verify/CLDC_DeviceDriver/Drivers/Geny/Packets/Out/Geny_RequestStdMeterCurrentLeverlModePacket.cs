using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 设置标准表 电流量程
    /// </summary>
    class Geny_RequestStdMeterCurrentLeverlModePacket : Geny_RequestStdMeterPacket
    {

        public GenyStdMeterCurrentLevel CurrentLevel
        {
            get;
            set;
        }

        public Geny_RequestStdMeterCurrentLeverlModePacket(string stdmeterType, GenyStdMeterCurrentLevel currentLevel)
            : base(stdmeterType)
        {
            this.CurrentLevel = currentLevel;
        }


        protected override byte[] GetBody()
        {
            string va = null;

            if (CurrentLevel == GenyStdMeterCurrentLevel.Level_1A)
            {
                va = "IR1A";
            }
            else if (CurrentLevel == GenyStdMeterCurrentLevel.Level_10A)
            {
                va = "IR10A";
            }
            else if (CurrentLevel == GenyStdMeterCurrentLevel.Level_100A)
            {
                va = "IR100A";
            }
            else
            {
                va = "IR1A";
            }

            return Encoding.ASCII.GetBytes(va.PadRight(7, ' '));
        }
    }
}
