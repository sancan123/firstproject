
using System;
using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;

namespace CLDC_VerifyAdapter.Multi
{
    /// <summary>
    /// /*
    ///         实验方法：读取每一个表位时间。然后和GPS时间比对，误差不超过5分钟
    ///        */
    /// </summary>
    class Dgn_TimeError : DgnBase
    {
        public Dgn_TimeError(object plan) : base(plan) { }

        public override void Verify()
        {
            base.Verify();

            DateTime GPSTime = DateTime.Now;
            string[] MeterDataTime = new string[BwCount];

            string[] arrStrResultKey = new string[BwCount];
            string strKey = ItemKey;
            string strDataKey = string.Format("{0}01", strKey);
            if (!PowerOn())
            {
                return;
            }
            GPSTime = Helper.EquipHelper.Instance.ReadGpsTime();


            DateTime[] arrReadData = MeterProtocolAdapter.Instance.ReadDateTime();

            MeterDgn _GPSError = new MeterDgn();
            MeterDgn _DataItem = new MeterDgn();
            for (int iNum = 0; iNum < BwCount; iNum++)
            {
                arrStrResultKey[iNum] = ItemKey;
                if (!Helper.MeterDataHelper.Instance.Meter(iNum).YaoJianYn) continue;
                
                if (Helper.MeterDataHelper.Instance.Meter(iNum).MeterDgns.ContainsKey(strKey))
                    _GPSError = Helper.MeterDataHelper.Instance.Meter(iNum).MeterDgns[strKey];
                else
                {
                    _GPSError = new MeterDgn();
                    _GPSError.Md_PrjID = strKey;
                    _GPSError.Md_PrjName = Cus_DgnItem.时间误差.ToString();
                    Helper.MeterDataHelper.Instance.Meter(iNum).MeterDgns.Add(strKey, _GPSError);
                }
                //数据项目

                if (Helper.MeterDataHelper.Instance.Meter(iNum).MeterDgns.ContainsKey(strDataKey))
                    _DataItem = Helper.MeterDataHelper.Instance.Meter(iNum).MeterDgns[strDataKey];
                else
                {
                    _DataItem = new MeterDgn();
                    _DataItem.Md_PrjID = strDataKey;
                    _DataItem.Md_PrjName = "时间误差数据";
                    Helper.MeterDataHelper.Instance.Meter(iNum).MeterDgns.Add(strDataKey, _DataItem);
                }
                DateTime meterTime = arrReadData[iNum];
                int iTime = CLDC_DataCore.Function.DateTimes.DateDiff(meterTime, GPSTime);
                if (System.Math.Abs(iTime) > 300)
                    _GPSError.Md_chrValue = Variable.CTG_BuHeGe;
                else
                    _GPSError.Md_chrValue = Variable.CTG_HeGe;
                //记录检定数据
                _DataItem.Md_chrValue = string.Format("{0}|{1}|{2}", meterTime.ToString(), GPSTime.ToString(), iTime);

                
            }
            MessageController.Instance.AddMessage("GPS对时试验完毕");
         
        }
    }
}
