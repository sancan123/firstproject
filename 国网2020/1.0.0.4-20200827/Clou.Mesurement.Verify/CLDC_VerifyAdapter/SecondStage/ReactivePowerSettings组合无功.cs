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
    ///组合无功设置
    /// </summary>
    class ReactivePowerSettings :VerifyBase
    {


           #region ----------构造函数----------

        public ReactivePowerSettings(object plan)
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
            ResultNames = new string[] { "测试前组合无功1电量", "测试前组合无功2电量", "组合无功1和2特征字", "设置后组合无功特征字", "第一象限无功电能", "第二象限无功电能", "第三象限无功电能", "第四象限无功电能", "测试后组合无功1电量", "测试后组合无功2电量", "组合无功特征字", "组合无功电量", "结论", "不合格原因" };
            return true;
        }

        #endregion                
        public override void Verify()
        {
            base.Verify();
           bool bPowerOn = PowerOn();
           bool[] Result = new bool[BwCount];
           string[] Fail = new string[BwCount];



        
            MessageController.Instance.AddMessage("正在读取组合无功1电量");
            float[] flt_DLW1 = MeterProtocolAdapter.Instance.ReadData("00030000", 4, 2);
            MessageController.Instance.AddMessage("正在读取组合无功1电量");
            float[] flt_DLW2 = MeterProtocolAdapter.Instance.ReadData("00040000", 4, 2);
            MessageController.Instance.AddMessage("正在读取组合无功1特征字");
            string[] flt_TzzW1 = MeterProtocolAdapter.Instance.ReadData("04000602", 1);
            MessageController.Instance.AddMessage("正在读取组合无功2特征字");
            string[] flt_TzzW2 = MeterProtocolAdapter.Instance.ReadData("04000603", 1);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    //"功率元件","功率方向","电流倍数","功率因数","误差下限","误差上限","误差圈数"
                    ResultDictionary["测试前组合无功1电量"][i] = flt_DLW1[i].ToString();
                    ResultDictionary["测试前组合无功2电量"][i] = flt_DLW2[i].ToString();
                    ResultDictionary["组合无功1和2特征字"][i] = flt_TzzW1[i].ToString() + "|" + flt_TzzW2[i].ToString();    
                   
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "测试前组合无功1电量", ResultDictionary["测试前组合无功1电量"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "测试前组合无功2电量", ResultDictionary["测试前组合无功2电量"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "组合无功1和2特征字", ResultDictionary["组合无功1和2特征字"]);



            if (Stop) return;

            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strDataCode = new string[BwCount];
            string[] strData = new string[BwCount];
            int[] iFlag = new int[BwCount];          
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            string writedata = FormatWriteData("14", "NN", 1, 0);
            Common.Memset(ref strDataCode, "04000602");
            Common.Memset(ref strData, "04000602" + writedata);
            MessageController.Instance.AddMessage("正在设置组合无功1特征字");
            bool[] bResultW1 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            writedata = FormatWriteData("41", "NN", 1, 0);
            Common.Memset(ref strDataCode, "04000603");
            Common.Memset(ref strData, "04000603" + writedata);
            MessageController.Instance.AddMessage("正在设置组合无功2特征字");
            bool[] bResultW2 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);


            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (bResultW1[i] == false || bResultW2[i] == false)
                    {
                        Result[i] = false;
                        Fail[i] = "设置组合无功特征字失败";
                    }
                    else
                    {
                        Result[i] = true;
                    }

                }
            }



            MessageController.Instance.AddMessage("正在读取组合无功1特征字");
            string[] flt_Tzz2w1 = MeterProtocolAdapter.Instance.ReadData("04000602", 1);
            MessageController.Instance.AddMessage("正在读取组合无功2特征字");
            string[] flt_Tzz2w2 = MeterProtocolAdapter.Instance.ReadData("04000603", 1);


            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    //"功率元件","功率方向","电流倍数","功率因数","误差下限","误差上限","误差圈数"
                    ResultDictionary["设置后组合无功特征字"][i] = flt_Tzz2w1[i].ToString() + "|" + flt_Tzz2w2[i].ToString();
                   
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "设置后组合无功特征字", ResultDictionary["设置后组合无功特征字"]);
            if (Stop) return;
         
          
            
            #region 走字 正向有功
            MessageController.Instance.AddMessage("最大电流进行正向无功走字20S，请稍候......");
            bool ret1 = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Imax, 1, 3, "1.0", true, false);
            if (!ret1)
            {
                throw new Exception("升源失败");
            }
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 20);
            if (Stop) return;
            CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOff();

            #endregion
            PowerOn();
        


            MessageController.Instance.AddMessage("正在读取第一象限电量");
            float[] flt_DLZWX1 = MeterProtocolAdapter.Instance.ReadData("00050000", 4, 2);
            MessageController.Instance.AddMessage("正在读取第二象限电量");
            float[] flt_DLZWX2 = MeterProtocolAdapter.Instance.ReadData("00060000", 4, 2);
            MessageController.Instance.AddMessage("正在读取第三象限电量");
            float[] flt_DLZWX3 = MeterProtocolAdapter.Instance.ReadData("00070000", 4, 2);
            MessageController.Instance.AddMessage("正在读取第四象限电量");
            float[] flt_DLZWX4 = MeterProtocolAdapter.Instance.ReadData("00080000", 4, 2);

            MessageController.Instance.AddMessage("正在读取组向无功1电量");
            float[] flt_DLZW1 = MeterProtocolAdapter.Instance.ReadData("00030000", 4, 2);
            MessageController.Instance.AddMessage("正在读取组向无功2电量");
            float[] flt_DLZW2 = MeterProtocolAdapter.Instance.ReadData("00040000", 4, 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    //"功率元件","功率方向","电流倍数","功率因数","误差下限","误差上限","误差圈数"
                    ResultDictionary["第一象限无功电能"][i] = flt_DLZWX1[i].ToString();
                    ResultDictionary["第二象限无功电能"][i] = flt_DLZWX2[i].ToString();
                    ResultDictionary["第三象限无功电能"][i] = flt_DLZWX3[i].ToString();
                    ResultDictionary["第四象限无功电能"][i] = flt_DLZWX4[i].ToString();
                    ResultDictionary["测试后组合无功1电量"][i] = flt_DLZW1[i].ToString();
                    ResultDictionary["测试后组合无功2电量"][i] = flt_DLZW2[i].ToString();

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一象限无功电能", ResultDictionary["第一象限无功电能"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二象限无功电能", ResultDictionary["第二象限无功电能"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第三象限无功电能", ResultDictionary["第三象限无功电能"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第四象限无功电能", ResultDictionary["第四象限无功电能"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "测试后组合无功1电量", ResultDictionary["测试后组合无功1电量"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "测试后组合无功2电量", ResultDictionary["测试后组合无功2电量"]);

            string W1 = "41";
            string W2 = "14";
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                     W1 = flt_TzzW1[i].ToString();
                     W2 = flt_TzzW2[i].ToString();
                    break;
                }
            }

            string writedata1 = FormatWriteData(W1, "NN", 1, 0);
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            Common.Memset(ref strDataCode, "04000602");
            Common.Memset(ref strData, "04000602" + writedata1);
            bool[] bResult1W1 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            writedata1 = FormatWriteData(W2, "NN", 1, 0);
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            Common.Memset(ref strDataCode, "04000603");
            Common.Memset(ref strData, "04000603" + writedata1);
            bool[] bResult1W2 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            MessageController.Instance.AddMessage("正在读取组合无功1特征字");
            string[] flt_Tzz3W1 = MeterProtocolAdapter.Instance.ReadData("04000602", 1);
            MessageController.Instance.AddMessage("正在读取组合无功2特征字");
            string[] flt_Tzz3W2 = MeterProtocolAdapter.Instance.ReadData("04000603", 1);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取组向无功1电量");
            float[] flt_DLZ1W1 = MeterProtocolAdapter.Instance.ReadData("00030000", 4, 2);

            MessageController.Instance.AddMessage("正在读取组向无功2电量");
            float[] flt_DLZ1W2 = MeterProtocolAdapter.Instance.ReadData("00040000", 4, 2);

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    //"功率元件","功率方向","电流倍数","功率因数","误差下限","误差上限","误差圈数"
                    ResultDictionary["组合无功特征字"][i] = flt_Tzz3W1[i].ToString() + "|" + flt_Tzz3W2[i].ToString();
                    ResultDictionary["组合无功电量"][i] = flt_DLZ1W1[i].ToString() + "|" + flt_DLZ1W2[i].ToString();

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "组合无功特征字", ResultDictionary["组合无功特征字"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "组合无功电量", ResultDictionary["组合无功电量"]);

            

            if (Stop) return;

                 for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (Result[i] == false)
                    {
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["不合格原因"][i] = Fail[i].ToString();

                    }
                    else if (flt_DLZW1[i] -(flt_DLZWX2[i] + flt_DLZWX3[i]) <=0.1 && flt_DLZW2[i] - (flt_DLZWX1[i] + flt_DLZWX4[i])<=0.1)
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";

                        ResultDictionary["不合格原因"][i] = "组合无功电量不符合设定值";
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
