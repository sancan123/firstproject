
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.MeterClearZero
{
    /// <summary>
    /// 计 异常禁止清零
    /// </summary>
    class ExceptMeterNoSetJ : VerifyBase
    {
        public ExceptMeterNoSetJ(object plan)
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
            bool[] fResult = new bool[BwCount];//清零操作

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取校时事件产生总次数");
            string[] strTime0 = MeterProtocolAdapter.Instance.ReadData("03300400", 3);
            if (Stop) return;
            //设置1次校时
            for (int it = 0; it < 1; it++)
            {
                if (Stop) return;
                MessageController.Instance.AddMessage("正在让电能表校时到现在");
                //  DateTime time = DateTime.Now
                MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);
            }

            //产生电量，给清零做铺垫
            if (Stop) return;
            float f = 0.3F;
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
             MessageController.Instance.AddMessage("正在模拟管理芯异常禁止清零操作");
             System.Windows.Forms.MessageBox.Show("请手动插上工装模块并拔掉管理芯模块，完成后点击确定。");


            if (Stop) return;
          
            PowerOn();

            //需要工装模块，变波特率115200
            GlobalUnit.IsGZMK = true;

            //设置是否要加密，发明文还是密文
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(VerifyPara))
                {
                    Helper.MeterDataHelper.Instance.Meter(i).DgnProtocol.IsSouthEncryption = false;
                }
            }
            MessageController.Instance.AddMessage("正在进行第" + 1 + "次清零...");
            fResult = MeterProtocolAdapter.Instance.ClearEnergy();

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(VerifyPara))
                {
                    Helper.MeterDataHelper.Instance.Meter(i).DgnProtocol.IsSouthEncryption = true;
                }
            }
            //变回原来波特率 9600
            GlobalUnit.IsGZMK = false;

            System.Windows.Forms.MessageBox.Show("请手动插拔掉工装模块并接上正常掉管理芯模块，完成后点击确定。");

            //读电能量值，判断是否被清空
            if (Stop) return;
            float[] flt_DL = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "检定数据", ConvertArray.ConvertFloat2Str(flt_DL));
            //读校时记录值，判断是否被清空
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取校时事件产生总次数");
            string[] strTime = MeterProtocolAdapter.Instance.ReadData("03300400", 3);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (flt_DL[i] == 0)//判断电量
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    ResultDictionary["不合格原因"][i] = "异常清零，读取电能量数据不应该为0";
                    continue;
                }
                if (strTime[i] == "000000")//判断结论
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    ResultDictionary["不合格原因"][i] = "异常清零，校时后的记录不应该为0";
                    continue;
                }
                else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;

                }

            }
            UploadTestResult("不合格原因");
            UploadTestResult("结论");

         
        }

    }
}
