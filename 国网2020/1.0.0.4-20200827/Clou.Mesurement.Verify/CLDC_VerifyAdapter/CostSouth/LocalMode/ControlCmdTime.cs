using System;
using CLDC_DataCore;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 控制命令有效时间合法性
    /// </summary>
    public class ControlCmdTime : VerifyBase
    {
        protected override string ItemKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string ResultKey
        {
            get { throw new NotImplementedException(); }
        }

        //远程拉闸命令帧时间1|表时间1|拉闸命令1|命令时间超限不执行1|
       //远程合闸命令帧时间2|表时间2|合闸命令2|命令时间超限不执行2

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "远程拉闸命令帧时间1", "表时间1","拉闸命令1","命令时间超限不执行1",
                                         "远程合闸命令帧时间2","表时间2","合闸命令2", "命令时间超限不执行2","结论" };
            return true;
        }

        public ControlCmdTime(object plan)
            : base(plan)
        {
        }
        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            PowerOn();
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strData = new string[BwCount];//明文
            string[] strID = new string[BwCount];
            string[] strDataTmp = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 4];
            string[] strShowData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            bool[] result = new bool[BwCount];

            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strID[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

            //1-----------------
            if (Stop) return;
            Common.Memset(ref  strData, "1A00" + DateTime.Now.AddMinutes(-10).ToString("yyMMddHHmmss"));
            MessageController.Instance.AddMessage("正在通过远程发送跳闸命令,请稍候....");
            Common.Memset(ref strDataTmp,DateTime.Now.AddMinutes(-10).ToString("yyMMddHHmmss"));
            result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "远程拉闸命令帧时间1", strDataTmp);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取表里时间,请稍候....");
            DateTime[] MeterDate = MeterProtocolAdapter.Instance.ReadDateTime();
            for (int i = 0; i < MeterDate.Length; i++)
            {
                strShowData[i] = MeterDate[i].ToString();
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表时间1", strShowData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                blnRet[i, 0] = result[i];
                ResultDictionary["拉闸命令1"][i] = result[i] ? "正常应答" : "异常应答";
                ResultDictionary["命令时间超限不执行1"][i] = !result[i] ? "通过" : "不通过";
            }
            UploadTestResult("拉闸命令1");
            UploadTestResult("命令时间超限不执行1");

            //远程拉闸命令帧时间1|表时间1|拉闸命令1|命令时间超限不执行1|


            //2----------------------
            if (Stop) return;
            Common.Memset(ref strData, "1C00" + System.DateTime.Now.AddMinutes(-10).ToString("yyMMddHHmmss"));
            MessageController.Instance.AddMessage("正在通过远程发送合闸命令,请稍候....");
            Common.Memset(ref strDataTmp,DateTime.Now.AddMinutes(-10).ToString("yyMMddHHmmss"));
            result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "远程合闸命令帧时间2", strDataTmp);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取表里时间,请稍候....");
            MeterDate = MeterProtocolAdapter.Instance.ReadDateTime();
            for (int i = 0; i < MeterDate.Length; i++)
            {
                strShowData[i] = MeterDate[i].ToString();
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表时间2", strShowData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                blnRet[i, 1] = result[i];
                ResultDictionary["合闸命令2"][i] = result[i] ? "正常应答" : "异常应答";
                ResultDictionary["命令时间超限不执行2"][i] = !result[i] ? "通过" : "不通过";
            }
            UploadTestResult("合闸命令2");
            UploadTestResult("命令时间超限不执行2");

            //远程合闸命令帧时间2|表时间2|合闸命令2|命令时间超限不执行2

            //处理结果
            MessageController.Instance.AddMessage("正在处理结果,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!blnRet[i, 0] && !blnRet[i, 1])
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
            }
            UploadTestResult("结论");
        }
    }
}
