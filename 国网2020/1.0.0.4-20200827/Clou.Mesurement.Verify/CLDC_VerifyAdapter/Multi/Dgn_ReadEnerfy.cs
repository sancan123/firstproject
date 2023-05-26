
using CLDC_DataCore;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.Multi
{
    /// <summary>
    /// 读取电量试验
    /// </summary>
    class Dgn_ReadEnerfy : DgnBase
    {
        public Dgn_ReadEnerfy(object plan)
            : base(plan)
        { }

        private byte byteFangxiang = 1;

        private byte feilv = 0;

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "检定数据", "结论" };
            string[] arrayTemp = VerifyProcess.Instance.CurrentKey.Split('_');
            if (arrayTemp.Length != 2 && arrayTemp[1].Length != 2)
            { return false; }
            if (!byte.TryParse(arrayTemp[1].Substring(0, 1), out byteFangxiang))
            {
                return false;
            }
            if (!byte.TryParse(arrayTemp[1].Substring(1, 1), out feilv))
            {
                return false;
            }
            return true;
        }

        public override void Verify()
        {
            base.Verify();
            if (Stop)
            {
                return;
            }
            if (!PowerOn())
            {
                MessageController.Instance.AddMessage("控制源输出失败",6,2);
                return;
            }
            if(Stop)
            {
                return;
            }
            float[] floatArray = MeterProtocolAdapter.Instance.ReadEnergy(byteFangxiang, feilv);
            ConvertTestResult("检定数据", floatArray);
            MeterBasicInfo meterTemp = null;
            for(int i=0;i<BwCount;i++)
            {
                meterTemp = Helper.MeterDataHelper.Instance.Meter(i);
                if(meterTemp.YaoJianYn)
                {
                    if(floatArray[i]>=0)
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
            }
            UploadTestResult("检定数据");
            UploadTestResult("结论");
            MessageController.Instance.AddMessage("读取电量完毕");
        }
    }
}
