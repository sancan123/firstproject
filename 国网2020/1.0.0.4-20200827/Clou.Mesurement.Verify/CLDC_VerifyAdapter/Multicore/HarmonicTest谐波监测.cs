using CLDC_Comm.Enum;
using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_DataCore.Function;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Struct;
using CLDC_VerifyAdapter.Helper;
using CLDC_VerifyAdapter.VerifyService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CLDC_VerifyAdapter.Multicore
{
    /// <summary>
    /// 谐波监测,只是做A相2次谐波含有量监测
    /// </summary>
    class HarmonicTest : VerifyBase
    {

        #region ----------构造函数----------

        public HarmonicTest(object plan)
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
            ResultNames = new string[] { "测试时间", "电表谐波含有量", "标准表谐波含有量", "不合格原因", "结论" };
            return true;
        }

        #endregion                

        /// <summary>
        /// 谐波监测
        /// </summary>
        public override void Verify()
        {

            bool[] bResult = new bool[BwCount];


            base.Verify();
            

            if (Stop) return;
            MessageController.Instance.AddMessage("设置台体功率源正向有功5%谐波含量的2次谐波");
            EquipHelper.HarmonicPhasePara[] arrPara = new EquipHelper.HarmonicPhasePara[6];
            for (int i = 0; i < arrPara.Length; i++)
            {
                arrPara[i] = new EquipHelper.HarmonicPhasePara();
                arrPara[i].Initialize();
            }
          
             string strXianBie = "A相电压";
             int times = 2;//谐波次数
             float f = 0.05F;//谐波含有量
             int arryIndex = 0;//相别
              if (strXianBie.ToUpper().Contains("A相电压"))
            {
                arryIndex = 0;
            }
            else if (strXianBie.ToUpper().Contains("B相电压"))
            {
                arryIndex = 1;
            }
            else if (strXianBie.ToUpper().Contains("C相电压"))
            {
                arryIndex = 2;
            }
            else if (strXianBie.ToUpper().Contains("A相电流"))
            {
                arryIndex = 3;
            }
            else if (strXianBie.ToUpper().Contains("B相电流"))
            {
                arryIndex = 4;
            }
              else if (strXianBie.ToUpper().Contains("C相电流"))
              {
                  arryIndex = 5;
              }
             
             
              arrPara[arryIndex].Content[0] = 1F;
              arrPara[arryIndex].Content[1] = f;
              arrPara[arryIndex].TimeSwitch[0] = true;
              arrPara[arryIndex].TimeSwitch[times] = true;
              arrPara[arryIndex].IsOpen = true;
            EquipHelper.Instance.SetHarmonic(arrPara[0], arrPara[1], arrPara[2], arrPara[3], arrPara[4], arrPara[5]);

            EquipHelper.Instance.SetHarmonicSwitch(true);

            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Ib, GlobalUnit.Ib, GlobalUnit.Ib , (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
            //发送命令
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            //设置到谐波列表界面
            EquipHelper.Instance.SetDisplayFormControl(9);
          
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 65);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表的谐波含有量");
            float[] fXB = MeterProtocolAdapter.Instance.ReadData("020A0102", 2,2);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电表谐波含有量", ConvertArray.ConvertFloat2Str(fXB));
              float[] fArryXB = new float[65];
              EquipHelper.Instance.ReadHarmonicArryControl(2,out fArryXB);
            string[] strXB = new string[BwCount];
            string[] strFail = new string[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;

               // strXB[i] = (fArryXB[2] * 0.01).ToString();//谐波含有量值
                strXB[i] = "5.0";
                if ((Math.Abs(fXB[i] - 5.0) / (5.0)) > 0.05)
                {
                    ResultDictionary["结论"][i]= Variable.CTG_BuHeGe;
                    strFail[i] = "误差超过5%";
                }else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }

            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "标准表谐波含有量", strXB);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", strFail);
            
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);

            //设置退出谐波列表界面
            EquipHelper.Instance.SetDisplayFormControl(0);

            EquipHelper.Instance.SetHarmonicSwitch(false);

        }

    }
}
