using System;

namespace CLDC_SafeFileProtocol.Protocols
{
    public class SouthSafeFile : FileProtocolBase
    {
        public override int GetParamCardFileParam(string[] Params, out string OutFile)
        {
            OutFile = "";
            if (Params.Length == 9)
            {
                try
                {
                    Model.ParamCardFileParam Mpcfp = new Model.ParamCardFileParam();
                    Mpcfp.updateFlag = Params[1];
                    Mpcfp.RateChangeTime = Params[3];
                    Mpcfp.warningMoney1 = Params[5];
                    Mpcfp.warningMoney2 = Params[6];
                    Mpcfp.currentRate = Params[7];
                    Mpcfp.voltageRate = Params[8];
                    byte[] tmp = OrgFrame(0x02, Mpcfp.GetData());
                    OutFile = BitConverter.ToString(tmp).Replace("-", "");
                    return 0;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
            else
            {
                return 1;
            }
        }
        public override int GetParamCardFileMoney(string[] Params, out string OutFile)
        {
            OutFile = "";
            if (Params.Length == 2)
            {
                try
                {
                    Model.ParamCardFileMoney Mpcfp = new Model.ParamCardFileMoney();
                    Mpcfp.buyMoney = Params[0];
                    Mpcfp.buyCount = Params[1];
                    
                    byte[] tmp = Mpcfp.GetData();
                    OutFile = BitConverter.ToString(tmp).Replace("-", "");
                    return 0;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
            else
            {
                return 1;
            }
        }
        public override int GetParamCardFilePrice1(string[] Params, out string OutFile)
        {
            return GetCardFilePrice1(Params, out OutFile);
        }

        private int GetCardFilePrice1(string[] Params, out string OutFile)
        {
            OutFile = "";
            if (Params.Length >= 50)
            {
                try
                {
                    Model.CardFilePrice1 Mpcfp = new Model.CardFilePrice1();
                    Mpcfp.rate1 = Params[0];
                    Mpcfp.rate2 = Params[1];
                    Mpcfp.rate3 = Params[2];
                    Mpcfp.rate4 = Params[3];
                    Mpcfp.rate5 = Params[4];
                    Mpcfp.rate6 = Params[5];
                    Mpcfp.rate7 = Params[6];
                    Mpcfp.rate8 = Params[7];
                    Mpcfp.rate9 = Params[8];
                    Mpcfp.rate10 = Params[9];
                    Mpcfp.rate11 = Params[10];
                    Mpcfp.rate12 = Params[11];
                    
                    Mpcfp.step1Value1 = Params[12];
                    Mpcfp.step1Value2 = Params[13];
                    Mpcfp.step1Value3 = Params[14];
                    Mpcfp.step1Value4 = Params[15];
                    Mpcfp.step1Value5 = Params[16];
                    Mpcfp.step1Value6 = Params[17];
                    Mpcfp.step1Price1 = Params[18];
                    Mpcfp.step1Price2 = Params[19];
                    Mpcfp.step1Price3 = Params[20];
                    Mpcfp.step1Price4 = Params[21];
                    Mpcfp.step1Price5 = Params[22];
                    Mpcfp.step1Price6 = Params[23];
                    Mpcfp.step1Price7 = Params[24];
                    Mpcfp.priceDay1 = Params[25];
                    Mpcfp.priceDay2 = Params[26];
                    Mpcfp.priceDay3 = Params[27];
                    Mpcfp.priceDay4 = Params[28];
                    Mpcfp.priceDay5 = Params[29];
                    Mpcfp.priceDay6 = Params[30];

                    Mpcfp.step2Value1 = Params[31];
                    Mpcfp.step2Value2 = Params[32];
                    Mpcfp.step2Value3 = Params[33];
                    Mpcfp.step2Value4 = Params[34];
                    Mpcfp.step2Value5 = Params[35];
                    Mpcfp.step2Value6 = Params[36];
                    Mpcfp.step2Price1 = Params[37];
                    Mpcfp.step2Price2 = Params[38];
                    Mpcfp.step2Price3 = Params[39];
                    Mpcfp.step2Price4 = Params[40];
                    Mpcfp.step2Price5 = Params[41];
                    Mpcfp.step2Price6 = Params[42];
                    Mpcfp.step2Price7 = Params[43];
                    Mpcfp.price2Day1 = Params[44];
                    Mpcfp.price2Day2 = Params[45];
                    Mpcfp.price2Day3 = Params[46];
                    Mpcfp.price2Day4 = Params[47];
                    Mpcfp.price2Day5 = Params[48];
                    Mpcfp.price2Day6 = Params[49];
                    //Mpcfp.standby1 = Params[50];
                    byte[] tmp = OrgFrame(0x01, Mpcfp.GetData());
                    OutFile = BitConverter.ToString(tmp).Replace("-", "");
                    return 0;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
            else
            {
                return 1;
            }
        }

        public override int GetParamCardFilePrice2(string[] Params, out string OutFile)
        {
            return GetCardFilePrice2(Params, out OutFile);
        }

        private int GetCardFilePrice2(string[] Params, out string OutFile)
        {
            OutFile = "";
            if (Params.Length >= 50)
            {
                try
                {
                    Model.CardFilePrice2 Mpcfp = new Model.CardFilePrice2();
                    Mpcfp.rate1 = Params[0];
                    Mpcfp.rate2 = Params[1];
                    Mpcfp.rate3 = Params[2];
                    Mpcfp.rate4 = Params[3];
                    Mpcfp.rate5 = Params[4];
                    Mpcfp.rate6 = Params[5];
                    Mpcfp.rate7 = Params[6];
                    Mpcfp.rate8 = Params[7];
                    Mpcfp.rate9 = Params[8];
                    Mpcfp.rate10 = Params[9];
                    Mpcfp.rate11 = Params[10];
                    Mpcfp.rate12 = Params[11];

                    Mpcfp.step1Value1 = Params[12];
                    Mpcfp.step1Value2 = Params[13];
                    Mpcfp.step1Value3 = Params[14];
                    Mpcfp.step1Value4 = Params[15];
                    Mpcfp.step1Value5 = Params[16];
                    Mpcfp.step1Value6 = Params[17];
                    Mpcfp.step1Price1 = Params[18];
                    Mpcfp.step1Price2 = Params[19];
                    Mpcfp.step1Price3 = Params[20];
                    Mpcfp.step1Price4 = Params[21];
                    Mpcfp.step1Price5 = Params[22];
                    Mpcfp.step1Price6 = Params[23];
                    Mpcfp.step1Price7 = Params[24];
                    Mpcfp.priceDay1 = Params[25];
                    Mpcfp.priceDay2 = Params[26];
                    Mpcfp.priceDay3 = Params[27];
                    Mpcfp.priceDay4 = Params[28];
                    Mpcfp.priceDay5 = Params[29];
                    Mpcfp.priceDay6 = Params[30];

                    Mpcfp.step2Value1 = Params[31];
                    Mpcfp.step2Value2 = Params[32];
                    Mpcfp.step2Value3 = Params[33];
                    Mpcfp.step2Value4 = Params[34];
                    Mpcfp.step2Value5 = Params[35];
                    Mpcfp.step2Value6 = Params[36];
                    Mpcfp.step2Price1 = Params[37];
                    Mpcfp.step2Price2 = Params[38];
                    Mpcfp.step2Price3 = Params[39];
                    Mpcfp.step2Price4 = Params[40];
                    Mpcfp.step2Price5 = Params[41];
                    Mpcfp.step2Price6 = Params[42];
                    Mpcfp.step2Price7 = Params[43];
                    Mpcfp.price2Day1 = Params[44];
                    Mpcfp.price2Day2 = Params[45];
                    Mpcfp.price2Day3 = Params[46];
                    Mpcfp.price2Day4 = Params[47];
                    Mpcfp.price2Day5 = Params[48];
                    Mpcfp.price2Day6 = Params[49];

                    Mpcfp.stepChangeTime = Params[50];
                    byte[] tmp = OrgFrame(0x01, Mpcfp.GetData());
                    OutFile = BitConverter.ToString(tmp).Replace("-", "");
                    return 0;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
            else
            {
                return 1;
            }
        }

        public override int GetUserCardFileParam(string[] Params, out string OutFile)
        {
            OutFile = "";
            if (Params.Length >= 12)
            {
                try
                {
                    Model.UserCardFileParam Mpcfp = new Model.UserCardFileParam();
                    //Mpcfp.standby1 = Params[0];
                    Mpcfp.updateFlag = Params[1];
                    //Mpcfp.standby2 = Params[2];
                    Mpcfp.RateChangeTime = Params[3];
                    //Mpcfp.standby3 = Params[4];
                    Mpcfp.warningMoney1 = Params[5];
                    Mpcfp.warningMoney2 = Params[6];
                    Mpcfp.currentRate = Params[7];
                    Mpcfp.voltageRate = Params[8];
                    Mpcfp.meterNo = Params[9];
                    Mpcfp.userNo = Params[10];
                    Mpcfp.userCardType = Params[11];
                    byte[] tmp = OrgFrame(0x03, Mpcfp.GetData());
                    OutFile = BitConverter.ToString(tmp).Replace("-", "");
                    return 0;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
            else
            {
                return 1;
            }
        }
        public override int GetUserCardFileMoney(string[] Params, out string OutFile)
        {
            OutFile = "";
            if (Params.Length == 2)
            {
                try
                {
                    Model.UserCardFileMoney Mpcfp = new Model.UserCardFileMoney();
                    Mpcfp.buyMoney = Params[0];
                    Mpcfp.buyCount = Params[1];

                    byte[] tmp = Mpcfp.GetData();
                    OutFile = BitConverter.ToString(tmp).Replace("-", "");
                    return 0;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
            else
            {
                return 1;
            }
        }
        public override int GetUserCardFilePrice1(string[] Params, out string OutFile)
        {
            return GetCardFilePrice1(Params, out OutFile);
        }
        public override int GetUserCardFilePrice2(string[] Params, out string OutFile)
        {
            return GetCardFilePrice2(Params, out OutFile);
        }
        public override int GetUserCardFileControl(string[] Params, out string OutFile)
        {
            OutFile = "";//明文在上层自己组
            if (Params != null && Params.Length > 0)
            {
                OutFile = Params[0];
                return 0;
            }
            return 2;
        }
    }
}
