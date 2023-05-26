using System;
using System.Collections.Generic;
using System.Text;
using DeviceDriver.Drivers.Geny.Packets.Out;
using Comm.Enum;

namespace DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 设置标准表 接线，有无功，档位
    /// 参数包，无返回
    /// </summary>
    internal class Geny_RequestStdMeterWireModePacket : GenySendPacket
    {
        /// <summary>
        /// 测量方式
        /// </summary>
        public Geny_StdK6DTestType CLFS
        {
            get;
            set;
        }

        /// <summary>
        /// 有功无功类型
        /// </summary>
        public GenyActiveType ActiveType
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或者设置电流档位
        /// </summary>
        public GenyCurrentLevel CurrentLevel
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="clfs">测量方式</param>
        /// <param name="activeType"></param>
        /// <param name="currentLevel"></param>
        public Geny_RequestStdMeterWireModePacket(Geny_StdK6DTestType clfs, GenyActiveType activeType, GenyCurrentLevel currentLevel)
        {
            this.CLFS = clfs;
            this.ActiveType = activeType;
            this.CurrentLevel = currentLevel;
            this.IsNeedReturn = false;
        }

        /// <summary>
        /// 已重写，返回设置的数据
        /// 
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            List<byte> buf = new List<byte>();

            buf.Add((byte)('S'));
            buf.Add((byte)('e'));
            buf.Add((byte)('t'));
            buf.Add((byte)(':'));

            buf.Add((byte)(this.CLFS));
            buf.Add((byte)(ActiveType));
            buf.Add((byte)(this.CurrentLevel));
            return buf.ToArray();
        }
    }
}
