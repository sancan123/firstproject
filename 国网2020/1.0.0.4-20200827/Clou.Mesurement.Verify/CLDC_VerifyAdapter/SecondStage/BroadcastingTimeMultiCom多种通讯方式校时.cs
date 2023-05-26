using System;
using System.Collections.Generic;
using System.Text;

using CLDC_DataCore.Struct;
using CLDC_Comm;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using System.Threading;

using System.Windows.Forms;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;


namespace CLDC_VerifyAdapter
{

        /// <summary>
    /// 功能描述：多种通讯方式校时
        /// 作    者：
        /// 编写日期：2020-04-14
        /// 修改记录：
        ///         修改日期		     修改人	            修改内容
        ///
        /// </summary>
        public class BroadcastingTimeMultiCom : VerifyBase
        {
            public BroadcastingTimeMultiCom(object plan)
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
            ResultNames = new string[] { "测试时间", "蓝牙校时时间","蓝牙校时后时间","485校时前时间","485校时后时间", "结论", "不合格原因" };
            return true;
        }

        /// 重写基类测试方法
        /// </summary>
        /// <param name="ItemNumber">检定方案序号</param>
        public override void Verify()
        {        
            base.Verify();
            PowerOn();
            string[] strShowData = new string[BwCount];
            string[] str_DataFirst = new string[BwCount];
            string[] str_DataSecond = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strID = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strSetData = new string[BwCount];
            string[] strCode = new string[BwCount];
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机
            string[] strEsamNo = new string[BwCount];//Esam序列号
            bool[] result = new bool[BwCount];
            string[] strEnerZQ = new string[BwCount];//

            System.Windows.Forms.MessageBox.Show("请手动插上蓝牙模块，完成后点击确定。");
            GlobalUnit.g_CommunType = CLDC_Comm.Enum.Cus_CommunType.通讯蓝牙;

            string[] address_MAC = new string[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                address_MAC[i] = Helper.MeterDataHelper.Instance.Meter(i).Mb_chrAddr_MAC;
            }

            //读数据
            MessageController.Instance.AddMessage("正在进行蓝牙连接...");
            bool[] bResult = MeterProtocolAdapter.Instance.ConnectBlueTooth(address_MAC);

            //写数据
            if (Stop) return;
            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            if (Stop) return;
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            Random rand = new Random();
            int randDays = rand.Next(600);
            DateTime dt = DateTime.Now.AddDays(randDays);
            //1.今天，时间差2分钟
            if (Stop) return;

            MessageController.Instance.AddMessage("正在修改电表时间为当前时间早2分钟...");
            string strSetTime = dt.AddMinutes(-2).ToString("yyMMddHHmmss");
            MeterProtocolAdapter.Instance.WriteDateTime(strSetTime);

            if (Stop) return;
            string strBroadCastTime = dt.ToString("yyMMddHHmmss");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "蓝牙校时时间", setSameStrArryValue(strBroadCastTime));

            MessageController.Instance.AddMessage("正在进行将电表时间广播校时到" + strBroadCastTime);
            DateTime dtMeterTime = CLDC_DataCore.Function.DateTimes.FormatStringToDateTime(strBroadCastTime);
            MeterProtocolAdapter.Instance.BroadCastTime(dtMeterTime);

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表时间...");
            DateTime[] strMeterTime = MeterProtocolAdapter.Instance.ReadDateTime();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电脑时间...");
            DateTime strSystemTime = DateTime.Now.AddDays(randDays);

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);
            PowerOn();
            //2. 485通讯线
            System.Windows.Forms.MessageBox.Show("请手动插上485通讯线，完成后点击确定。");
            GlobalUnit.g_CommunType = CLDC_Comm.Enum.Cus_CommunType.通讯485;
            if (Stop) return;
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

          
            if (Stop) return;
            DateTime dt2 = DateTime.Now.AddDays(randDays).AddDays(1);
            MessageController.Instance.AddMessage("正在修改电表时间早2分钟...");
            string strSetTime2 = dt2.AddMinutes(-2).ToString("yyMMddHHmmss");
            MeterProtocolAdapter.Instance.WriteDateTime(strSetTime2);

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 3);
            DateTime[] strMeterTime2Before = MeterProtocolAdapter.Instance.ReadDateTime();
            for (int i = 0; i < strMeterTime2Before.Length; i++)
            {
                strShowData[i] = strMeterTime2Before[i].ToString();
            }


            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "485校时前时间", strShowData);
            if (Stop) return;
            string strBroadCastTime2 = dt2.ToString("yyMMddHHmmss");
            MessageController.Instance.AddMessage("正在进行将电表时间广播校时到" + strBroadCastTime2);
            DateTime dtMeterTime2 = CLDC_DataCore.Function.DateTimes.FormatStringToDateTime(strBroadCastTime2);
            MeterProtocolAdapter.Instance.BroadCastTime(dtMeterTime2);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表时间...");
            DateTime[] strMeterTime2 = MeterProtocolAdapter.Instance.ReadDateTime();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电脑时间...");
            DateTime strSystemTime2 = DateTime.Now.AddDays(randDays).AddDays(1);


            MessageController.Instance.AddMessage("正在处理结果...");
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                double Err1 = CLDC_DataCore.Function.DateTimes.DateDiffSeconds(strMeterTime[i], strSystemTime);
                double Err2 = CLDC_DataCore.Function.DateTimes.DateDiffSeconds(strMeterTime2[i], strSystemTime2);
                //2次读电表后的值
                str_DataFirst[i] = strMeterTime[i].ToString("yyyy-MM-dd HH:mm:ss");
                str_DataSecond[i] = strMeterTime2[i].ToString("yyyy-MM-dd HH:mm:ss");
                if (Err1 <= 60 && Err2 <=60 )
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }
                else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    if (Math.Abs(Err1) >60)
                    {
                        reasonS[i] = "蓝牙校时后误差超过60s，";
                    }
                    if ((Math.Abs(Err2) > 60))
                    {
                        reasonS[i] += "485校时后误差超过60s，";
                    }
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "蓝牙校时后时间", str_DataFirst);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "485校时后时间", str_DataSecond);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);

            //日期设置为现在   
            MessageController.Instance.AddMessage("正在设置日期为现在...");
            MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));

            UploadTestResult("结论");
        }
            
        }
    }

