
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Struct;

namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// 检定器构造工厂,主要负责根据检定方案内容初始化对应的检定器
    /// </summary>
    class VerifyFactory
    {
        #region----------构造函数----------
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="verifyPlan">当前检定方案</param>
        public VerifyFactory(object verifyPlan)
        {
            CreateVerifyContext(verifyPlan);
        }
        #endregion

        #region----------获取已经创建好的检定器对象----------
        /// <summary>
        /// 返回当前检定控制器
        /// </summary>
        private VerifyBase m_curControl = null;
        public VerifyBase GetVerifyControl()
        {
            return m_curControl;
        }
        #endregion

        #region ----------检定器工厂：创建相应检定器----------
        /// <summary>
        /// 根据当前方案类型创建检定器
        /// </summary>
        /// <param name="verifyPlan">要检定的方案</param>
        public void CreateVerifyContext(object verifyPlan)
        {
            string strInfo = string.Format("开始为方案[{0}]创建检定器",verifyPlan);
            Helper.LogHelper.Instance.WriteInfo(strInfo); 
            object curVerifyPlan = verifyPlan;
            //TODO:添加新实验项目，需求分析中所有项目
            //清空上一次检定控制器对象
            m_curControl = null;
            GlobalUnit.g_CommunType = Cus_CommunType.通讯485;//485//zhengrubin
            //生产检定器
            //if (curVerifyPlan is StPlan_ConnProtocol)
            //{
            //    //通讯协议检查试验
            //    m_curControl = new ConnProtocol(verifyPlan);
            //}
            //else 
            if (curVerifyPlan is CLDC_DataCore.Struct.StPlan_Dgn)
            {
                //多功能检定
                bool bLoadProtocol = true;
                int i = 0;
                for (i = 0; i < Adapter.Instance.BwCount; i++)
                {
                    bLoadProtocol = GlobalUnit.Meter(GlobalUnit.FirstYaoJianMeter).DgnProtocol.Loading;
                    if (!bLoadProtocol) break;
                }
                if (bLoadProtocol)
                {
                    //多功能检定器工厂
                    CLDC_DataCore.Struct.StPlan_Dgn dgnPlan = (CLDC_DataCore.Struct.StPlan_Dgn)curVerifyPlan;
                    Multi.DgnFactory dgnHelper = new CLDC_VerifyAdapter.Multi.DgnFactory();
                    m_curControl = dgnHelper.CreateDgnControler(dgnPlan);
                }
                else
                {
                    Helper.LogHelper.Instance.WriteWarm("初始电能表协议失败", null);
                    return;
                }
            }
            else
            {
                m_curControl = null;
            }
            if (m_curControl == null)
            {
                strInfo = string.Format("创建检定器失败，不支持的方案类型:{0}",curVerifyPlan.ToString());
            }
            else
            {
                strInfo = string.Format("创建检定器成功:{0}",m_curControl.ToString());
            }
            Helper.LogHelper.Instance.WriteDebug(strInfo);
        }
        #endregion
    }
}