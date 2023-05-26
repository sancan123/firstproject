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
using System.Threading;

namespace CLDC_VerifyAdapter.Function.RateTimeFunction
{
    class RateTimeSet:VerifyBase
    {


           #region ----------构造函数----------

        public RateTimeSet(object plan)
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
        bool setsj = false;

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "测试时间", "运行时段(前)", "年时区数(前)", "日时段表数(前)", "日时段数(前)", "费率数(前)", "第二套时区表(前)", "第二套时段表(前)", "年时区数(后)", "日时段表数(后)", "日时段数(后)", "费率数(后)", "第二套时区表(后)", "第二套时段表(后)", "运行时段(后)", "结论", "不合格原因" };
            return true;
        }

        #endregion                
        public override void Verify()
        {
            base.Verify();
           bool bPowerOn = PowerOn();
          
            bool[] ZTZresult = new bool[BwCount];
           bool[] Result = new bool[BwCount];
           string[] Fail = new string[BwCount];

           bool[] YaoJian = new bool[BwCount];
           for (int i = 0; i < BwCount; i++)
           {
               YaoJian[i] = Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn;
           }
           MessageController.Instance.AddMessage("正在读取电表运行状态字3");
           string[] str_DBZTZ = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
           for (int i = 0; i < BwCount; i++)
           {
               if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
               if (!string.IsNullOrEmpty(str_DBZTZ[i]))
               {
                   string str = CLDC_DataCore.Function.Common.HexStrToBinStr(str_DBZTZ[i]);
                   if (str.Substring(10, 1) == "0" && str.Substring(15, 1) == "0")
                   {
                       ZTZresult[i] = true;
                       ResultDictionary["运行时段(前)"][i] = "第一套";
                   }
                   else
                   {
                       ZTZresult[i] = false;
                       Fail[i] = "当前运行非第一套";
                       ResultDictionary["运行时段(前)"][i] = "第二套";
                   }
               }
           }
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "运行时段(前)", ResultDictionary["运行时段(前)"]);
           Thread.Sleep(1000);
           MessageController.Instance.AddMessage("正在读取年时区数");
           string[] flt_SQS = MeterProtocolAdapter.Instance.ReadData("04000201", 1);
           MessageController.Instance.AddMessage("正在读取日时段表数");
           string[] flt_SDBS = MeterProtocolAdapter.Instance.ReadData("04000202", 1);  
            MessageController.Instance.AddMessage("正在读取日时段数");
            string[] flt_SDS = MeterProtocolAdapter.Instance.ReadData("04000203", 1);
            MessageController.Instance.AddMessage("正在读取费率数");
            string[] flt_FLS = MeterProtocolAdapter.Instance.ReadData("04000204", 1);    
            MessageController.Instance.AddMessage("正在读取第二套时区表");
            string[] str_SQB2 = MeterProtocolAdapter.Instance.ReadData("04020000", 6);          
            MessageController.Instance.AddMessage("正在读取第二套时段表");
            string[] str_SDB2 = MeterProtocolAdapter.Instance.ReadData("04020001", 24);
            for (int i = 0; i < BwCount; i++)
            {
                str_SDB2[i] = Common.SortMin(str_SDB2[i]);
              
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "年时区数(前)", flt_SQS);
            Thread.Sleep(1000);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "日时段表数(前)", flt_SDBS);
            Thread.Sleep(1000);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "日时段数(前)", flt_SDS);
            Thread.Sleep(1000);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "费率数(前)", flt_FLS);
            Thread.Sleep(1000);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二套时区表(前)", str_SQB2);
            Thread.Sleep(1000);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二套时段表(前)", str_SDB2);
            Thread.Sleep(1000);

            if (Stop) return;


            CheckRate(Fail);
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strDataCode = new string[BwCount];
            string[] strData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            string writedata = FormatWriteData("02", "NN", 1, 0);
            Common.Memset(ref strDataCode, "04000201");
            Common.Memset(ref strData, "04000201" + writedata);
            MessageController.Instance.AddMessage("正在设置年时区数");
            bool[] bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
             writedata = FormatWriteData("02", "NN", 1, 0);
            Common.Memset(ref strDataCode, "04000202");
            Common.Memset(ref strData, "04000202" + writedata);
            MessageController.Instance.AddMessage("正在设置日时段表数");
             bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
             writedata = FormatWriteData("08", "NN", 1, 0);
            Common.Memset(ref strDataCode, "04000203");
            Common.Memset(ref strData, "04000203" + writedata);
            MessageController.Instance.AddMessage("正在设置日时段数");
           bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            writedata = FormatWriteData("04", "NN", 1, 0);
            Common.Memset(ref strDataCode, "04000204");
            Common.Memset(ref strData, "04000204" + writedata);
            MessageController.Instance.AddMessage("正在设置费率数");
           bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
         
           str_DBZTZ = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
           for (int i = 0; i < BwCount; i++)
           {
               if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
               if (!string.IsNullOrEmpty(str_DBZTZ[i]))
               {
                   string str = CLDC_DataCore.Function.Common.HexStrToBinStr(str_DBZTZ[i]);
                   if (str.Substring(10, 1) == "0" && str.Substring(15, 1) == "0")
                   {
                       Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn = false;
                   }
                   else
                   {
                       Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn = true;
                   }
               }
           }

           if (setsj == true)
           {
               string dateTime = DateTime.Now.AddDays(2).ToString("yyMMdd") + "0000";
               iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

               writedata = FormatWriteData(dateTime, "yyMMddHHmm", 5, 0);
               Common.Memset(ref strDataCode, "04000106");
               Common.Memset(ref strData, "04000106" + writedata);
               MessageController.Instance.AddMessage("正在设置两套时区表切换时间");
               bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
               iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
               dateTime = DateTime.Now.AddDays(3).ToString("yyMMdd") + "0000";
               writedata = FormatWriteData(dateTime, "yyMMddHHmm", 5, 0);
               Common.Memset(ref strDataCode, "04000107");
               Common.Memset(ref strData, "04000107" + writedata);
               MessageController.Instance.AddMessage("正在设置两套日时段表切换时间");
               bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
               MessageController.Instance.AddMessage("正在设置时间为明天23:59:40...");
               string Time = DateTime.Now.AddDays(1).ToString("yyMMdd") + "235940";
               bResult = MeterProtocolAdapter.Instance.WriteDateTime(Time);
               ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 40);
               MessageController.Instance.AddMessage("正在设置时间为明天23:59:40...");
                Time = DateTime.Now.AddDays(2).ToString("yyMMdd") + "235940";
               bResult = MeterProtocolAdapter.Instance.WriteDateTime(Time);
               ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 40);
             
           }
           iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
         
           for (int i = 0; i < BwCount; i++)
           {
               Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn = YaoJian[i];
           }

         
           string dateTime1 = "0000000000";
           iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

           writedata = FormatWriteData(dateTime1, "yyMMddHHmm", 5, 0);
           Common.Memset(ref strDataCode, "04000106");
           Common.Memset(ref strData, "04000106" + writedata);
           MessageController.Instance.AddMessage("正在设置两套时区表切换时间");
           bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
           iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
           writedata = FormatWriteData(dateTime1, "yyMMddHHmm", 5, 0);
           Common.Memset(ref strDataCode, "04000107");
           Common.Memset(ref strData, "04000107" + writedata);
           MessageController.Instance.AddMessage("正在设置两套日时段表切换时间");
           bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
           iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
           MessageController.Instance.AddMessage("正在设置第二套时区表");
           for (int i = 0; i < BwCount; i++)
           {
               if (!string.IsNullOrEmpty(str_SQB2[i]))
               {
                   strData[i] = "04020000" + str_SQB2[i];
               }
           }
           Common.Memset(ref strDataCode, "04020000");
           bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

           iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
           MessageController.Instance.AddMessage("正在设置第二套时段表");
           for (int i = 0; i < BwCount; i++)
           {
               if (!string.IsNullOrEmpty(str_SDB2[i]))
               {
                   strData[i] = "04020001" + str_SDB2[i];
               }
           }
           Common.Memset(ref strDataCode, "04020001");
           bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
           MessageController.Instance.AddMessage("正在设置时间为当前时间");
           string Time1 = DateTime.Now.ToString("yyMMddHHmmss");
           MeterProtocolAdapter.Instance.WriteDateTime(Time1);
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", Fail);
        }



        private void CheckRate( string[]  fail)
        {
            string[] Fail = new string[BwCount];
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strDataCode = new string[BwCount];
            string[] strData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            string writedata = FormatWriteData("01", "NN", 1, 0);
            Common.Memset(ref strDataCode, "04000201");
            Common.Memset(ref strData, "04000201" + writedata);
            MessageController.Instance.AddMessage("正在设置年时区数");
            bool[] bResultNsqs = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            for (int i = 0; i < BwCount; i++)
            {
                Fail[i] = fail[i] + Fail[i];
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (bResultNsqs[i] == false)
                    {

                        Fail[i] = Fail[i] + ",设置年时区数为1失败";
                    }


                }
            }
            writedata = FormatWriteData("01", "NN", 1, 0);
            Common.Memset(ref strDataCode, "04000202");
            Common.Memset(ref strData, "04000202" + writedata);
            MessageController.Instance.AddMessage("正在设置日时段表数");
            bool[] bResultSdbs = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (bResultSdbs[i] == false)
                    {

                        Fail[i] = Fail[i] + ",设置日时段表数为1失败";
                    }


                }
            }
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            writedata = FormatWriteData("09", "NN", 1, 0);
            Common.Memset(ref strDataCode, "04000203");
            Common.Memset(ref strData, "04000203" + writedata);
            MessageController.Instance.AddMessage("正在设置日时段数");
            bool[] bResultSds = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (bResultSds[i] == false)
                    {

                        Fail[i] = Fail[i] + ",设置日时段数为9失败";
                    }


                }
            }
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            writedata = FormatWriteData("05", "NN", 1, 0);
            Common.Memset(ref strDataCode, "04000204");
            Common.Memset(ref strData, "04000204" + writedata);
            MessageController.Instance.AddMessage("正在设置费率数");
            bool[] bResultFls = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (bResultFls[i] == false)
                    {

                        Fail[i] = Fail[i] + ",设置费率数为5失败";
                    }


                }
            }

            string TimeSq = DateTime.Now.AddDays(1).ToString("MMdd") + "01";
          
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            writedata = FormatWriteData(TimeSq, "NNNNNN", 3, 0);
            Common.Memset(ref strDataCode, "04020000");
            Common.Memset(ref strData, "04020000" + writedata);
            MessageController.Instance.AddMessage("正在设置第二套时区表");
            bool[] bResultSqb = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (bResultSqb[i] == false)
                    {

                        Fail[i] = Fail[i] + ",设置第二套时区表失败";
                    }


                }
            }




            string Str_Qtime = "000005060002120003180004180004180004180004180004180004";
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
       
            Common.Memset(ref strDataCode, "04020001");
            Common.Memset(ref strData, "04020001" + Str_Qtime);
            MessageController.Instance.AddMessage("正在设置第二套时段表");
            bool[] bResultSdb = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (bResultSdb[i] == false)
                    {

                        Fail[i] = Fail[i] + ",设置第二套时段表失败";
                    }


                }
            }
            string dateTime = DateTime.Now.AddDays(1).ToString("yyMMdd")+"0000";
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            writedata = FormatWriteData(dateTime, "yyMMddHHmm", 5, 0);
            Common.Memset(ref strDataCode, "04000106");
            Common.Memset(ref strData, "04000106" + writedata);
            MessageController.Instance.AddMessage("正在设置两套时区表切换时间");
            bool[] bResultSQbqhsj = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (bResultSQbqhsj[i] == false)
                    {

                        Fail[i] = Fail[i] + ",设置第两套时区表切换时间失败";
                    }


                }
            }



            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            writedata = FormatWriteData(dateTime, "yyMMddHHmm", 5, 0);
            Common.Memset(ref strDataCode, "04000107");
            Common.Memset(ref strData, "04000107" + writedata);
            MessageController.Instance.AddMessage("正在设置两套日时段表切换时间");
            bool[] bResultSDbqhsj = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (bResultSDbqhsj[i] == false)
                    {

                        Fail[i] = Fail[i] + "设置两套日时段表切换时间失败";
                    }


                }
            }

            MessageController.Instance.AddMessage("正在读取年时区数");
            string[] flt_SQS1 = MeterProtocolAdapter.Instance.ReadData("04000201", 1);
            Thread.Sleep(1000);
            MessageController.Instance.AddMessage("正在读取日时段表数");
            string[] flt_SDBS1 = MeterProtocolAdapter.Instance.ReadData("04000202", 1);
            Thread.Sleep(1000);
            MessageController.Instance.AddMessage("正在读取日时段数");
            string[] flt_SDS1 = MeterProtocolAdapter.Instance.ReadData("04000203", 1);
            Thread.Sleep(1000);
            MessageController.Instance.AddMessage("正在读取费率数");
            string[] flt_FLS1 = MeterProtocolAdapter.Instance.ReadData("04000204", 1);
            Thread.Sleep(1000);
            MessageController.Instance.AddMessage("正在读取第二套时区表");
            string[] str_SQB21 = MeterProtocolAdapter.Instance.ReadData("04020000", 3);
            Thread.Sleep(1000);
            MessageController.Instance.AddMessage("正在读取第二套时段表");
            string[] str_SDB21 = MeterProtocolAdapter.Instance.ReadData("04020001", 27);
            for (int i = 0; i < BwCount; i++)
            {
                str_SDB21[i] = Common.SortMin(str_SDB21[i]);
            }
            Thread.Sleep(1000);

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (flt_SQS1[i] != "01")
                    {
                        Fail[i] = Fail[i] + ",年时区数错误";
                    }
                    if (flt_SDBS1[i] != "01")
                    {
                        Fail[i] = Fail[i] + ",日时段表错误";
                    }
                    if (flt_SDS1[i] != "09")
                    {
                        Fail[i] = Fail[i] + ",日时段数错误";
                    }
                    if (flt_FLS1[i] != "05")
                    {
                        Fail[i] = Fail[i] + ",费率数错误";
                    }
                    if (str_SQB21[i] != TimeSq)
                    {
                        Fail[i] = Fail[i] + ",第二套时区表错误";
                    }

                    if (str_SDB21[i] != Str_Qtime)
                    {
                        Fail[i] = Fail[i] + ",第二套时段表错误";
                    }
                }
            }







            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "年时区数(后)", flt_SQS1);
            Thread.Sleep(1000);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "日时段表数(后)", flt_SDBS1);
            Thread.Sleep(1000);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "日时段数(后)", flt_SDS1);
            Thread.Sleep(1000);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "费率数(后)", flt_FLS1);
            Thread.Sleep(1000);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二套时区表(后)", str_SQB21);
            Thread.Sleep(1000);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二套时段表(后)", str_SDB21);
            Thread.Sleep(1000);
            if (Stop) return;

            MessageController.Instance.AddMessage("正在设置时间为今天23:59:40...");
            string Time = DateTime.Now.ToString("yyMMdd") + "235940";
            MeterProtocolAdapter.Instance.WriteDateTime(Time);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 40);

            MessageController.Instance.AddMessage("正在读取电表运行状态字3");
            string[] str_DBZTZ = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(str_DBZTZ[i]))
                {
                    string str = CLDC_DataCore.Function.Common.HexStrToBinStr(str_DBZTZ[i]);
                    if (str.Substring(10, 1) == "1" && str.Substring(15, 1) == "1")
                    {
                        ResultDictionary["运行时段(前)"][i] = "第二套";
                    }
                    else
                    {
                        ResultDictionary["运行时段(前)"][i] = "第一套";
                        Fail[i] = Fail[i] + ",当前运行非第二套";
                    }
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "运行时段(后)", ResultDictionary["运行时段(前)"]);
            Thread.Sleep(1000);




            setsj = true;

            System.Windows.Forms.MessageBox.Show("请查看表上是否显示费率为T5。按确定之后填写结论！");

            Form_Ariticial fs = new Form_Ariticial("费率时段设置");
            fs.ShowDialog();
            string[] bResultXs = GlobalUnit.ManualResult;
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (bResultXs[i] == "不合格")
                    {

                        Fail[i] = Fail[i] + ",表上显示错误失败";
                    }


                }
            }

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!string.IsNullOrEmpty(Fail[i]))
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                }

            }
         
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
        public string ConvertArryToOne(string[] Data)
        {
            for (int i = 0; i < Data.Length; i++)
            {
                  return Data[i];
                
            }
            return "";
        }

    }
}
