using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore;

namespace CLDC_VerifyAdapter.Multi
{
    class Dgn_VoltageMomentaryBreakOff : DgnBase
    {
        public Cus_DgnItem Item { get; set; }
        public Cus_VolFallOffType VolType { get; set; }
        public int TestTime { get; set; }
        public Dgn_VoltageMomentaryBreakOff(object plan)
            : base(plan)
        {
        }
        public override void Verify()
        {
            base.Verify();

            string[] arrStrResultKey = new string[BwCount];
            float[] startDL = new float[BwCount];
            float[] endDL = new float[BwCount];
            if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
            {
                return;
            }
            Dictionary<int, float[]> dicStartEnergy = new Dictionary<int, float[]>();//起始电量
            Dictionary<int, float[]> dicEndEnergy = new Dictionary<int, float[]>();//终止电量
            //升源
            if (!PowerOn())
            {
                MessageController.Instance.AddMessage("源输出失败");
                return;
            }
            if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
            {
                return;
            }
            //读取各表位总电量
            MessageController.Instance.AddMessage("正在读取试验前电量...");
            //dicStartEnergy= MeterProtocolAdapter.Instance.ReadEnergy(0);
            startDL = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)0);
            //Array.Copy(Control485.CurResurnFloat, startDL, BwCount);
            //
            if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
            {
                return;
            }
            // 第一种 ：发送 81 40 01 07 01 FF B8  返回 81 40 01 08 11 FF 32 95  
            // 1秒中断一次，连续中断3次，恢复时间为50毫秒
            // 第二种:  发送 81 40 01 07 02 FF BB  返回 81 40 01 08 22 FF 35 A1  
            // 20毫秒中断一次，仅中断一次
            if (!CLDC_VerifyAdapter.Helper.EquipHelper.Instance.SetVoltageShortTimeBreakOff(0x01))
            {
                MessageController.Instance.AddMessage("发送" + "1秒中断一次，连续中断3次" + "命令失败");
                return;
            }
            Thread.Sleep(300);
            if (!CLDC_VerifyAdapter.Helper.EquipHelper.Instance.SetVoltageShortTimeBreakOff(0x02))
            {
                MessageController.Instance.AddMessage("发送" + "20毫秒中断一次，仅中断一次" + "命令失败");
                return;
            }
            //MessageController.Instance.AddMessage("正在进行" + Item.ToString() + "，请等待" + (TestTime / 1000F) + "秒...", false);
            //if (CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
            //{
            //    return;
            //}
            //Thread.Sleep(TestTime);
            //再次读取总电量
            if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
            {
                return;
            }
            MessageController.Instance.AddMessage("正在读取试验后电量...");

            endDL = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)0);
            if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
            {
                return;
            }
            MeterBasicInfo curMeter;
            MeterDgn curResult;
            string strKey = base.ItemKey;
            for (int k = 0; k < BwCount; k++)
            {
                arrStrResultKey[k] = ItemKey;
                curMeter = Helper.MeterDataHelper.Instance.Meter(k);
                if (curMeter.YaoJianYn)
                {
                    //挂接结论
                    if (!curMeter.MeterDgns.ContainsKey(strKey))
                    {
                        curResult = new MeterDgn();
                        curResult.Md_PrjID = strKey;
                        curResult.Md_PrjName = Item.ToString();
                        curMeter.MeterDgns.Add(strKey, curResult);
                    }
                    else
                        curResult = curMeter.MeterDgns[strKey];
                    if (endDL[k] - startDL[k] < 0.01)
                        curResult.Md_chrValue = Variable.CTG_HeGe;
                    else
                        curResult.Md_chrValue = Variable.CTG_BuHeGe;
                    //上报数据
                    
                    
                }
                
            }
            MessageController.Instance.AddMessage("试验完毕...");
            GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);

        }
    }
}
