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
    /// 剩余电流准确度检测
    /// </summary>
    class RemainingCurrent : VerifyBase
    {


        #region ----------构造函数----------

        public RemainingCurrent(object plan)
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
            ResultNames = new string[] { "测试时间", "10Itr", "Imax", "10mA实际", "10mA标准", "10mA差值","30mA实际", "30mA标准", "30mA差值", "100mA实际", "100mA标准", "100mA差值","400mA实际", "结论", "不合格原因" };
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



            string[] Fail = new string[BwCount];


          
            OpenRelay=new int[] {6};
            CloseRelay = new int[] { 0, 1, 2, 3, 4, 5 };
         
            Helper.EquipHelper.Instance.SetRelay(OpenRelay, CloseRelay);
            MessageController.Instance.AddMessage("正在进行1min，10Itr平衡试验，请稍候......");

            bool ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 10 * GlobalUnit.Itr, 1, 1, "1.0", true, false);
            if (!ret)
            {
                throw new Exception("升源失败");
            }
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);
            MessageController.Instance.AddMessage("正在读取剩余电流");
            float[] flt_DLItr = MeterProtocolAdapter.Instance.ReadData("02800030", 3, 3);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["10Itr"][i] = flt_DLItr[i].ToString();

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "10Itr", ResultDictionary["10Itr"]);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (flt_DLItr[i] == -1 )
                    {
                        Fail[i] = "10Itr时读取剩余电流失败";
                    }
                    string str = flt_DLItr[i].ToString("0.0000");
                
                    if (double.Parse(str) > 0.003)
                    {
                        Fail[i] = "10Itr时读取剩余电流大于3mA";
                    }
                }
            }
            Helper.EquipHelper.Instance.PowerOff();
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行1min，Imax平衡试验，请稍候......");

           ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Imax, 1, 1, "1.0", true, false);
            if (!ret)
            {
                throw new Exception("升源失败");
            }
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);
            MessageController.Instance.AddMessage("正在读取剩余电流");
            float[] flt_DLImax = MeterProtocolAdapter.Instance.ReadData("02800030", 3, 3);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["Imax"][i] = flt_DLImax[i].ToString();

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "Imax", ResultDictionary["Imax"]);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (flt_DLImax[i] == -1)
                    {
                        Fail[i] = Fail[i] + "，Imax时读取剩余电流失败";
                    }
                    string str = flt_DLImax[i].ToString("0.0000");
                    if (double.Parse(str) > 0.003)
                    {
                        Fail[i] = "10Imax时读取剩余电流大于3mA";
                    }

                   
                }
            }
            Helper.EquipHelper.Instance.PowerOff();
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行10Itr,10mA不平衡试验，请稍候......");
           
            MessageController.Instance.AddMessage("设置电流，请稍候......");
            RemainingCurrent = 10;
            getRelaySetting(RemainingCurrent, out OpenRelay, out CloseRelay);
            ret = Helper.EquipHelper.Instance.SetRelay(OpenRelay, CloseRelay);
            if (!ret)
            {
                throw new Exception("剩余电流设置失败");
            }
            ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 10*GlobalUnit.Itr, 1, 1, "1.0", true, false);
            if (!ret)
            {
                throw new Exception("升源失败");
            }
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            if (Stop) return;

            MessageController.Instance.AddMessage("正在读取剩余电流");
            float[] flt_DL10 = MeterProtocolAdapter.Instance.ReadData("02800030", 3, 3);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["10mA实际"][i] = flt_DL10[i].ToString();

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "10mA实际", ResultDictionary["10mA实际"]);
            CLDC_DataCore.Struct.StPower Datas10mA = Helper.EquipHelper.Instance.ReadPowerInfo();
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["10mA标准"][i] = Datas10mA.Ia.ToString("0.0000");
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "10mA标准", ResultDictionary["10mA标准"]);
            float[] flt_cz10 = new float[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (flt_DL10[i] == -1 )
                    {
                        Fail[i] = Fail[i] + "，10mA时读取剩余电流失败";
                        flt_cz10[i] = -999;
                         ResultDictionary["10mA差值"][i] =  flt_cz10[i].ToString();
                    }
                    else if (Datas10mA.Ia == -1)
                    {
                        Fail[i] = Fail[i] + "，10mA时读取标准电流失败";
                        flt_cz10[i] = -999;
                         ResultDictionary["10mA差值"][i] =  flt_cz10[i].ToString();
                    }
                  else 
                    {
                        flt_cz10[i] = Math.Abs(flt_DL10[i] - Datas10mA.Ia);
                         ResultDictionary["10mA差值"][i] =  flt_cz10[i].ToString("0.00000");
                        if (flt_cz10[i] > 0.003)
                        {
                            Fail[i] = Fail[i] + "，10mA时差值大于3mA";
                        }
                       
                    }
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "10mA差值", ResultDictionary["10mA差值"]);

            Helper.EquipHelper.Instance.PowerOff();
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行10Itr,30mA不平衡试验，请稍候......");

            MessageController.Instance.AddMessage("设置电流，请稍候......");
            RemainingCurrent = 30;
            getRelaySetting(RemainingCurrent, out OpenRelay, out CloseRelay);
            ret = Helper.EquipHelper.Instance.SetRelay(OpenRelay, CloseRelay);
            if (!ret)
            {
                throw new Exception("剩余电流设置失败");
            }
            ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 10 * GlobalUnit.Itr, 1, 1, "1.0", true, false);
            if (!ret)
            {
                throw new Exception("升源失败");
            }
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            if (Stop) return;

            MessageController.Instance.AddMessage("正在读取剩余电流");
            float[] flt_DL30 = MeterProtocolAdapter.Instance.ReadData("02800030", 3, 3);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["30mA实际"][i] = flt_DL30[i].ToString();

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "30mA实际", ResultDictionary["30mA实际"]);
            if (Stop) return;
            CLDC_DataCore.Struct.StPower Datas30mA = Helper.EquipHelper.Instance.ReadPowerInfo();
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["30mA标准"][i] = Datas30mA.Ia.ToString("0.0000");
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "30mA标准", ResultDictionary["30mA标准"]);
            float[] flt_cz30 = new float[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (flt_DL30[i] == -1)
                    {
                        Fail[i] = Fail[i] + "，30mA时读取剩余电流失败";
                        flt_cz30[i] = -999;
                        ResultDictionary["30mA差值"][i] = flt_cz30[i].ToString();
                    }
                    else if (Datas30mA.Ia == -1)
                    {
                        Fail[i] = Fail[i] + "，30mA时读取标准电流失败";
                        flt_cz30[i] = -999;
                        ResultDictionary["30mA差值"][i] = flt_cz30[i].ToString();
                    }
                    else
                    {
                        flt_cz30[i] = Math.Abs(flt_DL30[i] - Datas30mA.Ia);
                        ResultDictionary["30mA差值"][i] = flt_cz30[i].ToString("0.00000");
                        if (flt_cz30[i] > 0.003)
                        {
                            Fail[i] = Fail[i] + "，30mA时差值大于3mA";
                        }

                    }
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "30mA差值", ResultDictionary["30mA差值"]);


            Helper.EquipHelper.Instance.PowerOff();
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行10Itr,100mA不平衡试验，请稍候......");

            MessageController.Instance.AddMessage("设置电流，请稍候......");
            RemainingCurrent = 100;
            getRelaySetting(RemainingCurrent, out OpenRelay, out CloseRelay);
            ret = Helper.EquipHelper.Instance.SetRelay(OpenRelay, CloseRelay);
            if (!ret)
            {
                throw new Exception("剩余电流设置失败");
            }
            ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 10 * GlobalUnit.Itr, 1, 1, "1.0", true, false);
            if (!ret)
            {
                throw new Exception("升源失败");
            }
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            if (Stop) return;

            MessageController.Instance.AddMessage("正在读取剩余电流");
            float[] flt_DL100 = MeterProtocolAdapter.Instance.ReadData("02800030", 3, 3);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["100mA实际"][i] = flt_DL100[i].ToString();

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "100mA实际", ResultDictionary["100mA实际"]);
            if (Stop) return;
            CLDC_DataCore.Struct.StPower Datas100mA = Helper.EquipHelper.Instance.ReadPowerInfo();
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["100mA标准"][i] = Datas100mA.Ia.ToString("0.0000");
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "100mA标准", ResultDictionary["100mA标准"]);
            float[] flt_cz100 = new float[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (flt_DL100[i] == -1)
                    {
                        Fail[i] = Fail[i] + "，100mA时读取剩余电流失败";
                        flt_cz100[i] = -999;
                        ResultDictionary["100mA差值"][i] = flt_cz100[i].ToString();
                    }
                    else if (Datas100mA.Ia == -1)
                    {
                        Fail[i] = Fail[i] + "，100mA时读取标准电流失败";
                        flt_cz100[i] = -999;
                        ResultDictionary["100mA差值"][i] = flt_cz100[i].ToString();
                    }
                    else
                    {
                        flt_cz100[i] = Math.Abs(flt_DL100[i] - Datas100mA.Ia);
                        ResultDictionary["100mA差值"][i] = flt_cz100[i].ToString("0.00000");
                        if (flt_cz100[i] > 0.003)
                        {
                            Fail[i] = Fail[i] + "，100mA时差值大于3mA";
                        }

                    }
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "100mA差值", ResultDictionary["100mA差值"]);



            Helper.EquipHelper.Instance.PowerOff();
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行10Itr,300mA不平衡试验，请稍候......");

            MessageController.Instance.AddMessage("设置电流，请稍候......");
            RemainingCurrent = 400;
            getRelaySetting(RemainingCurrent, out OpenRelay, out CloseRelay);
            ret = Helper.EquipHelper.Instance.SetRelay(OpenRelay, CloseRelay);
            if (!ret)
            {
                throw new Exception("剩余电流设置失败");
            }
            ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 10 * GlobalUnit.Itr, 1, 1, "1.0", true, false);
            if (!ret)
            {
                throw new Exception("升源失败");
            }
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            if (Stop) return;

            MessageController.Instance.AddMessage("正在读取剩余电流");
            float[] flt_DL400 = MeterProtocolAdapter.Instance.ReadData("02800030", 3, 3);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["400mA实际"][i] = flt_DL400[i].ToString();

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "400mA实际", ResultDictionary["400mA实际"]);
            if (Stop) return;
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (flt_DL400[i] == -1)
                    {
                        Fail[i] = Fail[i] + "，400mA时读取剩余电流失败";                     
                    }
                    else if (flt_DL400[i] < 0.297)
                    {
                        Fail[i] = Fail[i] + "，400mA时差值小于297mA";                       
                    }
                }
            }
          
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