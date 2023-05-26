using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 设置标准表接线方式 
    /// </summary>
    class Geny_RequestStdMeterConnectionModePacket : Geny_RequestStdMeterPacket
    {
        /// <summary>
        /// 测试类型
        /// </summary>
        Geny_StdK6DTestType TestType
        {
            get;
            set;
        }

        public Geny_RequestStdMeterConnectionModePacket(string stdmeterType, Geny_StdK6DTestType testType)
            : base(stdmeterType)
        {
            this.TestType = testType;
        }

        protected override byte[] GetBody()
        {
            return Encoding.ASCII.GetBytes("Mode" + ((byte)(TestType)).ToString().PadRight(3, ' '));
        }
    }
}
