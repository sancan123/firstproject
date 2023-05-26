using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Geny.Packets.In
{

    /// <summary>
    /// 处理返回的QQ数据包
    /// </summary>
    class Geny_ReplyReadQQDataPacket : GenyRecvPacket
    {
        /// <summary>
        /// 已重写，
        /// 提取7个字符作为结果
        /// </summary>
        /// <param name="s"></param>
        protected override void ParseData(string s)
        {
            this.resultData = this.resultData.Substring(0, this.resultData.Length - 1);
        }
    }
}
