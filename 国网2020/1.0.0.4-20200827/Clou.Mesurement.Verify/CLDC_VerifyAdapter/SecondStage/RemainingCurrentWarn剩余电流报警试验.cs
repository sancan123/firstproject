using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CLDC_DataCore;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_DataCore.Const;

namespace CLDC_VerifyAdapter.SecondStage
{
    class RemainingCurrentWarn: VerifyBase
    {


        #region ----------构造函数----------

        public RemainingCurrentWarn(object plan)
            : base(plan)
        {
        }

        protected override string ResultKey
        {

            //get { throw new NotImplementedException(); }
            get { return null; }
        }

        protected override string ItemKey
        {
            //get { throw new NotImplementedException(); }
            get { return null; }
        }


        protected override bool CheckPara()
        {
            ResultNames = new string[] { "测试时间", "事件产生前事件次数", "设定后阈值","剩余电流值(实际)" ,"电表运行状态(剩余电流超限)", "主动上报状态(剩余电流超限)", "事件产生后事件次数", "事件产生后发生时刻", "结论", "不合格原因" };
            return true;
        }

        private void getRelaySetting(float Current, out int[] OpenRelay, out int[] CloseRelay)
        {
            OpenRelay = new int[1];
            CloseRelay = new int[1];
            if (Current < 20)
            {
                OpenRelay = new int[] { 0 };
                CloseRelay = new int[] { 1, 2, 3, 4, 5 };
            }
            else if (Current < 30)
            {
                OpenRelay = new int[] { 1 };
                CloseRelay = new int[] { 0, 2, 3, 4, 5 };
            }
            else if (Current < 40)
            {
                OpenRelay = new int[] { 0, 1 };
                CloseRelay = new int[] { 2, 3, 4, 5 };
            }
            else if (Current < 50)
            {
                OpenRelay = new int[] { 1, 2 };
                CloseRelay = new int[] { 0, 3, 4, 5 };
            }
            else if (Current < 60)
            {
                OpenRelay = new int[] { 0, 1, 2 };
                CloseRelay = new int[] { 3, 4, 5 };
            }
            else if (Current < 70)
            {
                OpenRelay = new int[] { 0, 3 };
                CloseRelay = new int[] { 1, 2, 4, 5 };
            }
            else if (Current < 80)
            {
                OpenRelay = new int[] { 1, 3 };
                CloseRelay = new int[] { 0, 2, 4, 5 };
            }
            else if (Current < 90)
            {
                OpenRelay = new int[] { 0, 1, 3 };
                CloseRelay = new int[] { 2, 4, 5 };
            }
            else if (Current < 100)
            {
                OpenRelay = new int[] { 1, 2, 3 };
                CloseRelay = new int[] { 0, 4, 5 };
            }
            else if (Current < 200)
            {
                OpenRelay = new int[] { 0, 1, 2, 3 };
                CloseRelay = new int[] { 4, 5 };
            }
            else if (Current < 300)
            {
                OpenRelay = new int[] { 0, 1, 2, 3, 4 };
                CloseRelay = new int[] { 5 };
            }
            else if (Current < 400)
            {
                OpenRelay = new int[] { 0, 1, 2, 3, 5 };
                CloseRelay = new int[] { 4 };
            }
            else
            {
                OpenRelay = new int[] { 0, 1, 2, 3, 4, 5 };
                CloseRelay = new int[] { 6 };
            }
        }

        #endregion
        public override void Verify()
        {
            base.Verify();
            bool bPowerOn = PowerOn();          
            int[] OpenRelay;
            int[] CloseRelay;
            float RemainingCurrent = 0;
            if (!string.IsNullOrEmpty(VerifyPara))
            {
                RemainingCurrent = int.Parse(VerifyPara.Replace("mA",""));
            }
            else
            {
                MessageController.Instance.AddMessage("请在方案里输入剩余电流值");
                Stop = true;
                return;
            }
            if (Stop) return;
            string[] strID = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strSetData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strShowData = new string[BwCount];
            string[] strCode = new string[BwCount];
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] Fail = new string[BwCount];
            string[] strDataCode = new string[BwCount];
            OpenRelay=new int[] {6};
            CloseRelay = new int[] { 0, 1, 2, 3, 4, 5 };
         
            Helper.EquipHelper.Instance.SetRelay(OpenRelay, CloseRelay);
            MessageController.Instance.AddMessage("读取剩余电流监测报警事件产生前总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("035C0000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);
            if (Stop) return;
            MessageController.Instance.AddMessage("开启新功能");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);          
            Common.Memset(ref strDataCode, "04091802");
            Common.Memset(ref strData, "04091802" + "001F");
            bool[] bResult_xgn= MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!bResult_xgn[i])
                    {
                        Fail[i] = "开启新功能失败,";
                    }
                }
            }
            MessageController.Instance.AddMessage("读取报警输出配置模式字");
            string[] Bjscpzmsz = MeterProtocolAdapter.Instance.ReadData("04001801",4);

            MessageController.Instance.AddMessage("设置报警输出配置模式字，开启剩余电流超限");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            Common.Memset(ref strDataCode, "04001801");
            Common.Memset(ref strData, "04001801" + "00000400");
            bool[] bResult_bjscpzmsz= MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            if (Stop) return;
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!bResult_bjscpzmsz[i])
                    {
                        Fail[i] = Fail[i] + "设置报警输出配置模式字失败,";
                    }
                }
            }

            MessageController.Instance.AddMessage("读取报警输出配置模式字");
            string[] Bjscpzmsz1 = MeterProtocolAdapter.Instance.ReadData("04001801", 4);

            if (Stop) return;
            MessageController.Instance.AddMessage("读取主动上报模式字");
            string[] Zdsbmsz = MeterProtocolAdapter.Instance.ReadData("04001104", 8);

            MessageController.Instance.AddMessage("设置主动上报模式字，开启剩余电流超限");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            Common.Memset(ref strDataCode, "04001104");
            Common.Memset(ref strData, "04001104" + "0000000200000000");
            bool[] bResult_zdsbmsz = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!bResult_zdsbmsz[i])
                    {
                        Fail[i] = Fail[i] + "设置主动上报模式字失败,";
                    }
                }
            }
            MessageController.Instance.AddMessage("读取主动上报模式字");
            string[] Zdsbmsz1 = MeterProtocolAdapter.Instance.ReadData("04001104", 8);
            string YZ = RemainingCurrent.ToString();
            if (RemainingCurrent > 300)
            {
                YZ = "300";
            }
            MessageController.Instance.AddMessage("设置剩余电流报警判断阈值");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            Common.Memset(ref strDataCode, "04092301");
            Common.Memset(ref strData, "04092301" + YZ.PadLeft(4, '0'));
            bool[] bResult_sydlbjpdfz = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!bResult_sydlbjpdfz[i])
                    {
                        Fail[i] = Fail[i] + "设置剩余电流报警判断阈值失败,";
                    }
                }
            }
            MessageController.Instance.AddMessage("读取剩余电流报警判断阈值");
            string[] Sydlbjyz = MeterProtocolAdapter.Instance.ReadData("04092301", 2);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "设定后阈值", Sydlbjyz);
            if (Stop) return;
            MessageController.Instance.AddMessage("设置剩余电流报警判定延时时间");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            Common.Memset(ref strDataCode, "04092302");
            Common.Memset(ref strData, "04092302" + "10");
            bool[] bResult_sydlbjpdsj = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!bResult_sydlbjpdsj[i])
                    {
                        Fail[i] = Fail[i] + "设置剩余电流报警判定延时时间失败,";
                    }
                }
            }
            if (Stop) return;

            MessageController.Instance.AddMessage("开始进行剩余电流报警试验");
        
            getRelaySetting(RemainingCurrent, out OpenRelay, out CloseRelay);
            bool ret = Helper.EquipHelper.Instance.SetRelay(OpenRelay, CloseRelay);
            if (!ret)
            {
                throw new Exception("剩余电流设置失败");
            }
            ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 10 * GlobalUnit.Itr, 1, 1, "1.0", true, false);
            if (!ret)
            {
                throw new Exception("升源失败");
            }
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 90);
            MessageController.Instance.AddMessage("读取剩余电流值(实际)");
            float[] flt_DL10 = MeterProtocolAdapter.Instance.ReadData("02800030", 3, 3);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["剩余电流值(实际)"][i] = flt_DL10[i].ToString();

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余电流值(实际)", ResultDictionary["剩余电流值(实际)"]);
            if (Stop) return;
            MessageController.Instance.AddMessage("读取电表运行状态字1");
            string[] Dbyxztz1 = MeterProtocolAdapter.Instance.ReadData("04000501", 2);
            string[] Dbzt = new string[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!string.IsNullOrEmpty(Dbyxztz1[i]))
                    {
                        string str = CLDC_DataCore.Function.Common.HexStrToBinStr(Dbyxztz1[i]);
                        if (str.Substring(13, 1) == "1")
                        {
                            Dbzt[i] = "发生";
                        }
                        else
                        {
                            Dbzt[i] = "未发生";
                            Fail[i] = Fail[i] + "电表运行状态未发生剩余电流超限,";
                        }
                    }
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电表运行状态(剩余电流超限)", Dbzt);


            if (Stop) return;


            MessageController.Instance.AddMessage("读取主动上报状态字");
            string[] Zdsbztz = MeterProtocolAdapter.Instance.ReadData("04001501", 12);

            string[] Zdsb = new string[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!string.IsNullOrEmpty(Zdsbztz[i]))
                    {
                        string str = CLDC_DataCore.Function.Common.HexStrToBinStr(Zdsbztz[i]);
                        if (str.Substring(30, 1) == "1")
                        {
                            Zdsb[i] = "发生";
                        }
                        else
                        {
                            Zdsb[i] = "未发生";
                            Fail[i] = Fail[i] + "主动上报状态未发生剩余电流超限,";
                        }
                    }
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "主动上报状态(剩余电流超限)", Zdsb);

            if (Stop) return;

            MessageController.Instance.AddMessage("读取剩余电流监测报警事件产生后事件总次数");
            string[] Sydljcbjsjzcs = MeterProtocolAdapter.Instance.ReadData("035C0000", 3);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!string.IsNullOrEmpty(Sydljcbjsjzcs[i]) && !string.IsNullOrEmpty(strLoseCountQ[i]))
                    {
                        float str = float.Parse(Sydljcbjsjzcs[i]) - float.Parse(strLoseCountQ[i]);
                        if (str>0)
                        {
                            
                        }
                        else
                        {

                            Fail[i] = Fail[i] + "剩余电流监测报警事件未产生,";
                        }
                    }
                    else
                    {
                        Fail[i] = Fail[i] + "剩余电流监测报警事件未产生,";
                    }
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", Sydljcbjsjzcs);


            if (Stop) return;

            MessageController.Instance.AddMessage("读取上1次剩余电流监测报警事件发生时刻");
            string[] Sydlfssj = MeterProtocolAdapter.Instance.ReadData("035C0001", 6);

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!string.IsNullOrEmpty(Sydlfssj[i]))
                    {
                        if (float.Parse(Sydlfssj[i]) == 0)
                        {
                            Fail[i] = Fail[i] + "剩余电流监测报警事件未产生";
                        }
                    }
                    else
                    {
                        Fail[i] = Fail[i] + "剩余电流监测报警事件未产生";
                    }
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后发生时刻", Sydlfssj);





            if (Stop) return;
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!string.IsNullOrEmpty(Fail[i]))
                    {
                        ResultDictionary["不合格原因"][i] = Fail[i];
                        ResultDictionary["结论"][i] = "不合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = " 合格";
                    }
                   

                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);


            MessageController.Instance.AddMessage("设置剩余电流报警判断阈值");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            Common.Memset(ref strDataCode, "04092301");
            Common.Memset(ref strData, "04092301" + "0030");
            bool[] bResult_sydlbjpdfz2 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);


            MessageController.Instance.AddMessage("设置剩余电流报警判定延时时间");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            Common.Memset(ref strDataCode, "04092302");
            Common.Memset(ref strData, "04092302" + "60");
            bool[] bResult_sydlbjpdsj2 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
         
            
            MessageController.Instance.AddMessage("设置报警输出配置模式字");
            for (int i = 0; i < BwCount; i++)
            {
                if (!string.IsNullOrEmpty(Bjscpzmsz[i]))
                {
                    strData[i] = "04001801" + Bjscpzmsz[i];
                }
            }
            Common.Memset(ref strDataCode, "04001801");
            bool[] bResult1 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
          
            
            MessageController.Instance.AddMessage("设置主动上报模式字");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            for (int i = 0; i < BwCount; i++)
            {
                if (!string.IsNullOrEmpty(Zdsbmsz[i]))
                {
                    strData[i] = "04001104" + Zdsbmsz[i];
                }
            }
            Common.Memset(ref strDataCode, "04001104");
            bool[] bResult2 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
        
        }

        /// <summary>
        /// 格式化写字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="strformat"></param>
        /// <param name="len"></param>
        /// <param name="pointindex"></param>
        /// <returns></returns>
        private string FormatWriteData(string data, string strformat, int len, int pointindex)
        {
            string formatdata = "";
            try
            {
                if (data == "" || data == null) return "";
                formatdata = data;
                bool blnIsNum = true;           //判断读取的数据是不是数字
                List<char> splitChar = new List<char>(new char[] { '.', 'N' });
                for (int i = 0; i < strformat.Length; i++)
                {
                    if (!splitChar.Contains(strformat[i]))
                    {
                        blnIsNum = false;
                        break;
                    }
                }
                if (pointindex != 0)
                {
                    if (blnIsNum)
                    {
                        int left = len * 2 - pointindex;
                        int right = pointindex;
                        formatdata = float.Parse(formatdata).ToString();
                        string[] newdata = formatdata.Split('.');
                        if (newdata.Length == 1)
                        {
                            if (newdata[0].Length <= left)
                            {
                                newdata[0] = newdata[0].PadLeft(left, '0');
                            }
                            else
                            {
                                newdata[0] = newdata[0].Substring(0, left);
                            }
                            formatdata = newdata[0] + "".PadRight(right, '0');
                        }
                        else
                        {
                            if (newdata[0].Length <= left)
                            {
                                newdata[0] = newdata[0].PadLeft(left, '0');
                            }
                            else
                            {
                                newdata[0] = newdata[0].Substring(0, left);
                            }
                            if (newdata[1].Length <= right)
                            {
                                newdata[1] = newdata[1].PadRight(right, '0');
                            }
                            else
                            {
                                newdata[1] = newdata[1].Substring(0, right);
                            }
                            formatdata = newdata[0] + newdata[1];
                        }
                    }
                    else
                    {
                        formatdata = formatdata.Replace(".", "");
                        formatdata = formatdata.Replace("-", "");
                        if (formatdata.Length <= len * 2)
                        {
                            formatdata = formatdata.PadRight(len * 2, '0');
                        }
                        else
                        {
                            formatdata = formatdata.Substring(0, len * 2);
                        }
                    }
                }
                else
                {
                    if (formatdata.Length <= len * 2)
                    {
                        formatdata = formatdata.PadLeft(len * 2, '0');
                    }
                    else
                    {
                        formatdata = formatdata.Substring(0, len * 2);
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogHelper.Instance.WriteInfo(ex.StackTrace);
            }
            return formatdata;
        }

    }
}