using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Geny.Packets.In
{

    /// <summary>
    /// 读取最大需量，
    /// 返回数据包
    /// </summary>
    class Geny_ReplyReadMaxXLPacket : GenyRecvPacket
    {

        /// <summary>
        /// 最大需量
        /// </summary>
        public double MaxXL
        {
            get;
            set;
        }

        /// <summary>
        /// 已重写
        /// 解析最大需量
        /// </summary>
        /// <param name="s"></param>
        protected override void ParseData(string s)
        {
            // CtBianBi- 电流变比  PtBianBi- 电压变比  JXXS-接线系数
            s = s.Trim();
            this.MaxXL = double.Parse(s);
        }
    }
}
