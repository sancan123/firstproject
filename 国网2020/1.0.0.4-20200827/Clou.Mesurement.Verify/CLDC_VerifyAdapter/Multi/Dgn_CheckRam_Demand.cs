
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Function;


namespace CLDC_VerifyAdapter.Multi
{
    class Dgn_CheckRam_Demand : DgnBase
    {
        public Dgn_CheckRam_Demand(object plan) : base(plan) { }


        public float[] ReadDemand()
        {
            Dictionary<int, float[]> allMeterDemand = MeterProtocolAdapter.Instance.ReadDemand(0);
            float[] values = new float[allMeterDemand.Keys.Count];

            for (int i = 0; i < values.Length; i++)
            {
                float[] tmpV = allMeterDemand[i];
                if (tmpV.Length > 0)
                    values[i] = allMeterDemand[i][0];
            }

            return values;
        }

        public override void Verify()
        {
            int maxRunTime = 960;                   //预定检定时间900秒
            base.Verify();
            if (!PowerOn())
            {
                MessageController.Instance.AddMessage("控制源输出失败");
                return;
            }
            ShowWirteMeterWwaring();
            string[] arrStrResultKey = new string[BwCount];
            bool[] clearResult = MeterProtocolAdapter.Instance.ClearDemand();
            if (CLDC_Comm.Utils.ArrayHelper.IsAllValueMatch(clearResult, true) == false)
            {
                MessageController.Instance.AddMessage("清空电表需量失败,本项检定已终止", 6, 2);
                return;
            }

            float[] readStartDemand = ReadDemand();
            DateTime startTime = DateTime.Now;
            //给最大电压跑二分钟

            if (!Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, _xIb * GlobalUnit.Ib, (int)Cus_PowerYuanJian.H, (int)Cus_PowerFangXiang.正向有功, "1.0", true, false))
            {
                MessageController.Instance.AddMessage("控制源输出失败!");
                return;
            }
            MessageController.Instance.AddMessage("加最大电流走16分钟");
            int lastMinute = 0;
            while (true)
            {
                Thread.Sleep(1000);
                int pastTime = DateTimes.DateDiff(startTime);
                int fMinute = (int)(pastTime * 100 / 60F);
                float curProcess = fMinute / 100F;
                GlobalUnit.g_CUS.DnbData.NowMinute = curProcess;
                MessageController.Instance.AddMessage(string.Format("最多需要16分(有些表只需2分)，已经经过{0}分", GlobalUnit.g_CUS.DnbData.NowMinute));
                if (pastTime >= maxRunTime)
                    break;
                if (lastMinute < fMinute)
                { 
                    //每分钟读取一次数据，检测有没有合格
                    if (ReadAndCheckMeterDemand(readStartDemand, true,arrStrResultKey))
                        break;
                }
            }
            //最后再读取并保存一下数据
            ReadAndCheckMeterDemand(readStartDemand, false, arrStrResultKey);
            GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);
            
        }


        /// <summary>
        /// 挂接默认数据
        /// </summary>
        protected override void DefaultItemData()
        {
            string strKey = ItemKey;
            MeterBasicInfo curMeter = null;
            MeterDgn curResult = null;
            for (int i = 0; i < BwCount; i++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                if (!curMeter.MeterDgns.ContainsKey(strKey))
                {
                    curResult = new MeterDgn();
                    curResult.Md_PrjID = strKey;
                    curResult.Md_PrjName = Cus_DgnItem.需量寄存器检查.ToString();
                    curMeter.MeterDgns.Add(strKey, curResult);
                }
            }
            base.DefaultItemData();
        }

        /// <summary>
        /// 检测当前被检表的需量有没有增加
        /// </summary>
        /// <param name="checkOnly">是否只做检测</param>
        /// <returns></returns>
        private bool ReadAndCheckMeterDemand(float[] readStartDemand, bool checkOnly,string[] keys)
        {
            float[] readEndDemand = ReadDemand();
            string strKey = ItemKey;
            MeterDgn curResult;
            MeterBasicInfo curmeter;
            bool isDemandAdded = false;
            for (int k = 0; k < BwCount; k++)
            {
                curmeter = Helper.MeterDataHelper.Instance.Meter(k);
                if (!curmeter.MeterDgns.ContainsKey(strKey))
                {
                    curResult = new MeterDgn();
                    curResult.Md_PrjID = strKey;
                    curResult.Md_PrjName = Cus_DgnItem.需量寄存器检查.ToString();
                    curmeter.MeterDgns.Add(strKey, curResult);
                }
                else
                    curResult = curmeter.MeterDgns[strKey];
                if (readEndDemand[k] > readStartDemand[k])
                {
                    curResult.Md_chrValue = Variable.CTG_HeGe;
                    isDemandAdded = true;
                    if (checkOnly) break;
                }
                else
                    curResult.Md_chrValue = Variable.CTG_BuHeGe;
                keys[k] = ItemKey;
                
            }
            return isDemandAdded;
        }
    }
}
