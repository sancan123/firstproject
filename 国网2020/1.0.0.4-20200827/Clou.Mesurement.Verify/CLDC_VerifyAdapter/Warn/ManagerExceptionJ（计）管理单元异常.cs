
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.Warn
{
    /// <summary>
    /// （计）管理单元异常
    /// </summary>
    class ManagerExceptionJ : VerifyBase
    {
        public ManagerExceptionJ(object plan)
            : base(plan)
        {

        }
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
            ResultNames = new string[] { "测试时间","结论", "不合格原因" };
            return true;
        }

        /// 重写基类测试方法
        /// </summary>
        /// <param name="ItemNumber">检定方案序号</param>
        public override void Verify()
        {

            if (Stop) return;
            base.Verify();
            PowerOn();
            string[] strRand1 = new string[BwCount];//随机数1
            string[] strRand2 = new string[BwCount];//随机数2
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strData = new string[BwCount];//明文
            int[] iFlag = new int[BwCount];
            bool[] result = new bool[BwCount];


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取管理单元异常前总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03830000", 3);
         
            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥更新....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);

          
          
       
            if (Stop) return;
            MessageController.Instance.AddMessage("正在模拟计量清零操作");
            System.Windows.Forms.MessageBox.Show("请手动互换管理芯模块（同个厂商），完成后点击确定。");
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 62);

          

            if (Stop) return;
            MessageController.Instance.AddMessage("正在模拟计量清零操作");
            System.Windows.Forms.MessageBox.Show("请手动互为原来管理芯模块，完成后点击确定。");
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 62);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取管理单元异常后总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03830000", 3);
         

            if (Stop) return;
            CLDC_DataCore.Function.Common.Memset(ref iFlag, 1);
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥恢复....");
            bool[] blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
           
            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strLoseCountQ[i] == "" || strLoseCountH[i] == ""  )
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回日期或次数值为空";
                    continue;
                }
                if (!string.IsNullOrEmpty(strLoseCountQ[i]) && !string.IsNullOrEmpty(strLoseCountH[i]) && Convert.ToInt32(strLoseCountQ[i]) + 1 == Convert.ToInt32(strLoseCountH[i]))
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }
                else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "管理单元异常总次数校时次数不匹配";
                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            UploadTestResult("结论");
        }
    }
}
