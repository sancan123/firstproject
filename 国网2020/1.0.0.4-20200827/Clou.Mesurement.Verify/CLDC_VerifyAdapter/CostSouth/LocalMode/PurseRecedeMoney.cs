using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_SafeFileProtocol;
using System.Windows.Forms;
using CLDC_DataCore.Function;
using CLDC_SafeFileProtocol.Protocols;
using CLDC_DataCore.Const;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 钱包退费
    /// </summary>
    public class PurseRecedeMoney : VerifyBase
    {
        protected override string ItemKey
        {
            // get { throw new System.NotImplementedException(); }
            get { return null; }
        }
        protected override string ResultKey
        {
            //get { throw new System.NotImplementedException(); }
            get { return null; }
        }

        public PurseRecedeMoney(object plan)
            : base(plan)
        {
        }

        //剩余金额退费前-退费后1|退费金额1|退费金额大于剩余金额不可退费1|剩余金额退费前-退费后2|退费金额2|退费事件记录2|退费金额小于剩余金额可退费2
        //不可连续退费3|剩余金额退费前-退费后4|退费金额4|退费事件记录4|剩余金额可退费至0

        protected override bool CheckPara()
        {

            ResultNames = new string[] { "剩余金额退费前一退费后1","退费金额1","退费金额大于剩余金额不可退费1", 
                                         "剩余金额退费前一退费后2","退费金额2","退费事件记录2","退费金额小于剩余金额可退费2",
                                         "不可连续退费3",
                                         "剩余金额退费前一退费后4","退费金额4","退费事件记录4","剩余金额可退费至0",
                                         "结论" };
            return true;
        }

        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            PowerOn();
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strRevData = new string[BwCount];
            string[] strOutMac1 = new string[BwCount];
            string[] strOutMac2 = new string[BwCount];
            string[] strRevCode = new string[BwCount];
            string[] strSyMoneyQ = new string[BwCount]; //钱包初始化剩余金额
            string[] strSyMoney = new string[BwCount]; //当前剩余金额
            string[] strKhID = new string[BwCount]; //当前客户编号
            string[] strGdkKhID = new string[BwCount];
            string[] strGdCount = new string[BwCount]; //购电次数
            string[] paraFile = new string[12]; //参数信息文件
            string[] walletFile = new string[2];//钱包文件
            string[] priceFile1 = new string[51];//当前套电价文件
            string[] priceFile2 = new string[51];//备用套电价文件
            string[] ControlFilePlain = new string[1]; //合闸明文
            string[] strData = new string[BwCount];
            string[] strParaFileArr = new string[BwCount];  //参数信息文件
            string[] strwalletFileArr = new string[BwCount];//钱包文件
            string[] strpriceFile1Arr = new string[BwCount];//当前套电价文件
            string[] strpriceFile2Arr = new string[BwCount];//备用套电价文件
            string[] strfileReplyArr = new string[BwCount];
            string[] strControlFilePlainArr = new string[BwCount];  //合闸明文
            bool[] rstTmp = new bool[BwCount];
            bool[,] blnRet = new bool[BwCount, 12];
            int[] iFlag = new int[BwCount];
            bool[] WriteUserResult = new bool[BwCount];
            bool[] BlnIniRet = new bool[BwCount];
            bool[] blnsfRet = new bool[BwCount];
            string[] MyStatus = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] outData = new string[BwCount];
            bool[] result = new bool[BwCount];
            string[] status3 = new string[BwCount];
            string[] strPutApdu = new string[BwCount];
            string[] strGdCountQ = new string[BwCount];
            int iSelectBwCount = 0;
            string[] strErrInfo = new string[BwCount];
            string[] strTfMoneyDate = new string[BwCount];
            string[] strShowMoney = new string[BwCount];

            //Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);

            #region 准备步骤
            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
            Common.Memset(ref strData, "00000000");
            result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥更新,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行客户编号为112233445566的远程开户,请稍候....");
            //购电金额+购电次数+客户编号
            Common.Memset(ref strKhID, "112233445566");
            Common.Memset(ref strData, "00000000" + "00000000" + "112233445566");
            bool[] blnKhRet = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置囤积金额限值为0,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strRevCode[i] = "04001004";
                strData[i] = strRevCode[i] + "00000000";
            }
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            //设置报警金额2限值
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置报警金额2限值0元,请稍候....");
            Common.Memset(ref  strRevCode, "04001002");
            Common.Memset(ref  strData, "00000500");
            Common.Memset(ref strPutApdu, "04D6811008");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strRevCode, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strRevCode[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            #endregion

            //1--------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行第1次充值200元,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                strGdCountQ[i] = "00000001";
                BuyMoney[i] = "00004E20";
                strData[i] = BuyMoney[i] + strGdCountQ[i] + strKhID[i];
            }
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行第2次充值200元,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                strGdCountQ[i] = "00000002";
                BuyMoney[i] = "00004E20";
                strData[i] = BuyMoney[i] + strGdCountQ[i] + strKhID[i];
            }
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行第3次充值200元,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                strGdCountQ[i] = "00000003";
                BuyMoney[i] = "00004E20";
                strData[i] = BuyMoney[i] + strGdCountQ[i] + strKhID[i];
            }
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额....");
            strSyMoneyQ = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
            strSyMoneyQ = Common.StringConverToDecima(strSyMoneyQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行退费金额=剩余金额+1,请稍候....");
            Common.Memset(ref strData, "0000EAC4");
            result = MeterProtocolAdapter.Instance.SouthDecreasePurse(strRand2, strData);
            Common.Memset(ref strShowMoney, "601");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "退费金额1", strShowMoney);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额....");
            strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
            strSyMoney = Common.StringConverToDecima(strSyMoney);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                blnRet[i, 0] = result[i];
                ResultDictionary["剩余金额退费前一退费后1"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
                ResultDictionary["退费金额大于剩余金额不可退费1"][i] = !result[i] ? "通过" : "不通过";

            }
            UploadTestResult("剩余金额退费前一退费后1");
            UploadTestResult("退费金额大于剩余金额不可退费1");

            //2-----------------------------

            strSyMoneyQ = strSyMoney;

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行退费金额=剩余金额-1,请稍候....");
            Common.Memset(ref strData, "0000E9FC");
            string strDateTmp = DateTime.Now.ToString("yyMMddHHmmss");
            result = MeterProtocolAdapter.Instance.SouthDecreasePurse(strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            Common.Memset(ref strShowMoney, "599");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "退费金额2", strShowMoney);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额....");
            strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
            strSyMoney = Common.StringConverToDecima(strSyMoney);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次退费记录内容,请稍候....");
            string[]  strTfjlData = MeterProtocolAdapter.Instance.ReadData("03340001", 19);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "退费事件记录2", strTfjlData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(strTfjlData[i]) && strTfjlData[i].Length >= 38)
                {
                    strTfMoneyDate[i] = strTfjlData[i].ToString().Substring(28, 10);
                }
                if (!string.IsNullOrEmpty(strTfMoneyDate[i]) && strTfMoneyDate[i] != "0000000000")
                {
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strDateTmp), DateTimes.FormatStringToDateTime(strTfMoneyDate[i]));

                    string strTfhSyMoney = strTfjlData[i].Substring(0, 8);
                    string strTfqSyMoney = strTfjlData[i].Substring(8, 8);
                    string strTfMoney = strTfjlData[i].Substring(16, 8);
                    string strTfqGdCount = strTfjlData[i].Substring(24,4);

                    if (iErr < 121 && result[i]
                        && strTfhSyMoney.Equals("00000100")
                        && strTfqSyMoney.Equals("00060000")
                        && strTfMoney.Equals("00059900")
                        && strTfqGdCount.Equals("0003"))
                    {
                        blnRet[i, 1] = true;
                    }
                    
                    ResultDictionary["退费金额小于剩余金额可退费2"][i] = blnRet[i, 1] ? "通过" : "不通过";
                }
                else
                {
                    ResultDictionary["退费金额小于剩余金额可退费2"][i] = "不通过";
                }
                ResultDictionary["剩余金额退费前一退费后2"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
            }
            UploadTestResult("剩余金额退费前一退费后2");
            UploadTestResult("退费金额小于剩余金额可退费2");

            //3------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行再次退费,退费金额=1,请稍候....");
            Common.Memset(ref strData, "00000001");
            result = MeterProtocolAdapter.Instance.SouthDecreasePurse(strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                ResultDictionary["不可连续退费3"][i] = !result[i] ? "通过" : "不通过";
                blnRet[i, 2] = result[i];
            }
            UploadTestResult("不可连续退费3");

            //4-----------------

            if (Stop) return;
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                strGdCountQ[i] = "00000004";
                BuyMoney[i] = "00000064";
                strData[i] = BuyMoney[i] + strGdCountQ[i] + strKhID[i];
            }
            MessageController.Instance.AddMessage("正在进行充值1元,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额....");
            strSyMoneyQ = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
            strSyMoneyQ = Common.StringConverToDecima(strSyMoneyQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行退费金额=2元,请稍候....");
            Common.Memset(ref strData, "000000C8");
            strDateTmp = DateTime.Now.ToString("yyMMddHHmmss");
            result = MeterProtocolAdapter.Instance.SouthDecreasePurse(strRand2, strData);

            Common.Memset(ref strShowMoney, "2");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "退费金额4", strShowMoney);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额....");
            strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
            strSyMoney = Common.StringConverToDecima(strSyMoney);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次退费记录内容,请稍候....");
            strTfjlData = MeterProtocolAdapter.Instance.ReadData("03340001", 19);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "退费事件记录4", strTfjlData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(strTfjlData[i]) && strTfjlData[i].Length >= 38)
                {
                    strTfMoneyDate[i] = strTfjlData[i].ToString().Substring(28, 10);
                    //ResultDictionary["退费日期4"][i] = strTfMoneyDate[i];
                    //ResultDictionary["退费前购电次数4"][i] = strData[i].Substring(24, 4);
                    //ResultDictionary["退费金额4"][i] = strData[i].Substring(16, 8);
                    //ResultDictionary["退费前剩余金额4"][i] = strData[i].Substring(8, 8);
                    //ResultDictionary["退费后剩余金额4"][i] = strData[i].Substring(0, 8);
                }
                if (!string.IsNullOrEmpty(strTfMoneyDate[i]) && strTfMoneyDate[i] != "0000000000")
                {
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strDateTmp), DateTimes.FormatStringToDateTime(strTfMoneyDate[i]));

                    string strTfhSyMoney = strTfjlData[i].Substring(0, 8);
                    string strTfqSyMoney = strTfjlData[i].Substring(8, 8);
                    string strTfMoney = strTfjlData[i].Substring(16, 8);
                    string strTfqGdCount = strTfjlData[i].Substring(24, 4);
                    if (iErr < 121 && result[i]
                        && strTfhSyMoney.Equals("00000000")
                        && strTfqSyMoney.Equals("00000200")
                        && strTfMoney.Equals("00000200")
                        && strTfqGdCount.Equals("0004"))
                    {
                        blnRet[i, 3] = true;
                    }
                    
                    ResultDictionary["剩余金额可退费至0"][i] = blnRet[i, 3] ? "通过" : "不通过";
                }
                else
                {
                    ResultDictionary["剩余金额可退费至0"][i] = "不通过";
                }
                ResultDictionary["剩余金额退费前一退费后4"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
            }

            UploadTestResult("剩余金额退费前一退费后4");
            UploadTestResult("剩余金额可退费至0");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!blnRet[i, 0] && blnRet[i, 1] && !blnRet[i, 2] && blnRet[i, 3])
                {
                    ResultDictionary["结论"][i] = "合格";
                }
                else
                {
                    ResultDictionary["结论"][i] = "不合格";
                }
            }
            UploadTestResult("结论");
        }
    }
}
