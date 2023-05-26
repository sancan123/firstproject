using System;
using CLDC_Encryption.CLEncryption.Interface;
using CLDC_Encryption.CLEncryption.API;

namespace CLDC_Encryption.CLEncryption
{
    public class DevelopEncryption : EncryptionBase, IAmMeterEncryption
    {
        private static object LockAP = new object();
        #region IAmMeterEncryption 成员


        public bool Link()
        {
            return true;
        }

        public bool UnLink()
        {
            return true;
        }


        #endregion



        private string CheckResult(int result)
        {
            string str_LostMessage = "";
            switch (result)
            {
                case 0:
                    str_LostMessage = "" ;
                    break;
                case 200:
                    str_LostMessage = "200.连接加密机失败；" ;
                    break;
                case 201:
                    str_LostMessage = "201.写卡失败；";
                    break;
                case 202:
                    str_LostMessage = "202.读卡失败；";
                    break;
                case 203:
                    str_LostMessage = "203.计算密文失败；计算MAC 失败；";
                    break;
                case 204:
                    str_LostMessage = "204.数据加密失败；" ;
                    break;
                case 205:
                    str_LostMessage = "205.取密文失败；" ;
                    break;
                default:
                    str_LostMessage = "其它错误；" ;
                    break;
            }
            return str_LostMessage;
        }

        public int SouthOpenDevice(string szType, string cHostIp, int uiPort, int timeout, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthCloseDevice(out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthIdentityAuthentication(int Flag, string PutDiv, out string OutRand, out string OutEndata, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthUserControl(int Flag, string PutRand, string PutDiv, string PutEsamNo, string PutData, out string OutEndataout, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthParameterUpdate(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, out string OutData, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthPrice1Update(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, out string OutData, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthPrice2Update(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, out string OutData, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthParameterElseUpdate(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, out string OutEndata, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthIncreasePurse(int Flag, string PutRand, string PutDiv, string PutData, out string OutData, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthInitPurse(int Flag, string PutRand, string PutDiv, string PutData, out string OutData, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthKeyUpdateV2(int PutKeySum, string PutKeyState, string PutKeyId, string PutRand, string PutDiv, string PutEsamNo, out string OutData, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthDataClear1(int Flag, string PutRand, string PutDiv, string PutData, out string OutData, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthInfraredRand(out string OutRand1, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthInfraredAuth(int Flag, string PutDiv, string PutEsamNo, string PutRand1, string PutRand1Endata, string PutRand2, out string OutRand2Endata, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthMacCheck(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, string PutMac, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthMacWrite(int Flag, string PutRand, string PutDiv, string PutEsamNo, string PutFileID, string PutDataBegin, string PutData, out string OutData, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthEncMacWrite(int Flag, string PutRand, string PutDiv, string PutEsamNo, string PutFileID, string PutDataBegin, string PutData, out string OutData, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthEncForCompare(string PutKeyid, string PutDiv, string PutData, out string OutData, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthDecreasePurse(int Flag, string PutRand, string PutDiv, string PutData, out string OutEndata, out string Message)
        {
            throw new NotImplementedException();
        }

        public int SouthSwitchChargeMode(int Flag, string PutRand, string PutDiv, string PutData, out string OutData, out string Message)
        {
            throw new NotImplementedException();
        }

    }
}
