using CLDC_DataCore.SocketModule.Packet;
using System;

namespace CLDC_SafeFileProtocol.Protocols
{
    public class CtrlProtocolBase : ICardControlProtocol
    {
        /// <summary>
        /// 数据端口名称
        /// </summary>
        private string portName = string.Empty;
        private string strSetting = "9600,e,8,1";

        #region

        public void SetPortName(string szPortName)
        {
            portName = szPortName;
        }

        public void SetPortSetting(string szSetting)
        {
            strSetting = szSetting;
        }
        public virtual int ResetDevice(int meterIndex)
        {
            throw new NotImplementedException();
        }

        public virtual int ReadParamPresetCard(out string fileParamData, out string fileMoneyData, out string filePrice1Data, out string filePrice2Data)
        {
            throw new NotImplementedException();
        }

        public virtual int ReadParamPresetCardNum(out string cardNum)
        {
            throw new NotImplementedException();
        }

        public virtual int ReadUserCard( out string fileParamData, out string fileMoneyData, out string filePrice1Data, out string filePrice2Data, out string fileReplyData, out string enfileControlData)
        {
            throw new NotImplementedException();
        }

        public virtual int ReadUserCardNum(out string cardNum)
        {
            throw new NotImplementedException();
        }

        public virtual int WriteParamPresetCard(string fileParam, string fileMoney, string filePrice1, string filePrice2)
        {
            throw new NotImplementedException();
        }

        public virtual int WriteUserCard(string fileParam, string fileMoney, string filePrice1, string filePrice2, string fileReply,string fileControl)
        {
            throw new NotImplementedException();
        }

        public virtual int ReadUserCardMAC(string rand,out string fileParam,out string fileMoney,out string filePrice1,out string filePrice2, out string fileControl )
        {

            throw new NotImplementedException();
        }

        public virtual int WriteUserCardReply(string fileReply)
        {
            throw new NotImplementedException();
        }

        public virtual int ReadTerminalToCardInfo(string CardNo, string strRand, out string parainfo)
        {
            throw new NotImplementedException();
        }

        public virtual int WriteUserCardReplyPrice2(string fileReplyPrice2)
        {
            throw new NotImplementedException();
        }

        public virtual int ReadTerminalToCardPrice1(string CardNo, string strRand, out string price1)
        {
            throw new NotImplementedException();
        }

        public virtual int ReadTerminalToCardPrice2(string CardNo, string strRand, out string price2)
        {
            throw new NotImplementedException();
        }


        #endregion

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
            return CardControlManager.Instance.SendData(portName, sendPacket, recvPacket, strSetting);
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sendData"></param>
        /// <param name="recvData"></param>
        /// <returns></returns>
        protected bool SendData(byte[] sendData, ref byte[] recvData)
        {
            PacketRecv recvPacket = new PacketRecv();
            PacketSend sendPacket = new PacketSend()
            {
                SendData = sendData
            };
            //if (recvData == null) sendPacket.IsNeedReturn = false;
            bool ret = SendData(sendPacket, recvPacket);
            recvData = recvPacket.RecvData;
            return ret;
        }

        
    }
}
