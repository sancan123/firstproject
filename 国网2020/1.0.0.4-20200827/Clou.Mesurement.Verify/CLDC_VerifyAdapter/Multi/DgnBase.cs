
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Struct;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.Helper;
using System.Windows.Forms;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.Multi
{
    /// <summary>
    /// 多功能检定基类
    /// </summary>
    public class DgnBase : VerifyBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DgnBase(object plan)
            : base(plan)
        {
            if (!Helper.MeterDataHelper.Instance.Meter(GlobalUnit.g_CUS.DnbData.GetFirstYaoJianMeterBwh()).DgnProtocol.HaveProgrammingkey)
            {
                encryptionFactory = new CLDC_Encryption.CLEncryption.EncryptionFactory();
                EncryptionTool = encryptionFactory.CreateEncryptionControler();
                if (EncryptionTool.Status.Length != BwCount)
                {
                    EncryptionTool.Status = new int[BwCount];
                }
            }
        }

        public CLDC_Encryption.CLEncryption.Interface.IAmMeterEncryption EncryptionTool;

        public CLDC_Encryption.CLEncryption.EncryptionFactory encryptionFactory;
        /// <summary>
        /// 多功能项目ID
        /// </summary>
        protected override string ItemKey
        {
            get
            {
                return CurPlan.DgnPrjID;
            }
        }

        /// <summary>
        /// 结论数据结点
        /// </summary>
        protected override string ResultKey
        {
            get
            {
                return string.Format("{0}",(int)Cus_MeterResultPrjID.多功能试验);   
            }
        }

        /// <summary>
        /// 当前多功能方案
        /// </summary>
        protected new StPlan_Dgn CurPlan
        {
            get { return new StPlan_Dgn(); }
        }

        /// <summary>
        /// 清理节点数据[重写]
        /// </summary>
        protected override void ClearItemData()
        {
            MeterBasicInfo meter = null;
            for (int i = 0; i < BwCount; i++)
            {
                meter = Helper.MeterDataHelper.Instance.Meter(i);
                if (meter != null)
                {
                    if (meter.MeterDgns.ContainsKey(ItemKey))
                    {
                        meter.MeterDgns.Remove(ItemKey);
                    }
                }
            }
        }

        /// <summary>
        /// 按方案参数进行控源操作
        /// </summary>
        /// <returns></returns>
        protected override bool PowerOn()
        {
            //if (string.IsNullOrEmpty(CurPlan.OutPramerter) || CurPlan.OutPramerter.Length < 6)
            //    return false;
            if (!Helper.MeterDataHelper.Instance.Meter(GlobalUnit.g_CUS.DnbData.GetFirstYaoJianMeterBwh()).DgnProtocol.HaveProgrammingkey)
            {
                EncryptionTool.Link();
            }

            bool isYouGong = true;//(CurPlan.OutPramerter.GLFX == Cus_PowerFangXiang.正向有功 || CurPlan.OutPramerter.GLFX == Cus_PowerFangXiang.反向有功);
            float xIb = 0F;
            //float.TryParse( CurPlan.OutPramerter.xIb.Replace("Ib", ""),out xIb); 
            //电流倍数与功率因素方案内容与文档内容不符合，默认按无电流，1.0输出
            bool ret = EquipHelper.Instance.PowerOn( GlobalUnit.U
                                                      , xIb * GlobalUnit.Ib
                                                      , (int)Cus_PowerYuanJian.H
                                                      , (int)Cus_PowerFangXiang.正向有功
                                                      , "1.0"
                                                      , isYouGong
                                                      , false);
            //bool ret= EquipHelper.Instance.PowerOn(CurPlan.OutPramerter.xU * GlobalUnit.U
                                                      //, xIb * GlobalUnit.Ib
                                                      //, (int)CurPlan.OutPramerter.YJ
                                                      //, (int)CurPlan.OutPramerter.GLFX
                                                      //, CurPlan.OutPramerter.GLYS
                                                      //, isYouGong
                                                      //, false);
            if (ret)
            {
                MessageController.Instance.AddMessage("正在等待多功能操作电能表正常运行时间" + _DgnWaitTime + "秒。");
                Thread.Sleep(_DgnWaitTime*1000);
            }
            return ret;
        }

        /// <summary>
        /// 获取多功能方案参数
        /// </summary>
        protected string[] PrjPara
        {
            get { return CurPlan.PrjParm.Split('|'); }
        }

        /// <summary>
        /// 处理数据结果,适用于只带单一结论结点型测试
        /// </summary>
        protected  void ControlResult(bool[] result)
        {
            //防止切点时发一意外
            //if (!(CurPlan is StDgnPlan)) return;
            //处理结果
            for (int k = 0; k < BwCount; k++)
            {
                ControlResult(k,result[k]);   //处理一块表的检定结论
                
            }
        }

        /// <summary>
        /// 处理一块表的结论
        /// </summary>
        /// <param name="Index">表位序号：从0开始</param>
        protected void ControlResult(int Index,bool result)
        {
            MeterBasicInfo curMeter;
            string strItemName = ((StPlan_Dgn)CurPlan).DgnPrjName;

            curMeter = Helper.MeterDataHelper.Instance.Meter(Index);
            if (!curMeter.YaoJianYn)
                return;
            //挂结论
            MeterDgn _DgnResult;
            if (!curMeter.MeterDgns.ContainsKey(ItemKey))
            {
                _DgnResult = new MeterDgn();
                _DgnResult.Md_PrjID = ItemKey;
                _DgnResult.Md_PrjName = strItemName;
                curMeter.MeterDgns.Add(ItemKey, _DgnResult);
            }
            else
            {
                _DgnResult = curMeter.MeterDgns[ItemKey];
            }
            //bool ret = Helper.Rs485Helper.Instance.GetResult(Index);
            _DgnResult.Md_chrValue = (result ? Variable.CTG_HeGe : Variable.CTG_BuHeGe);
            if (result)
            {
                //只外发成功的结论
                
                Thread.Sleep(10);
                
            }
        }

        /// <summary>
        /// 费率名称转化到费率编号
        /// </summary>
        /// <param name="FeiLvName"></param>
        /// <param name="bwIndex"></param>
        /// <returns>被检表的第几个费率</returns>
        protected int SwitchFeiLvNameToOrder(string FeiLvName, int bwIndex)
        {
            if (FeiLvName == "总") return 0;
            if (FeiLvName.Length > 1) return 0;
            string[] arrSourceFL = new string[] { "总", "尖", "峰", "平", "谷" };
            int sourceOrder = 0;
            for (int j = 0; j < arrSourceFL.Length; j++)
            {
                sourceOrder = j;
                if (arrSourceFL[j].Equals(FeiLvName))
                {
                    break;
                }
            }
            //  sourceOrder++;
            return sourceOrder;

            /*
           int returnOrder = 0;
            string meterFeiLvOrder = Helper.MeterDataHelper.Instance.Meter(bwIndex).DgnProtocol.TariffOrderType;
            for (int k = 0; k < meterFeiLvOrder.Length; k++)
            {
                returnOrder = k;
                if (meterFeiLvOrder.Substring(k, 1) == sourceOrder.ToString())
                {
                    break;
                }
            }
            return returnOrder;
        */

        }


        /// <summary>
        /// 获取多功能协议配置参数
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="PramKey"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>

        protected int getType(int Index, string PramKey, int DefaultValue)
        {
            Dictionary<string, string> DgnPram = Helper.MeterDataHelper.Instance.Meter(Index).DgnProtocol.DgnPras;
            if (!DgnPram.ContainsKey(PramKey))
            {
                return DefaultValue;
            }
            string[] Arr_Pram = DgnPram[PramKey].Split('|');
            if (Arr_Pram.Length == 2)
            {
                return int.Parse(Arr_Pram[0]);
            }
            else
            {
                return DefaultValue;
            }
        }
        /// <summary>
        /// 获取一块表多功能协议节点值
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="PramKey"></param>
        /// <returns></returns>
        protected string[] getType(int Index, string PramKey)
        {
            Dictionary<string, string> DgnPram = Helper.MeterDataHelper.Instance.Meter(Index).DgnProtocol.DgnPras;
            if (!DgnPram.ContainsKey(PramKey))
            {
                return new string[0];
            }
            return DgnPram[PramKey].Split('|');
        }

 
        /// <summary>
        /// 当前检定方案源输出的电流Ib的倍数
        /// </summary>
        protected float _xIb
        {
            get
            {
                float m_xIb = CLDC_DataCore.Function.Number.getxIb(CurPlan.OutPramerter.xIb, GlobalUnit.Meter(GlobalUnit.FirstYaoJianMeter).Mb_chrIb);
                return m_xIb;
            }
        }
        /// <summary>
        /// 多功能检定源稳定操作时间（秒）
        /// </summary>
        protected int _DgnWaitTime
        {
            get
            {
                return 8;
            }
        }
        /// <summary>
        /// 编程键提示
        /// </summary>
        public static void ShowWirteMeterWwaring()
        {
            if (GlobalUnit.GetConfig(Variable.CTC_DGN_WRITEMETERALARM, "是") == "是" && Helper.MeterDataHelper.Instance.Meter(GlobalUnit.g_CUS.DnbData.GetFirstYaoJianMeterBwh()).DgnProtocol.HaveProgrammingkey)
            {
                MessageBox.Show("请打开电能表编程开关后点击[确定]", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        /// <summary>
        /// 读取表号
        /// </summary>
        public void ReadMeterNo()
        {
            MessageController.Instance.AddMessage("正在进行【读取表号】操作...");

            string[] meterNo = MeterProtocolAdapter.Instance.ReadData("04000402", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表号", meterNo);
            bool[] result = new bool[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                MeterBasicInfo curMeter = MeterDataHelper.Instance.Meter(i);
                curMeter._Mb_MeterNo = meterNo[i];
            }
        }

    }
}
