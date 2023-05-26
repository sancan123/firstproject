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
using CLDC_Comm.Enum;
using System.Threading;

namespace CLDC_VerifyAdapter.SecondStage
{
    class BlueSetTime:VerifyBase
    {


           #region ----------构造函数----------

        public BlueSetTime(object plan)
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
            ResultNames = new string[] { "测试时间", "写入时间", "电表时间", "结论", "不合格原因" };
            return true;
        }

        #endregion                
        public override void Verify()
        {
            base.Verify();
           bool bPowerOn = PowerOn();
           bool[] Result = new bool[BwCount];
           string[] Fail = new string[BwCount];

           DateTime[] arrReadData = new DateTime[BwCount];
           DateTime GPSTime = DateTime.Now;
           DateTime readTime = DateTime.Now;
           if (Stop) return;
           MessageController.Instance.AddMessage("正在准备蓝牙操作");
           System.Windows.Forms.MessageBox.Show("请手动插上蓝牙模块，完成后点击确定。");
           ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

           string[] strID = new string[BwCount];
           string[] strData = new string[BwCount];
           string[] strSetData = new string[BwCount];
           int[] iFlag = new int[BwCount];
           string[] strShowData = new string[BwCount];
           string[] strCode = new string[BwCount];
           string[] strRand1 = new string[BwCount];//随机数
           string[] strRand2 = new string[BwCount];//随机
           string[] strEsamNo = new string[BwCount];//Esam序列号
           bool[] result = new bool[BwCount];
           string[] strEnerZQ = new string[BwCount];//

           GlobalUnit.g_CommunType = CLDC_Comm.Enum.Cus_CommunType.通讯蓝牙;

           string[] address_MAC = new string[BwCount];
           for (int i = 0; i < BwCount; i++)
           {
               address_MAC[i] = Helper.MeterDataHelper.Instance.Meter(i).Mb_chrAddr_MAC;
           }


           MessageController.Instance.AddMessage("正在进行蓝牙连接...");
           bool[] bResult = MeterProtocolAdapter.Instance.ConnectBlueTooth(address_MAC);






           if (Stop) return;
           MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();
          
    
         string dateTime = DateTime.Now.ToString("yyMMddHHmmss");

        MessageController.Instance.AddMessage("正在写入时间");
        Result = MeterProtocolAdapter.Instance.WriteDateTime(dateTime);


        MessageController.Instance.AddMessage("正在读取电表时间");

        arrReadData = MeterProtocolAdapter.Instance.ReadDateTime();


        SwitchCarrierOr485(Cus_CommunType.通讯485);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["写入时间"][i] = dateTime;
                    ResultDictionary["电表时间"][i] = arrReadData[i].ToString("yyMMddHHmmss");
                    if (Result[i])
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                    }
                    else
                    {                      
                            ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                            ResultDictionary["不合格原因"][i] = "写入时间失败";
                    }
                   
                   
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "写入时间", ResultDictionary["写入时间"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电表时间", ResultDictionary["电表时间"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);




            GlobalUnit.g_CommunType = Cus_CommunType.通讯485;



        }

      

    }
}