
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore;

namespace CLDC_VerifyAdapter.Multi
{
    class Dgn_ClearDemand : DgnBase
    {
        public Dgn_ClearDemand(object plan) : base(plan) { }

        public override void Verify()
        {
            base.Verify();
            if (!PowerOn())
            {
                MessageController.Instance.AddMessage("源输出失败", 6, 2);
                return;
            
            }
            string[] arrStrResultKey = new string[BwCount];
            ShowWirteMeterWwaring();
            bool[] clearResult = MeterProtocolAdapter.Instance.ClearDemand();
            string strKey = ((int)Cus_DgnItem.需量清空).ToString("000");
            float[] readDemand = MeterProtocolAdapter.Instance.ReadDemand(0x00,(byte) 0);
            if (CLDC_Comm.Utils.ArrayHelper.IsAllValueMatch(readDemand, -1F))
            {
                MessageController.Instance.AddMessage("读取电能表需量失败");
                return;
            }
            bool[] result=new bool[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                MeterDgn resultData = new MeterDgn();
                CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                if (Helper.MeterDataHelper.Instance.Meter(i).MeterDgns.ContainsKey(strKey))
                    resultData = Helper.MeterDataHelper.Instance.Meter(i).MeterDgns[strKey];
                else
                    Helper.MeterDataHelper.Instance.Meter(i).MeterDgns.Add(strKey, resultData);
                if (readDemand[i] == 0F)
                {
                    resultData.Md_chrValue = Variable.CTG_HeGe;
                }
                else
                {
                    resultData.Md_chrValue = Variable.CTG_BuHeGe;
                }
                result[i] = (resultData.Md_chrValue == Variable.CTG_HeGe);

                string keyitem = ((int)CLDC_Comm.Enum.Cus_DgnItem.需量清空).ToString().PadLeft(3, '0');
                string strResult = result[i] ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;
                if (curMeter.MeterDgns.ContainsKey(keyitem))
                {
                    curMeter.MeterDgns[keyitem].Md_chrValue = strResult;
                    curMeter.MeterDgns[keyitem].Md_PrjID = keyitem;
                    curMeter.MeterDgns[keyitem].Md_PrjName = CLDC_Comm.Enum.Cus_DgnItem.需量清空.ToString();
                }
                else
                {
                    CLDC_DataCore.Model.DnbModel.DnbInfo.MeterDgn mr = new CLDC_DataCore.Model.DnbModel.DnbInfo.MeterDgn();
                    mr.Md_PrjName = CLDC_Comm.Enum.Cus_DgnItem.需量清空.ToString();
                    mr.Md_PrjID = keyitem;
                    mr._intMyId = curMeter._intMyId;
                    mr.Md_chrValue = strResult;
                    curMeter.MeterDgns.Add(keyitem, mr);
                }
                arrStrResultKey[i] = ItemKey;

                
            }
        //    GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);
            ControlResult(result);
        }
    }
}
