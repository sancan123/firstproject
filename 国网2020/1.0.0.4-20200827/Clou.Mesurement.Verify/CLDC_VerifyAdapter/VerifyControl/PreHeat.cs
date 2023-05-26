
using System;
using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_DataCore.Struct;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using System.Threading;


namespace CLDC_VerifyAdapter
{

    class PreHeat : VerifyBase
    {
        #region----------构造函数----------
        public PreHeat(object plan) : base(plan) { }
        #endregion

        #region----------检定控制----------
        /// <summary>
        /// 开始预热检定,预热不需要检定数据
        /// </summary>
        public override void Verify()
        {
            base.Verify();
            PowerOn();

            string[] verPlan = VerifyPara.Split('|');
            if (verPlan[0] != "0")
            {
                int TIME = int.Parse(verPlan[0]) * 60;
                MessageController.Instance.AddMessage("开始预热，请等待" + TIME + "秒");
                if (Stop) return;

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * TIME);

            }
            Helper.EquipHelper.Instance.PowerOff();


        }
        #endregion

        #region 初始化设备参数
        /// <summary>
        /// 初始化设备参数
        /// </summary>
        /// <returns></returns>
        private bool InitEquipment()
        {

            return true;
        }
        #endregion

        #region 控制源输出
        /// <summary>
        /// 控制源输出
        /// </summary>
        /// <returns>控源结果</returns>
        //protected override bool PowerOn()
        //{
        //    int firstYaoJianMeterIndex = Helper.MeterDataHelper.Instance.FirstYaoJianMeter;
        //    MeterBasicInfo firstYaoJianMeter = Helper.MeterDataHelper.Instance.Meter(firstYaoJianMeterIndex);
        //    float powerOutI = CLDC_DataCore.Function.Number.GetCurrentByIb(CurPlan.xIb, firstYaoJianMeter.Mb_chrIb);
        //    Cus_PowerYuanJian ele = Cus_PowerYuanJian.H;
        //    //如果是单相，只输出A元
        //    if (GlobalUnit.IsDan) ele = Cus_PowerYuanJian.A;
        //    MessageController.Instance.AddMessage(string.Format("开始控制功率源输出:{0}V {1}A", GlobalUnit.U, powerOutI));
        //    bool ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U,
        //                          powerOutI,
        //                          (int)ele, (int)CurPlan.PowerFangXiang,
        //                          FangXiangStr + "1.0",
        //                          IsYouGong, false);
        //    MessageController.Instance.AddMessage(string.Format("控制功率源输出:{0}V {1}A {2}", GlobalUnit.U, powerOutI, ret ? "成功" : "失败"));
        //    return ret;
        //}

        #endregion

        

        #region -----------基类重写---------
        /// <summary>
        /// 当前项目数据节点键值，预热不需要保存数据，故主键可为空
        /// </summary>
        protected override string ItemKey
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// 当前项目总结论键值，预热不需要保存数据，故主键可为空
        /// </summary>
        protected override string ResultKey
        {
            get { return string.Empty; }
        }
        #endregion
    }
}
