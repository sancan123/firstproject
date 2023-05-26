using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.SocketModule.Packet;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets
{
    /// <summary>
    /// 格宁电气发送数据包基类
    /// 默认需要回复
    /// 帧结构：地址(1)+功能码(1)+数据长度(1)+数据域+CRC校验(2)
    /// </summary>
    internal abstract class GenySendPacket : SendPacket
    {
        /// <summary>
        /// 设备地址
        /// </summary>
        public byte SendID { get; set; }

        /// <summary>
        /// 功能码
        /// </summary>
        public byte CmdCode { get; set; }


        /// <summary>
        /// 生成一个新实例
        /// </summary>
        public GenySendPacket()
        {
            this.IsNeedReturn = true;
        }

        /// <summary>
        /// 生成一个新实例
        /// </summary>
        /// <param name="sendID">目地 地址</param>
        /// <param name="cmdCode">功能码</param>
        public GenySendPacket(byte sendID, byte cmdCode)
            : this()
        {
            this.SendID = sendID;
            this.CmdCode = cmdCode;
        }

        /// <summary>
        /// 返回组成帧的数据，
        /// 子类通常不需要重写
        /// </summary>
        /// <returns></returns>
        public override byte[] GetPacketData()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(SendID);                        //地址域
            buf.Put(CmdCode);                       //控制码
            byte[] body = GetBody();
            //buf.Put(20);
            buf.Put((byte)body.Length);                   //长度
            buf.Put(body);                          //数据域
            ushort crcValue = CRC.GetCRCValue(buf.ToByteArray());
            buf.PutUShort_S(crcValue);                //CRC校验
            return buf.ToByteArray();
        }

        /// <summary>
        /// 数据域填充
        /// 返回数据
        /// </summary>
        /// <returns>数据域内容，只包含帧结构中数据域部分，不包括帧头、帧尾、数据长度、CRC校验</returns>
        protected abstract byte[] GetBody();

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <param name="v"></param>
        public static void SetArrayValue(byte[] value, byte v, int offset, int len)
        {
            for (int i = 0; i < len; i++)
            {
                value[i + offset] = v;
            }
        }

        /// <summary>
        /// 设置值,默认为 32
        /// 也就是 空格 的ascii 值
        /// </summary>
        /// <param name="value"></param>
        public static void SetArrayValue(byte[] value)
        {
            SetArrayValue(value, (byte)' ', 0, value.Length);
        }


        /// <summary>
        /// 获取 设备地址
        /// </summary>
        /// <param name="xiang"></param>
        /// <returns></returns>
        public static byte GetDriverId(PhaseType phase)
        {
            switch (phase)
            {
                case PhaseType.A: return 220;
                case PhaseType.B: return 221;
                case PhaseType.C: return 222;
                default: return 0;
            }
        }
    }
}
