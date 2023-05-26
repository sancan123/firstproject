using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_VerifyAdapter.Helper;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.EventLog
{
    /// <summary>
    /// 谐波超限
    /// </summary>
    class EL_OverHarmonic : EventLogBase
    {
        public EL_OverHarmonic(object plan)
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
            ResultNames = new string[] { "测试时间", "事件产生前事件次数", "事件产生后事件次数", "上1次事件记录发生时刻", "结论", "不合格原因" };
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

            string strDiCount = "", strDiTime = "";


            if (Stop) return;


            string strXianBie = VerifyPara;
            int times = 2;//谐波次数
            float f = 0.6F;//谐波含有量
            int arryIndex = 0;//相别
            if (!string.IsNullOrEmpty(VerifyPara))
            {


                if (strXianBie.ToUpper().Contains("A相电压"))
                {
                    arryIndex = 0;
                    strDiCount = "03B70100";
                    strDiTime = "03B70101";
                }
                else if (strXianBie.ToUpper().Contains("B相电压"))
                {
                    arryIndex = 1;
                    strDiCount = "03B70200";
                    strDiTime = "03B70201";
                }
                else if (strXianBie.ToUpper().Contains("C相电压"))
                {
                    arryIndex = 2;
                    strDiCount = "03B70300";
                    strDiTime = "03B70301";
                }
                else if (strXianBie.ToUpper().Contains("A相电流"))
                {
                    arryIndex = 3;
                    strDiCount = "03B80100";
                    strDiTime = "03B80101";
                }
                else if (strXianBie.ToUpper().Contains("B相电流"))
                {
                    arryIndex = 4;
                    strDiCount = "03B80200";
                    strDiTime = "03B80201";
                }
                else if (strXianBie.ToUpper().Contains("C相电流"))
                {
                    arryIndex = 5;
                    strDiCount = "03B80300";
                    strDiTime = "03B80301";
                }
            }
            if (Stop) return;
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strDataCode = new string[BwCount];
            string[] strData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            MessageController.Instance.AddMessage("正在设置电压总谐波畸变率阈值");
            Common.Memset(ref strDataCode, "04093901");
            Common.Memset(ref strData, "04093901" + "0500");
            bool[] bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            MessageController.Instance.AddMessage("正在设置电压总谐波畸变率超限判定延时时间");
            Common.Memset(ref strDataCode, "04093902");
            Common.Memset(ref strData, "04093902" + "10");
            bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            MessageController.Instance.AddMessage("正在设置电流总谐波畸变率阈值");
            Common.Memset(ref strDataCode, "04093A01");
            Common.Memset(ref strData, "04093A01" + "0500");
            bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            MessageController.Instance.AddMessage("正在设置电流总谐波畸变率超限判定延时时间");
            Common.Memset(ref strDataCode, "04093A02");
            Common.Memset(ref strData, "04093A02" + "0500");
            bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在事件前谐波超限总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData(strDiCount, 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("设置台体功率源正向有功5%谐波含量的2次谐波");
            EquipHelper.HarmonicPhasePara[] arrPara = new EquipHelper.HarmonicPhasePara[6];
            for (int i = 0; i < arrPara.Length; i++)
            {
                arrPara[i] = new EquipHelper.HarmonicPhasePara();
                arrPara[i].Initialize();
            }
            arrPara[arryIndex].Content[0] = 1F;
            arrPara[arryIndex].Content[1] = f;
            arrPara[arryIndex].TimeSwitch[0] = true;
            arrPara[arryIndex].TimeSwitch[times] = true;
            arrPara[arryIndex].IsOpen = true;
            EquipHelper.Instance.SetHarmonic(arrPara[0], arrPara[1], arrPara[2], arrPara[3], arrPara[4], arrPara[5]);

            EquipHelper.Instance.SetHarmonicSwitch(true);

            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Ib, GlobalUnit.Ib, GlobalUnit.Ib, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
            //发送命令
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            //设置到谐波列表界面
            EquipHelper.Instance.SetDisplayFormControl(9);

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 63);
            //设置退出谐波列表界面
            EquipHelper.Instance.SetDisplayFormControl(0);

            EquipHelper.Instance.SetHarmonicSwitch(false);
            PowerOn();
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 63);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在事件后谐波超限总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData(strDiCount, 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次谐波超限发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData(strDiTime, 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strLoseCountQ[i] == "" || strLoseCountH[i] == "" || strLoseTimeQ[i] == "")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回日期或次数值为空";
                    continue;
                }
                if (strLoseTimeQ[i] == "000000000000")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回日期为0";
                    continue;
                }
                if (!string.IsNullOrEmpty(strLoseCountQ[i]) && !string.IsNullOrEmpty(strLoseCountH[i]) && Convert.ToInt32(strLoseCountQ[i]) + 1 == Convert.ToInt32(strLoseCountH[i]))
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }
                else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "次数不匹配";
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            UploadTestResult("结论");



        }


    }
}
