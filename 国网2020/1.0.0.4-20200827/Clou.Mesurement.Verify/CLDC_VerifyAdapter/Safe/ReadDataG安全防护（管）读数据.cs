
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

namespace CLDC_VerifyAdapter.Safe
{
    /// <summary>
    /// 安全防护（管）读数据
    /// </summary>
    public class ReadDataG : VerifyBase
    {
                      
        protected override string ItemKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string ResultKey
        {
            get { throw new NotImplementedException(); }
        }

        public ReadDataG(object plan)
            : base(plan)
        {
             ResultNames = new string[] { "测试时间","不合格原因", "结论" };
        }




            #region ----------开始检定----------
        /// <summary>
        /// 开始检定
        /// </summary>
        public override void Verify()
        {
            if (Stop) return;
            PowerOn();
            base.Verify();

            float f = 0.3f;
            //产生电量，给读电量做铺垫
            if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相四线)
            {
                CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Imax * f, GlobalUnit.Imax * f, GlobalUnit.Imax * f, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
            }
            else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相三线)
            {
                bool bPowerOn = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, GlobalUnit.U, GlobalUnit.Imax * f, 0, GlobalUnit.Imax * f, 1, 50, "1.0", true, false, false, 0);
            }
            else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.单相)
            {
                CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, 0, GlobalUnit.Imax * f, 0, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);

            }
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 62);
            PowerOn();
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取当前正向有功总电能,请稍候....");
            string[] strEnerZQ = new string[BwCount];//
            float[] fEnerZQ = MeterProtocolAdapter.Instance.ReadData("00010000", 4,2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                strEnerZQ[i] = fEnerZQ[i].ToString();
            }
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "读数据值", strEnerZQ);
            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {

                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strEnerZQ[i] == "")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回状态值为空";
                    continue;
                }
                else
                {
                    if (strEnerZQ[i] == "-1" || strEnerZQ[i] == "0")
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                        reasonS[i] = "返回正向有功总电能值不正确";
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
        #endregion

      
        
        
    }
}

