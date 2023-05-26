
using System;
using CLDC_DataCore;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.Multi
{
    /// <summary>
    /// 清空电量
    /// </summary>
    class Dgn_ClearEnerfy : VerifyBase
    {
        public Dgn_ClearEnerfy(object plan) : base(plan) { }


        protected override string ResultKey
        {

            //get { throw new NotImplementedException(); }
            get { return null; }
        }

        protected override string ItemKey
        {
            //get { throw new NotImplementedException(); }
            get { return null; }
        }

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "检定数据", "结论" };
            return true;
        }

        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            PowerOn();

            if (Stop) return;
            bool[] result = MeterProtocolAdapter.Instance.ClearEnergy();

            int _MaxStartTime = 30;
            m_StartTime = DateTime.Now;
            while (true)
            {
                //每一秒刷新一次数据
                long _PastTime = base.VerifyPassTime;
                Thread.Sleep(1000);

                float pastMinute = _PastTime / 60F;
                GlobalUnit.g_CUS.DnbData.NowMinute = pastMinute;
                string strDes = "清零等待时间" + (_MaxStartTime / 60.0f).ToString("F2") + "分，已经经过" + pastMinute.ToString("F2") + "分";

                MessageController.Instance.AddMessage(strDes);

                if ((_PastTime >= _MaxStartTime) || Stop)
                {
                    GlobalUnit.g_CUS.DnbData.NowMinute = _MaxStartTime / 60F;
                    break;
                }
            }

            if (Stop) return;
            float[] flt_DL = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "检定数据", ConvertArray.ConvertFloat2Str(flt_DL));

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                ResultDictionary["结论"][i] = result[i] ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;
            }
            UploadTestResult("结论");
        }
    }
}
