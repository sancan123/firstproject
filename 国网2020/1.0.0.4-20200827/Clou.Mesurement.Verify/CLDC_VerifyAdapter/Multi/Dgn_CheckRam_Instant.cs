
using System;
using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;

namespace CLDC_VerifyAdapter.Multi
{
    class Dgn_CheckRam_Instant : DgnBase
    {
        public Dgn_CheckRam_Instant(object plan) : base(plan) { }

        /// <summary>
        /// 重写参数检测
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            MeterBasicInfo curMter = null;
            for (int nType = 0; nType < Helper.MeterDataHelper.Instance.TypeCount; nType++)
            {
                curMter = Helper.MeterDataHelper.Instance.Meter(int.Parse(Helper.MeterDataHelper.Instance.MeterType(nType)[0]));
                if (curMter == null) return false;
                string strKey = ((int)Cus_DgnProcotolPara.瞬时寄存器).ToString("D3");
                if (!curMter.DgnProtocol.DgnPras.ContainsKey(strKey))
                {
                    MessageController.Instance.AddMessage("瞬时寄存器读取参数检测失败");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检定控制
        /// </summary>
        /// <param name="ItemNumber"></param>
        public override void Verify()
        {
            base.Verify();
            if (!PowerOn())
            {
                throw new Exception("控制源输出失败");
            }
            string strKey = ItemKey;
            MeterBasicInfo curMeter = null;
            MeterDgn curResult = null;
            string[] arrStrResultKey = new string[BwCount];
            float[] arrReadData = new float[BwCount];
            int curMeterIndex = 0;
            //每次试验一种表
            for (int nType = 0; nType < Helper.MeterDataHelper.Instance.ProtocolCount; nType++)
            {
                //得到本种表的表号列表
                string[] strMeterList = Helper.MeterDataHelper.Instance.ProtocolType(nType);
                int firstMeterOfThisType = int.Parse(strMeterList[0]);
                string[] arrDgnPara = getType(firstMeterOfThisType, ((int)Cus_DgnProcotolPara.瞬时寄存器).ToString("D3"));
                Check.Require(arrDgnPara.Length > 0, "没有为协议指定事件记录读取参数，请在协议配置工具中设置后再试!");
                arrReadData = MeterProtocolAdapter.Instance.ReadData(arrDgnPara[0], int.Parse(arrDgnPara[1]), int.Parse(arrDgnPara[2]));
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
                            curResult.Md_PrjName = Cus_DgnItem.瞬时寄存器检查.ToString();
                            curMeter.MeterDgns.Add(strKey, curResult);
                        }
                        else
                            curResult = curMeter.MeterDgns[strKey];
                        float fltP = 0F;
                        
                            fltP = arrReadData[curMeterIndex];
                       
                        if (fltP > 0)
                            curResult.Md_chrValue = Variable.CTG_HeGe;
                        else
                            curResult.Md_chrValue = Variable.CTG_BuHeGe;
                        //上报检定结果
                    }
                    arrStrResultKey[k] = ItemKey;
                    
                    
                }
                GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);
                
            }
        }
    }
}
