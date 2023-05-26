using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Geny.Packets.In
{

    /// <summary>
    /// 读取对色标 的结果数据包
    /// </summary>
    class Geny_ReplyDuSheBiaoPacket : GenyRecvPacket
    {
        /// <summary>
        /// 已重写，解析色标数据
        /// </summary>
        /// <param name="s"></param>
        protected override void ParseData(string s)
        {
            this.resultData = this.resultData.Substring(2, 5);
            this.resultData = this.resultData.Trim();
        }
    }
}
