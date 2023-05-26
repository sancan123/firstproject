using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_Comm.Enum;
namespace CLDC_VerifyAdapter.Warn
{
    /// <summary>
    /// 报警配置,配置失压
    /// </summary>
    class EL_WarnSetG : VerifyBase
    {
        public EL_WarnSetG(object plan)
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
            ResultNames = new string[] { "测试时间",  "结论", "不合格原因" };
            return true;
        }

        /// 重写基类测试方法
        /// </summary>
        /// <param name="ItemNumber">检定方案序号</param>
        public override void Verify()
        {

            if (Stop) return;
            base.Verify();
            PowerOn();
            string[] strID = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strSetData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strShowData = new string[BwCount];
            string[] strCode = new string[BwCount];
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机
            string[] strEsamNo = new string[BwCount];//Esam序列号


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取报警输出配置模式");
            string[] strStatusOpen = MeterProtocolAdapter.Instance.ReadData("04001801", 4);//00405829


            if (Stop) return;
            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            //身份认证

            string[] strDataCode = new string[BwCount];
            //报警输出配置模式,配置失压
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            Common.Memset(ref strDataCode, "04001801");
            Common.Memset(ref strData, "04001801" + "00100000");// 
            bool[] bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);



            if (Stop) return;

          
                if (Stop) return;
                MessageController.Instance.AddMessage("正在让电能表失压");
                //  DateTime time = DateTime.Now
                CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U * 0.70F, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Itr, GlobalUnit.Itr, GlobalUnit.Itr, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 65);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取开状态下的电表运行状态字4");
                strStatusOpen = MeterProtocolAdapter.Instance.ReadData("04000504", 2);
              
            CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Itr, GlobalUnit.Itr, GlobalUnit.Itr, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 62);
            

          





            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {

                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strStatusOpen[i] == "")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回状态值为空";
                    continue;
                }
                else
                {
                    string strOpen = CLDC_DataCore.Function.Common.HexStrToBinStr(strStatusOpen[i].Substring(2, 2)).Substring(7, 1);// 

                    if (strOpen == "0")
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                        reasonS[i] = "电表运行状态字4不应该为0,应该为1;";
                        continue;
                    }

                    else
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                    }
                }
               
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);
            UploadTestResult("结论");
        }
    }
}

