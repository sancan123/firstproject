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
using CLDC_Comm.Enum;

namespace CLDC_VerifyAdapter.Function.HarmonicMonitor
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
            ResultNames = new string[] { "测试时间","谐波电压含有率", "谐波电流含有率", "电压总谐波畸变率", "电流总谐波畸变率", "谐波电压含量", "谐波电流含量", "基波电压", "基波电流", "结论", "不合格原因" };
            return true;
        }

        #endregion                
        public override void Verify()
        {
            base.Verify();

            string dyHylDI = "",dlHylDI = "",dyJblDI="",dlJblDI="",dyHlDI = "",dlHlDI = "",jbDyDI = "",jbDlDI = "";


            string[] arrPara = VerifyPara.Split('|');
            if (arrPara[0] == "H")
            {
                MessageController.Instance.AddMessage("请重新配置方案，谐波相别错误...");
                Stop = true;
            }
            if (Stop) return;

            string DIcs = Convert.ToInt16(arrPara[1]).ToString().PadLeft(2,'0');
            int cs = Convert.ToInt32(arrPara[1])-1;
            float Content = float.Parse(arrPara[1].Replace("%", "")) * 0.01f;
            EquipHelper.HarmonicPhasePara[] arr = new EquipHelper.HarmonicPhasePara[6];
            for (int i = 0; i < arrPara.Length; i++)
            {
                arr[i] = new EquipHelper.HarmonicPhasePara();
                arr[i].Initialize();
            }

            int PhDy = 2,PhDl = 5;
         
            if (arrPara[0] == "A")
            {
                //电压含有率
                dyHylDI = "020A01" + DIcs;

                //电流含有率
                dlHylDI = "020B01" + DIcs;

                //电压总谐波畸变率
                dyJblDI = "02080100";

                //电流总谐波畸变率
                dlJblDI = "02090100";

                //谐波电压含量
                dyHlDI = "020D0100";

                //谐波电流含量
                dlHlDI = "020E0100";

                //基波电压
                jbDyDI = "02170100";

                //基波电流
                jbDlDI = "02180100";
                arr[0].Content[0] = 1F;
                arr[0].Content[cs] = Content;
                arr[0].TimeSwitch[0] = true;
                arr[0].TimeSwitch[cs] = true;
                arr[0].IsOpen = true;

                arr[3].Content[0] = 1F;
                arr[3].Content[cs] = Content;
                arr[3].TimeSwitch[0] = true;
                arr[3].TimeSwitch[cs] = true;
                arr[3].IsOpen = true;
                PhDy = 2;
                PhDl = 5;

            }
            else if (arrPara[0] == "B")
            {
                //电压含有率
                dyHylDI = "020A02" + DIcs;

                //电流含有率
                dlHylDI = "020B02" + DIcs;

                //电压总谐波畸变率
                dyJblDI = "02080200";

                //电流总谐波畸变率
                dlJblDI = "02090200";

                //谐波电压含量
                dyHlDI = "020D0200";

                //谐波电流含量
                dlHlDI = "020E0200";

                //基波电压
                jbDyDI = "02170200";

                //基波电流
                jbDlDI = "02180200";
                arr[1].Content[0] = 1F;
                arr[1].Content[cs] = Content;
                arr[1].TimeSwitch[0] = true;
                arr[1].TimeSwitch[cs] = true;
                arr[1].IsOpen = true;

                arr[4].Content[0] = 1F;
                arr[4].Content[cs] = Content;
                arr[4].TimeSwitch[0] = true;
                arr[4].TimeSwitch[cs] = true;
                arr[4].IsOpen = true;
                PhDy = 1;
                PhDl = 4;

            } if (arrPara[0] == "C")
            {
                //电压含有率
                dyHylDI = "020A03" + DIcs;

                //电流含有率
                dlHylDI = "020B03" + DIcs;

                //电压总谐波畸变率
                dyJblDI = "02080300";

                //电流总谐波畸变率
                dlJblDI = "02090300";

                //谐波电压含量
                dyHlDI = "020D0300";

                //谐波电流含量
                dlHlDI = "020E0300";

                //基波电压
                jbDyDI = "02170300";

                //基波电流
                jbDlDI = "02180300";
                arr[2].Content[0] = 1F;
                arr[2].Content[cs] = Content;
                arr[2].TimeSwitch[0] = true;
                arr[2].TimeSwitch[cs] = true;
                arr[2].IsOpen = true;

                arr[5].Content[0] = 1F;
                arr[5].Content[cs] = Content;
                arr[5].TimeSwitch[0] = true;
                arr[5].TimeSwitch[cs] = true;
                arr[5].IsOpen = true;
                PhDy = 0;
                PhDl = 3;
            }


            EquipHelper.Instance.SetHarmonic(arr[0], arr[1], arr[2], arr[3], arr[4], arr[5]);

            EquipHelper.Instance.SetHarmonicSwitch(true);

         //   ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            //设置到谐波列表界面
            EquipHelper.Instance.SetDisplayFormControl(9);



            MessageController.Instance.AddMessage("开始控制源输出!");
            bool result = false;




            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Ib, GlobalUnit.Ib, GlobalUnit.Ib, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);




            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);

            float[] fArryXBDy = new float[65];
            float[] fArryXBDl = new float[65];

            EquipHelper.Instance.ReadHarmonicArryControl(PhDy, out fArryXBDy);
            EquipHelper.Instance.ReadHarmonicArryControl(PhDy, out fArryXBDl);













        }

       

    }
}
