
using System.Threading;
using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;

namespace CLDC_VerifyAdapter.Multi
{
    class Dgn_CheckRam_State : DgnBase
    {

        public Dgn_CheckRam_State(object plan) : base(plan) { }

        /// <summary>
        /// 检定控制
        /// </summary>
        /// <param name="ItemNumber"></param>
        public override void Verify()
        {
            base.Verify();

            if (!PowerOn())
            {
                MessageController.Instance.AddMessage("控制源输出失败");
                Thread.Sleep(100);
                return;
            }
            string[] arrStrResultKey = new string[BwCount];
            string[] strMeterList = new string[0];
            int firstMeterOfThisType = 0;
            float[] arrReadData = new float[BwCount];
            string ReadID = ((int)Cus_DgnProcotolPara.状态寄存器).ToString("D3");
            string[] arrPata = new string[0];
            int intLen = 0;
            int intDot = 0;
            string strKey = ItemKey;
            MeterBasicInfo curMeter;
            MeterDgn curResult;
            int curMeterIndex = 0;
            //每次试验一种表
            for (int nType = 0; nType < Helper.MeterDataHelper.Instance.ProtocolCount; nType++)
            {
                strMeterList = Helper.MeterDataHelper.Instance.ProtocolType(nType);
                firstMeterOfThisType = int.Parse(strMeterList[0]);
                arrPata = getType(firstMeterOfThisType, ReadID);
                Check.Require(arrPata.Length > 0, "没有为协议指定状态寄存器读取参数，请在协议配置工具中设置后再试");

                intLen = int.Parse(arrPata[1]);
                intDot = int.Parse(arrPata[2]);
                arrReadData = MeterProtocolAdapter.Instance.ReadData(arrPata[0], intLen, intDot); 
                for (int k = 0; k < strMeterList.Length; k++)
                {
                    curMeterIndex = int.Parse(strMeterList[k]);
                    curMeter = Helper.MeterDataHelper.Instance.Meter(curMeterIndex);
                    if (curMeter.YaoJianYn)
                    {
                        if (!curMeter.MeterDgns.ContainsKey(strKey))
                        {
                            curResult = new MeterDgn();
                            curResult.Md_PrjID = strKey;
                            curResult.Md_PrjName = Cus_DgnItem.状态寄存器检查.ToString();
                            curMeter.MeterDgns.Add(strKey, curResult);
                        }
                        else
                            curResult = curMeter.MeterDgns[strKey];
                        if (arrReadData[k]!=-1)
                            curResult.Md_chrValue = Variable.CTG_HeGe;
                        else
                            curResult.Md_chrValue = Variable.CTG_BuHeGe;
                        
                        
                    }
                    
                    arrStrResultKey[k] = ItemKey;
                }
            }
            GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);
        }
    }
}
