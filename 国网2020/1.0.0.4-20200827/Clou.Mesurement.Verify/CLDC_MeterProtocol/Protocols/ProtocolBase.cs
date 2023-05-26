using System;
using CLDC_DataCore.SocketModule.Packet;

namespace CLDC_MeterProtocol.Protocols
{
    /// <summary>
    /// 电能表基类
    /// </summary>
    public class ProtocolBase : IMeterProtocol
    {
       
      
        /// <summary>
        /// 多功能协议配置对象
        /// </summary>
        protected CLDC_DataCore.Model.DgnProtocol.DgnProtocolInfo protocolInfo = null;

        /// <summary>
        /// 数据端口名称
        /// </summary>
        private string portName = string.Empty;

        /// <summary>
        /// 电表地址
        /// </summary>
        protected string MeterAddress { get; set; }

        /// <summary>
        /// 电表MAC地址
        /// </summary>
        protected string MeterAddress_MAC { get; set; }
      
        /// <summary>
        /// 通讯测试方式
        /// </summary>
        protected int CommTestType { get { return GetProtocolConfigValue("001", 1); } }
        /// <summary>
        /// 写日期方式
        /// </summary>
        protected int WriteDateTimeType { get { return GetProtocolConfigValue("002", 1); } }
        /// <summary>
        /// 读时间方式
        /// </summary>
        protected int ReadDateTimeType { get { return GetProtocolConfigValue("003", 1); } }
        /// <summary>
        /// 清除需量方式
        /// </summary>
        protected int ClearDemandType { get { return GetProtocolConfigValue("004", 1); } }
        /// <summary>
        /// 读需量方式
        /// </summary>
        protected int ReadDemandType { get { return GetProtocolConfigValue("005", 1); } }
        /// <summary>
        /// 读电量方式
        /// </summary>
        protected int ReadEnergyType { get { return GetProtocolConfigValue("006", 1); } }
        /// <summary>
        /// 读时段方式
        /// </summary>
        protected int ReadPeriodTimeType { get { return GetProtocolConfigValue("007", 1); } }
        /// <summary>
        /// 清电量方式
        /// </summary>
        protected int ClearEnergyType { get { return GetProtocolConfigValue("008", 1); } }


        #region 获取多功能协议配置参数:GetProtocolConfigValue(string PramKey, int DefaultValue)
        /// <summary>
        /// 获取多功能协议配置参数
        /// </summary>
        /// <param name="PramKey">要取值的KEY</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns>对应配置项的值，如果没有对应的配置项目则返回默认值</returns>
        protected int GetProtocolConfigValue(string PramKey, int DefaultValue)
        {
            //如果无协议或是参数中不包括主键则返回默认值
            if (protocolInfo == null || protocolInfo.DgnPras == null || !protocolInfo.DgnPras.ContainsKey(PramKey))
            {
                return DefaultValue;
            }
            string[] Arr_Pram = protocolInfo.DgnPras[PramKey].Split('|');
            try
            {
                if (Arr_Pram.Length == 2)
                {
                    return int.Parse(Arr_Pram[0]);
                }
                else
                {
                    return DefaultValue;
                }
            }
            catch
            {
                return DefaultValue;
            }
        }
        #endregion

        #region 数据收发:bool SendData(SendPacket sendPacket, RecvPacket recvPacket)
        /// <summary>
        /// 数据收发
        /// </summary>
        /// <param name="sendPacket">发送数据包</param>
        /// <param name="recvPacket">接收数据包</param>
        /// <returns>发送结果</returns>
        protected bool SendData(SendPacket sendPacket, RecvPacket recvPacket)
        {
            sendPacket.IsNeedReturn = true;
            if (recvPacket == null) sendPacket.IsNeedReturn = false;
            //发送前更新一下波特率
            return MeterProtocolManager.Instance.SendData(portName, sendPacket, recvPacket, protocolInfo.Setting);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sendData"></param>
        /// <param name="recvData"></param>
        /// <returns></returns>
        protected bool SendData(byte[] sendData, ref byte[] recvData)
        {
            Packet.MeterProtocolRecvPacket recvPacket = new CLDC_MeterProtocol.Packet.MeterProtocolRecvPacket();
            Packet.MeterProtocolSendPacket sendPacket = new CLDC_MeterProtocol.Packet.MeterProtocolSendPacket()
            {
                SendData = sendData
            };
            if (recvData == null) sendPacket.IsNeedReturn = false;
            bool ret = SendData(sendPacket, recvPacket);
            recvData = recvPacket.RecvData;
            return ret;
        }
        #endregion

        /// <summary>
        /// 获取电表地址数组
        /// </summary>
        /// <returns></returns>
        protected byte[] GetAddressByte()
        {
            string str_Tmp = "0x";
            if (MeterAddress.Length > 12)
                str_Tmp += MeterAddress.Substring(0, 12);
            else if (MeterAddress.Length == 0)
                str_Tmp += "0";
            else
                str_Tmp += MeterAddress;
            byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt64(str_Tmp, 16));
            return byt_Tmp;
        }

        /// <summary>
        /// 获取电表MAC地址数组
        /// </summary>
        /// <returns></returns>
        protected byte[] GetMACAddressByte()
        {
            string str_Tmp = "0x";
            if (MeterAddress_MAC.Length > 12)
                str_Tmp += MeterAddress_MAC.Substring(0, 12);
            else if (MeterAddress_MAC.Length == 0)
                str_Tmp += "0";
            else
                str_Tmp += MeterAddress_MAC;
            byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt64(str_Tmp, 16));
            return byt_Tmp;
        }

        #region IMeterProtocol 成员

        /// <summary>
        /// 设置协议
        /// </summary>
        /// <param name="protocol">电能表协议</param>
        /// <returns>设置是否成功</returns>
        public bool SetProtocol(CLDC_DataCore.Model.DgnProtocol.DgnProtocolInfo protocol)
        {
            //合并一下密码以及密码等级
            protocolInfo = protocol;

            return true;
        }


        /// <summary>
        /// 设置数据端口名称
        /// </summary>
        /// <param name="szPortName">数据端口名称</param>
        public void SetPortName(string szPortName)
        {
            portName = szPortName;
        }

        /// <summary>
        /// 设置电表地址
        /// </summary>
        /// <param name="szMeterAddress">电表地址</param>
        public void SetMeterAddress(string szMeterAddress)
        {
            MeterAddress = szMeterAddress;
        }

        /// <summary>
        /// 设置电表MAC地址
        /// </summary>
        /// <param name="szMeterAddress_MAC">电表MAC地址</param>
        public void SetMeterAddress_MAC(string szMeterAddress_MAC)
        {
            MeterAddress_MAC = szMeterAddress_MAC;
        }

       

        /// <summary>
        /// 读取电量
        /// </summary>
        /// <param name="energyType"></param>
        /// <param name="tariffType"></param>
        /// <returns></returns>
        public virtual float ReadEnergy(byte energyType, byte tariffType)
        {
            throw new NotImplementedException();
        }

      
        public virtual float ReadData(string str_ID, int int_Len, int int_Dot)
        {
            throw new NotImplementedException();
        }
        public virtual string ReadData(string str_ID, int int_Len, string strItem)
        {
            throw new NotImplementedException();
        }

        public virtual string ReadData(string str_ID, int int_Len)
        {
            throw new NotImplementedException();
        }
        public virtual string ReadData(string str_ID, int int_Len,  out string revData)
        {
            throw new NotImplementedException();
        }
        public virtual string ReadData(string sendData)
        {
            throw new NotImplementedException();
        }


        public virtual bool WriteData(string str_ID, byte[] byt_Value)
        {
            throw new NotImplementedException();
        }

        public virtual bool WriteDataByMac(string str_ID, byte[] byt_Value)
        {
            throw new NotImplementedException();
        }

        public virtual bool WriteData(string str_ID, int int_Len, string str_Value)
        {
            throw new NotImplementedException();
        }


        public virtual bool WriteData(string str_ID, int int_Len, string[] str_Value)
        {
            throw new NotImplementedException();
        }


        public virtual string ReadAddress()
        {
            throw new Exception("The method or operation is not implemented.");
        }


        public virtual bool ClearEnergy(string strEndata)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// 钱包初始化
        /// </summary>
        /// <param name="str_Endata">密文</param>
        /// <returns>是否成功</returns>
        public virtual bool InitPurse(string str_Endata)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public virtual float[] ReadEnergy(byte energyType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual DateTime ReadDateTime()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool WriteDateTime(string str_DateTime)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        /// <summary>
        /// 密钥下装指令
        /// </summary>
        /// <param name="byt_Addr">地址</param>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据域</param>
        /// <param name="bln_Sequela">是否有后续帧</param>
        /// <param name="byt_RevDataF">返回帧数据域</param>
        /// <returns></returns>
        public virtual bool UpdateRemoteEncryptionCommand(byte byt_Cmd, byte[] byt_Data, ref bool bln_Sequela, ref byte[] byt_RevDataF)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 密钥下装指令 交互终端专用
        /// </summary>
        /// <param name="byt_Addr">地址</param>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据域</param>
        /// <param name="bln_Sequela">是否有后续帧</param>
        /// <param name="byt_RevDataF">返回帧数据域</param>
        /// <returns></returns>
        public virtual bool UpdateRemoteEncryptionCommandByTerminal(byte byt_Cmd, byte[] byt_Data, ref bool bln_Sequela, ref byte[] byt_RevDataF)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool ReadPatternWord(int int_type, int int_PatternType, ref string str_PatternWord)
        {
            throw new NotImplementedException();
        }

        public virtual bool WriteFreezeInterval(int int_PatternType, string str_DateTime)
        {
            throw new NotImplementedException();
        }
        public virtual bool FreezeCmd(string str_DateHour)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool ReadSpecialEnergy(int int_type, int int_DLType, int int_Times, ref float[] flt_CurDL)
        {
            throw new NotImplementedException();
        }
        public virtual float[] ReadEnergys(byte energyType, int int_FreezeTimes)
        {
            throw new NotImplementedException();
        }

        public virtual float ReadDemand(byte energyType, byte tariffType)
        {
            throw new NotImplementedException();
        }


        public virtual float[] ReadDemands(byte energyType, int int_FreezeTimes)
        {
            throw new NotImplementedException();
        }

        public virtual float[] ReadDemand(byte energyType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual string[] ReadDataBlock(string str_ID, int int_Len)
        {
            throw new NotImplementedException();
        }
        public virtual float[] ReadDataBlock(string str_ID, int int_Len, int int_Dot)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool BroadcastTime(DateTime broadCaseTime)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool BroadcastTimeByPoint(DateTime broadCaseTime)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool SetBreakRelayTime(int Time)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool WritePatternWord(int int_Type, string data)
        {
            throw new NotImplementedException();
        }

        public virtual bool ReadFreezeTime(int int_FreezeType, ref string str_FreezeTime)
        {
            throw new NotImplementedException();
        }


        public virtual float[] ReadFreezeInterval(int int_Type, ref string str_FTime)
        {
            throw new NotImplementedException();
        }

        public virtual bool ClearEnergy()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string ToString()
        {
            if (this.protocolInfo != null)
                return this.protocolInfo.ToString();
            return this.GetHashCode().ToString();
        }

        public virtual bool SetPulseCom(byte ecp_PulseType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool ClearEventLog(string str_ID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool ClearEventLog(string str_ID, string strEndata)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool ConnectBlueTooth(string strAddress_MAC)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #region IMeterProtocol 成员


        public virtual bool WriteRatesPrice(string str_ID, byte[] byt_Value)
        {
            throw new NotImplementedException();
        }
        public virtual bool ClearDemand(string strEndata)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public virtual bool ClearDemand()
        {
            throw new Exception("The method or operation is not implemented.");
        }

     

        #endregion
    }
}
