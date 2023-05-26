using System;
using CLDC_DataCore;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.CostSouth.RemoteMode
{
    /// <summary>
    /// 远程模式下错误参数的密钥更新测试
    /// </summary>
    public class MistakeParameterUpdateKey : VerifyBase
    {
        protected override string ItemKey
        {
            // get { throw new System.NotImplementedException(); }
            get { return null; }
        }
        protected override string ResultKey
        {
            //get { throw new System.NotImplementedException(); }
            get { return null; }
        }

        public MistakeParameterUpdateKey(object plan)
            : base(plan)
        {
        }

        //密钥状态字（更新前）1|密钥状态字（更新后）1|密钥更新总次数（更新前）1|密钥更新总次数（更新后）1|上1次密钥更新记录1|更新密钥数量不正确密钥更新失败1
        //更新的正式密钥条数-测试密钥条数2|密钥状态字（更新前）2|密钥状态字（更新后）2|密钥更新总次数（更新前）2|密钥更新总次数（更新后）2|上1次密钥更新记录2|更新密钥状态非法密钥更新失败2
        //密钥状态字（更新前）3|密钥状态字（更新后）3|密钥更新总次数（更新前）3|密钥更新总次数（更新后）3|上1次密钥更新记录3|更新密钥编号非法密钥更新失败3


        protected override bool CheckPara()
        {
            //return base.CheckPara();
            ResultNames = new string[] { "密钥状态字（更新前）1","密钥状态字（更新后）1","密钥更新总次数（更新前）1","密钥更新总次数（更新后）1","上1次密钥更新记录1","更新密钥数量不正确密钥更新失败1",
                                         "更新的正式密钥条数一测试密钥条数2","密钥状态字（更新前）2","密钥状态字（更新后）2","密钥更新总次数（更新前）2","密钥更新总次数（更新后）2","上1次密钥更新记录2","更新密钥状态非法密钥更新失败2",
                                         "密钥状态字（更新前）3","密钥状态字（更新后）3","密钥更新总次数（更新前）3","密钥更新总次数（更新后）3","上1次密钥更新记录3","更新密钥编号非法密钥更新失败3",
                                         "结论" };
            return true;
        }

        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            PowerOn();
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            int[] iFlag = new int[BwCount];
            bool[] rstTmp = new bool[BwCount];
            bool[,] blnRet = new bool[BwCount, 3];
            string[] strRedData = new string[BwCount];
            int iSelectBwCount = 0;
            string[] strData = new string[BwCount];
            string[] SumNoQ = new string[BwCount];
            string[] SumNoH = new string[BwCount];
            string[] strRevCode = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] strRevMac = new string[BwCount];
            string[] status = new string[BwCount];

            #region 准备
            //准备工作
            ChangRemotePreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送电表清零命令,请稍候....");
            bool[] blnMeterClearRet = MeterProtocolAdapter.Instance.SouthDataClear1(iFlag, strRand2);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            #endregion

            //1------------

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥更新前总次数");
            SumNoQ = MeterProtocolAdapter.Instance.ReadData("03301200", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥更新总次数（更新前）1", SumNoQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥状态字");
            status = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥状态字（更新前）1", status);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥信息总条数=16的密钥更新....");
            bool[] blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 16, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥状态字");
            status = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥状态字（更新后）1", status);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥更新前总次数");
            SumNoH = MeterProtocolAdapter.Instance.ReadData("03301200", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥更新总次数（更新后）1", SumNoH);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次密钥更新记录");
            strRedData = MeterProtocolAdapter.Instance.ReadData("03301201", 15);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次密钥更新记录1", strRedData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strRedData[i] == "00".PadLeft(30, '0') && status[i] == "00000000" && SumNoQ[i] == SumNoH[i])
                {
                    blnRet[i, 0] = true;
                }
                ResultDictionary["更新密钥数量不正确密钥更新失败1"][i] = blnRet[i, 0] ? "通过" : "不通过";
            }
            UploadTestResult("更新密钥数量不正确密钥更新失败1");

            //2---------------
            SumNoQ = SumNoH;
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥更新总次数（更新前）2", SumNoQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥状态字");
            status = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥状态字（更新前）2", status);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行前16条为正式密钥，后1条为测试密钥的密钥更新....");
            blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateOfficialAndTest("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref strRedData, "16-1");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "更新的正式密钥条数一测试密钥条数2", strRedData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥状态字");
            status = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥状态字（更新后）2", status);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥更新前总次数");
            SumNoH = MeterProtocolAdapter.Instance.ReadData("03301200", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥更新总次数（更新后）2", SumNoH);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次密钥更新记录");
            strRedData = MeterProtocolAdapter.Instance.ReadData("03301201", 15);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次密钥更新记录2", strRedData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strRedData[i] == "00".PadLeft(30, '0') && status[i] == "00000000" && SumNoQ[i] == SumNoH[i])
                {
                    blnRet[i, 1] = true;
                }
                ResultDictionary["更新密钥状态非法密钥更新失败2"][i] = blnRet[i, 1] ? "通过" : "不通过";
            }
            UploadTestResult("更新密钥状态非法密钥更新失败2");

            //3--------------
            SumNoQ = SumNoH;
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥更新总次数（更新前）3", SumNoQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥状态字");
            status = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥状态字（更新前）3", status);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行17条密钥乱序排列的密钥更新....");
            blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2DisorderedKey("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥状态字");
            status = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥状态字（更新后）3", status);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥更新前总次数");
            SumNoH = MeterProtocolAdapter.Instance.ReadData("03301200", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥更新总次数（更新后）3", SumNoH);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次密钥更新记录");
            strRedData = MeterProtocolAdapter.Instance.ReadData("03301201", 15);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次密钥更新记录3", strRedData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strRedData[i] == "00".PadLeft(30, '0') && status[i] == "00000000" && SumNoQ[i] == SumNoH[i])
                {
                    blnRet[i, 2] = true;
                }
                ResultDictionary["更新密钥编号非法密钥更新失败3"][i] = blnRet[i, 2] ? "通过" : "不通过";
            }
            UploadTestResult("更新密钥编号非法密钥更新失败3");

            //处理结论
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2])
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
            }
            //通知界面
            UploadTestResult("结论");

        }
    }
}
