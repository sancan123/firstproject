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
using CLDC_VerifyAdapter.Helper;


namespace CLDC_VerifyAdapter.Function.PowerQualityMonitor
{
    class InterHarmonicsMonitor:VerifyBase
    {


           #region ----------构造函数----------

        public InterHarmonicsMonitor(object plan)
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
            ResultNames = new string[] { "A相谐间波电压含有率", "A相谐间波电流含有率", "结论", "不合格原因" };
            return true;
        }

        #endregion                
        public override void Verify()
        {
            base.Verify();

            float Content = 0.4F * 0.01F;
            EquipHelper.Instance.SettingWaveformSelection(0, 9, 0, 9, 0, 9);

            EquipHelper.Instance.SetHarmonicSwitch(true);







            bool result = false;
            bool ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U,10* GlobalUnit.Itr, 1, 1, "1.0", true, false);

            if (!result)
            {
                MessageController.Instance.AddMessage("控制源输出失败", 6, 2);

            }
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 20);

           bool[] Result = new bool[BwCount];
           string[] Fail = new string[BwCount];




           MessageController.Instance.AddMessage("正在读取A相谐间波电压含有率");
            float[] A_Dyhyl= MeterProtocolAdapter.Instance.ReadData("021B0102", 2, 2);
            MessageController.Instance.AddMessage("正在读取A相谐间波电流含有率");
            float[] A_Dlhyl = MeterProtocolAdapter.Instance.ReadData("021C0102", 2, 2);
           




            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    ResultDictionary["A相谐间波电压含有率"][i] = A_Dyhyl[i].ToString();
                    ResultDictionary["A相谐间波电流含有率"][i] = A_Dlhyl[i].ToString();
                                                                          //"功率元件","功率方向","电流倍数","功率因数","误差下限","误差上限","误差圈数"
                    if (A_Dyhyl[i] != -1 && A_Dlhyl[i] != -1 )
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                  
                   
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "A相谐间波电压含有率", ResultDictionary["A相谐间波电压含有率"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "A相谐间波电流含有率", ResultDictionary["A相谐间波电流含有率"]);
          
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);
         




        }

   

    }
}