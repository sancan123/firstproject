using System;
using System.Collections.Generic;
using System.Xml;
using CommDataBase = CLDC_DataCore.DataBase;
namespace CLDC_DataCore.Model.DgnProtocol
{
    [Serializable()]
    /// <summary>
    /// 多功能通信协议配置模型
    /// </summary>
    public class DgnProtocolInfo
    {
        #region---------------------------------------------协议模型结构部分-----------------------------------------
        /// <summary>
        /// 协议名称
        /// </summary>
        public string ProtocolName = "";

        /// <summary>
        /// 协议库名称
        /// </summary>
        public string DllFile = "";

        /// <summary>
        /// 协议类
        /// </summary>
        public string ClassName = "";

        /// <summary>
        /// 通信参数
        /// </summary>
        public string Setting = "";

        /// <summary>
        /// 用户代码
        /// </summary>
        public string UserID = "";

        /// <summary>
        /// 验证密码类型
        /// </summary>
        public int VerifyPasswordType = 0;
        #region 密码
        /// <summary>
        /// 一类写操作密码
        /// </summary>
        public string WritePassword = "";

        /// <summary>
        /// 一类写操作密码等级
        /// </summary>
        public string WriteClass = "";

        /// <summary>
        /// 二类写操作密码/写密码
        /// </summary>
        public string WritePassword2 = "";

        /// <summary>
        /// 二类写操作密码等级/写等级
        /// </summary>
        public string WriteClass2 = "";

        private string clearDemandPassword = "";

        /// <summary>
        /// 清需量密码
        /// </summary>
        public string ClearDemandPassword
        {
            get { return this.clearDemandPassword; }
            set
            {
                this.clearDemandPassword = value;
            }

        }

        /// <summary>
        /// 清需量密码等级
        /// </summary>
        public string ClearDemandClass = "";

        /// <summary>
        /// 清电量密码
        /// </summary>
        public string ClearDLPassword = "";

        /// <summary>
        /// 清电量密码等级
        /// </summary>
        public string ClearDLClass = "";

        /// <summary>
        /// 清事件密码
        /// </summary>
        public string ClearEventPassword = "";

        /// <summary>
        /// 清事件等级
        /// </summary>
        public string ClearEventClass = "";

        /// <summary>
        /// 拉合闸密码
        /// </summary>
        public string RelayPassword = "";

        /// <summary>
        /// 拉合闸等级
        /// </summary>
        public string RelayClass = "";
        #endregion
        /// <summary>
        /// 费率排序（峰平谷尖2341）
        /// </summary>
        public string TariffOrderType = "2341";
        /// <summary>
        /// 日期时间格式
        /// </summary>
        public string DateTimeFormat = "";
        /// <summary>
        /// 星期天序号
        /// </summary>
        public int SundayIndex = 0;
        /// <summary>
        /// 下发帧的唤醒符个数
        /// </summary>
        public int FECount = 0;

        /// <summary>
        /// 时钟频率
        /// </summary>
        public float ClockPL = 1;

        /// <summary>
        /// 数据域是否包含密码
        /// </summary>
        public bool DataFieldPassword = false;

        /// <summary>
        /// 写块操作是否加AA
        /// </summary>
        public bool BlockAddAA = false;
        /// <summary>
        /// 配置文件
        /// </summary>
        public string ConfigFile = "";

        /// <summary>
        /// 协议参数列表，KEY值为协议测试项目ID，并非多功能试验项目ID
        /// </summary>
        public Dictionary<string, string> DgnPras;

        /// <summary>
        /// 区别有无编程键，false：无，true：有
        /// </summary>
        public bool HaveProgrammingkey = false;

        private bool _Loading = false;
        /// <summary>
        /// 标志检查（只读），如果loading为假表示加载协议失败！
        /// </summary>
        public bool Loading
        {
            get
            {
                return _Loading;
            }
        }

        #endregion

        #region ---------------------------------下面部分为协议文件操作配置部分--------------------------------------


        /// <summary>
        /// 电能表制造厂家
        /// </summary>
        private string _DnbFactroy = "";
        /// <summary>
        /// 电能表型号
        /// </summary>
        private string _DnbSize = "";

        /// <summary>
        /// 电能表制造厂家
        /// </summary>
        public string DnbFactroy
        {
            get
            {
                return _DnbFactroy;
            }
            set
            {
                _DnbFactroy = value;
            }
        }
        /// <summary>
        /// 电能表型号
        /// </summary>
        public string DnbSize
        {
            get
            {
                return _DnbSize;
            }
            set
            {
                _DnbSize = value;
            }
        }


        public DgnProtocolInfo()
        {

        }
        /// <summary>
        /// 构造函数，根据协议名称获取通信协议
        /// </summary>
        /// <param name="ProtocolName">协议名称</param>
        public DgnProtocolInfo(string ProtocolName)
        {
            this.ProtocolName = ProtocolName;
            this.Load(ProtocolName);
        }
        /// <summary>
        /// 构造函数，根据生产厂家和表型号获取通信协议
        /// </summary>
        /// <param name="Factroy">生产厂家</param>
        /// <param name="Size">表型号</param>
        public DgnProtocolInfo(string Factroy, string Size)
        {
            this._DnbSize = Size;
            this._DnbFactroy = Factroy;
            this.Load(Factroy, Size);
        }


        /// <summary>
        /// 加载协议信息，调用该函数的前提是要么协议名称有值，要么制造厂家和表型号有值
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            if (this.ProtocolName == "" || (this._DnbFactroy == "" && this._DnbSize == ""))
            {
                return false;
            }

            this.LoadXmlData(this.ProtocolName, this._DnbFactroy, this._DnbSize);

            return true;
        }
        /// <summary>
        /// 根据协议名称加载协议信息
        /// </summary>
        /// <param name="ProtocolName"></param>
        public void Load(string ProtocolName)
        {
            this.LoadXmlData(ProtocolName, "", "");
        }
        /// <summary>
        /// 根据制造厂家和表型号加载协议信息
        /// </summary>
        /// <param name="Factroy"></param>
        /// <param name="Size"></param>
        private void Load(string Factroy, string Size)
        {
            this.LoadXmlData("", Factroy, Size);
        }


        public static XmlNode NodeProtocols = null;
        /// <summary>
        /// 加载XML文档
        /// </summary>
        /// <param name="protocolname"></param>
        /// <param name="factroy"></param>
        /// <param name="size"></param>
        private void LoadXmlData(string protocolname, string factory, string size)
        {
            if (protocolname == "" && (factory == "" || size == ""))
                return;

            CommDataBase.clsXmlControl _XmlNode = new CommDataBase.clsXmlControl(NodeProtocols);
            //new CommDataBase.clsXmlControl(System.Windows.Forms.Application.StartupPath + Const.Variable.CONST_DGNPROTOCOL);

            if (_XmlNode == null || _XmlNode.Count() == 0) return;

            System.Xml.XmlNode _FindXmlNode = null;

            if (protocolname != "")
                _FindXmlNode = CLDC_DataCore.DataBase.clsXmlControl.FindSencetion(_XmlNode.toXmlNode()
                                                                , CLDC_DataCore.DataBase.clsXmlControl.XPath(string.Format("R,Name,{0}", protocolname)));
            //else if (factory != "" && size != "")
            //    _FindXmlNode = CLDC_DataCore.DataBase.clsXmlControl.FindSencetion(_XmlNode.toXmlNode()
            //                                                    , CLDC_DataCore.DataBase.clsXmlControl.XPath(string.Format("R,ZZCJ,{0},BXH,{1}", factory, size)));
            if (_FindXmlNode == null) return;

            #region----------------------------加载协议文件信息----------------------------------------------------------------------

            this.ProtocolName = protocolname;         //协议名称 

            this.DnbFactroy = "";
            // CLDC_DataCore.DataBase.clsXmlControl.getNodeAttributeValue(_FindXmlNode, "ZZCJ");                  //制造厂家

            this.DnbSize = "";
            //CLDC_DataCore.DataBase.clsXmlControl.getNodeAttributeValue(_FindXmlNode, "BXH");                      //表型号

            this.DllFile = CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode, "ClassName");
            //CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode
            //                                                    , CLDC_DataCore.DataBase.clsXmlControl.XPath("C,Name,DllFile"));             //协议库名称
            this.ClassName = CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode, "ClassName");
            //CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode
            //                                                , CLDC_DataCore.DataBase.clsXmlControl.XPath("C,Name,ClassName"));           //说使用协议类名称
            this.Setting = CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode, "Setting");
            //CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode
            //                                                , CLDC_DataCore.DataBase.clsXmlControl.XPath("C,Name,Setting"));             //通信参数
            this.UserID = CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode, "UserID");
            //CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode
            //                                                , CLDC_DataCore.DataBase.clsXmlControl.XPath("C,Name,UserID"));              //用户名
            this.VerifyPasswordType = int.Parse(CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode, "VerifyPasswordType"));
            //int.Parse(CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode
            //                                                , CLDC_DataCore.DataBase.clsXmlControl.XPath("C,Name,VerifyPasswordType"))); //验证类型
            
            XmlNode nodeFeiLv = _FindXmlNode.SelectSingleNode("FeiLvId");
            this.TariffOrderType = nodeFeiLv.Attributes["Jian"].Value + nodeFeiLv.Attributes["Feng"].Value + nodeFeiLv.Attributes["Ping"].Value + nodeFeiLv.Attributes["Gu"].Value;
            //CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode
            //, CLDC_DataCore.DataBase.clsXmlControl.XPath("C,Name,TariffOrderType"));     //费率类型
            this.DateTimeFormat = CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode, "DateTimeFormat");
            //CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode
            //, CLDC_DataCore.DataBase.clsXmlControl.XPath("C,Name,DateTimeFormat"));      //日期格式
            this.SundayIndex = int.Parse(CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode, "SundayIndex"));
            //int.Parse(CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode
            //, CLDC_DataCore.DataBase.clsXmlControl.XPath("C,Name,SundayIndex")));        //星期天表示
            this.ClockPL = 1F;
            //float.Parse(CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode, CLDC_DataCore.DataBase.clsXmlControl.XPath("C,Name,ClockPL")) == "" ? "1"
            //: CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode, CLDC_DataCore.DataBase.clsXmlControl.XPath("C,Name,ClockPL")));        //时钟频率
            this.FECount = int.Parse(CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode, "FECount"));
            //CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode
            //                                                , CLDC_DataCore.DataBase.clsXmlControl.XPath("C,Name,FECount")));            //唤醒FE个数
            this.DataFieldPassword = bool.Parse(CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode, "DataFieldPassword"));
            //CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode
            //                                                , CLDC_DataCore.DataBase.clsXmlControl.XPath("C,Name,DataFieldPassword")) == "0" ? false : true;   //数据域是否包含密码
            this.BlockAddAA = bool.Parse(CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode, "BlockAddAA"));
            //CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode
            //                                                , CLDC_DataCore.DataBase.clsXmlControl.XPath("C,Name,BlockAddAA")) == "0" ? false : true;    //写数据块是否加AA    
            this.ConfigFile = "";
            //CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode
            //                                                , CLDC_DataCore.DataBase.clsXmlControl.XPath("C,Name,ConfigFile"));                          //配置文件    
            this.HaveProgrammingkey = bool.Parse(CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode, "HaveProgrammingkey"));
            //CLDC_DataCore.DataBase.clsXmlControl.getNodeValue(_FindXmlNode, CLDC_DataCore.DataBase.clsXmlControl.XPath("C,Name,HaveProgrammingkey")) == "0" ? false : true; //有无编程键
            //有编程键
            XmlNodeList nodeWithKey;
            if (HaveProgrammingkey)
            {
                nodeWithKey = _FindXmlNode.SelectNodes("OperationsHaveKey/Operation");

                this.WritePassword = nodeWithKey[0].Attributes["Password"].Value;
                this.WriteClass = nodeWithKey[0].Attributes["Class"].Value;
                this.ClearDemandPassword = nodeWithKey[1].Attributes["Password"].Value;
                this.ClearDemandClass = nodeWithKey[1].Attributes["Class"].Value;
                this.ClearDLPassword = nodeWithKey[2].Attributes["Password"].Value;
                this.ClearDLClass = nodeWithKey[2].Attributes["Class"].Value;
                ClearEventPassword= nodeWithKey[3].Attributes["Password"].Value;
                ClearDLClass = nodeWithKey[3].Attributes["Class"].Value;
                RelayPassword = nodeWithKey[4].Attributes["Password"].Value;
                RelayClass = nodeWithKey[4].Attributes["Class"].Value;
            }
            else
            {
                //无编程键
                nodeWithKey = _FindXmlNode.SelectNodes("OperationsNoKey/Operation");

                WritePassword = nodeWithKey[0].Attributes["Password"].Value;
                WriteClass = nodeWithKey[0].Attributes["Class"].Value;
                WritePassword2 = nodeWithKey[1].Attributes["Password"].Value;
                WriteClass2 = nodeWithKey[1].Attributes["Class"].Value;
                ClearDemandPassword = nodeWithKey[2].Attributes["Password"].Value;
                ClearDemandClass = nodeWithKey[2].Attributes["Class"].Value;
                ClearDLPassword = nodeWithKey[3].Attributes["Password"].Value;
                ClearDLClass = nodeWithKey[3].Attributes["Class"].Value;
                ClearEventPassword = nodeWithKey[4].Attributes["Password"].Value;
                ClearEventClass = nodeWithKey[4].Attributes["Class"].Value;
                RelayPassword = nodeWithKey[5].Attributes["Password"].Value;
                RelayClass = nodeWithKey[5].Attributes["Class"].Value;
            }
            

            _FindXmlNode = CLDC_DataCore.DataBase.clsXmlControl.FindSencetion(_FindXmlNode, CLDC_DataCore.DataBase.clsXmlControl.XPath("Prjs"));          //转到项目数据节点


            this._Loading = true;                //改写加载标志，表示加载协议成功

            this.DgnPras = new Dictionary<string, string>();

            if (_FindXmlNode == null) return;

            for (int i = 0; i < _FindXmlNode.ChildNodes.Count; i++)
            {
                this.DgnPras.Add(_FindXmlNode.ChildNodes[i].Attributes["ID"].Value, _FindXmlNode.ChildNodes[i].ChildNodes[0].Value);        //加入ID，值
            }

            if (this.DgnPras.Count == 0) return;



            #endregion

        }

        #endregion


        public override string ToString()
        {
            return "HashCode:" + this.GetHashCode();
        }

        #region ---------------------------------------------协议库模型---------------------------------------
        /// <summary>
        /// 协议编号
        /// </summary>
        public int Pro_ProtocolID = 0;
        /// <summary>
        /// 通讯协议归属
        /// </summary>
        public int Pro_proNameNo = 0;
        /// <summary>
        /// 信息序号
        /// </summary>
        public int Pro_intInfoNo = 0;
        /// <summary>
        /// 协议名称
        /// </summary>
        public string Pro_chrPname = "";
        /// <summary>
        /// 对应值
        /// </summary>
        public string Pro_chrValue = "";
        #endregion

        #region ---------------------------------------------协议标识字典模型---------------------------------------
        /// <summary>
        /// 编号
        /// </summary>
        public int Pd_dltID = 0;
        /// <summary>
        /// 协议代号
        /// </summary>
        public int Pd_proNameNo = 0;
        /// <summary>
        /// 数据标识编码类型
        /// </summary>
        public int Pd_intIdentType = 0;
        /// <summary>
        /// 权限
        /// </summary>
        public int Pd_intClass = 0;
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Pd_chrItemName = "";
        /// <summary>
        /// 数据标识
        /// </summary>
        public string Pd_chrID = "";
        /// <summary>
        /// 长度
        /// </summary>
        public int Pd_intLength = 0;
        /// <summary>
        /// 小数位
        /// </summary>
        public int Pd_intDot = 0;
        /// <summary>
        /// 操作方式
        /// </summary>
        public int Pd_intType = 0;
        /// <summary>
        /// 格式串
        /// </summary>
        public string Pd_chrFormat = "";
        /// <summary>
        /// 定义值
        /// </summary>
        public string Pd_chrDefValue = "";

        /// <summary>
        /// 是否是南网13加密机，false：不是，true：是
        /// </summary>
        public bool IsSouthEncryption = true;
        #endregion 

    }
}
