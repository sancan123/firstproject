
using System;
using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
namespace CLDC_VerifyAdapter.Multi
{
    /// <summary>
    /// 读取费率信息
    /// </summary>
    class Dgn_ReadPeriod : DgnBase
    {
        public Dgn_ReadPeriod(object plan)
            : base(plan)
        { 
        
        
        }
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "运行时区","运行时段","第一套时区表数据","第二套时区表数据","第一套第1日时段表数据","第二套第1日时段表数据", "结论" };
            return true;
        }

        /// <summary>
        /// 通讯测试
        /// </summary>
        public override void Verify()
        {            
            //更新一下电能表数据
            Helper.MeterDataHelper.Instance.Init();
            //更新多功能协议
            Adapter.Instance.UpdateMeterProtocol();
            bool bPowerOn = PowerOn();
            Check.Require(bPowerOn, "控制源输出失败");
            MessageController.Instance.AddMessage("正在进行" + CurPlan.ToString());

            string[] arrStrResultKey = new string[BwCount];
            MeterBasicInfo curMeter;
            MeterDgn _Result;
            MeterDgn _Total;
            string[] strReadData = null;
            string[] strRun = new string[GlobalUnit.g_CUS.DnbData._Bws];

            string totalKey = ((int)Cus_DgnItem.费率时段检查).ToString("D3");//总结论节点主键
            string ItemKey;//当前项目节点主键
            string strSD = "";
            string[] arrStatusPeriod = new string[BwCount];
            string[] arrStatusZone = new string[BwCount];

         //   ItemKey = totalKey + "01";
            MessageController.Instance.AddMessage("读取状态运行字3");
            strReadData = MeterProtocolAdapter.Instance.ReadData("04000503", 4);

            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)
            {
                byte byt_Run;
                string str_Tmp;
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn == false) continue;
                byt_Run = Convert.ToByte(strReadData[i].Substring(2, 2), 16);
                str_Tmp = Convert.ToString(byt_Run, 2);
                if (!string.IsNullOrEmpty(strReadData[i]))
                {
                    int warningValue = Convert.ToInt32(strReadData[i], 16);

                    if ((warningValue & 0x01) == 0x01)
                    {
                        arrStatusPeriod[i] = "第二套";
                    }
                    else
                    {
                        arrStatusPeriod[i] = "第一套";
                    }

                    if ((warningValue & 0x20) == 0x20)
                    {
                        arrStatusZone[i] = "第二套";
                    }
                    else
                    {
                        arrStatusZone[i] = "第一套";
                    }
                }
                else
                {
                    arrStatusPeriod[i] = "未读到";
                    arrStatusZone[i] = "未读到";
                }
                ResultDictionary["运行时区"][i] = arrStatusPeriod[i];
                ResultDictionary["运行时段"][i] = arrStatusZone[i];
            }
            UploadTestResult("运行时区");
            UploadTestResult("运行时段");

          
            MessageController.Instance.AddMessage("读取第一套时区表数据");
            strReadData = MeterProtocolAdapter.Instance.ReadData("04010000", 42);
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                ResultDictionary["第一套时区表数据"][i] = strReadData[i];                
            }

            UploadTestResult("第一套时区表数据");
           
            MessageController.Instance.AddMessage("读取第二套时区表数据");
            strReadData = MeterProtocolAdapter.Instance.ReadData("04020000", 42);
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                ResultDictionary["第二套时区表数据"][i] = strReadData[i];
            }
            UploadTestResult("第二套时区表数据");

         
            MessageController.Instance.AddMessage("读取第一套第1日时段表数据");
            strReadData = MeterProtocolAdapter.Instance.ReadData("04010001", 42);
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                ResultDictionary["第一套第1日时段表数据"][i] = GetFeiLvInfo(strReadData[i]);
                if (strRun[i] == "第一套")
                {
                    ResultDictionary["结论"][i] = "合格";
                }
            }
            UploadTestResult("第一套第1日时段表数据");
            UploadTestResult("结论");
          
            MessageController.Instance.AddMessage("读取第二套第1日时段表数据");
            strReadData = MeterProtocolAdapter.Instance.ReadData("04020002", 42);
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                ResultDictionary["第二套第1日时段表数据"][i] = GetFeiLvInfo(strReadData[i]);

                if (strRun[i] == "第二套")
                {
                  
                  
                }
            }

            UploadTestResult("第二套第1日时段表数据");
                        
        }

        private string GetFeiLvInfo(string strValue)
        {
            string strFeilv = "";
            string strPeriod = "";
            while (strValue.Length >= 6)
            {
                strPeriod = strValue.Substring(strValue.Length - 6, 6);
                strPeriod = strPeriod.Substring(0, 2) + ":" + strPeriod.Substring(2, 2) + "(" + getFeiDlValue(int.Parse(strPeriod.Substring(4, 2))) + ")";
                strFeilv += strPeriod;
                strValue = strValue.Substring(0, strValue.Length - 6);
            }
            return strFeilv;
        }

        private string getFeiDlValue(int FeiLvID)
        {
            switch (FeiLvID)
            {
                case 1:
                    return "尖";
                case 2:
                    return "峰";
                case 3:
                    return "平";
                case 4:
                    return "谷";
                default:
                    return "尖";
            }
        }
        
    }
}
/*===========================================================================================================*/