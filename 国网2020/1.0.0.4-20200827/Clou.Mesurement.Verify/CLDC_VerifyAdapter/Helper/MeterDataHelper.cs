
using System;
using System.Collections.Generic;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_DataCore.Struct;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Model.DgnProtocol;
using CLDC_Comm.BaseClass;
/*
 电能表管理类
 
 */
namespace CLDC_VerifyAdapter.Helper
{
    public class MeterDataHelper : SingletonBase<MeterDataHelper>
    {
        /// <summary>
        /// 电能表类型列表，每种类型一个元素。每个元素中包括当前类型电能表的表位
        /// </summary>
        private Dictionary<string, string> m_MeterTypeList = new Dictionary<string, string>();
        /// <summary>
        /// 电能表协议分类，每种类型一个元素.其中包括当前协议类型的表位号
        /// </summary>
        private Dictionary<string, string> m_MeterProtocolList = new Dictionary<string, string>();

        /// <summary>
        /// 电能表所使用的协议列表
        /// </summary>
        private DgnProtocolInfo[] m_MeterProtocols = null;
        /// <summary>
        /// 当前检定器实例
        /// </summary>
        //private VerifyBase m_Base = null;
        /// <summary>
        /// 更新标识
        /// </summary>
        private string[] arrStrResultKey = new string[0];
        /// <summary>
        /// 更新标识
        /// </summary>
        private object[] objResultValue = new object[0];



        public MeterDataHelper()
        {
        }


        #region ----------表类型管理----------
        /// <summary>
        /// 是否所有表属性都相同
        /// </summary>
        public bool IsAllTheSame
        {
            get
            {
                return TypeCount == 1;
            }
        }

        /// <summary>
        /// 返回电能表类型数量
        /// </summary>
        public int TypeCount
        {
            get
            {
                return m_MeterTypeList.Count;
            }
        }

        /// <summary>
        /// 取要检表的数量
        /// </summary>
        public int YaoJianMeterCount
        {
            get
            {
                int mcount = 0;
                for (int i = 0; i < TypeCount; i++)
                {
                    mcount += MeterType(i).Length;
                }
                return mcount;
            }
        }
        /// <summary>
        /// 获取一种类型的表
        /// </summary>
        /// <param name="iType">第几种类型，从0开始</param>
        /// <returns></returns>
        public string[] MeterType(int iType)
        {
            if (iType >= 0 && iType < TypeCount)
            {
                int i = 0;
                foreach (string strKey in m_MeterTypeList.Keys)
                {
                    if (i == iType)
                    {
                        return m_MeterTypeList[strKey].Split('|');
                    }
                    i++;
                }
            }
            return new string[] { };
        }

        /// <summary>
        /// 初始化表数据
        /// </summary>
        public bool Init()
        {
            if (GlobalUnit.g_CUS == null)
            {
                return false;
            }
            m_MeterTypeList.Clear();
            m_MeterProtocols = new DgnProtocolInfo[GlobalUnit.g_CUS.DnbData._Bws];
            CLDC_DataCore.Const.GlobalUnit.CarrierInfos = new CLDC_DataCore.Model.CarrierProtocol.CarrierProtocolInfo[CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData._Bws];
            for (int k = 0; k < GlobalUnit.g_CUS.DnbData._Bws; k++)
            {
                if (Meter(k) == null) continue;
                if (Meter(k).DgnProtocol == null || Meter(k).DgnProtocol.ProtocolName != Meter(k).AVR_PROTOCOL_NAME)
                {
                    Meter(k).DgnProtocol = new CLDC_DataCore.Model.DgnProtocol.DgnProtocolInfo();
                    if (Meter(k).AVR_PROTOCOL_NAME != "")               //如果选择协议不为空则加载，如果为空的话，就不加载多功能协议
                    {
                        Meter(k).DgnProtocol.Load(Meter(k).AVR_PROTOCOL_NAME);
                    }
                    //continue;
                }
                //加载一下电能表协议,有可能多功能协议被编辑过
                Helper.MeterDataHelper.Instance.Meter(k).DgnProtocol.Load();
                string strKey = string.Format("{0}_{1}", Meter(k).Mb_chrBdj, Meter(k).Mb_chrBcs);
                string strKey2 = Helper.MeterDataHelper.Instance.Meter(k).DgnProtocol.ProtocolName;
                m_MeterProtocols[k] = Helper.MeterDataHelper.Instance.Meter(k).DgnProtocol;
                MeterBasicInfo curMeter = Helper.MeterDataHelper.Instance.Meter(k);
                if (!curMeter.YaoJianYn) continue;
                //常规
                if (m_MeterTypeList.ContainsKey(strKey))
                {
                    m_MeterTypeList[strKey] += "|" + k.ToString();
                }
                else
                {
                    m_MeterTypeList.Add(strKey, k.ToString());
                }
                //协议
                if (m_MeterProtocolList.ContainsKey(strKey2))
                {
                    m_MeterProtocolList[strKey2] += "|" + k.ToString();
                }
                else
                {
                    m_MeterProtocolList.Add(strKey2, k.ToString());
                }

                CLDC_DataCore.Const.GlobalUnit.CarrierInfos[k] = new CLDC_DataCore.Model.CarrierProtocol.CarrierProtocolInfo();
                if (!string.IsNullOrEmpty(Meter(k).AVR_CARR_PROTC_NAME))
                {
                    CLDC_DataCore.Const.GlobalUnit.CarrierInfos[k].Load(Meter(k).AVR_CARR_PROTC_NAME);
                }

            }
            if (null != CLDC_DataCore.Const.GlobalUnit.CarrierInfos[CLDC_DataCore.Const.GlobalUnit.FirstYaoJianMeter == -1 ? 0 : CLDC_DataCore.Const.GlobalUnit.FirstYaoJianMeter])
            {
                GlobalUnit.CarrierInfo = new StCarrierInfo();
                CLDC_DataCore.Model.CarrierProtocol.CarrierProtocolInfo FiratCarrierInfo = CLDC_DataCore.Const.GlobalUnit.CarrierInfos[CLDC_DataCore.Const.GlobalUnit.FirstYaoJianMeter == -1 ? 0 : CLDC_DataCore.Const.GlobalUnit.FirstYaoJianMeter];

                GlobalUnit.CarrierInfo.BaudRate = FiratCarrierInfo.BaudRate;
                GlobalUnit.CarrierInfo.ByteTime = FiratCarrierInfo.ByteTime.ToString();
                GlobalUnit.CarrierInfo.CarrierName = FiratCarrierInfo.ProtocolName;
                GlobalUnit.CarrierInfo.CarrierType = FiratCarrierInfo.CarrierType;
                GlobalUnit.CarrierInfo.CmdTime = FiratCarrierInfo.CmdTime.ToString();
                GlobalUnit.CarrierInfo.Comm = FiratCarrierInfo.ComPort;
                GlobalUnit.CarrierInfo.CommuType = FiratCarrierInfo.CommuType;
                GlobalUnit.CarrierInfo.RdType = FiratCarrierInfo.ReadType;
                GlobalUnit.CarrierInfo.RouterID = FiratCarrierInfo.RouterID;
                GlobalUnit.CarrierInfo.IsCheck_SM = FiratCarrierInfo.Is_IsCheckSM;
                GlobalUnit.CarrierInfo.WXParame = FiratCarrierInfo.WxParam;
                GlobalUnit.CarrierInfo.ZBParame = FiratCarrierInfo.ZbParame;
            }
            return true;
        }


        #endregion

        #region ----------表数据统计----------
        /// <summary>
        /// 统计要检表列表
        /// </summary>
        /// <returns></returns>
        public bool[] GetYaoJian()
        {
            
            bool[] arrResult = new bool[GlobalUnit.g_CUS.DnbData._Bws];

            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)
            {
                arrResult[i] = Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn;
            }
            return arrResult;
        }
        /// <summary>
        /// 统计要检表列表
        /// </summary>
        /// <returns></returns>
        public bool[] GetYaoJianSave()
        {
            bool[] arrResult = new bool[GlobalUnit.g_CUS.DnbData._Bws];
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)
            {
                arrResult[i] = Helper.MeterDataHelper.Instance.Meter(i).YaoJianYnSave;
            }
            return arrResult;
        }

        /// <summary>
        /// 获取所有电能表的通讯地址
        /// </summary>
        /// <returns></returns>
        public string[] GetMeterAddress()
        {
            List<string> _MeterAddrList = new List<string>();
            if (GlobalUnit.g_CUS == null)
                throw new Exception("需要在检定前设置被检表数据");
            MeterBasicInfo _Meter = null;
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)
            {
                _Meter = Helper.MeterDataHelper.Instance.Meter(i);
                if (_Meter.Mb_chrAddr != null && _Meter.Mb_chrAddr.Length > 0)
                    _MeterAddrList.Add(_Meter.Mb_chrAddr);
                else
                    _MeterAddrList.Add(_Meter.Mb_ChrCcbh);
            }
            return _MeterAddrList.ToArray();
        }

        /// <summary>
        /// 获取所有电能表MAC的通讯地址
        /// </summary>
        /// <returns></returns>
        public string[] GetMeterAddress_MAC()
        {
            List<string> _MeterAddrList = new List<string>();
            if (GlobalUnit.g_CUS == null)
                throw new Exception("需要在检定前设置被检表数据");
            MeterBasicInfo _Meter = null;
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)
            {
                _Meter = Helper.MeterDataHelper.Instance.Meter(i);
                if (_Meter.Mb_chrAddr_MAC != null && _Meter.Mb_chrAddr_MAC.Length > 0)
                    _MeterAddrList.Add(_Meter.Mb_chrAddr_MAC);
                else
                    _MeterAddrList.Add(_Meter.Mb_chrAddr);
            }
            return _MeterAddrList.ToArray();
        }


        #endregion
        /// <summary>
        /// 脉冲通道转换
        /// </summary>
        /// <returns></returns>
        public Cus_GyGyType[] GetIm()
        {
            Cus_GyGyType[] values = new Cus_GyGyType[GlobalUnit.g_CUS.DnbData._Bws];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = GlobalUnit.Meter(i).Mb_gygy;
            }
            return values;
        }

        /// <summary>
        /// 获取所有表的常数
        /// </summary>
        /// <returns></returns>
        public int[] MeterConst(bool bYouGong)
        {
            List<int> _MeterList = new List<int>();
            if (GlobalUnit.g_CUS == null)
                throw new Exception("需要在检定前设置被检表数据");
            MeterBasicInfo _Meter = null;
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)
            {
                _Meter = Helper.MeterDataHelper.Instance.Meter(i);
                if (!_Meter.YaoJianYn)
                    _Meter = Helper.MeterDataHelper.Instance.Meter(FirstYaoJianMeter);
                int[] cs = _Meter.GetBcs();
                int bcs = cs[bYouGong ? 0 : 1];
                _MeterList.Add(bcs);
            }
            return _MeterList.ToArray();
        }


        /// <summary>
        /// 清理多功能数据
        /// </summary>
        /// <param name="ItemKey">多功能项目KEY值</param>
        /// <param name="AddNew">是否新增加一个空节点</param>
        /// <returns></returns>
        public bool ClearDgnData(string ItemKey, bool AddNew)
        {
            if (GlobalUnit.g_CUS == null) return false;
            MeterBasicInfo curMeter = null;
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                if (curMeter == null)
                    continue;
                if (curMeter.MeterDgns.ContainsKey(ItemKey))
                {
                    curMeter.MeterDgns.Remove(ItemKey);
                }
                if (AddNew)                                             //如果要添加新节点
                {
                    MeterDgn newResult = new MeterDgn();
                    newResult.Md_PrjID = ItemKey;
                    curMeter.MeterDgns.Add(ItemKey, newResult);
                }
            }

            return true;

        }

        /// <summary>
        /// 获取当前检定表中最小表常数
        /// </summary>
        /// <param name="bYouGong">是否是有功常数</param>
        /// <returns>当前被检表中最小表常数</returns>
        public float MeterConstMin(bool bYouGong)
        {
            int[] curMeterConst = MeterConst(bYouGong);
            CLDC_DataCore.Function.Number.PopDesc(ref curMeterConst, true);
            return curMeterConst[0];
        }

        /// <summary>
        /// 取当前检定所有表中最小表常数
        /// </summary>
        /// <returns>包括二个元素的数组，第一个元素为有功最小常数，第二个为无功</returns>
        public int[] MeterConstMin()
        {
            int[] mconst = new int[2];
            mconst[0] = (int)MeterConstMin(true);
            mconst[1] = (int)MeterConstMin(false);
            return mconst;
        }
        #region ----------协议类型管理----------
        /// <summary>
        /// 协议类型数量
        /// </summary>
        public int ProtocolCount
        {
            get { return m_MeterProtocolList.Count; }
        }

        /// <summary>
        /// 使用当前协议的电能表
        /// </summary>
        /// <param name="iType"></param>
        /// <returns></returns>
        public string[] ProtocolType(int iType)
        {
            if (iType >= 0 && iType < ProtocolCount)
            {
                int i = 0;
                foreach (string strKey in m_MeterProtocolList.Keys)
                {
                    if (i == iType)
                    {
                        return m_MeterProtocolList[strKey].Split('|');
                    }
                    i++;
                }
            }
            return new string[] { };
        }

        /// <summary>
        /// 所有电能表的协议
        /// </summary>
        /// <returns></returns>
        public DgnProtocolInfo[] GetAllProtocols()
        {
            return m_MeterProtocols;
        }
        #endregion

        #region ---------表管理----------

        /// <summary>
        /// 第一块表有效位
        /// </summary>
        public int FirstYaoJianMeter
        {
            get
            {
                if (GlobalUnit.g_CUS == null)
                    return -1;
                for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)
                {
                    if (GlobalUnit.g_CUS.DnbData.MeterGroup[i].YaoJianYn)
                        return i;
                }
                return -1;
            }
        }

        /// <summary>
        /// 获取一块表基本信息
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public MeterBasicInfo Meter(int Index)
         {
            if (GlobalUnit.g_CUS == null) return null;
            if (Index < 0 || Index > GlobalUnit.g_CUS.DnbData._Bws)
                return null;
            return GlobalUnit.g_CUS.DnbData.MeterGroup[Index];
        }
        #endregion

        #region ----------表数据清理操作----------



        /// <summary>
        /// 清理结论节点数据
        /// </summary>
        /// <param name="ItemKey">数据节点KEY</param>
        /// <param name="dataType">数据类型</param>
        /// <returns></returns>
        public bool ClearResultData(string ItemKey, Cus_MeterDataType dataType)
        {
            if (GlobalUnit.g_CUS == null) return false;
            MeterBasicInfo curMeter = null;
            arrStrResultKey = new string[GlobalUnit.g_CUS.DnbData._Bws];
            objResultValue = new object[GlobalUnit.g_CUS.DnbData._Bws];

            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                if (curMeter == null || !curMeter.YaoJianYn)
                    continue;
                if (curMeter.MeterResults.ContainsKey(ItemKey))
                {
                    curMeter.MeterResults.Remove(ItemKey);
                    arrStrResultKey[i] = ItemKey;
                    objResultValue[i] = null;
                }
            }
            
            //更新UI
            
            return true;
        }






        #endregion




    }
}