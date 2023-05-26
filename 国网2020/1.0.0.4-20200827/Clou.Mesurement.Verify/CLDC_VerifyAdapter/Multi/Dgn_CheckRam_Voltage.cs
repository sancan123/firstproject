
using System;
using CLDC_DataCore;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;

namespace CLDC_VerifyAdapter.Multi
{
    class Dgn_CheckRam_Voltage : DgnBase
    {
        public Dgn_CheckRam_Voltage(object plan) : base(plan) { }

        public override void Verify()
        {
            string strKey = ItemKey;
            MeterBasicInfo curMeter;
            MeterDgn curResult;
            string[] arrStrResultKey = new string[BwCount];
            string[] strMeterList = new string[0];
            int firstMeterOfThisType = 0;
            string readID = ((int)Cus_DgnProcotolPara.失压寄存器).ToString("D3");
            string[] arrPara = new string[0];
            int intLen = 0;
            int intDot = 0;
            float[] arrReadData = new float[BwCount];
            float[] arrReadData2 = new float[BwCount];

            base.Verify();

            if (!PowerOn())
            {
                return;
            }

            //每次试验一种表
            for (int nType = 0; nType < Helper.MeterDataHelper.Instance.ProtocolCount; nType++)
            {
                strMeterList = Helper.MeterDataHelper.Instance.ProtocolType(nType);
                firstMeterOfThisType = int.Parse(strMeterList[0]);
                arrPara = getType(firstMeterOfThisType, readID);
                Check.Require(arrPara.Length > 0, "没有为协议指定失压寄存器读取参数，请在协议配置工具中设置后再试");
                intLen = int.Parse(arrPara[1]);
                intDot = int.Parse(arrPara[2]);

                arrReadData = MeterProtocolAdapter.Instance.ReadData(arrPara[0], intLen, intDot);
                //探源，失压


                if (Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U * 0.5F, (int)Cus_PowerFangXiang.正向有功) == false)
                {
                    MessageController.Instance.AddMessage(string.Format("控制源输出失败"));
                    return;
                }
                //运行二分
                m_StartTime = DateTime.Now;
                while (true)
                {
                    int pastTime = (int)VerifyPassTime;//DateTimes.DateDiff(startTime);
                    if (pastTime > 120)
                        break;
                    MessageController.Instance.AddMessage(string.Format("存在检定，需要120秒，已经进行{0}秒", pastTime));
                    Thread.Sleep(GlobalUnit.g_ThreadWaitTime);
                }
                //再读取一次
                // for (int nType = 0; nType < Helper.MeterDataHelper.Instance.ProtocolCount; nType++)
                // {
                strMeterList = Helper.MeterDataHelper.Instance.ProtocolType(nType);
                firstMeterOfThisType = int.Parse(strMeterList[0]);
                arrPara = getType(firstMeterOfThisType, readID);
                intLen = int.Parse(arrPara[1]);
                intDot = int.Parse(arrPara[2]);

                arrReadData2 = MeterProtocolAdapter.Instance.ReadData(arrPara[0], intLen, intDot);
                //分析结果
                for (int k = 0; k < strMeterList.Length; k++)
                {
                    firstMeterOfThisType = int.Parse(strMeterList[k]);
                    curMeter = Helper.MeterDataHelper.Instance.Meter(firstMeterOfThisType);
                    if (curMeter.YaoJianYn)
                    {
                        if (!curMeter.MeterDgns.ContainsKey(strKey))
                        {
                            curResult = new MeterDgn();
                            curResult.Md_PrjID = strKey;
                            curResult.Md_PrjName = Cus_DgnItem.失压寄存器检查.ToString();
                            curMeter.MeterDgns.Add(strKey, curResult);
                        }
                        else
                            curResult = curMeter.MeterDgns[strKey];
                        if (arrReadData2[firstMeterOfThisType] - arrReadData[firstMeterOfThisType] > 0)
                            curResult.Md_chrValue = Variable.CTG_HeGe;
                        else
                            curResult.Md_chrValue = Variable.CTG_BuHeGe;
                        
                        
                        arrStrResultKey[k] = ItemKey;
                    }
                }
                
            }
            //}
            GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);
        }
    }
}