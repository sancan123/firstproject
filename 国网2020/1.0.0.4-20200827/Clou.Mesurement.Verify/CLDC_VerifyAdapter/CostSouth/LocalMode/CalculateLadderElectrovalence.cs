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
using System.Threading;
using System.Globalization;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 阶梯电价结算
    /// </summary>
    public class CalculateLadderElectrovalence : VerifyBase
    {
        protected override string ItemKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string ResultKey
        {
            get { throw new NotImplementedException(); }
        }

        //理论电能总增量|实际电能总增量|剩余电能量递减准确度|起始剩余金额|结束理论剩余金额|结束实际剩余金额|理论金额差值|实际金额差值|剩余金额递减准确度

        //P+起码|P+止码|P+增量|剩余金额起始值|剩余金额实际止值|剩余金额标准止值|电费结算实际值|电费结算标准值|电费结算
        protected override bool CheckPara()
        {
            ResultNames = new string[] {"设置当前套和备用套费率电价","设置当前套阶梯时区表","设置当前套第1张阶梯表","设置ＣＴ变比","设置ＰＴ变比",
                                        "设置为1月2日","当前阶梯","当前阶梯表",
                                        "P＋起码", "P＋止码", "P＋增量","剩余金额起始值", "剩余金额实际止值", "剩余金额标准止值", 
                                        "电费结算实际值", "电费结算标准值", "当前套第1张阶梯表下电费结算", "结论" };
            return true;
        }

        string strPlan = "";

        public CalculateLadderElectrovalence(object plan)
            : base(plan)
        {
            strPlan = plan.ToString();
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
            string[] strQsSyMoney = new string[BwCount];//起始剩余金额
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
            string[] strID = new string[BwCount];
            string[] strPutApdu = new string[BwCount];
            string[] strLlEnergy = new string[BwCount];//理论电能总增量
            string[] strSjEnergy = new string[BwCount];//实际电能总增量
            string[] strSydlzqd = new string[BwCount];// 剩余电量递减准确度 = 理论总增量-实际总增量
            string[] strLlJineChaZhi = new string[BwCount];
            string[] strSjJineChaZhi = new string[BwCount];
            string[] strSyJineDjZqd = new string[BwCount];// 剩余金额递减准确度
            string[] strErrInfo = new string[BwCount];

            bool[] bSetTFldj = new bool[BwCount]; //设置当前套和备用套费率电价
            bool[] bChangSetTFldj = new bool[BwCount];
            bool[] bSetJtsqb = new bool[BwCount];//设置当前套阶梯时区表
            bool[] bChangSetJtsqb = new bool[BwCount];
            bool[] bSetJtb = new bool[BwCount];//设置当前套第1张阶梯表
            bool[] bChangSetJtb = new bool[BwCount];
            bool[] bSetCT = new bool[BwCount];//设置CT
            bool[] bSetPT = new bool[BwCount];//设置PT
            bool[] bSetTime = new bool[BwCount];//设置为1月2日
            bool[] bSetDqjt = new bool[BwCount];//当前阶梯
            bool[] bSetDqjtb = new bool[BwCount];//当前阶梯表

            #region 方案配置参数

            //strPlan = 钱包初始化金额|电压变比|电流变比|走字电流|走字时间|误差值
            string[] ArrPlan = strPlan.Split('|');
            string strMoneyTmp = ArrPlan[0];
            string strVoltageScale = ArrPlan[1].PadLeft(6, '0');
            string strCurrentScale = ArrPlan[2].PadLeft(6, '0');
            float Current = 0;
            int ZzTime = (int)(float.Parse(ArrPlan[4]) * 60);
            float ErrMoney = float.Parse(ArrPlan[5]);

            string strMoney = "";
            if (strMoneyTmp.Contains("."))
            {
                strMoney = strMoney.Split('.')[1];
                if (strMoney.Length == 2)
                {
                    strMoneyTmp = strMoneyTmp.Replace(".", "");
                }
                else if (strMoney.Length == 1)
                {
                    strMoneyTmp = strMoneyTmp.Replace(".", "") + "0";
                }
                int iMoneyTmp = Convert.ToInt32(strMoneyTmp);
                strMoney = (iMoneyTmp).ToString("X2").PadLeft(8, '0');
            }
            else
            {
                strMoney = (Convert.ToInt32(strMoneyTmp) * 100).ToString("X2").PadLeft(8, '0');
            }

            if (ArrPlan[3] == "Imax")
            {
                Current = GlobalUnit.Imax;
            }
            else if (ArrPlan[3] == "0.5Imax")
            {
                Current = GlobalUnit.Imax / 2;
            }
            else
            {
                Current = int.Parse(ArrPlan[3].Replace("Ib", "")) * GlobalUnit.Ib;
            }



            #endregion

            #region 准备步骤
            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置透支金额限值0元,请稍候....");
            Common.Memset(ref strRevCode, "04001003");
            Common.Memset(ref strData, "04001003" + "00000000");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
            //Common.Memset(ref strData, "000186A0");
            Common.Memset(ref strData, strMoney);
            bool[] blnQbRet = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行远程开户,请稍候....");
            Common.Memset(ref strData, "00000000" + "00000000" + "112233445566");
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在对时,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strID[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);



            //设置备用套费率
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置备用套费率,请稍候....");
            Common.Memset(ref strID, "040502FF");
            Common.Memset(ref strData, "00000000" + "00000000" + "00000000" + "00000000");
            Common.Memset(ref strPutApdu, "04D6840414");
            bSetTFldj = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置两套费率电价切换时间,请稍候....");
            Common.Memset(ref strID, "04000108");
            Common.Memset(ref strData, DateTime.Now.AddMinutes(-50).ToString("yyMMddHHmm"));
            Common.Memset(ref strPutApdu, "04D6810A09");
            bChangSetTFldj = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置备用套阶梯值,请稍候....");
            Common.Memset(ref strID, "04060AFF");
            Common.Memset(ref strData, "00000010" + "00000020" + "00000030" + "00000040" + "00000050" + "00000060"
                        + "00010000" + "00020000" + "00030000" + "00040000" + "00050000" + "00060000" + "00070000"
                        + "010100" + "010100" + "010100" + "010100" + "010100" + "010100");
            Common.Memset(ref strPutApdu, "04D684344A");
            bSetJtb = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置两套阶梯切换时间,请稍候....");
            Common.Memset(ref strID, "04000109");
            Common.Memset(ref strData, DateTime.Now.AddMinutes(-5).ToString("yyMMddHHmm"));
            Common.Memset(ref strPutApdu, "04D684C009");
            bChangSetJtb = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置备用套阶梯时区表,请稍候....");
            Common.Memset(ref strID, "04070001");
            Common.Memset(ref strData, "04070001" + "010101" + "050102" + "110102" + "110102");
            bSetJtsqb = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置两套阶梯时区切换时间,请稍候....");
            Common.Memset(ref strID, "0400010A");
            Common.Memset(ref strData, "0400010A" + DateTime.Now.AddMinutes(-5).ToString("yyMMddHHmm"));
            bChangSetJtsqb = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置电流变比,请稍候....");
            Common.Memset(ref strID, "04000306");
            //Common.Memset(ref strData, "000003");
            Common.Memset(ref strData, strCurrentScale);
            Common.Memset(ref strPutApdu, "04D6811807");
            bSetCT = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置电压变比,请稍候....");
            Common.Memset(ref strID, "04000307");
            //Common.Memset(ref strData, ("000002"));
            Common.Memset(ref strData, strVoltageScale);
            Common.Memset(ref strPutApdu, "04D6811B07");
            bSetPT = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间到1月2日....");
            Common.Memset(ref strRevCode, "04000101");
            string strDatetime = DateTime.Now.ToString("yy") + "-01-02";
            DateTime dt = DateTime.Parse(strDatetime, DateTimeFormatInfo.CurrentInfo);
            Common.Memset(ref strData, "04000101" + DateTime.Now.ToString("yy") + "0102" + "0" + ((int)dt.DayOfWeek).ToString());
            bSetTime = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            #endregion


            int SumBianb = int.Parse(strVoltageScale) * int.Parse(strCurrentScale);
            //ErrMoney = 0.01f * 7 * SumBianb;

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                ResultDictionary["设置当前套和备用套费率电价"][i] = (bSetTFldj[i] && bChangSetTFldj[i]) ? "成功" : "失败";
                ResultDictionary["设置当前套阶梯时区表"][i] = (bSetJtsqb[i] && bChangSetJtsqb[i]) ? "成功" : "失败";
                ResultDictionary["设置当前套第1张阶梯表"][i] = (bSetJtb[i] && bChangSetJtb[i]) ? "成功" : "失败";
                ResultDictionary["设置ＣＴ变比"][i] = bSetCT[i] ? "成功" : "失败";
                ResultDictionary["设置ＰＴ变比"][i] = bSetPT[i] ? "成功" : "失败";
                ResultDictionary["设置为1月2日"][i] = bSetTime[i] ? "成功" : "失败";
            }
            UploadTestResult("设置当前套和备用套费率电价");
            UploadTestResult("设置当前套阶梯时区表");
            UploadTestResult("设置当前套第1张阶梯表");
            UploadTestResult("设置ＣＴ变比");
            UploadTestResult("设置ＰＴ变比");
            UploadTestResult("设置为1月2日");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            string[] status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if ((Convert.ToInt32(status[i], 16) & 0x0800) == 0x0800)
                    {
                        ResultDictionary["当前阶梯"][i] = "备用套阶梯";
                    }
                    else
                    {
                        ResultDictionary["当前阶梯"][i] = "当前套阶梯";
                    }
                    if ((Convert.ToInt32(status[i], 16) & 0x0400) == 0x0400)
                    {
                        ResultDictionary["当前阶梯表"][i] = "第2张";
                    }
                    else
                    {
                        ResultDictionary["当前阶梯表"][i] = "第1张";
                    }
                }
                else
                {
                    ResultDictionary["当前阶梯"][i] = "异常";
                    ResultDictionary["当前阶梯表"][i] = "异常";
                }
            }
            UploadTestResult("当前阶梯");
            UploadTestResult("当前阶梯表");

            //1-------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
            //起始剩余金额
            strQsSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);

            for (int i = 0; i < strQsSyMoney.Length; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                strQsSyMoney[i] = string.IsNullOrEmpty(strQsSyMoney[i]) ? "0" : (Convert.ToDouble(strQsSyMoney[i]) / 100).ToString("F2");
            }

            //剩余金额起始值
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额起始值", strQsSyMoney);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取起码,请稍候....");
            float[] QiMaF = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)0);
            //P+起码
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "P＋起码", GetConvertEnegy(QiMaF));


            if (Stop) return;
            MessageController.Instance.AddMessage("正在走字分钟,请稍候....");
            bool blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Imax, 1, 1, "1.0", true, false);

            if (Stop) return;
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 20);

            if (Stop) return;
            PowerOn();


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取止码,请稍候....");
            float[] ZiMaF = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)0);
            //P+止码
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "P＋止码", GetConvertEnegy(ZiMaF));


            MessageController.Instance.AddMessage("正在读取剩余金额....");
            strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在对时,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strID[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

            string[] strLlSyMoney = new string[BwCount];
            MessageController.Instance.AddMessage("正在计算结果,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                Double MaCha = Convert.ToDouble((ZiMaF[i] - QiMaF[i]).ToString("0.00"));

                strSjEnergy[i] = MaCha.ToString("F2");
                Double JtMaCha = 0f;
                JtMaCha = MaCha;
                Double UseMenoy = 0f;


                UseMenoy = CalculateBzMoney(JtMaCha, SumBianb);

                strLlJineChaZhi[i] = UseMenoy.ToString("F2");
                strLlSyMoney[i] = (Convert.ToDouble(strMoneyTmp) - UseMenoy).ToString("F2");

                strSyMoney[i] = string.IsNullOrEmpty(strSyMoney[i]) ? "0" : (Convert.ToDouble(strSyMoney[i]) / 100).ToString("F2");

                //实际剩余金额差值
                Double fJinEChaZhi = Convert.ToDouble(strQsSyMoney[i]) - Convert.ToDouble(strSyMoney[i]);
                strSjJineChaZhi[i] = fJinEChaZhi.ToString("0.00");
                fJinEChaZhi =  Convert.ToDouble(fJinEChaZhi.ToString("0.00"));

                //剩余金额递减准确度
                if (Math.Abs(UseMenoy - fJinEChaZhi) <= ErrMoney && strQsSyMoney[i] != strSyMoney[i])
                {
                    strSyJineDjZqd[i] = "通过";
                }
                else
                {
                    strSyJineDjZqd[i] = "不通过";
                }
            }




            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "P＋增量", strSjEnergy);

            //实际剩余金额
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额实际止值", strSyMoney);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额标准止值", strLlSyMoney);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电费结算实际值", strSjJineChaZhi);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电费结算标准值", strLlJineChaZhi);
            //
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电费结算", strSyJineDjZqd);

            MessageController.Instance.AddMessage("正在处理结果");


            try
            {
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strLlSyMoney[i]) && !string.IsNullOrEmpty(strSyMoney[i]))
                    {
                        if (strSyJineDjZqd[i].Equals("通过"))
                        {
                            ResultDictionary["结论"][i] = "合格";
                        }
                        else
                        {
                            ResultDictionary["结论"][i] = "不合格";
                        }
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
            }
            catch (Exception)
            { }

            UploadTestResult("结论");
        }
        /// <summary>
        /// 转换电量
        /// </summary>
        /// <returns></returns>
        private string[] GetConvertEnegy(float[] enegy)
        {
            string[] strEnegy = new string[enegy.Length];
            for (int i = 0; i < enegy.Length; i++)
            {
                strEnegy[i] = enegy[i].ToString("F2");
            }
            return strEnegy;
        }

        /// <summary>
        ///计算标准误差 
        /// </summary>
        /// <param name="JtMaCha"></param>
        /// <param name="iBb"></param>
        /// <returns></returns>
        private double CalculateBzMoney(double JtMaCha, int iBb)
        {
            double result = -1;
            if (JtMaCha <= 0)
            {
                result = 0;
            }
            else if (JtMaCha > 0 && JtMaCha < 0.1)
            {
                result = 0.1 * 1 * iBb;
            }
            else if (JtMaCha < 0.2)
            {
                result = (0.1 + (JtMaCha - 0.1) * 2) * iBb;
            }
            else if (JtMaCha < 0.3)
            {
                result = (0.1 + 0.2 + (JtMaCha - 0.2) * 3) * iBb;
            }
            else if (JtMaCha < 0.4)
            {
                result = (0.1 + 0.2 + 0.3 + (JtMaCha - 0.3) * 4) * iBb;
            }
            else if (JtMaCha < 0.5)
            {
                result = (0.1 + 0.2 + 0.3 + 0.4 + (JtMaCha - 0.4) * 5) * iBb;
            }
            else if (JtMaCha < 0.6)
            {
                result = (0.1 + 0.2 + 0.3 + 0.4 + 0.5 + (JtMaCha - 0.5) * 6) * iBb;
            }
            else if (JtMaCha < 0.7)
            {
                result = (0.1 + 0.2 + 0.3 + 0.4 + 0.5 + 0.6 + (JtMaCha - 0.6) * 7) * iBb;
            }
            else if (JtMaCha >= 0.7)
            {
                result = (0.1 + 0.2 + 0.3 + 0.4 + 0.5 + 0.6 + +0.7 + (JtMaCha - 0.7) * 7) * iBb;
            }
            return result;
        }
    }
}
