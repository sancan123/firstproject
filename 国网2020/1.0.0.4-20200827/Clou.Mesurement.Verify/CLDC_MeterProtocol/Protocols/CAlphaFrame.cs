using System;

namespace CLDC_MeterProtocol.Protocols
{
    /// <summary>
    /// Alpha协议帧
    /// </summary>
    public class CAlphaFrame
    {

        #region -----------关键字，常数定义-------------------------
        /// <summary>
        /// 帧起始符
        /// </summary>
        public const byte CST_BYT_STX = 0x02;           //帧超始符

        /// <summary>
        /// 操作成功
        /// </summary>
        public const byte CST_BYT_ACK = 0x00;           //操作成功

        //-----------NAK关键字，失败代码---------------------------

        /// <summary>
        /// 校验错误
        /// </summary>
        public const byte CST_BYT_NAK_CRC = 0x01;       //CRC校验错误

        /// <summary>
        /// 通信锁定错误
        /// </summary>
        public const byte CST_BYT_NAK_CMM = 0x02;       //通信锁定错误

        /// <summary>
        /// 不合法命令
        /// </summary>
        public const byte CST_BYT_NAK_CMD = 0x03;       //不合法命令

        /// <summary>
        /// 帧错误
        /// </summary>
        public const byte CST_BYT_NAK_FRM = 0x04;       //帧错误

        /// <summary>
        /// 超时
        /// </summary>
        public const byte CST_BYT_NAK_OTM = 0x05;       //超时

        /// <summary>
        /// 无效密码
        /// </summary>
        public const byte CST_BYT_NAK_PSW = 0x06;       //无效密码

        /// <summary>
        /// 没有回答
        /// </summary>
        public const byte CST_BYT_NAK_NAS = 0x07;       //没有回答

        /// <summary>
        /// C模式关闭
        /// </summary>
        public const byte CST_BYT_NAK_CCL = 0x0E;       //C模式关闭

        //----------CB关键字-----------------------------
        /// <summary>
        /// 结束对话
        /// </summary>
        public const byte CST_BYT_CB_END = 0x80;        //结束对话

        /// <summary>
        /// 续读
        /// </summary>
        public const byte CST_BYT_CB_CNE = 0x81;        //续读

        /// <summary>
        /// 再发上一打包
        /// </summary>
        public const byte CST_BYT_CB_AGN = 0x82;        //再发上一打包

        /// <summary>
        /// 接受控制
        /// </summary>
        public const byte CST_BYT_CB_ACE = 0x84;        //接受控制

        /// <summary>
        /// 与数据有关操作
        /// </summary>
        public const byte CST_BYT_CB_DAT = 0x18;        //与数据有关操作

        /// <summary>
        /// 与数据有关操作
        /// </summary>
        public const byte CST_BYT_CB_NDAT = 0x08;        //与数据无关操作

    
        /// <summary>
        /// 类操作
        /// </summary>
        public const byte CST_BYT_CB_CAS = 0x05;        //类操作

        /// <summary>
        /// 密码检定
        /// </summary>
        public const byte CST_BYT_FUN_PSW = 0x01;        //密码检定

        /// <summary>
        /// 设置时钟
        /// </summary>
        public const byte CST_BYT_FUN_SET = 0x02;        //设置时钟

        /// <summary>
        /// 你是谁
        /// </summary>
        public const byte CST_BYT_FUN_WHO = 0x06;        //你是谁

        /// <summary>
        /// 回叫命令
        /// </summary>
        public const byte CST_BYT_FUN_BCK = 0x08;        //回叫命令

        /// <summary>
        /// 打包尺寸(帧数据长度)
        /// </summary>
        public const byte CST_BYT_FUN_PAK = 0x09;        //打包尺寸(帧数据长度)

        /// <summary>
        /// 写擦除填充缓冲区
        /// </summary>
        public const byte CST_BYT_FUN_ERA = 0x0A;        //写擦除填充缓冲区

        /// <summary>
        /// 时钟同步
        /// </summary>
        public const byte CST_BYT_FUN_SYN = 0x0C;        //时钟同步


        /// <summary>
        /// F2 通信超时门槛值 数据为1字节（6~255，步长为 0.5秒）
        /// </summary>
        public const byte CST_BYT_FUN_COST = 0xF2;        //通信超时门槛值



        #endregion

        public CAlphaFrame()
        {




        }

        /// <summary>
        /// 组帧，短信息格式
        /// </summary>
        /// <returns></returns>
        public byte[] OrgFrame(byte byt_Cmd)
        {
            byte[] byt_Frame = new byte[4];
            byt_Frame[0]=CST_BYT_STX ;
            byt_Frame[1] = byt_Cmd;
            Array.Copy(GetCRC(byt_Frame, 0, 2), 0, byt_Frame, 2, 2);
            return byt_Frame;
        }


        /// <summary>
        /// 组帧，短信息格式
        /// </summary>
        /// <returns></returns>
        public byte[] OrgFrame(byte byt_Cmd, byte[] byt_Data)
        {
            int int_Len = 4 + byt_Data.Length;
            byte[] byt_Frame = new byte[int_Len];
            byt_Frame[0] = CST_BYT_STX;
            byt_Frame[1] = byt_Cmd;
            Array.Copy(byt_Data, 0, byt_Frame, 2, byt_Data.Length);
            byte[] byt_Tmp = GetCRC(byt_Frame, 0, byt_Data.Length + 2);
            Array.Copy(byt_Tmp, 0, byt_Frame, byt_Data.Length + 2, 2);
            return byt_Frame;
        }


        /// <summary>
        ///  组帧，长信息格式
        /// </summary>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_FunCmd">功能命令</param>
        /// <param name="byt_FillData">填充数</param>
        /// <param name="byt_Data">数据</param>
        /// <returns></returns>
        public byte[] OrgFrame(byte byt_Cmd, byte byt_FunCmd, byte[] byt_Data)
        {

            //起始符 命令 功能码 填充数 数据长度 数据 CRC高字节 CRC低字节
            //02 18 06 00 01 01 89BE

            int int_Len = 7 + byt_Data.Length;
            byte[] byt_Frame = new byte[int_Len];
            byt_Frame[0] = CST_BYT_STX;
            byt_Frame[1] = byt_Cmd;
            byt_Frame[2] = byt_FunCmd;
            byt_Frame[3] = 0x00;
            byt_Frame[4] = (byte)byt_Data.Length;
            Array.Copy(byt_Data, 0, byt_Frame, 5, byt_Data.Length);

            byte[] byt_Tmp = GetCRC(byt_Frame, 0, byt_Data.Length + 5);

            Array.Copy(byt_Tmp, 0, byt_Frame, byt_Data.Length + 5, 2);
            return byt_Frame;
        }

        /// <summary>
        ///  组帧（一般用于05读类）
        /// </summary>
        /// <param name="byt_Cmd"></param>
        /// <param name="byt_FillData"></param>
        /// <param name="byt_ClassID"></param>
        /// <param name="byt_AddrOffset"></param>
        /// <param name="byt_Data"></param>
        /// <returns></returns>
        public byte[] OrgFrame(byte byt_Cmd, byte byt_ClassID)
        {
            //起始符 05 填充数 数据长度高字节 数据长度低字节 地址偏移高字节 地址偏移低字节 类 CRC高字节 CRC低字节

            byte[] byt_Frame = new byte[10];
            byt_Frame[0] = CST_BYT_STX;
            byt_Frame[1] = byt_Cmd;
            byt_Frame[2] = 0x00;
            byt_Frame[3] = 0x00;        //数据长度高、低字节只用于类数据的读取，如果为零，则表示长度缺省
            byt_Frame[4] = 0x00;
            byt_Frame[5] = 0x00;
            byt_Frame[6] = 0x00;
            byt_Frame[7] = byt_ClassID;
            byte[] byt_Tmp = GetCRC(byt_Frame, 0, 8);
            Array.Copy(byt_Tmp, 0, byt_Frame, 8, 2);
            return byt_Frame;

        }


        /// <summary>
        /// 检测帧合格性
        /// </summary>
        /// <param name="byt_Frame">被检测帧</param>
        /// <param name="byt_RevFrame">返回合格帧</param>
        /// <returns></returns>
        public bool CheckFrame(byte[] byt_Frame, ref byte[] byt_RevFrame)
        {
            int int_STXStart = Array.IndexOf(byt_Frame, CST_BYT_STX);
            if (int_STXStart < 0)       //没有帧起始码
                return false;

            int int_Len = byt_Frame.Length - int_STXStart;

            byte[] byt_TmpCRC = GetCRC(byt_Frame, int_STXStart, int_Len - 2);

            if (byt_TmpCRC[0] == byt_Frame[byt_Frame.Length - 2]
                && byt_TmpCRC[1] == byt_Frame[byt_Frame.Length - 1])
            {
                byt_RevFrame = new byte[int_Len];
                Array.Copy(byt_Frame, int_STXStart, byt_RevFrame, 0, int_Len);
                return true;
            }
            else
                return false;
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
