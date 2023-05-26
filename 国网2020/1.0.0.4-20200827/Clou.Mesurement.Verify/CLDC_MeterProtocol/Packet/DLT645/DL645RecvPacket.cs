using System;
using System.Collections.Generic;
using System.Text;
using SocketModule.Packet;

namespace MeterProtocol.Packet.DLT645
{
    /// <summary>
    /// DL645接收包
    /// </summary>
   public  class DL645RecvPacket:RecvPacket
    {
        /// <summary>
        /// 表地址
        /// </summary>
        public string Address { get; set; }

        public byte CmdCode { get; set; }

        public byte DataLen { get; set; }

        public bool IsCmdCodeOk
        {
            get
            {

                return (CmdCode & 0x80) == 0x80;
            }
        }

        public override bool ParsePacket(byte[] buf)
        {
            int startpos=0;

            if (buf.Length < 12) return false;
            while(buf[startpos++]==0xFE)
            {
                if (startpos >= buf.Length) break;
            }
            ByteBuffer framebuf = new ByteBuffer(buf);
            framebuf.Position = startpos;
            //地址
            byte[] addr = framebuf.GetByteArray(6);
            Array.Reverse(addr);
            Address = BitConverter.ToString(addr);
            Address = Address.Replace("-", "");
            framebuf.Get();
            //控制码
            CmdCode = framebuf.Get();
            //数据长度
            DataLen = framebuf.Get();
            if (buf.Length < startpos-1 + 12 + DataLen) return false;
            //数据域
            byte[] databuf = framebuf.GetByteArray(DataLen);
            //校验码
            byte chkSum = framebuf.Get();
            byte chkSum2 = Util.Functions.GetChkSum(framebuf.ToByteArray(), startpos-1, framebuf.Position - startpos);
            if (chkSum != chkSum2) return false;
            databuf = Util.Functions.Sub33H(databuf); 
            return ParseBody(databuf);
        }

       /// <summary>
       /// 解析包体数据
       /// </summary>
       /// <param name="buf">数据域，已经做了减33处理，没有做翻转处理</param>
       /// <returns>解析是否成功</returns>
        protected virtual bool ParseBody(byte[] buf)
        {
            return true;
        }
    }
}
