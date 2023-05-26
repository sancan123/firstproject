using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In
{
    /// <summary>
    /// 读取功能数据1回复包
    /// </summary>
    class RequestReadVerifyDataReplyPacket : ClouRecvPacket_CLT11
    {

        /// <summary>
        /// 误差数据结构体
        /// </summary>
        public struct ErrorData
        {
            /// <summary>
            /// 误差数据
            /// </summary>
            public string Error;
            /// <summary>
            /// 误差次数
            /// </summary>
            public int ErrorTimes;
        }

        private int m_StartPos = 0;

        public int StartPos
        {
            get { return m_StartPos; }
            set { m_StartPos = value; }
        }

        /// <summary>
        /// 误差数据
        /// Byte1-3 数据 byte4 指数 也就是Inte3
        /// Byte5 次数
        /// </summary>
        public ErrorData[] BasicError = new ErrorData[0];

        /// <summary>
        /// 解析误差数据
        /// </summary>
        /// <param name="byt_Data">数据</param>
        /// <returns></returns>
        private string ExplainWCData(byte[] byt_Data)
        {
            byte[] byt_Tmp = new byte[4];
            Array.Copy(byt_Data, 0, byt_Tmp, 0, 3);
            int int_Single = byt_Tmp[2] & 0x80;
            int int_Data = BitConverter.ToInt32(byt_Tmp, 0);
            if (int_Single == 0x80)
            {
                int_Data = 0 - int_Data;
                int_Data ^= 0xffffff;
            }


            int int_Dot = byt_Data[3] - 256;

            Double dbl_Data = int_Data * Math.Pow(10, int_Dot);

            return dbl_Data.ToString("#0.00000");
        }

        protected override void ParseBody(byte[] data)
        {
            if (data[0] == 0x55)
            {
                ByteBuffer buf = new ByteBuffer();
                buf.Initialize();

                buf.Get();
                buf.GetUShort();
                ushort start = buf.GetUShort_S();
                StartPos = (int)start;
                byte len = buf.Get();
                len = (byte)(len / 5);
                BasicError = new ErrorData[len];
                for (int i = 0; i < len; i++)
                {
                    ErrorData err = new ErrorData();
                    err.Error = ExplainWCData(buf.GetByteArray(4));
                    err.ErrorTimes = buf.Get();
                    BasicError[i] = err;
                }

                ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.OK;
            }
            else if (data[0] == 0x33)
            {
                ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.DataError;
            }
            else
            {
                ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.Unknow;
            }
        }

        public override string GetPacketName()
        {
            return "RequestReadVerifyDataReplyPacket";
        }
    }
}
