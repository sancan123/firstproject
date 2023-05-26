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
using CLDC_VerifyAdapter.Helper;
using CLDC_Comm.Enum;
using System.Threading;
using CLDC_DeviceDriver;

namespace CLDC_VerifyAdapter
{

    /// <summary>
    /// 组合有功设置
    /// </summary>
    class ConstantVerify:VerifyBase
    {
        //标准表累积电量
        private float _StarandMeterDl = 0F;

           #region ----------构造函数----------

        public ConstantVerify(object plan)
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
            ResultNames = new string[] { "起码", "止码", "表码差", "表脉冲", "标准表脉冲", "误差", "结论", "不合格原因" };
            return true;
        }

        #endregion                
        public override void Verify()
        {
            base.Verify();


            string[] array = VerifyPara.Split('|');
            string DLDI= "";
            int GLYJ = 1;
            string glys = "1.0";
            float testI = 0;
            float ZZds = 0;
            Cus_PowerFangXiang glfx = Cus_PowerFangXiang.正向有功;
            if (!string.IsNullOrEmpty(array[0]))
            {
                switch (array[0])
                {
                    case "正向有功":
                        DLDI = "00010000";
                        glfx = Cus_PowerFangXiang.正向有功;
                        break;
                    case "正向无功":
                        DLDI = "00030000";
                        glfx= Cus_PowerFangXiang.正向无功;
                        break;
                    case "反向有功":
                        DLDI = "00020000";
                        glfx= Cus_PowerFangXiang.反向有功;
                        break;
                    case "反向无功":
                        DLDI = "00040000";
                        glfx= Cus_PowerFangXiang.反向无功;
                        break;
                    default :
                         DLDI = "00010000";
                         glfx = Cus_PowerFangXiang.正向有功;
                        break;

                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("请检查方案参数是否正确");
            }

            if (!string.IsNullOrEmpty(array[1]))
            {
                switch (array[1])
                {
                    case "H":
                        GLYJ = 1;
                        break;
                    case "A":
                        GLYJ = 2;
                        break;
                    case "B":
                        GLYJ = 3;
                        break;
                    case "C":
                        GLYJ = 4;
                        break;
                    default:
                        GLYJ = 1;
                        break;


                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("请检查方案参数是否正确");
            }

            if (!string.IsNullOrEmpty(array[2]))
            {
                glys = array[2];
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("请检查方案参数是否正确");
            }

            if (!string.IsNullOrEmpty(array[3]))
            {
                testI = Number.GetCurrentByIb(array[3], Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_chrIb, Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_BlnHgq);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("请检查方案参数是否正确");
            }
            if (!string.IsNullOrEmpty(array[6]))
            {
                ZZds = float.Parse(array[6]);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("请检查方案参数是否正确");
            }













           bool bPowerOn = PowerOn();
           bool[] Result = new bool[BwCount];
           string[] Fail = new string[BwCount];


           MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

        
            MessageController.Instance.AddMessage("正在给电表清零");
          bool[]  fResult = MeterProtocolAdapter.Instance.ClearEnergy();
          ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 20);
          MessageController.Instance.AddMessage("正在读取起码");
          float[] flt_QM = MeterProtocolAdapter.Instance.ReadData(DLDI, 4, 2);
           
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    //"功率元件","功率方向","电流倍数","功率因数","误差下限","误差上限","误差圈数"
                    ResultDictionary["起码"][i] = flt_QM[i].ToString();
                   
                   
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "起码", ResultDictionary["起码"]);



            MessageController.Instance.AddMessage("启动误差板走字指令");

            if (EquipHelper.Instance.InitPara_Constant(glfx, null) == false)
            {
                //Stop = true;
                MessageController.Instance.AddMessage("启动误差板走字指令失败", 6, 2);
                //return;
            }

            if (Stop)
            {
                return;
            }
            if (EquipHelper.Instance.PowerOn(GlobalUnit.U, testI, GLYJ, (int)glfx, glys, IsYouGong, false) == false)
            {
                //Stop = true;
                //return;
            }
            string[] arrData = new string[0]; 
            string stroutmessage = string.Empty;        //外发消息
            DateTime startTime = DateTime.Now.AddSeconds(-6);   //检定开始时间,减掉源等待时间
            _StarandMeterDl = 0;                        //标准表电量
            DateTime lastTime = DateTime.Now.AddSeconds(-6);

            //开始走字
            while (true)
            {
                Thread.Sleep(1000);
                if (Stop) return;
                int _PastTime = DateTimes.DateDiff(startTime);
                //处理跨天
                if (_PastTime < 0) _PastTime += 43200;      //如果当前PC为12小时制
                if (_PastTime < 0) _PastTime += 43200;      //24小时制
                if (_PastTime < 0)
                {
                    //处理人为修改时间
                    //跨度超过24小时，肯定是系统日期被修改
                    MessageController.Instance.AddMessage("系统时间被人为修改超过24小时，检定停止");
                    Stop = true;
                    return;
                }
                float pSum = 0;
                if (IsYouGong)
                {
                    //if (GlobalUnit.g_StrandMeterP[0] > 0)
                    pSum = Math.Abs(GlobalUnit.g_StrandMeterP[0]);
                }
                else
                {
                    //if (GlobalUnit.g_StrandMeterP[1] > 0)
                    pSum = Math.Abs(GlobalUnit.g_StrandMeterP[1]);
                }



                int pastSecond = (int)(DateTime.Now - lastTime).TotalSeconds;
                //lastTime = DateTime.Now;
                _StarandMeterDl = pastSecond * pSum / 3600 / 1000;
                //同步记录（读）脉冲数
                if (arrData.Length < BwCount)
                {
                    arrData = new string[BwCount];
                }
                int num = 0;
                string data = string.Empty;
                string time = string.Empty;
                for (int k = 0; k < BwCount; k++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(k).YaoJianYn)
                    {

                        continue;
                    }
                    Helper.EquipHelper.Instance.ReadQueryCurrentErrorControl(k + 1, 3, out num, out data, out time);
                    arrData[k] = data;
                }
              
           
                if (IsYouGong)
                {
                    //if (GlobalUnit.g_StrandMeterP[0] > 0)
                    pSum = Math.Abs(GlobalUnit.g_StrandMeterP[0]);
                }
                else
                {
                    //if (GlobalUnit.g_StrandMeterP[1] > 0)
                    pSum = Math.Abs(GlobalUnit.g_StrandMeterP[1]);
                }
                //再算一次电量
                pastSecond = (int)(DateTime.Now - lastTime).TotalSeconds;
                //lastTime = DateTime.Now;
                _StarandMeterDl = pastSecond * pSum / 3600 / 1000;
                if (_StarandMeterDl >= ZZds - 0.002)
                {
                    m_CheckOver = true;
                }
                //如果脉冲数达到设定，也停止
                float flt_C = 0;
                if (arrData != null && arrData.Length > 0)
                {
                    float.TryParse(arrData[GlobalUnit.FirstYaoJianMeter], out flt_C);
                }
                float flt_C2 = flt_C / Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).GetBcs()[0];
               
                //外发检定消息
                GlobalUnit.g_CUS.DnbData.NowMinute = _StarandMeterDl;
                stroutmessage = string.Format("方案设置走字电量：{0}度，已经走字：{1}度", ZZds, _StarandMeterDl.ToString("F5"));

                for (int i = 0; i < BwCount; i++)
                {
                    MeterBasicInfo _meterInfo = Helper.MeterDataHelper.Instance.Meter(i);
                    if (!_meterInfo.YaoJianYn)
                    {
                        continue;
                    }

                    //"表脉冲", "标准表脉冲"
                    if (arrData != null && arrData.Length > i)
                    {
                        ResultDictionary["表脉冲"][i] = arrData[i];
                    }
                    ResultDictionary["标准表脉冲"][i] = ((_StarandMeterDl * _meterInfo.GetBcs()[0])).ToString("F2");
                }
                UploadTestResult("表脉冲");
                
                UploadTestResult("标准表脉冲");
                MessageController.Instance.AddMessage(stroutmessage);
                if (m_CheckOver)
                {
                    break;
                }
            }
            PowerOn();

            //读结束时脉冲和电量
            if (arrData.Length < BwCount)
            {
                arrData = new string[BwCount];
            }
            int num1 = 0;
            string data1 = string.Empty;
            string time1 = string.Empty;
            for (int k = 0; k < BwCount; k++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(k).YaoJianYn)
                {

                    continue;
                }
                Helper.EquipHelper.Instance.ReadQueryCurrentErrorControl(k + 1, 3, out num1, out data1, out time1);
                arrData[k] = data1;
                if (arrData != null )
                {
                    ResultDictionary["表脉冲"][k] = arrData[k];
                }
            }
            UploadTestResult("表脉冲");
            if (Stop)
            {
                return;
            }

            MessageController.Instance.AddMessage("正在读取止码");
            float[] flt_ZM = MeterProtocolAdapter.Instance.ReadData(DLDI, 4, 2);

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                   
                    ResultDictionary["止码"][i] = flt_ZM[i].ToString();


                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "止码", ResultDictionary["止码"]);

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (flt_ZM[i] == flt_QM[i] )
                    {
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["误差"][i] = "-999";
                        ResultDictionary["不合格原因"][i] = "止码，起码相等";
                    }
                    else if (flt_ZM[i] == 0 || flt_QM[i] == -1.0 || flt_ZM[i] == -1.0)
                    {
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["误差"][i] = "-999";
                        ResultDictionary["不合格原因"][i] = "读不到电量";
                    }
                    else if (flt_ZM[i] - flt_QM[i] - ZZds >= 0.3)
                    {
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["误差"][i] = "-999";
                        ResultDictionary["不合格原因"][i] = "走字电量超设定值0.3";
                    }
                    else if (ZZds-( flt_ZM[i] - flt_QM[i])  > 0.3)
                    {
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["误差"][i] = "-999";
                        ResultDictionary["不合格原因"][i] = "走字电量小于设定值0.3";
                    }
                    else
                    {
                        ResultDictionary["表码差"][i] = (flt_ZM[i] - flt_QM[i]).ToString();
                        ResultDictionary["误差"][i] = ((flt_ZM[i] - flt_QM[i])  - _StarandMeterDl).ToString("0.0000");
                        ResultDictionary["结论"][i] = "合格";
                    }

                  

                }
            }
            UploadTestResult("表码差");
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 *1);
            UploadTestResult("误差");
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
            UploadTestResult("结论");

            MessageController.Instance.AddMessage("停止误差板!");
            Helper.EquipHelper.Instance.SetCurFunctionOnOrOff(false, 0);

            if (GlobalUnit.IsCL3112)
            {
                Helper.EquipHelper.Instance.FuncMstate(0xFE);
            }
            Helper.EquipHelper.Instance.SetErrCalcType(0);





            

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