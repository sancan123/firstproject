using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using System.Globalization;
using CLDC_SafeFileProtocol.Protocols;
using System.Windows.Forms;
using CLDC_SafeFileProtocol;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 远程充值
    /// </summary>
    public class RemoteInMoney :VerifyBase
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

        public RemoteInMoney(object plan)
            : base(plan)
        {

        }

      
        //输出项
        //远程开户状态位1|本地开户状态位1|未开户不可远程充值1|远程开户状态位2|充值命令客户编号2|表内客户编号2|剩余金额前-后2|应答(客户编号不匹配)2|客户编号不一致不可远程充值2
        //充值命令购电次数3|表内购电次数3|剩余金额前-后3|应答(充值次数错误)3|购电次数比表内大2不可远程充值3|囤积金额限值4|购电次数前-后4|剩余金额前-后4|上1次购电日期4
        //上1次购电后总购电次数4|上1次购电金额4|上1次购电前剩余金额4|上1次购电后剩余金额4|上1次购电后累计购电金额4|购电次数比表内大1可远程充值4|充值命令购电次数5
        //表内购电次数5|剩余金额前-后5|应答(重复充值)5|购电次数与表内相等不可远程充值5|囤积金额限值6|购电金额6|剩余金额前-后6|应答(购电超囤积)6|购电超囤积不可远程充值6

        protected override bool CheckPara()
        {

            ResultNames = new string[] {"远程开户状态位1","本地开户状态位1","未开户不可远程充值1",
                                        "远程开户状态位2","充值命令客户编号2","表内客户编号2","剩余金额前一后2","应答客户编号不匹配2","客户编号不一致不可远程充值2",
                                        "充值命令购电次数3","表内购电次数3","剩余金额前一后3","应答充值次数错误3","购电次数比表内大2不可远程充值3",
                                        "囤积金额限值4","购电次数前一后4","剩余金额前一后4","上1次购电日期4", "上1次购电后总购电次数4","上1次购电金额4","上1次购电前剩余金额4",
                                        "上1次购电后剩余金额4","上1次购电后累计购电金额4","购电次数比表内大1可远程充值4",
                                        "充值命令购电次数5","表内购电次数5","剩余金额前一后5","应答重复充值5","购电次数与表内相等不可远程充值5",
                                        "囤积金额限值6","购电金额6","剩余金额前一后6","应答购电超囤积6","购电超囤积不可远程充值6",
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
            string[] strData = new string[BwCount];
            string[] strRevData = new string[BwCount];
            string[] strOutMac1 = new string[BwCount];
            string[] strOutMac2 = new string[BwCount];
            string[] strRevCode = new string[BwCount];
            string[] strSyMoneyQ = new string[BwCount]; //钱包初始化剩余金额
            string[] strSyMoney = new string[BwCount]; //当前剩余金额
            string[] strGdCountQ = new string[BwCount];
            string[] strGdCount = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyKhID = new string[BwCount];
            string[] MyStatus = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] outData = new string[BwCount];
            bool[] result = new bool[BwCount];
            string[] status3 = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            bool[,] blnRet = new bool[BwCount, 6];
            string[] paraFile = new string[12]; //参数信息文件
            string[] walletFile = new string[2];//钱包文件
            string[] priceFile1 = new string[51];//当前套电价文件
            string[] priceFile2 = new string[51];//备用套电价文件
            string[] ControlFilePlain = new string[1]; //合闸明文
            string[] strParaFileArr = new string[BwCount];  //参数信息文件
            string[] strwalletFileArr = new string[BwCount];//钱包文件
            string[] strpriceFile1Arr = new string[BwCount];//当前套电价文件
            string[] strpriceFile2Arr = new string[BwCount];//备用套电价文件
            string[] strfileReplyArr = new string[BwCount];
            string[] strControlFilePlainArr = new string[BwCount];  //合闸明文
            string[] strApdu = new string[BwCount];
            int iSelectBwCount = 0;
            string[] strRemoteStatus = new string[BwCount];
            string[] strBdStatus = new string[BwCount];
            string[] strErrInfo = new string[BwCount];
            bool[] blnErrRet = new bool[BwCount];

            

            #region 准备步骤
            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
            Common.Memset(ref strData, "00002710");
            bool[] blnQbRet = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            string strDataTmp = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
            strDataTmp += DateTime.Now.ToString("HHmmss");
            Common.Memset(ref strRevCode, "0400010C");
            Common.Memset(ref strData, strDataTmp);
            bool[] blnSetDateRet = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            #endregion

            if(Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
            Common.Memset(ref strRevCode,"DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoneyQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥更新....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);


            //1-------------------

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
            string[] strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);

            if (Stop) return;
            MessageController.Instance.AddMessage("表未开户状态下,发送远程充值命令,客户编号与表内一致,请稍候....");
            //购电金额+购电次数+客户编号

            for (int i = 0; i < BwCount; i++)
            {
                BuyCount[i] = "00000001";
                BuyMoney[i] = "00002710";
                strData[i] = BuyMoney[i] + BuyCount[i] + strRedMeterKhID[i];
            }
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                    {
                        strRemoteStatus[i] = "未开户";
                    }
                    else
                    {
                        strRemoteStatus[i] = "开户";
                    }
                    if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                    {
                        strBdStatus[i] = "未开户";
                    }
                    else
                    {
                        strBdStatus[i] = "开户";
                    }
                }
                else
                {
                    strRemoteStatus[i] = "异常";
                    strBdStatus[i] = "异常";
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "远程开户状态位1", strRemoteStatus);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "本地开户状态位1", strBdStatus);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                blnRet[i, 0] = result[i];
                ResultDictionary["未开户不可远程充值1"][i] = !result[i] ? "通过" : "不通过";
            }
            UploadTestResult("未开户不可远程充值1");

            //2--------------------
            //购电金额+购电次数+客户编号
            MessageController.Instance.AddMessage("正在远程开户（不充值,客户编号为112233445566）,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                BuyKhID[i] = "112233445566";
                strData[i] = "00000000" + "00000000" + BuyKhID[i];
            }
            bool[] bIniUser = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电后剩余金额,请稍候....");
            strSyMoneyQ = MeterProtocolAdapter.Instance.ReadData("03330501", 4);
            strSyMoneyQ = Common.StringConverToDecima(strSyMoneyQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送远程充值命令：客户编号与表内不一致(665544332211),请稍候....");
            //购电金额+购电次数+客户编号
            for (int i = 0; i < BwCount; i++)
            {
                BuyCount[i] = "00000001";
                BuyMoney[i] = "00002710";
                BuyKhID[i] = "665544332211";
                strData[i] = BuyMoney[i] + BuyCount[i] + BuyKhID[i];
            }
            bool[] bInMoney = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "充值命令客户编号2", BuyKhID);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电后剩余金额,请稍候....");
            strSyMoney = MeterProtocolAdapter.Instance.ReadData("03330501", 4);
            strSyMoney = Common.StringConverToDecima(strSyMoney);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
            strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内客户编号2", strRedMeterKhID);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            Common.Memset(ref blnErrRet, false);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                
                if (!string.IsNullOrEmpty(strErrInfo[i]))
                {
                    if ((Convert.ToInt32(strErrInfo[i], 16) & 0x0010) == 0x0010)
                    {
                        ResultDictionary["应答客户编号不匹配2"][i] = "是";
                        blnErrRet[i] = true;
                    }
                    else
                    {
                        ResultDictionary["应答客户编号不匹配2"][i] = "无";
                        blnErrRet[i] = false;
                    }
                }
                else
                {
                    ResultDictionary["应答客户编号不匹配2"][i] = "异常";
                    blnErrRet[i] = false;
                }
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                    {
                        ResultDictionary["远程开户状态位2"][i] = "未开户";
                    }
                    else
                    {
                        ResultDictionary["远程开户状态位2"][i] = "开户";
                    }
                }
                else
                {
                    ResultDictionary["远程开户状态位2"][i] = "异常";
                }
                if (bInMoney[i])
                {
                    ResultDictionary["客户编号不一致不可远程充值2"][i] = "不通过";
                }
                else if (!bInMoney[i] && blnErrRet[i] && strSyMoneyQ[i] == strSyMoney[i])
                {
                    ResultDictionary["客户编号不一致不可远程充值2"][i] = "通过";
                    blnRet[i, 1] = true;
                }
                else
                {
                    ResultDictionary["客户编号不一致不可远程充值2"][i] = "异常";
                }

                ResultDictionary["剩余金额前一后2"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
            }
            UploadTestResult("远程开户状态位2");
            UploadTestResult("剩余金额前一后2");
            UploadTestResult("应答客户编号不匹配2");
            UploadTestResult("客户编号不一致不可远程充值2");

            //3-------------

            strSyMoneyQ = strSyMoney;

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送远程充值命令：客户编号与表内一致,购电次数=（表内购电次数+2）,请稍候....");
            //购电金额+购电次数+客户编号
            for (int i = 0; i < BwCount; i++)
            {
                BuyCount[i] = "00000002";
                BuyMoney[i] = "00002710";
                BuyKhID[i] = "112233445566";
                strData[i] = BuyMoney[i] + BuyCount[i] + BuyKhID[i];
            }
            bInMoney = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "充值命令购电次数3", Common.StringConverToIntger(BuyCount));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount, out strSyMoney);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内购电次数3",Common.StringConverToIntger(strGdCount));
            strSyMoney = Common.HexConverToDecimalism(strSyMoney);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            Common.Memset(ref blnErrRet, false);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(strErrInfo[i]))
                {
                    if ((Convert.ToInt32(strErrInfo[i], 16) & 0x0020) == 0x0020)
                    {
                        ResultDictionary["应答充值次数错误3"][i] = "是";
                        blnErrRet[i] = true;
                    }
                    else
                    {
                        ResultDictionary["应答充值次数错误3"][i] = "无";
                        blnErrRet[i] = false;
                    }
                }
                else
                {
                    ResultDictionary["应答充值次数错误3"][i] = "异常";
                    blnErrRet[i] = false;
                }
                if (bInMoney[i])
                {
                    ResultDictionary["购电次数比表内大2不可远程充值3"][i] = "不通过";
                }
                else if (!bInMoney[i] && blnErrRet[i])
                {
                    ResultDictionary["购电次数比表内大2不可远程充值3"][i] = "通过";
                    blnRet[i, 2] = true;
                }
                ResultDictionary["剩余金额前一后3"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];

            }
            UploadTestResult("剩余金额前一后3");
            UploadTestResult("应答充值次数错误3");
            UploadTestResult("购电次数比表内大2不可远程充值3");


            //4---------------
            string[] strTjMoney = new string[BwCount];
            if (Stop) return;
            Common.Memset(ref strRevCode, "04001004");
            Common.Memset(ref strTjMoney, "500");
            Common.Memset(ref strData, "04001004" + "00050000");
            MessageController.Instance.AddMessage("正在设置囤积金额限值,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "囤积金额限值4", strTjMoney);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoneyQ);
            strGdCountQ = Common.StringConverToIntger(strGdCountQ);
            strSyMoneyQ = Common.HexConverToDecimalism(strSyMoneyQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送远程充值命令,购电金额<囤积金额限值,客户编号与表内一致,购电次数=（表内购电次数+1）,请稍候....");
            //购电金额+购电次数+客户编号
            for (int i = 0; i < BwCount; i++)
            {
                BuyCount[i] = "00000001";
                BuyMoney[i] = "00004E20";
                BuyKhID[i] = "112233445566";
                strData[i] = BuyMoney[i] + BuyCount[i] + BuyKhID[i];
            }
            bInMoney = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount, out strSyMoney);
            strGdCount = Common.StringConverToIntger(strGdCount);
            strSyMoney = Common.HexConverToDecimalism(strSyMoney);

            string[] strShowData = new string[BwCount];
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电日期,请稍候....");
            string[] strBuyDate = MeterProtocolAdapter.Instance.ReadData("03330101", 5);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电日期4", strBuyDate);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数,请稍候....");
            strShowData = MeterProtocolAdapter.Instance.ReadData("03330201", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后总购电次数4", Common.StringConverToIntger(strShowData));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电金额,请稍候....");
            strShowData = MeterProtocolAdapter.Instance.ReadData("03330301", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电金额4", Common.StringConverToDecima(strShowData));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电前剩余金额,请稍候....");
            strShowData = MeterProtocolAdapter.Instance.ReadData("03330401", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电前剩余金额4", Common.StringConverToDecima(strShowData));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电后剩余金额,请稍候....");
            strShowData = MeterProtocolAdapter.Instance.ReadData("03330501", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后剩余金额4", Common.StringConverToDecima(strShowData));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电后累计购电金额,请稍候....");
            string[] strMoneyLast1 = MeterProtocolAdapter.Instance.ReadData("03330601", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后累计购电金额4", Common.StringConverToDecima(strMoneyLast1));

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (bInMoney[i] && strSyMoney[i] == "300" && strMoneyLast1[i] == "00030000" )
                {
                    blnRet[i, 3] = true;
                }
                ResultDictionary["购电次数前一后4"][i] = strGdCountQ[i] + "-" + strGdCount[i];
                ResultDictionary["剩余金额前一后4"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
                ResultDictionary["购电次数比表内大1可远程充值4"][i] = blnRet[i, 3] ? "通过" : "不通过";
            }
            UploadTestResult("购电次数前一后4");
            UploadTestResult("剩余金额前一后4");
            UploadTestResult("购电次数比表内大1可远程充值4");

            //5-----------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表剩余金额及购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoneyQ);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内购电次数5", Common.StringConverToIntger(strGdCountQ));
            strSyMoneyQ = Common.HexConverToDecimalism(strSyMoneyQ);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送远程充值命令：客户编号与表内一致,购电次数=表内购电次数,请稍候....");
            //购电金额+购电次数+客户编号
            for (int i = 0; i < BwCount; i++)
            {
                BuyCount[i] = "00000001";
                BuyMoney[i] = "00004E20";
                strData[i] = BuyMoney[i] + BuyCount[i] + BuyKhID[i];
            }
            bInMoney = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "充值命令购电次数5", Common.StringConverToIntger(BuyCount));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表剩余金额及购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount, out strSyMoney);
            strSyMoney = Common.HexConverToDecimalism(strSyMoney);


            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                Common.Memset(ref blnErrRet, false);
                if (!string.IsNullOrEmpty(strErrInfo[i]))
                {
                    if ((Convert.ToInt32(strErrInfo[i], 16) & 0x0002) == 0x0002)
                    {
                        ResultDictionary["应答重复充值5"][i] = "是";
                        blnErrRet[i] = true;
                    }
                    else
                    {
                        ResultDictionary["应答重复充值5"][i] = "无";
                        blnErrRet[i] = false;
                    }
                }
                else
                {
                    ResultDictionary["应答重复充值5"][i] = "异常";
                    blnErrRet[i] = false;
                }
                if (!bInMoney[i] && strSyMoney[i] == "300" && blnErrRet[i]) 
                {
                    blnRet[i, 4] = true;
                }
                ResultDictionary["剩余金额前一后5"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
                ResultDictionary["购电次数与表内相等不可远程充值5"][i] = blnRet[i, 4] ? "通过" : "不通过";
            }
            UploadTestResult("剩余金额前一后5");
            UploadTestResult("应答重复充值5");
            UploadTestResult("购电次数与表内相等不可远程充值5");

            //6-------------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取囤积金额限值,请稍候....");
            strTjMoney = MeterProtocolAdapter.Instance.ReadData("04001004", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "囤积金额限值6",Common.StringConverToDecima(strTjMoney));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表剩余金额及购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoneyQ);
            strSyMoneyQ = Common.HexConverToDecimalism(strSyMoneyQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送远程充值命令：（购电金额+剩余金额）＞囤积金额限值,请稍候....");
            //购电金额+购电次数+客户编号
            for (int i = 0; i < BwCount; i++)
            {
                BuyCount[i] = "00000002";
                BuyMoney[i] = "00004E84";
                strData[i] = BuyMoney[i] + BuyCount[i] + BuyKhID[i];
            }
            bInMoney = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "购电金额6", Common.HexConverToDecimalism(BuyMoney));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表剩余金额及购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount, out strSyMoney);
            strSyMoney = Common.HexConverToDecimalism(strSyMoney);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                Common.Memset(ref blnErrRet, false);
                if (!string.IsNullOrEmpty(strErrInfo[i]))
                {
                    if ((Convert.ToInt32(strErrInfo[i], 16) & 0x0040) == 0x0040)
                    {
                        ResultDictionary["应答购电超囤积6"][i] = "是";
                        blnErrRet[i] = true;
                    }
                    else
                    {
                        ResultDictionary["应答购电超囤积6"][i] = "无";
                        blnErrRet[i] = false;
                    }
                }
                else
                {
                    ResultDictionary["应答购电超囤积6"][i] = "异常";
                    blnErrRet[i] = false;
                }
                if (!bInMoney[i] && blnErrRet[i])
                {
                    ResultDictionary["购电超囤积不可远程充值6"][i] = "通过";
                    blnRet[i, 5] = true;
                }
                else
                {
                    ResultDictionary["购电超囤积不可远程充值6"][i] = "不通过";
                }
                ResultDictionary["剩余金额前一后6"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
            }
            UploadTestResult("剩余金额前一后6");
            UploadTestResult("应答购电超囤积6");
            UploadTestResult("购电超囤积不可远程充值6");

            //处理结论
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4] && blnRet[i, 5] )
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
