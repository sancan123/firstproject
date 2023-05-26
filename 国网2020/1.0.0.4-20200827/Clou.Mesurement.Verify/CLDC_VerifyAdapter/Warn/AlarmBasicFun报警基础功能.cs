using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.VerifyService;
using System;

namespace CLDC_VerifyAdapter.Warn
{ 
    /// <summary>
    /// 报警基础功能
    /// </summary>
    class AlarmBasicFun: VerifyBase
    {
        //1.设置报警功能 2A
        //2.等待10s，判断读取状态字3，判断bit7是否为1
        //3.设置解除报警功能 2B
        ///4.等待10s，判断读取状态字3，判断bit7是否为0
        protected override string ItemKey
        {
            // get { throw new System.NotImplementedException(); }
            get { return null; }
        }
        protected override string ResultKey
        {
            //get { throw new System.NotImplementedException(); }
            get { return null; }
        }

        public AlarmBasicFun(object plan)
            : base(plan)
        {
        }

 


        protected override bool CheckPara()
        {
            //return base.CheckPara();
            ResultNames = new string[] { "测试时间",   "不合格原因", "结论" };
            return true;
        }

        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            PowerOn();
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strData = new string[BwCount];//明文
            
            int[] iFlag = new int[BwCount];
            string[] strCode = new string[BwCount];
            string[] status3 = new string[BwCount];
            string[] statusTmp = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 2];
            bool[] result = new bool[BwCount];
            string[] strErrInfo = new string[BwCount];
            string[] strPutApdu = new string[BwCount];

            //设置报警功能
            Helper.EquipHelper.Instance.WarnFun("2A");
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);


            //判断读取状态字3，判断bit7是否为1
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
            string[] statusOpen = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {

                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (statusOpen[i] == "")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回状态值为空";
                    continue;
                }
                else
                {
                    string strAlarm = CLDC_DataCore.Function.Common.HexStrToBinStr(statusOpen[i].Substring(2, 2)).Substring(0, 1);//bit 80

                    if (strAlarm == "0")
                        {
                            ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                            reasonS[i] = "开情况，电表状态字3不应该为0，应该为1;";
                            continue;
                        }                   
                        else
                        {
                            ResultDictionary["结论"][i] = Variable.CMG_HeGe;
                        }
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

           

            if (Stop) return;
            //设置解除报警功能
            Helper.EquipHelper.Instance.WarnFun("2B");

            //判断读取状态字3，判断bit7是否为0
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
            string[] statusClose= MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {

                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (statusClose[i] == "")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回状态值为空";
                    continue;
                }
                else
                {
                    string strAlarm = CLDC_DataCore.Function.Common.HexStrToBinStr(statusClose[i].Substring(2, 2)).Substring(0, 1);//bit 


                    if (strAlarm == "1")
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                        reasonS[i] += "关情况，电表状态字3不应该为1，应该为0;";
                        continue;
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                    }
                }
            }

            UploadTestResult("结论");

        }
    }
}