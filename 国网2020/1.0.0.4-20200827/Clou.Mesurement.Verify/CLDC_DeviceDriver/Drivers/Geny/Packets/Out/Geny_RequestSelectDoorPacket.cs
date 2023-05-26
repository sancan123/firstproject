using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 打开设备通道请求包
    /// 误差板 1
    /// 控源 通道 2
    /// GPS 通道 4
    /// 标准表 7
    /// </summary>
    internal class Geny_RequestSelectDoorPacket : GenySendPacket
    {
        public GenyDoorType DoorType
        {
            get
            {
                return (GenyDoorType)(base.CmdCode);
            }
            set
            {
                this.CmdCode = (byte)value;
            }
        }


        public Geny_RequestSelectDoorPacket()
        { }

        /// <summary>
        /// 通道选择包
        /// </summary>
        /// <param name="doorType">要选择的通道，该数据其实是作为指令码发送</param>
        public Geny_RequestSelectDoorPacket(GenyDoorType doorType)
            : base(230, (byte)doorType)
        {
        }

        /// <summary>
        /// 已重写，返回选通道数据
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            return new byte[2] { 0xAA, 55 };
        }
    }
}
