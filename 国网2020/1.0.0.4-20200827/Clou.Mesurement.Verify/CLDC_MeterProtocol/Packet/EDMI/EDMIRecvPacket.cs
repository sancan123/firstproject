using System;
using System.Collections.Generic;
using System.Text;

namespace MeterProtocol.Packet.EDMI
{
   public class EDMIRecvPacket : SocketModule.Packet.RecvPacket
    {
        const byte packetHeader = 0x02;
        const byte packetFooter = 0x03;

        public EDMIRecvPacket()
        {
        }
        /// <summary>
        /// 数据域,每一段添加一次
        /// </summary>
        public List<byte[]> Data { get; set; }
        /// <summary>
        /// 主控制码
        /// </summary>
        public EDMICmdCode CmdCode { get; set; }
        /// <summary>
        /// 次控制码
        /// </summary>
        public byte SubCmd { get; set; }

        public byte[] RandData { get;  set; }

        /// <summary>
        /// 解析数据包
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public override bool ParsePacket(byte[] buf)
        {
            int startPos = Array.IndexOf(buf,packetHeader);
            int endPos = Array.IndexOf(buf,packetFooter);
            if (startPos == -1 || endPos == -1)
            {
                return false;
            }
            byte[] byt_Tmp = new byte[endPos - startPos + 1];
            Array.Copy(buf, startPos, byt_Tmp, 0, endPos - startPos + 1);

            byt_Tmp = ConvertCommData(byt_Tmp);

            byte[] byt_CRC = GetCRC(byt_Tmp, 0, byt_Tmp.Length - 3);
            if (byt_CRC[0] == byt_Tmp[byt_Tmp.Length - 3]                     //CRC校验比较
                && byt_CRC[1] == byt_Tmp[byt_Tmp.Length - 2])
            {
                //byt_RevFrame = byt_Tmp;
                ByteBuffer frame = new ByteBuffer(byt_Tmp);
                frame.Get();//0x02
                CmdCode = (EDMICmdCode)frame.Get();
                frame.GetByteArray(4);//4个字节的源地址,这儿没有作校验
                byte[] arrAdd = frame.GetByteArray(4);//4个字节的表地址
                Address = BitConverter.ToInt32(arrAdd, 0).ToString();//电表地址
                byte[] arrRnd = frame.GetByteArray(2);
                if (arrRnd[0] != RandData[0] || arrRnd[1] != RandData[1])
                    return false;
                //RandData = arrRnd;                                  //包序号
                SubCmd = frame.Get();                               //功能码
                byte[] databuf = frame.GetByteArray(frame.Length - frame.Position - 2);//帧总长度-固定位置长度-2字节CRC校验
                return ParseBody(databuf);
            }
            return false;
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

        /// <summary>
        /// 电表地址
        /// </summary>
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// 转换特殊字符(起始符和结束符除外),02=>1042,03=>1043,10=>1050,11=>1051,13=>1053
        /// </summary>
        /// <param name="byt_Frame">帧数据</param>
        /// <returns></returns>
        private byte[] ConvertSpecialData(byte[] byt_Frame)
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            for (int int_Inc = 0; int_Inc < byt_Frame.Length; int_Inc++)
            {
                if (int_Inc > 0 && int_Inc < byt_Frame.Length - 1)
                {
                    if (byt_Frame[int_Inc] == 0x02
                        || byt_Frame[int_Inc] == 0x03
                        || byt_Frame[int_Inc] == 0x10
                        || byt_Frame[int_Inc] == 0x11
                        || byt_Frame[int_Inc] == 0x13)
                    {
                        //Array.Resize(ref  byt_Tmp, byt_Tmp.Length + 1);
                        buf.Put(0x10);
                        buf.Put((byte)(byt_Frame[int_Inc] + 0x40));
                        //byt_Tmp[int_Go] = 0x10;
                        //byt_Tmp[int_Go + 1] = (byte)(byt_Frame[int_Inc] + 0x40);
                        //int_Go += 2;
                    }
                    else
                    {
                        buf.Put(byt_Frame[int_Inc]);
                        //byt_Tmp[int_Go] = byt_Frame[int_Inc];
                        //int_Go++;
                    }
                }
                else
                {
                    buf.Put(byt_Frame[int_Inc]);
                }
            }

            return buf.ToByteArray();
        }


        /// <summary>
        /// 换特殊字符转换成普通字符 02=1042,03=1043,10=1050,11=1051,13=1053
        /// </summary>
        /// <param name="byt_Frame"></param>
        /// <returns></returns>
        private byte[] ConvertCommData(byte[] byt_Frame)
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            for (int int_Inc = 0; int_Inc < byt_Frame.Length; int_Inc++)
            {
                if (int_Inc > 0 && int_Inc < byt_Frame.Length - 1)
                {
                    if (byt_Frame[int_Inc] == 0x10)
                    {
                        //Array.Resize(ref  byt_Tmp, byt_Tmp.Length - 1);
                        //byt_Tmp[int_Go] = ;
                        //int_Inc += 1;
                        //int_Go++;
                        buf.Put((byte)(byt_Frame[int_Inc + 1] - 0x40));
                        int_Inc++;//二字节合成一字节
                    }
                    else
                    {
                        buf.Put(byt_Frame[int_Inc]);
                        // byt_Tmp[int_Go] = byt_Frame[int_Inc];
                        // int_Go++;
                    }
                }
                else
                {
                    buf.Put(byt_Frame[int_Inc]);
                }
            }
            return buf.ToByteArray();
        }

        /// <summary>
        /// 计算CRC校验码
        /// </summary>
        /// <param name="byt_Frame">帧值</param>
        /// <param name="int_Start">起始位</param>
        /// <param name="int_Len">数据长度</param>
        /// <returns></returns>
        private byte[] GetCRC(byte[] byt_Frame, int int_Start, int int_Len)
        {
            UInt16[] uht_Table = new UInt16[256];
            IniCRC(ref uht_Table);
            UInt16 uht_CRC = 0;
            for (int int_Inc = 0; int_Inc < int_Len; int_Inc++)
                uht_CRC = (UInt16)((uht_CRC << 8) ^ (uht_Table[(uht_CRC >> 8) ^ byt_Frame[int_Start + int_Inc]]));
            byte[] byt_Tmp = BitConverter.GetBytes(uht_CRC);
            Array.Reverse(byt_Tmp);
            return byt_Tmp;
        }

        /// <summary>
        /// 初始CRC计算值
        /// </summary>
        /// <param name="uht_Table"></param>
        private void IniCRC(ref UInt16[] uht_Table)
        {
            UInt16 uht_Tmp;
            UInt16 uht_Crc;
            for (int int_Inc = 0; int_Inc < 256; int_Inc++)
            {
                uht_Tmp = (ushort)(int_Inc << 8);
                uht_Crc = 0;
                for (int int_Inb = 0; int_Inb < 8; int_Inb++)
                {
                    if (((uht_Crc ^ uht_Tmp) & 0x8000) != 0)
                        uht_Crc = (ushort)((uht_Crc << 1) ^ 0x1021);
                    else
                        uht_Crc <<= 1;
                    uht_Tmp <<= 1;
                }
                uht_Table[int_Inc] = uht_Crc;
            }
        }


    }
}
