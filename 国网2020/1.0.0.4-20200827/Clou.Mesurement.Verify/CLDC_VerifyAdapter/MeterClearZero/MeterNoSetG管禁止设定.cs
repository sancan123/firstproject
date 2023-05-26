
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_DataCore.Struct;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.Helper;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.MeterClearZero
{
    /// <summary>
    /// 管禁止设定
    /// </summary>
    class MeterNoSetG : VerifyBase
    {
        public MeterNoSetG(object plan)
            : base(plan)
        {

        }
        private StPlan_ConnProtocol paraStruct = new StPlan_ConnProtocol();
      
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

     
        //  private StPlan_ConnProtocol paraStruct = new StPlan_ConnProtocol();


        protected override bool CheckPara()
        {
            ResultNames = new string[] { "测试时间", "结论", "不合格原因" };
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
            bool[] fClear = new bool[BwCount];//清零操作

          
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取清零事件产生前清零总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
        
            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在模拟计量清零操作");
            //System.Windows.Forms.MessageBox.Show("请手动拔掉管理芯模块并插上工装模块，完成后点击确定。");


            ////需要工装模块，变波特率
            //GlobalUnit.IsGZMK = true;

            ////设置是否要加密，发明文还是密文
            //for (int i = 0; i < BwCount; i++)
            //{
            //    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
            //    if (!string.IsNullOrEmpty(VerifyPara))
            //    {
            //        Helper.MeterDataHelper.Instance.Meter(i).DgnProtocol.IsSouthEncryption = false;
            //    }
            //}
            MessageController.Instance.AddMessage("正在清零");
            fClear = MeterProtocolAdapter.Instance.ClearEnergy();

            //for (int i = 0; i < BwCount; i++)
            //{
            //    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
            //    if (!string.IsNullOrEmpty(VerifyPara))
            //    {
            //        Helper.MeterDataHelper.Instance.Meter(i).DgnProtocol.IsSouthEncryption = true;
            //    }
            //}
            ////变回原来波特率
            //GlobalUnit.IsGZMK = false;

            //System.Windows.Forms.MessageBox.Show("请手动插拔掉工装模块并接上正常掉管理芯模块，完成后点击确定。");

            //读清零总次数
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取清零事件产生后清零总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03300100", 3);

         
            string[] strDataCode = new string[BwCount];
           
          //设置正向有功总电能
            MessageController.Instance.AddMessage("正在设置电量");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            Common.Memset(ref strDataCode, "00010000");
            Common.Memset(ref strData, "00010000" + "00000100");         
            bool[] bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            //读电能量
            if (Stop) return;
            float[] flt_DL = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);
       

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strLoseCountH[i] == "" || strLoseCountQ[i] == "" )
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回次数值为空";
                    continue;
                }
                if(Convert.ToInt32(strLoseCountQ[i]) + 1 != Convert.ToInt32(strLoseCountH[i]))
                {
                     ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "清零次数值不匹配";
                    continue;
                }
                if (bResult[i])
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "电能量数据不能设置";
                    continue;
                }
              
                if (flt_DL[i]!= 0)
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "电能量数据不对,应该为0";
                    continue;
                }
              
               
                else
                {

                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            UploadTestResult("结论");
        }
    }
}