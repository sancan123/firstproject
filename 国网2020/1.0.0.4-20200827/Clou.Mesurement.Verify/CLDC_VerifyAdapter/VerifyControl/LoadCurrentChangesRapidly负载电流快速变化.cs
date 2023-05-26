using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CLDC_DataCore;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_DataCore.Const;
namespace CLDC_VerifyAdapter
{
    class LoadCurrentChangesRapidly : VerifyBase
    {
             #region ----------构造函数----------

        public LoadCurrentChangesRapidly(object plan)
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
            ResultNames = new string[] { "测试前组合有功电量", "测试前组合有功特征字", "设置后组合有功特征字", "正向有功电能", "反向有功电能", "测试后组合有功电量", "组合有功特征字", "组合有功电量", "结论", "不合格原因" };
            return true;
        }

        #endregion    
        
    


        public override void Verify()
        {
            base.Verify();
            bool bPowerOn = PowerOn();

            MessageController.Instance.AddMessage("测试前正在读取组合有功电量");









        }
    }
}
