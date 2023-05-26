using System;
using System.Collections.Generic;
using CLDC_Comm.Enum;

using CLDC_DataCore.Const;
namespace CLDC_DataCore.Model.DnbModel.DnbInfo
{
    [Serializable()]
    public class MeterBasicInfo : MeterErrorBase
    {
        #region 构造
        public MeterBasicInfo(int Bwh)
            : this()
        {
            _Mb_intBno = Bwh;
            _intBno = Bwh;
        }

        public MeterBasicInfo()
        {
            this.AddEquipmentInfo();
        }
        /// <summary>
        /// 表位号		在表架上所挂位置(只读)
        /// </summary>
        public int Mb_intBno
        {
            get
            {
                return _Mb_intBno;
            }
        }

        /// <summary>
        /// 设置表位号
        /// </summary>
        /// <param name="Bwh"></param>
        public void SetBno(int Bwh)
        {
            _Mb_intBno = Bwh;
            _intBno = Bwh;
        }


        /// <summary>
        /// 挂新表使用
        /// </summary>
        /// <returns></returns>
        public void GetNewMeter()
        {
            this.ClearData();                  //清理已经产生了的检定数据
            this.YaoJianYn = true;
            this.Mb_ChrJlbh = "";
            this.Mb_ChrCcbh = "";
            this.Mb_ChrTxm = "";
            this.Mb_chrAddr = "";
            this.Mb_chrBcs = "";
            this.Mb_chrBdj = "";
            this.Mb_Bxh = "";
            this.Mb_chrCcrq = "";
            this.Mb_ChrBmc = "";
            this.Mb_chrZsbh = "";
            this.AVR_WORK_NO = "";
            this.AVR_TASK_NO = "";
            this.Mb_chrQianFeng1 = "";
            this.Mb_chrQianFeng2 = "";
            this.Mb_chrQianFeng3 = "";
            this.AVR_SEAL_4 = "";
            this.AVR_SEAL_5 = "";
            this.Mb_chrSoftVer = "";
            this.Mb_chrHardVer = "";
            this.Mb_chrArriveBatchNo = "";
            this.Mb_chrOther1 = "";
            this.Mb_chrOther2 = "";
            this.Mb_chrOther3 = "";
            this.Mb_chrOther4 = "";
            this.Mb_chrOther5 = "";
        }
        /// <summary>
        /// 添加台体信息到扩展数据集合中
        /// </summary>
        /// <remarks>
        /// 2012-03-05 增加，每块表初始化时自带标准表信息和台体信息，方便服务器端输出报表以及MIS。
        /// </remarks>
        private void AddEquipmentInfo()
        {

        }
        #endregion

        #region 一块表基本信息

        /// <summary>
        /// 表位号		在表架上所挂位置
        /// </summary>
        private int _Mb_intBno = 0;
        /// <summary>
        /// 计量编号
        /// </summary>
        public string Mb_ChrJlbh = "";
        /// <summary>
        /// 出厂编号
        /// </summary>
        public string Mb_ChrCcbh = "";
        /// <summary>
        /// 条形码	
        /// </summary>
        public string Mb_ChrTxm = "";
        /// <summary>
        /// 表通信地址
        /// </summary>
        public string Mb_chrAddr = "";
        /// <summary>
        /// 表MAC通信地址
        /// </summary>
        public string Mb_chrAddr_MAC = "";
        /// <summary>
        /// 制造厂家
        /// </summary>
        public string Mb_chrzzcj = "";
        /// <summary>
        /// 表型号
        /// </summary>
        public string Mb_Bxh = "";
        /// <summary>
        /// 表号，用于加密参数
        /// </summary>
        public string _Mb_MeterNo = "";
        /// <summary>
        /// 表常数		有功（无功）
        /// </summary>
        public string Mb_chrBcs = "";
        /// <summary>
        /// 表类型
        /// </summary>
        public string Mb_chrBlx = "";
        /// <summary>
        /// 表等级		有功（无功）
        /// </summary>
        public string Mb_chrBdj = "";

        /// <summary>
        /// 共阴 共阳类型
        /// </summary>
        public Cus_GyGyType Mb_gygy = CLDC_Comm.Enum.Cus_GyGyType.共阴;
        /// <summary>
        /// 出厂日期		YYYY.MM.DD
        /// </summary>
        public string Mb_chrCcrq = "";
        /// <summary>
        /// 送检单位
        /// </summary>
        public string Mb_chrSjdwNo = "";
        /// <summary>
        /// 送检单位
        /// </summary>
        public string Mb_chrSjdw = "";
        /// <summary>
        /// 证书编号
        /// </summary>
        public string Mb_chrZsbh = "";
        /// <summary>
        /// 表名称	
        /// </summary>
        public string Mb_ChrBmc = "";
        /// <summary>
        /// 测量方式	
        /// </summary>
        public int Mb_intClfs = 0;
        /// <summary>
        /// 电压		XXX（不带单位）
        /// </summary>
        public string Mb_chrUb = "";
        /// <summary>
        /// 电流		Ib(Imax)（不带单位）
        /// </summary>
        public string Mb_chrIb = "";
        /// <summary>
        /// 频率		XX（不带单位）
        /// </summary>
        public string Mb_chrHz = "";
        /// <summary>
        /// 止逆器		1-有，0-无
        /// </summary>
        public bool Mb_BlnZnq = false;
        /// <summary>
        /// 互感器		1-经互感器
        /// </summary>
        public bool Mb_BlnHgq = false;
        /// <summary>
        /// 检测类型
        /// </summary>
        public string Mb_chrTestType = "";
        /// <summary>
        /// 检定日期		YYYY-MM-DD HH:NN:SS
        /// </summary>
        public string Mb_DatJdrq = "";
        /// <summary>
        /// 计检日期		YYYY-MM-DD HH:NN:SS
        /// </summary>
        public string Mb_Datjjrq = "";
        /// <summary>
        /// 温度		XX（不带单位）
        /// </summary>
        public string Mb_chrWd = "";
        /// <summary>
        /// 湿度		XX（不带单位）
        /// </summary>
        public string Mb_chrSd = "";
        /// <summary>
        /// 总结论		合格/不合格
        /// </summary>
        public string Mb_chrResult = "";
        /// <summary>
        /// 检验员
        /// </summary>
        public string Mb_ChrJyy = "";
        /// <summary>
        /// 核验员	
        /// </summary>
        public string Mb_ChrHyy = "";
        /// <summary>
        /// 主管
        /// </summary>
        public string Mb_chrZhuGuan = "";
        /// <summary>
        /// 检验员编号
        /// </summary>
        public string Mb_ChrJyyNo = "";
        /// <summary>
        /// 核验员编号
        /// </summary>
        public string Mb_ChrHyyNo = "";
        /// <summary>
        /// 主管人员编号
        /// </summary>
        public string Mb_chrZhuGuanNo = "";
        /// <summary>
        /// 是否上传到服务器
        /// </summary>
        public bool Mb_BlnToServer = false;
        /// <summary>
        /// 是否上传到MIS		在集控下无效
        /// </summary>
        public bool Mb_BlnToMis = false;
        /// <summary>
        /// 铅封1
        /// </summary>
        public string Mb_chrQianFeng1 = "";
        /// <summary>
        /// 铅封2
        /// </summary>
        public string Mb_chrQianFeng2 = "";
        /// <summary>
        /// 铅封3
        /// </summary>
        public string Mb_chrQianFeng3 = "";

        /// <summary>
        /// 37铅封号4
        /// </summary>
        public string AVR_SEAL_4 = "";
        /// <summary>
        /// 38铅封号5
        /// </summary>
        public string AVR_SEAL_5 = "";
        /// <summary>
        /// 软件版本号
        /// </summary>
        public string Mb_chrSoftVer = "";
        /// <summary>
        /// 硬件版本号
        /// </summary>
        public string Mb_chrHardVer = "";
        /// <summary>
        /// 到货批次号
        /// </summary>
        public string Mb_chrArriveBatchNo = "";
        /// <summary>
        /// 方案唯一编号
        /// </summary>
        public int Mb_intSchemeID = 0;
        /// <summary>
        /// 协议唯一编号
        /// </summary>
        public int Mb_intProtocolID = 0;
        /// <summary>
        /// 通讯协议名称
        /// </summary>
        public string AVR_PROTOCOL_NAME = "";
        /// <summary>
        /// 载波协议名称
        /// </summary>
        public string AVR_CARR_PROTC_NAME = "";
        /// <summary>
        /// 负荷开关控制方式：0:外置-A 无源无极性控制开关信号、1:外置-B交流电压控制信号2：内置
        /// </summary>
        public int Mb_intFKType = 0;
        /// <summary>
        /// 45任务编号
        /// </summary>
        public string AVR_TASK_NO = "";

        /// <summary>
        /// 46工单号
        /// </summary>
        public string AVR_WORK_NO = "";
        /// <summary>
        /// 备用1	
        /// </summary>
        public string Mb_chrOther1 = "";
        /// <summary>
        /// 备用2
        /// </summary>
        public string Mb_chrOther2 = "";
        /// <summary>
        /// 备用3
        /// </summary>
        public string Mb_chrOther3 = "";
        /// <summary>
        /// 备用4
        /// </summary>
        public string Mb_chrOther4 = "";
        /// <summary>
        /// 备用5
        /// </summary>
        public string Mb_chrOther5 = "";


        /// <summary>
        /// 表唯一ID
        /// </summary>
        public string Mb_ID = "";
        /// <summary>
        /// 是否要检,用于检定
        /// </summary>
        public bool YaoJianYn = false;
        /// <summary>
        /// 是否要检，只能在参数录入赋值。用于参数录入、数据保存。
        /// </summary>
        public bool YaoJianYnSave = false;

        /// <summary>
        /// 使用的规程名称
        /// </summary>
        public string GuiChengName = string.Empty;

        /// <summary>
        /// 选定的电子式表使用的规程
        /// </summary>
        public string GuiChengName_DianZi = string.Empty;

        /// <summary>
        /// 选定的感应式表使用规程
        /// </summary>
        public string GuiChengName_GanYing = string.Empty;

        /// <summary>
        /// 总结论
        /// </summary>
        public string Mb_Result
        {
            get
            {
                #region 获取总结论
                Mb_chrResult = Variable.CTG_HeGe;
                if (MeterResults != null)
                {
                    if (MeterResults.Count > 0)
                    {
                        string[] Keys = new string[MeterResults.Count];
                        MeterResults.Keys.CopyTo(Keys, 0);
                        foreach (string sKey in Keys)
                        {
                            if (MeterResults[sKey].Mr_chrRstValue == Variable.CTG_BuHeGe)
                            {
                                Mb_chrResult = Variable.CTG_BuHeGe;
                                return Mb_chrResult;
                            }
                        }
                    }
                }

                if (MeterDgns != null)
                {
                    if (MeterDgns.Count > 0)
                    {
                        string[] Keys = new string[MeterDgns.Count];
                        MeterDgns.Keys.CopyTo(Keys, 0);
                        foreach (string sKey in Keys)
                        {
                            if (MeterDgns[sKey].Md_chrValue == Variable.CTG_BuHeGe)
                            {
                                Mb_chrResult = Variable.CTG_BuHeGe;
                                return Mb_chrResult;
                            }
                        }
                    }
                }

                if (MeterDLTDatas != null)
                {
                    if (MeterDLTDatas.Count > 0)
                    {
                        string[] Keys = new string[MeterDLTDatas.Count];
                        MeterDLTDatas.Keys.CopyTo(Keys, 0);
                        foreach (string sKey in Keys)
                        {
                            if (MeterDLTDatas[sKey].Mdlt_chrValue == Variable.CTG_BuHeGe)
                            {
                                Mb_chrResult = Variable.CTG_BuHeGe;
                                return Mb_chrResult;
                            }
                        }
                    }
                }


                return Mb_chrResult;
                #endregion
            }
        }
        /// <summary>
        /// 表类型：电子式 Or 感应式
        /// </summary>
        public CLDC_Comm.Enum.Cus_MeterType_DianziOrGanYing MeterType_DzOrGy
        {
            get
            {
                if (Mb_chrBlx.IndexOf("机电") != -1
                    || Mb_chrBlx.IndexOf("感应") != -1
                    || Mb_chrBlx.IndexOf("机械") != -1)
                {
                    return CLDC_Comm.Enum.Cus_MeterType_DianziOrGanYing.GanYingShi;
                }
                else
                {
                    return CLDC_Comm.Enum.Cus_MeterType_DianziOrGanYing.DianZiShi;
                }
            }
        }
        #endregion

        #region 一块表检定数据模型
        /// <summary>
        /// 潜动启动数据；Key值为项目Prj_ID值
        /// </summary>
        public Dictionary<string, MeterQdQid> MeterQdQids = new Dictionary<string, MeterQdQid>();
        /// <summary>
        /// 电能表误差集合Key值为项目Prj_ID值，由于特殊检定部分被T出去单独建结构所以不会出现关键字重复的情况 
        /// </summary>
        public Dictionary<string, MeterError> MeterErrors = new Dictionary<string, MeterError>();
        /// <summary>
        /// 电能表结论集；Key值为检定项目ID编号格式化字符串。格式为[检定项目ID号]参照数据库结构设计文档中附2
        /// </summary>
        public Dictionary<string, MeterResult> MeterResults = new Dictionary<string, MeterResult>();
        /// <summary>
        /// 电能表多功能数据集； Key值为项目Prj_ID值
        /// </summary>
        public Dictionary<string, MeterDgn> MeterDgns = new Dictionary<string, MeterDgn>();
        /// <summary>
        /// 电能表扩展数据集，Key为标志ID,不能超过10个字节，标志值不能超过50个字节
        /// </summary>
        public Dictionary<string, string> MeterExtend = new Dictionary<string, string>();

        /// <summary>
        /// 电能表误差一致性集；Key值为项目Prj_ID值
        /// </summary>
        public Dictionary<string, MeterErrAccord> MeterErrAccords = new Dictionary<string, MeterErrAccord>();

        /// <summary>
        /// 电能表功耗测试数据集；key值为项目Md_PrjID值
        /// </summary>
        public Dictionary<string, MeterPower> MeterPowers = new Dictionary<string, MeterPower>();
         /// <summary>
        /// 规约一致性数据
        /// </summary>
        public Dictionary<string, MeterDLTData> MeterDLTDatas = new Dictionary<string, MeterDLTData>();
        /// 预先调试数据集； Key值为项目Prj_ID值
        /// </summary>
        public Dictionary<string, MeterPrepareTest> MeterPrepareTest = new Dictionary<string, MeterPrepareTest>();

        /// <summary>
        /// 电能表走字数据误差集；Key值为Prj_ID
        /// </summary>
        public Dictionary<string, MeterZZError> MeterZZErrors = new Dictionary<string, MeterZZError>();

        /// <summary>
        /// 人工检定数据结论集
        /// </summary>
        public Dictionary<string, MeterArtificial> MeterArtificial = new Dictionary<string, MeterArtificial>();
        #endregion

        /// <summary>
        /// 电能表多功能通信配置协议
        /// </summary>
        public DgnProtocol.DgnProtocolInfo DgnProtocol;

        /// <summary>
        /// 电能表载波检定数据集； Key值为项目Prj_ID值
        /// </summary>
        public Dictionary<string, MeterCarrierData> MeterCarrierDatas = new Dictionary<string, MeterCarrierData>();

        /// <summary>
        /// 电能表事件记录数据集； Key值为项目Prj_ID值
        /// </summary>
        public Dictionary<string, MeterEventLog> MeterEventLogs = new Dictionary<string, MeterEventLog>();
        /// <summary>
        /// 智能表功能数据集； Key值为项目Prj_ID值
        /// </summary>
        public Dictionary<string, MeterFunction> MeterFunctions = new Dictionary<string, MeterFunction>();

        /// <summary>
        /// 事件记录数据集； Key值为项目Prj_ID值
        /// </summary>
        public Dictionary<string, MeterSjJLgn> MeterSjJLgns = new Dictionary<string, MeterSjJLgn>();

        /// <summary>
        /// 电能表冻结数据集； Key值为项目Prj_ID值
        /// </summary>
        public Dictionary<string, MeterFreeze> MeterFreezes = new Dictionary<string, MeterFreeze>();

        /// <summary>
        /// 电能表特殊检定数据误差集；Key值为P_[下标序号]由于无法确定关键字，故只能使用下标序号来表示
        /// </summary>
        /// 
        public Dictionary<string, MeterSpecialErr> MeterSpecialErrs = new Dictionary<string, MeterSpecialErr>();

        /// <summary>
        /// 负荷记录数据集；Key值为项目prj_ID值
        /// </summary>
        public Dictionary<string, MeterLoadRecord> MeterLoadRecords = new Dictionary<string, MeterLoadRecord>();
        #region 公用
        /// <summary>
        /// 获取表常数 
        /// </summary>
        /// <returns>[有功，无功]</returns>
        public int[] GetBcs()
        {
            Mb_chrBcs = Mb_chrBcs.Replace("（", "(").Replace("）", ")");

            if (Mb_chrBcs.Trim().Length < 1)
            {
                //System.Windows.Forms.MessageBox.Show("没有录入常数");
                return new int[] { 1, 1 };
            }

            string[] arTmp = Mb_chrBcs.Trim().Replace(")", "").Split('(');

            if (arTmp.Length == 1)
            {
                if (CLDC_DataCore.Function.Number.IsNumeric(arTmp[0]))
                    return new int[] { int.Parse(arTmp[0]), int.Parse(arTmp[0]) };
                else
                    return new int[] { 1, 1 };
            }
            else
            {
                if (CLDC_DataCore.Function.Number.IsNumeric(arTmp[0]) && CLDC_DataCore.Function.Number.IsNumeric(arTmp[1]))
                    return new int[] { int.Parse(arTmp[0]), int.Parse(arTmp[1]) };
                else
                    return new int[] { 1, 1 };
            }
        }

        /// <summary>
        /// 获取电流
        /// </summary>
        /// <returns>[最小电流,最大电流]</returns>
        public float[] GetIb()
        {
            Mb_chrIb = Mb_chrIb.Replace("（", "(").Replace("）", ")");

            if (Mb_chrIb.Trim().Length < 1)
            {
                return new float[] { 1, 1 };
            }

            string[] arTmp = Mb_chrIb.Trim().Replace(")", "").Split('(');

            if (arTmp.Length == 1)
            {
                if (CLDC_DataCore.Function.Number.IsNumeric(arTmp[0]))
                    return new float[] { float.Parse(arTmp[0]), float.Parse(arTmp[0]) };
                else
                    return new float[] { 1, 1 };
            }
            else
            {
                if (CLDC_DataCore.Function.Number.IsNumeric(arTmp[0]) && CLDC_DataCore.Function.Number.IsNumeric(arTmp[1]))
                    return new float[] { float.Parse(arTmp[0]), float.Parse(arTmp[1]) };
                else
                    return new float[] { 1, 1 };
            }
        }

        /// <summary>
        /// 返回表位号格式化字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0:d2}表位", Mb_intBno);
        }
        /// <summary>
        /// 清理所有检定数据
        /// </summary>
        public void ClearData()
        {
            MeterDgns.Clear();
            MeterResults.Clear();

            if (MeterDLTDatas == null)
            {
                MeterDLTDatas = new Dictionary<string, MeterDLTData>();
            }
            MeterDLTDatas.Clear();
        }

        #endregion
    }
}
