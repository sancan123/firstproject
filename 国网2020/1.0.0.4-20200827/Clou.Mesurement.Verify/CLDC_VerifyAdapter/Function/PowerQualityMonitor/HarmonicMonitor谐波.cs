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
    class HarmonicMonitor:VerifyBase
    {


           #region ----------构造函数----------

        public HarmonicMonitor(object plan)
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
            ResultNames = new string[] { "A相谐波电压含有率", "A相谐波电流含有率", "A相谐波电压含量", "A相谐波电流含量", "总谐波电压畸变率", "总谐波电流畸变率", "总谐波功率", "结论", "不合格原因" };
            return true;
        }

        #endregion                
        public override void Verify()
        {
            base.Verify();

            float Content = 0.4F * 0.01F;
            EquipHelper.HarmonicPhasePara[] arrPara = new EquipHelper.HarmonicPhasePara[6];
            for (int i = 0; i < arrPara.Length; i++)
            {
                arrPara[i] = new EquipHelper.HarmonicPhasePara();
                arrPara[i].Initialize();
            }
            for (int g = 0; g < 3; g++)
            {
                arrPara[g].Content[0] = 1F;
                arrPara[g].Content[1] = 0.1f;
                arrPara[g].TimeSwitch[0] = true;
                arrPara[g].TimeSwitch[1] = true;
                arrPara[g].IsOpen = true;
            }
            for (int h = 3; h < 6; h++)
            {
                arrPara[h].Content[0] = 1F;
                arrPara[h].Content[1] = Content;
                arrPara[h].TimeSwitch[0] = true;
                arrPara[h].TimeSwitch[1] = true;
                arrPara[h].IsOpen = true;
            }

            EquipHelper.Instance.SetHarmonic(arrPara[0], arrPara[1], arrPara[2], arrPara[3], arrPara[4], arrPara[5]);

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



        
            MessageController.Instance.AddMessage("正在读取A相谐波电压含有率");
            float[] A_Dyhyl= MeterProtocolAdapter.Instance.ReadData("020A0102", 2, 2);
            MessageController.Instance.AddMessage("正在读取A相谐波电流含有率");
            float[] A_Dlhyl = MeterProtocolAdapter.Instance.ReadData("020B0102", 2, 2);
            MessageController.Instance.AddMessage("正在读取A相谐波电压含量");
            float[] A_DyHl = MeterProtocolAdapter.Instance.ReadData("020D0100", 3, 3);
            MessageController.Instance.AddMessage("正在读取A相谐波电流含量");
            float[] A_DlHl = MeterProtocolAdapter.Instance.ReadData("020E0100", 3, 3);
            MessageController.Instance.AddMessage("正在读取总谐波电压畸变率");
            float[] flt_DYJBL = MeterProtocolAdapter.Instance.ReadData("03080100", 2, 2);
            MessageController.Instance.AddMessage("正在读取总谐波电流畸变率");
            float[] flt_DLJBL = MeterProtocolAdapter.Instance.ReadData("03090100", 2, 2);
            MessageController.Instance.AddMessage("正在读取总谐波功率");
            float[] flt_XBGL = MeterProtocolAdapter.Instance.ReadData("020F0000", 4,6);




            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    ResultDictionary["A相谐波电压含有率"][i] = A_Dyhyl[i].ToString();
                    ResultDictionary["A相谐波电流含有率"][i] = A_Dlhyl[i].ToString();
                    ResultDictionary["A相谐波电压含量"][i] = A_DyHl[i].ToString();
                    ResultDictionary["A相谐波电流含量"][i] = A_DlHl[i].ToString();
                    ResultDictionary["总谐波电压畸变率"][i] = flt_DYJBL[i].ToString();
                    ResultDictionary["总谐波电流畸变率"][i] = flt_DLJBL[i].ToString();
                    ResultDictionary["总谐波功率"][i] = flt_XBGL[i].ToString();                                                             //"功率元件","功率方向","电流倍数","功率因数","误差下限","误差上限","误差圈数"
                    if (A_Dyhyl[i] != -1 && A_Dlhyl[i] != -1 && A_DyHl[i] != -1 && A_DlHl[i] != -1 && flt_DYJBL[i] != -1 && flt_DLJBL[i] != -1 && flt_XBGL[i] != -1)
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                  
                   
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "A相谐波电压含有率", ResultDictionary["A相谐波电压含有率"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "A相谐波电流含有率", ResultDictionary["A相谐波电流含有率"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "A相谐波电压含量", ResultDictionary["A相谐波电压含量"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "A相谐波电流含量", ResultDictionary["A相谐波电流含量"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "总谐波电压畸变率", ResultDictionary["总谐波电压畸变率"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "总谐波电流畸变率", ResultDictionary["总谐波电流畸变率"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "总谐波功率", ResultDictionary["总谐波功率"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);
         




        }

   

    }
}
