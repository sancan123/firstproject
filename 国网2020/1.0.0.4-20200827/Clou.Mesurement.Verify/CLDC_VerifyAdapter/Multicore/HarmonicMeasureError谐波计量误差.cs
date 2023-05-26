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
    class HarmonicMeasureError : VerifyBase
    {

        #region ----------构造函数----------

        public HarmonicMeasureError(object plan)
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
            ResultNames = new string[] { "电表的正向有功谐波总电能", "标准表的正向有功谐波总电能", "电表的反向有功谐波总电能", "标准表的反向有功谐波总电能", "结论" };
            return true;
        }

        #endregion                

        /// <summary>
        /// 基本误差和标准偏差误差检定
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
            arrPara[0].Content[0] = 1F;
            arrPara[0].Content[1] = 0.05F;
            arrPara[0].TimeSwitch[0] = true;
            arrPara[0].TimeSwitch[1] = true;
            arrPara[0].IsOpen = true;

            EquipHelper.Instance.SetHarmonic(arrPara[0], arrPara[1], arrPara[2], arrPara[3], arrPara[4], arrPara[5]);

            EquipHelper.Instance.SetHarmonicSwitch(true);

            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Ib, GlobalUnit.Ib, GlobalUnit.Ib , (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);




                //发送命令

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表的正向有功谐波总电能");
            string[] strEnergyP1 = MeterProtocolAdapter.Instance.ReadData("06100901", 5);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电表的正向有功谐波总电能", strEnergyP1);

            if (Stop) return;
     //       MessageController.Instance.AddMessage("正在读取电表的反向有功谐波总电能");
      //      string[] strEnergyP2 = MeterProtocolAdapter.Instance.ReadData("06100902", 5);
      //      MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电表的正向有功谐波总电能", strEnergyP2);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取标准表的正向有功谐波总电能");


            //发送命令

         //   if (Stop) return;
          //  MessageController.Instance.AddMessage("正在读取标准表的反向有功谐波总电能");
           //发送命令
          //  EquipHelper.Instance.SetHarmonicSwitch(false);

            MessageController.Instance.AddMessage("正在处理结果...");
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                //判断结论

                ResultDictionary["结论"][i] = "";
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);


        }

    }
}
