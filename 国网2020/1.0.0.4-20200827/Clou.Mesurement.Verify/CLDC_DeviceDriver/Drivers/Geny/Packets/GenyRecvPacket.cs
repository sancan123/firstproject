using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.SocketModule.Packet;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets
{
    /// <summary>
    /// 格宁电气接收包基类
    /// </summary>
    internal class GenyRecvPacket : RecvPacket
    {
        /// <summary>
        /// 返回确认
        /// </summary>
        protected const string WC_OK = "WC OK";

        protected const string OK = "OK";
        /// <summary>
        /// 无此命令
        /// </summary>
        protected const string NOCOMMAND = "NOCOMMAND";

        /// <summary>
        /// crc校验错误
        /// </summary>
        protected const string CRCERROR = "CRCERROR";

        /// <summary>
        /// 忙数据
        /// </summary>
        protected const string Busy = "Busy";

        protected const string END = "END";

        /// <summary>
        /// 发送方设备地址
        /// </summary>
        protected byte SendID { get; set; }
        /// <summary>
        /// 控制码
        /// </summary>
        protected byte CmdCode { get; set; }
        /// <summary>
        /// 数据域长度
        /// </summary>
        protected byte DataLength { get; set; }

        protected string resultData;

        /// <summary>
        /// 获取结果数据
        /// </summary>
        public string ResultData
        {
            get
            {
                return this.resultData;
            }
        }

        /// <summary>
        /// 使用 数据 与定义的 返回结果进行比较
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static RecvResult MatchValue(string data)
        {
            RecvResult result = RecvResult.Unknow;

            if (data.Length == OK.Length || data.Length == WC_OK.Length || data.Length == NOCOMMAND.Length || data.Length == CRCERROR.Length || data.Length == Busy.Length || data.Length == END.Length)
            {

                if (data.Equals(WC_OK))
                {
                    result = RecvResult.OK;
                }
                else if (data.Equals(NOCOMMAND))
                {
                    result = RecvResult.DataError;
                }
                else if (data.Equals(CRCERROR))
                {
                    result = RecvResult.CSError;
                }
                else if (data.Equals(Busy))
                {
                    result = RecvResult.OK;
                }
                else if (data.Equals(END))
                {
                    result = RecvResult.OK;
                }
                else if (data.Equals(OK))
                {
                    result = RecvResult.OK;
                }
                else
                {
                    result = RecvResult.Unknow;
                }
            }
            return result;
        }

        /// <summary>
        /// 解析帧
        /// 将解析出 发送方地址，功能码，校验码，数据域
        /// </summary>
        /// <param name="buf">接收到的缓冲区数据</param>
        /// <returns>解析是否成功</returns>
        public override bool ParsePacket(byte[] value)
        {
            //第一步，验证帧结构.帧长度最少为5字节
            if (value.Length < 6)
            {
                this.ReciveResult = RecvResult.FrameError;
                return false;
            }
            //第二步,验证校验码
            ByteBuffer buf = new ByteBuffer(value);
            SendID = buf.Get();                     //解析设备地址
            CmdCode = buf.Get();
            DataLength = buf.Get();
            //重新校验一下帧长度，有可能下面返回的数据帧长度不正确
            int logicLength = value.Length - 3 - 2;
            if (DataLength != logicLength) DataLength = (byte)logicLength;
            if (value.Length < DataLength + 3)
            {
                this.ReciveResult = RecvResult.FrameError;
                return false;
            }
            buf.Position = DataLength + 3;
            //ushort frameChk = buf.GetUShort_S();
            //ushort dataChk = CRC.GetCrcValue(value, 0, 3 + DataLength);
            //if (frameChk != dataChk)
            //{
            //    this.ReciveResult = RecvResult.CSError;
            //    return false;
            //}

            //第三步，解析数据
            byte[] data = new byte[DataLength];
            Array.Copy(value, 3, data, 0, DataLength);
            ParseBody(data);
            if (this.ReciveResult == RecvResult.None)
            {
                this.ReciveResult = RecvResult.OK;
            }
            return true;
        }

        /// <summary>
        /// 解析数据域
        /// </summary>
        /// <param name="data">数据域</param>
        protected void ParseBody(byte[] data)
        {
            this.resultData = Encoding.ASCII.GetString(data);
            this.ReciveResult = MatchValue(this.resultData);
            if (this.ReciveResult != RecvResult.Unknow)
            {
                return;
            }
            this.ParseData(this.resultData);
        }

        /// <summary>
        /// 转换数据
        /// 如果返回数据不在 定义的三种，则需要重写该方法
        /// </summary>
        /// <param name="s"></param>
        protected virtual void ParseData(string s)
        {
            //
        }
    }
}
