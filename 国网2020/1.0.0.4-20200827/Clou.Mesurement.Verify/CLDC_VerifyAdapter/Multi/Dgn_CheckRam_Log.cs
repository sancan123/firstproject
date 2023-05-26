
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_DataCore;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;

namespace CLDC_VerifyAdapter.Multi
{

    /// <summary>
    /// 事件记录检查
    /// </summary>
    class Dgn_CheckRam_Log : DgnBase
    {

        public Dgn_CheckRam_Log(object plan) : base(plan) { }

        private string ReadID
        {
            get
            {

                return ((int)Cus_DgnProcotolPara.事件记录).ToString("D3");
            }
        }

        protected override bool CheckPara()
        {

            // TODO: 协议参数检测
            return base.CheckPara();
        }



        public override void Verify()
        {
            base.Verify();
            if (!PowerOn())
            {
                MessageController.Instance.AddMessage("源输出失败");
                return;
            }
            //try
            //{
            //string strKey = ((int)Cus_DgnProcotolPara.事件记录).ToString("D3");
            //每一批表做一次
            string[] arrStrResultKey = new string[BwCount];
            string[] thisTypeMeter = new string[0];
            int meterNum = 0;
            string[] arrPara = new string[0];
            string strID = string.Empty;
            int intLen = 0;
            int intDot = 0;
            //int programCountBefore = 0;         //写时间前编程次数计数
            //int programCountAfterWriteTime = 0; //写时间后编程次数计数
            float[] strReturn = new float[0];//编程前事件数据
            float[] strReturn2 = new float[0]; //编程后事件数据

            for (int n = 0; n < Helper.MeterDataHelper.Instance.ProtocolCount; n++)
            {
                thisTypeMeter = Helper.MeterDataHelper.Instance.ProtocolType(n);
                meterNum = int.Parse(thisTypeMeter[0]);
                arrPara = getType(meterNum, ReadID);
                Check.Require(arrPara.Length > 0, "没有为协议指定事件记录读取参数，请在协议配置工具中设置后再试!");

                strID = arrPara[0];
                intLen = int.Parse(arrPara[1]);
                intDot = int.Parse(arrPara[2]);
                strReturn = new float[BwCount];
                //读一次
                MessageController.Instance.AddMessage("开始读取表编程次数");
                strReturn = MeterProtocolAdapter.Instance.ReadData(strID, intLen, intDot); 

                //写一次表时间
                MessageController.Instance.AddMessage("开始对表进行写操作");
                //System.Windows.Forms.MessageBox.Show("请确认打开电表编程键");
                //bool[] isSc= MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
                bool[] isSc = MeterProtocolAdapter.Instance.ClearDemand();
                if (CLDC_Comm.Utils.ArrayHelper.IsAllValueMatch(isSc, true) == false)
                {
                    MessageController.Instance.AddMessage("清空电能表需量失败", 6, 2);
                    return;
                }
                Thread.Sleep(1000);
                //再读一次
                MessageController.Instance.AddMessage("再次读取表编程次数");
                strReturn2=MeterProtocolAdapter.Instance.ReadData(strID, intLen, intDot);

                string strKey = ItemKey;
                MeterBasicInfo curMeter;
                MeterDgn curResult;
                for (int k = 0; k < thisTypeMeter.Length; k++)
                {
                
                    int curNum = int.Parse(thisTypeMeter[k]);
                    curMeter = Helper.MeterDataHelper.Instance.Meter(curNum);
                    if (!curMeter.YaoJianYn) continue;
                    if (!curMeter.MeterDgns.ContainsKey(strKey))
                    {
                        curResult = new MeterDgn();
                        curResult.Md_PrjID = strKey;
                        curResult.Md_PrjName = Cus_DgnItem.事件记录检查.ToString();
                        curMeter.MeterDgns.Add(strKey, curResult);
                    }
                    else
                    {
                        curResult = curMeter.MeterDgns[strKey];
                    }//结论
                    //int.TryParse(strReturn[curNum], out programCountBefore);
                   // int.TryParse(strReturn2[curNum], out programCountAfterWriteTime);
                    bool result = strReturn2[curNum] > strReturn[curNum];//int.Parse(Multi.Control485.CurReturnString[curNum]) > int.Parse(strReturn[curNum]);
                    curResult.Md_chrValue = CLDC_DataCore.Function.Common.ConverResult(result);
                    
                    
                    arrStrResultKey[k] = ItemKey;
                }
            //    GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);
                
            }
            //}
            //catch (System.Exception e)
            //{
            //    Comm.MessageController.Instance.AddMessage(e.Message);
            //    Thread.Sleep(100);
            //}
        }
    }
}
