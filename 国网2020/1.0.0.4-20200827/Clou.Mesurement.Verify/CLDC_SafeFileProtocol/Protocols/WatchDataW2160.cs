using CLDC_DataCore.Const;
using CLDC_SafeFileProtocol.CardAPI;
using System;
using System.Text;
using CLDC_DeviceDriver;

namespace CLDC_SafeFileProtocol.Protocols
{
    public class WatchDataW2160 : CtrlProtocolBase
    {
        Tools.DataConvert DataCvt = new Tools.DataConvert();
        /// <summary>
        /// 复位，每次换卡、重新上电要复位
        /// </summary>
        /// <returns></returns>
        public override int ResetDevice(int meterIndex)
        {
            int intRst = 1;
            int retrytime = 2;
            for (int i = 0; i < retrytime; i++)
            {
                //if (CLDC_DataCore.Const.GlobalUnit.g_Dev_CommunType == CLDC_Comm.Enum.Cus_CommunType.南网通讯DLL)
                {
                    if (CardReaderControl.Instance.Dev_CardControl != null && CardReaderControl.Instance.Dev_CardControl.Length > 0)
                    {
                        intRst = CardReaderControl.Instance.Dev_CardControl[0].ResetCard(meterIndex);
                        if (intRst == 0)
                            return intRst;
                    }
                }
            }
            return intRst;
        }
        /// <summary>
        /// 读用户卡卡号
        /// </summary>
        /// <param name="cardNum"></param>
        /// <returns></returns>
        public override int ReadUserCardNum(out string cardNum)
        {
            cardNum = "";
            int intRst = 1;
            if (SelDirMF())
            {
                StringBuilder CardNumCmd = new StringBuilder();
                int cmdRst = CardCmdAPI.CMD_ReadUserCardNum(CardNumCmd);
                if (0 == cmdRst)
                {
                    intRst = GetReadData(ref cardNum, CardNumCmd);
                }
            }
            return intRst;
        }
        /// <summary>
        /// 读用户卡文件数据
        /// </summary>
        /// <param name="fileParamData"></param>
        /// <param name="fileMoneyData"></param>
        /// <param name="filePrice1Data"></param>
        /// <param name="filePrice2Data"></param>
        /// <param name="fileReplyData"></param>
        /// <param name="enfileControlData"></param>
        /// <returns></returns>
        public override int ReadUserCard(out string fileParamData, out string fileMoneyData, out string filePrice1Data, out string filePrice2Data, out string fileReplyData, out string enfileControlData)
        {
            fileParamData = "";
            fileMoneyData = "";
            filePrice1Data = "";
            filePrice2Data = "";
            fileReplyData = "";
            enfileControlData = "";
            int intRst = 1;
            if (SelDirDF())
            {
                StringBuilder fileParamCmd = new StringBuilder();
                StringBuilder fileMoneyCmd = new StringBuilder();
                StringBuilder filePrice1Cmd = new StringBuilder();
                StringBuilder filePrice2Cmd = new StringBuilder();
                StringBuilder fileReplyCmd = new StringBuilder();
                StringBuilder enfileControlCmd = new StringBuilder();
                int cmdRst = CardCmdAPI.CMD_ReadUserCard(fileParamCmd, fileMoneyCmd, filePrice1Cmd, filePrice2Cmd, fileReplyCmd, enfileControlCmd);
                if (0 == cmdRst)
                {
                    intRst = GetReadData(ref fileParamData, fileParamCmd);
                    intRst += GetReadData(ref fileMoneyData, fileMoneyCmd);
                    intRst += GetReadData(ref filePrice1Data, filePrice1Cmd);
                    intRst += GetReadData(ref filePrice2Data, filePrice2Cmd);
                    intRst += GetReadData(ref fileReplyData, fileReplyCmd);
                    intRst += GetReadData(ref enfileControlData, enfileControlCmd);
                }
            }
            return intRst;
        }
        /// <summary>
        /// 写用户卡
        /// </summary>
        /// <param name="fileParam"></param>
        /// <param name="fileMoney"></param>
        /// <param name="filePrice1"></param>
        /// <param name="filePrice2"></param>
        /// <param name="fileReply"></param>
        /// <param name="fileControl"></param>
        /// <returns></returns>
        public override int WriteUserCard(string fileParam, string fileMoney, string filePrice1, string filePrice2, string fileReply,string fileControl)
        {
            if (string.IsNullOrEmpty(fileParam)|| string.IsNullOrEmpty(fileMoney) || string.IsNullOrEmpty(filePrice1) || string.IsNullOrEmpty(filePrice2) || string.IsNullOrEmpty(fileControl))
            {
                return 2;
            }
            string divNum = "0000";
            if (fileParam.Length == 90)
            {
                divNum = fileParam.Substring(60, 12).PadLeft(16, '0');
            }
            else
            {
                return 3;
            }
            int intRst = 1;
            string cardNum = "";
            string cardRand = "";
            if (0 == ReadUserCardNum(out cardNum) && 0 == ReadCardRand(ref cardRand))
            {

                string fileParamData = "";
                string fileMoneyData = "";
                string filePrice1Data = "";
                string filePrice2Data = "";
                string fileReplyData = "";
                string enfileControlData = "";
                //int readRst = ReadUserCard(out fileParamData, out fileMoneyData, out filePrice1Data, out filePrice2Data, out fileReplyData, out enfileControlData);
                //if (readRst <= 2)
                {
                    //if (!string.IsNullOrEmpty(fileParamData) && fileParamData.Length == 90 &&fileParamData.Substring(60,12) == "000000000000" && fileParamData.Substring(72,12)=="000000000000")
                    {
                        if (0 != UserCard_KeyUpdata(cardNum, cardRand, divNum))
                        {
                            return 4;
                        }
                    }
                    if (0 != ReadCardRand(ref cardRand))
                    {
                        return 6;
                    }
                    if (0 != UserCardAuth(cardNum, cardRand))
                    {
                        return 5;
                    }
                    StringBuilder cmd_fileParam = new StringBuilder(224);
                    StringBuilder cmd_fileMoney = new StringBuilder(224);
                    StringBuilder cmd_filePrice1 = new StringBuilder(224);
                    StringBuilder cmd_filePrice2 = new StringBuilder(224);
                    StringBuilder cmd_fileReply = new StringBuilder(224);
                    StringBuilder cmd_enfileControl = new StringBuilder(224);
                    int makeRst = CardCmdAPI.CMD_MakeUserCard(cardNum, cardRand, fileParam, fileMoney, filePrice1, filePrice2, fileReply, fileControl, cmd_fileParam, cmd_fileMoney, cmd_filePrice1, cmd_filePrice2, cmd_fileReply, cmd_enfileControl);
                    if (0 == makeRst)
                    {
                        intRst = GetReadData(ref fileParamData, cmd_fileParam);
                        intRst += GetReadData(ref fileMoneyData, cmd_fileMoney);
                        intRst += GetReadData(ref filePrice1Data, cmd_filePrice1);
                        intRst += GetReadData(ref filePrice2Data, cmd_filePrice2);
                        intRst += GetReadData(ref enfileControlData, cmd_enfileControl);
                        intRst += GetReadData(ref fileReplyData, cmd_fileReply);
                    }
                }
            }
            
            return intRst;
        }
        /// <summary>
        /// 读参数预置卡卡号
        /// </summary>
        /// <param name="cardNum"></param>
        /// <returns></returns>
        public override int ReadParamPresetCardNum(out string cardNum)
        {
            cardNum = "";
            int intRst = 1;
            if (SelDirMF())
            {
                StringBuilder CardNumCmd = new StringBuilder();
                int cmdRst = CardCmdAPI.CMD_ReadParamPresetCardNum(CardNumCmd);
                if (0 == cmdRst)
                {
                    intRst = GetReadData(ref cardNum, CardNumCmd);
                }
            }
            return intRst;
        }
        /// <summary>
        /// 读参数预置卡文件
        /// </summary>
        /// <param name="fileParamData"></param>
        /// <param name="fileMoneyData"></param>
        /// <param name="filePrice1Data"></param>
        /// <param name="filePrice2Data"></param>
        /// <returns></returns>
        public override int ReadParamPresetCard(out string fileParamData, out string fileMoneyData, out string filePrice1Data, out string filePrice2Data)
        {
            fileParamData = "";
            fileMoneyData = "";
            filePrice1Data = "";
            filePrice2Data = "";
            int intRst = 1;
            if (SelDirDF())
            {
                StringBuilder fileParamCmd = new StringBuilder();
                StringBuilder fileMoneyCmd = new StringBuilder();
                StringBuilder filePrice1Cmd = new StringBuilder();
                StringBuilder filePrice2Cmd = new StringBuilder();
                int cmdRst = CardCmdAPI.CMD_ReadParamPresetCard(fileParamCmd, fileMoneyCmd, filePrice1Cmd, filePrice2Cmd);
                if (0 == cmdRst)
                {
                    intRst = GetReadData(ref fileParamData, fileParamCmd);
                    intRst += GetReadData(ref fileMoneyData, fileMoneyCmd);
                    intRst += GetReadData(ref filePrice1Data, filePrice1Cmd);
                    intRst += GetReadData(ref filePrice2Data, filePrice2Cmd);
                }
            }
            return intRst;
        }
        public override int WriteParamPresetCard(string fileParam, string fileMoney, string filePrice1, string filePrice2)
        {
            if (string.IsNullOrEmpty(fileParam) || string.IsNullOrEmpty(fileMoney) || string.IsNullOrEmpty(filePrice1) || string.IsNullOrEmpty(filePrice2))
            {
                return 2;
            }
            int intRst = 1;
            string cardNum = "";
            string cardRand = "";
            if (0 == ReadParamPresetCardNum(out cardNum) && 0 == ReadCardRand(ref cardRand))
            {
                if (0 == ParamPresetCardAuth(cardNum, cardRand))
                {
                    string fileParamData = "";
                    string fileMoneyData = "";
                    string filePrice1Data = "";
                    string filePrice2Data = "";
                    StringBuilder cmd_fileParam = new StringBuilder(224);
                    StringBuilder cmd_fileMoney = new StringBuilder(224);
                    StringBuilder cmd_filePrice1 = new StringBuilder(224);
                    StringBuilder cmd_filePrice2 = new StringBuilder(224);
                    if (0 == CardCmdAPI.CMD_MakeParamPresetCard(fileParam, fileMoney, filePrice1, filePrice2, cmd_fileParam, cmd_fileMoney, cmd_filePrice1, cmd_filePrice2))
                    {
                        intRst = GetReadData(ref fileParamData, cmd_fileParam);
                        intRst += GetReadData(ref fileMoneyData, cmd_fileMoney);
                        intRst += GetReadData(ref filePrice1Data, cmd_filePrice1);
                        intRst += GetReadData(ref filePrice2Data, cmd_filePrice2);
                    }
                }
            }
            return intRst;
        }

        #region private
        private int UserCard_KeyUpdata(string cardNum, string randFromCard, string divNum)
        {
            int intRst = 1;
            
            {
                string[] strUpdateRst = new string[12];
                StringBuilder[] cmd_keys = new StringBuilder[12];
                for (int i = 0; i < cmd_keys.Length; i++)
                {
                    cmd_keys[i] = new StringBuilder(48);
                }
                int cmdRst = CardCmdAPI.CMD_UserCard_KeyUpdata(cardNum, divNum, cmd_keys[0], cmd_keys[1], cmd_keys[2], cmd_keys[3], cmd_keys[4], cmd_keys[5], cmd_keys[6], cmd_keys[7], cmd_keys[8], cmd_keys[9], cmd_keys[10], cmd_keys[11]);
                if (0 == cmdRst)
                {
                    intRst = GetReadData(ref strUpdateRst[0], cmd_keys[0]);//第一条主控不判断
                    intRst = GetReadData(ref strUpdateRst[1], cmd_keys[1]);
                    for (int i = 2; i < cmd_keys.Length; i++)
                    {
                        intRst += GetReadData(ref strUpdateRst[i], cmd_keys[i]);
                        if (intRst != 0) return intRst;
                    }
                }
            }
            return intRst;
        }
        /// <summary>
        /// 用户卡身份认证
        /// </summary>
        /// <param name="cardNum">用户卡序列号</param>
        /// <param name="randFromCard">随机数</param>
        /// <returns></returns>
        private int UserCardAuth(string cardNum,string randFromCard)
        {
            int intRst = 1;
            {
                string str_data_Auth1 = "";
                string str_Auth1 = "";
                string str_Auth2 = "";
                StringBuilder data_Auth1 = new StringBuilder();
                StringBuilder cmd_Auth1 = new StringBuilder();
                StringBuilder cmd_Auth2 = new StringBuilder();
                int cmdRst = CardCmdAPI.CMD_UserCardAuth(cardNum, randFromCard, data_Auth1, cmd_Auth1, cmd_Auth2);
                if (0 == cmdRst)
                {
                    str_data_Auth1= data_Auth1.ToString().Replace("\0", "");
                    if (0 == GetReadData(ref str_Auth1, cmd_Auth1))
                    {
                        if (str_data_Auth1 == str_Auth1)
                        {
                            intRst = GetReadData(ref str_Auth2, cmd_Auth2);
                        }
                    }
                }
            }
            return intRst;

        }
        /// <summary>
        /// 参数预置卡身份认证
        /// </summary>
        /// <param name="cardNum">参数卡序列号</param>
        /// <param name="randFromCard">随机数</param>
        /// <returns></returns>
        private int ParamPresetCardAuth(string cardNum, string randFromCard)
        {
            int intRst = 1;
            {
                string str_data_Auth1 = "";
                string str_Auth1 = "";
                string str_Auth2 = "";
                StringBuilder data_Auth1 = new StringBuilder();
                StringBuilder cmd_Auth1 = new StringBuilder();
                StringBuilder cmd_Auth2 = new StringBuilder();
                int cmdRst = CardCmdAPI.CMD_ParamPresetCardAuth(cardNum, randFromCard, data_Auth1, cmd_Auth1, cmd_Auth2);
                if (0 == cmdRst)
                {
                    str_data_Auth1 = data_Auth1.ToString().Replace("\0", "");
                    if (0 == GetReadData(ref str_Auth1, cmd_Auth1))
                    {
                        if (str_data_Auth1 == str_Auth1)
                        {
                            intRst = GetReadData(ref str_Auth2, cmd_Auth2);
                        }
                    }
                }
            }
            return intRst;

        }

        /// <summary>
        /// 读卡片合闸复电（带MAC）
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="fileParamData"></param>
        /// <param name="fileMoneyData"></param>
        /// <param name="filePrice1Data"></param>
        /// <param name="filePrice2Data"></param>
        /// <param name="enfileControlData"></param>
        /// <returns></returns>
        public override int ReadUserCardMAC(string rand, out string fileParamData, out string fileMoneyData, out string filePrice1Data, out string filePrice2Data, out string enfileControlData)
        {
            int intRst = 1;
            fileParamData = "";
            fileMoneyData = "";
            filePrice1Data = "";
            filePrice2Data = "";
            enfileControlData = "";

            StringBuilder cmd_fileParam = new StringBuilder(224);
            StringBuilder cmd_fileMoney = new StringBuilder(224);
            StringBuilder cmd_filePrice1 = new StringBuilder(224);
            StringBuilder cmd_filePrice2 = new StringBuilder(224);
            StringBuilder cmd_enfileControl = new StringBuilder(224);

            intRst = CardCmdAPI.CMD_ReadUserCardMAC(rand, cmd_fileParam, cmd_fileMoney, cmd_filePrice1, cmd_filePrice2, cmd_enfileControl);

            if (intRst == 0)
            {
                intRst = GetReadData(ref fileParamData, cmd_fileParam);
                intRst += GetReadData(ref fileMoneyData, cmd_fileMoney);
                intRst += GetReadData(ref filePrice1Data, cmd_filePrice1);
                intRst += GetReadData(ref filePrice2Data, cmd_filePrice2);
                intRst += GetReadData(ref enfileControlData, cmd_enfileControl);
            }
            return intRst;
        
        }

        /// <summary>
        ///  单独写返写信息文件
        /// </summary>
        /// <param name="fileReply"></param>
        /// <returns></returns>
        public override int WriteUserCardReply(string fileReply)
        {
            
            fileReply = "04D6860036" + fileReply;
            StringBuilder fileReplyTmp = new StringBuilder(fileReply);
            string cmd_fileReply = "";
            int intRst = GetReadData(ref cmd_fileReply, fileReplyTmp);
            return intRst;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileReplyPrice2"></param>
        /// <returns></returns>
        public override int WriteUserCardReplyPrice2(string fileReplyPrice2)
        {
            fileReplyPrice2 = "04D68400CB" + fileReplyPrice2;
            StringBuilder fileReplyTmp = new StringBuilder(fileReplyPrice2);
            string cmd_fileReplyPrice2 = "";
            int intRst = GetReadData(ref cmd_fileReplyPrice2, fileReplyTmp);
            return intRst;
        }


        public override int ReadTerminalToCardInfo(string CardNo, string strRand, out string parainfo)
        {
            string strOutDta = "";
            string strData = "";
            int intRst = -1;
            string cardRand = "";

            StringBuilder fileHead = new StringBuilder("00A4000002DF01"); //CPU卡选择DF01目录 FCI为8字节文件属性信息
            intRst = GetReadData(ref strData, fileHead);

            intRst = ReadCardRand(ref cardRand);  //获取卡片随机数
            intRst = UserCardAuth(CardNo, cardRand);  //卡片身份认证
            
            StringBuilder fileReplyTmp = new StringBuilder("04B0820009" + strRand + "843201140C"); //读购电金额+购电次数+Mac 12字节
            intRst = GetReadData(ref strData, fileReplyTmp);
            if (!string.IsNullOrEmpty(strData) && strData.Length == 24)
            {
                strOutDta += strData;
            }
            else
            {
                strOutDta += "00".PadLeft(24, '0');
            }


            fileReplyTmp = new StringBuilder("04B0810A09" + strRand + "04D6810A09");   //Mac2 4字节
            intRst = GetReadData(ref strData, fileReplyTmp);
            if (!string.IsNullOrEmpty(strData) && strData.Length == 18)
            {
                strOutDta += strData.Substring(strData.Length - 8, 8);
            }
            else
            {
                strOutDta += "00".PadLeft(8, '0');
            }


            fileReplyTmp = new StringBuilder("00B081002D"); //读参数信息文件 45字节
            intRst = GetReadData(ref strData, fileReplyTmp);
            if (!string.IsNullOrEmpty(strData) && strData.Length == 90)
            {
                strOutDta += strData;
            }
            else
            {
                strOutDta += "00".PadLeft(90, '0');
            }

            fileReplyTmp = new StringBuilder("04B0811009" + strRand + "04D6811012");   //Mac3 4字节
            intRst = GetReadData(ref strData, fileReplyTmp);
            if (!string.IsNullOrEmpty(strData) && strData.Length == 36)
            {
                strOutDta += strData.Substring(strData.Length - 8, 8);
            }
            else
            {
                strOutDta += "00".PadLeft(8, '0');
            }
            fileReplyTmp = new StringBuilder("04B0812409" + strRand + "04D681240A");   //Mac4 4字节
            intRst = GetReadData(ref strData, fileReplyTmp);
            if (!string.IsNullOrEmpty(strData) && strData.Length == 20)
            {
                strOutDta += strData.Substring(strData.Length - 8, 8);
            }
            else
            {
                strOutDta += "00".PadLeft(8, '0'); 
            }
            fileReplyTmp = new StringBuilder("0084000004");   //卡片随机数(1) 4字节
            intRst = GetReadData(ref strData, fileReplyTmp);
            if (!string.IsNullOrEmpty(strData) && strData.Length == 8)
            {
                strOutDta += strData;
            }
            else
            {
                strOutDta += "00".PadLeft(8, '0'); 
            }
            parainfo = strOutDta;
            return intRst;
        }

        /// <summary>
        /// 读用户卡当前套电价文件
        /// </summary>
        /// <param name="CardNo"></param>
        /// <param name="strRand"></param>
        /// <param name="price2"></param>
        /// <returns></returns>
        public override int ReadTerminalToCardPrice1(string CardNo, string strRand, out string price1)
        {
            string strOutDta = "";
            int intRst = -1;

            StringBuilder fileReplyTmp = new StringBuilder("04B0830009" + strRand + "04D68300CB"); //读购当前套电价文件+Mac 203字节

            intRst = GetReadData(ref strOutDta, fileReplyTmp);
            if (!string.IsNullOrEmpty(strOutDta) && strOutDta.Length == 406)
            {
                price1 = strOutDta;
            }
            else
            {
                price1 = "00".PadLeft(406, '0'); ;
            }

            return intRst;
        }

        /// <summary>
        /// 读用户卡备用套电价文件
        /// </summary>
        /// <param name="CardNo"></param>
        /// <param name="strRand"></param>
        /// <param name="price2"></param>
        /// <returns></returns>
        public override int ReadTerminalToCardPrice2(string CardNo, string strRand, out string price2)
        {
            string strOutDta = "";
            int intRst = -1;

            StringBuilder fileReplyTmp = new StringBuilder("04B0840009" + strRand + "04D68400CB"); //读购备用套电价文件+Mac 203字节

            intRst = GetReadData(ref strOutDta, fileReplyTmp);
            if (!string.IsNullOrEmpty(strOutDta) && strOutDta.Length == 406)
            {
                price2 = strOutDta;
            }
            else
            {
                price2 = "00".PadLeft(406, '0'); ;
            }
            
            return intRst;
        }



        /// <summary>
        /// 读卡片随机数
        /// </summary>
        /// <param name="cardRand"></param>
        /// <returns></returns>
        private int ReadCardRand(ref string cardRand)
        {
            cardRand = "";
            int intRst = 1;
            if (SelDirDF())
            {
                StringBuilder cardRandCmd = new StringBuilder();
                int cmdRst = CardCmdAPI.CMD_ReadCardRand(cardRandCmd);
                if (0 == cmdRst)
                {
                    intRst = GetReadData(ref cardRand, cardRandCmd);
                }
            }
            return intRst;
        }
        private int GetReadData(ref string fileData, StringBuilder fileParamCmd)
        {
            int intRst = 1;
            int retrytime = 2;
            string tmp = fileParamCmd.ToString().Replace("\0", "");
            for (int i = 0; i < retrytime; i++)
            {
                //if (GlobalUnit.g_Dev_CommunType == CLDC_Comm.Enum.Cus_CommunType.南网通讯DLL)
                {
                    byte[] send = DataCvt.stringToByte(tmp);
                    byte[] recvFrame = new byte[0];
                    if (SendData(send, ref recvFrame))
                    {
                        intRst = 0;
                        fileData = BitConverter.ToString(recvFrame).Replace("-", "");
                        return intRst;
                    }
                }
            }
            return intRst;
        }

        private bool SelDirMF()
        {
            StringBuilder selMF = new StringBuilder();
            StringBuilder selDF = new StringBuilder();
            int retrytime = 2;
            for (int i = 0; i < retrytime; i++)
            {
                int cmdRst = CardCmdAPI.CMD_SelDir(selMF, selDF);
                if (0 == cmdRst)
                {
                    string tmp = selMF.ToString().Replace("\0", "");
                    //if (GlobalUnit.g_Dev_CommunType == CLDC_Comm.Enum.Cus_CommunType.南网通讯DLL)
                    {
                        byte[] send = DataCvt.stringToByte(tmp);
                        byte[] recvFrame = new byte[0];
                        System.Threading.Thread.Sleep(200);
                        if (SendData(send, ref recvFrame))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private bool SelDirDF()
        {
            StringBuilder selMF = new StringBuilder();
            StringBuilder selDF = new StringBuilder();
            int retrytime = 2;
            for (int i = 0; i < retrytime; i++)
            {
                int cmdRst = CardCmdAPI.CMD_SelDir(selMF, selDF);
                if (0 == cmdRst)
                {
                    string tmp = selDF.ToString().Replace("\0", "");

                    //if (GlobalUnit.g_Dev_CommunType == CLDC_Comm.Enum.Cus_CommunType.南网通讯DLL)
                    {
                        byte[] send = DataCvt.stringToByte(tmp);
                        byte[] recvFrame = new byte[0];
                        System.Threading.Thread.Sleep(200);
                        if (SendData(send, ref recvFrame))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        #endregion
    }
}
