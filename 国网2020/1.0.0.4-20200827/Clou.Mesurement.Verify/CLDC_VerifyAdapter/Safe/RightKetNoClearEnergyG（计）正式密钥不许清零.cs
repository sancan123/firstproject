
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.Safe
{
    /// <summary>
    /// （计）正式密钥不许清零
    /// </summary>
    class RightKetNoClearEnergyG : VerifyBase
    {
        public RightKetNoClearEnergyG(object plan)
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
            bool[] f = new bool[BwCount];// 操作
             
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取清零事件产生前清零总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            f = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥更新....");
            f = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            string UpdateKeyDate = DateTime.Now.ToString("yyMMddHHmmss");
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            //设置1次清零
            for (int i = 0; i < 1;i++ )
            {
                //产生电量，给清零做铺垫
                if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相四线)
                {
                    CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Imax, GlobalUnit.Imax, GlobalUnit.Imax, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
                }
                else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相三线)
                {
                    bool bPowerOn = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, GlobalUnit.U, GlobalUnit.Imax, 0, GlobalUnit.Imax, 1, 50, "1.0", true, false, false, 0);
                }
                else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.单相)
                {
                    CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, 0, GlobalUnit.Imax, 0, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
         
                }

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 62);
                PowerOn();
                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行第" + (i + 1) + "次清零...");
                PowerOn();
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 *5);
                f=MeterProtocolAdapter.Instance.ClearEnergy();
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 3);
              
            }
          

          
         
            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            CLDC_DataCore.Function.Common.Memset(ref iFlag, 1);
            f = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
        
            MessageController.Instance.AddMessage("正在进行密钥恢复....");
            bool[] blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
      
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取清零事件产生后清零总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
          

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strLoseCountH[i] == "" || strLoseCountQ[i] == "" )
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回日期或次数值为空";
                    continue;
                }
              
                else if ((!string.IsNullOrEmpty(strLoseCountQ[i]) && !string.IsNullOrEmpty(strLoseCountH[i]) && Convert.ToInt32(strLoseCountQ[i]) + 1 == Convert.ToInt32(strLoseCountH[i])) || f[i])
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe ;
                    reasonS[i] = "正式密钥下，不许清零！";
                  
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
