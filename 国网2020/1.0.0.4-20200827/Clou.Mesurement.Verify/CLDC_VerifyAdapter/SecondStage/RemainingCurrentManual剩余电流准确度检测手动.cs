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
    /// <summary>
    /// 剩余电流准确度检测-手动检测
    /// 
    /// </summary>
    class RemainingCurrentManual : VerifyBase
    {


        #region ----------构造函数----------

        public RemainingCurrentManual(object plan)
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
            ResultNames = new string[] { "测试时间", "实际剩余电流","标准剩余电流","差值","结论","不合格原因" };
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
                RemainingCurrent = int.Parse(VerifyPara.Replace("mA", ""));
            }
            else
            {
                MessageController.Instance.AddMessage("请在方案里输入剩余电流值");
                Stop = true;
                return;
            }


            string[] Fail = new string[BwCount];         
            OpenRelay=new int[] {6};
            CloseRelay = new int[] { 0, 1, 2, 3, 4, 5 };
         
            Helper.EquipHelper.Instance.SetRelay(OpenRelay, CloseRelay);
            MessageController.Instance.AddMessage("正在进行10Itr,"+RemainingCurrent.ToString()+"mA试验，请稍候......");
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
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);
            MessageController.Instance.AddMessage("正在读取剩余电流");
            float[] flt_DL = MeterProtocolAdapter.Instance.ReadData("02800030", 3, 3);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["实际剩余电流"][i] = flt_DL[i].ToString();

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "实际剩余电流", ResultDictionary["实际剩余电流"]);

            CLDC_DataCore.Struct.StPower Datas = Helper.EquipHelper.Instance.ReadPowerInfo();
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["标准剩余电流"][i] = Datas.Ia.ToString("0.0000");
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "标准剩余电流", ResultDictionary["标准剩余电流"]);
            float[] flt_cz = new float[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (flt_DL[i] == -1)
                    {
                        Fail[i] = Fail[i] + "读取剩余电流失败";
                        flt_cz[i] = -999;
                        ResultDictionary["差值"][i] = flt_cz[i].ToString();
                    }
                    else if (Datas.Ia == -1)
                    {
                        Fail[i] = Fail[i] + "10mA时读取标准电流失败";
                        flt_cz[i] = -999;
                        ResultDictionary["差值"][i] = flt_cz[i].ToString();
                    }
                    else
                    {
                        flt_cz[i] = Math.Abs(flt_DL[i] - Datas.Ia);
                        ResultDictionary["差值"][i] = flt_cz[i].ToString("0.00000");
                        if (RemainingCurrent < 300)
                        {
                            if (flt_cz[i] > 0.003)
                            {
                                Fail[i] = Fail[i] + "差值大于3mA";
                            }
                        }
                        else
                        {
                            if (flt_DL[i] < 297.0)
                            {
                                Fail[i] = Fail[i] + "剩余电流小于297mA";
                            }
                        }
                        

                    }
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "差值", ResultDictionary["差值"]);         
          
            MessageController.Instance.AddMessage("正在恢复电压，继电器");
            bPowerOn = PowerOn();
            OpenRelay = new int[] { 6 };
            CloseRelay = new int[] { 0, 1, 2, 3, 4, 5 };

            Helper.EquipHelper.Instance.SetRelay(OpenRelay, CloseRelay);

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