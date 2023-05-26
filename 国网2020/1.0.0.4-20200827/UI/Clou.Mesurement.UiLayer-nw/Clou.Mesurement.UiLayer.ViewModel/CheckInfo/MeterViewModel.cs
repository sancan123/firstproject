using System.Collections.Generic;
using System.Linq;
using Clou.Mesurement.UiLayer.DAL;
using Clou.Mesurement.UiLayer.ViewModel.Model;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.ComponentModel;
using Clou.Mesurement.UiLayer.Utility.Log;
using System.Threading.Tasks;
using Clou.Mesurement.UiLayer.ViewModel.WcfService;
using Clou.Mesurement.UiLayer.VerifyService;
using Clou.Mesurement.UiLayer.DAL.DataBaseView;
using System.Windows;

namespace Clou.Mesurement.UiLayer.ViewModel.CheckInfo
{
    /// 树形节点,用于检定方案
    /// <summary>
    /// 树形节点,用于检定方案
    /// </summary>
    public class MeterViewModel : ViewModelBase
    {
        #region 模型用到的数据
        private string equipmentNo = EquipmentData.Equipment.ID;
        /// <summary>
        /// 数据库对应的字段名称
        /// </summary>
        private List<string> fieldNames;
        /// 台体编号
        /// <summary>
        /// 台体编号
        /// </summary>
        public string EquipmentNo
        {
            get { return equipmentNo; }
        }

        public MeterViewModel()
        {
            List<FieldModel> fields = DALManager.MeterTempDbDal.GetFields("TMP_METER_INFO");
            IEnumerable<string> names = from item in fields select item.FieldName;
            fieldNames = names.ToList();
            InitializeMeters();
            FirstMeterToCheck.PropertyChanged += meterBasicInfo_PropertyChanged;
        }
        /// 表的基本信息发生变化时,将所有的表的相关信息更改
        /// <summary>
        /// 表的基本信息发生变化时,将所有的表的相关信息更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void meterBasicInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string temp = FirstMeterToCheck.GetProperty(e.PropertyName) as string;
            string propertyName = e.PropertyName;
            switch (propertyName)
            {
                case "CurrentMax":
                case "CurrentRating":
                    propertyName = "AVR_IB";
                    temp = string.Format("{0}({1})", FirstMeterToCheck.GetProperty("CurrentRating"), FirstMeterToCheck.GetProperty("CurrentMax"));
                    break;
                default:
                    temp = ConvertViewStringlToDbString(propertyName, temp);
                    break;
            }
            for (int i = 0; i < count; i++)
            {
                Meters[i].SetProperty(propertyName, temp);
            }
        }
        /// 表数量
        /// <summary>
        /// 表数量
        /// </summary>
        private readonly int count = EquipmentData.Equipment.MeterCount;

        private AsyncObservableCollection<DynamicViewModel> meters = new AsyncObservableCollection<DynamicViewModel>();
        public void Initialize()
        {
        }
        /// <summary>
        /// 初始化表信息
        /// </summary>
        private void InitializeMeters()
        {
            meters.Clear();
            for (int i = 0; i < count; i++)
            {
                DynamicViewModel viewModel = new DynamicViewModel(i+1);
                viewModel.SetProperty("AVR_DEVICE_ID", EquipmentNo);
                viewModel.SetProperty("LNG_BENCH_POINT_NO", i + 1);
                meters.Add(viewModel);
            }
            LoadMetersFromTempDb();
            for (int i = 0; i < count; i++)
            {
                meters[i].PropertyChanged += MeterInfo_PropertyChanged;
            }
        }
        /// 所有表的信息
        /// <summary>
        /// 所有表的信息
        /// </summary>
        public AsyncObservableCollection<DynamicViewModel> Meters
        {
            get
            {
                #region 是否是第一次赋值
                if (meters.Count != count)
                {
                    InitializeMeters();
                }
                #endregion
                return meters;
            }
            set { SetPropertyValue(value, ref meters, "Meters"); }
        }
        /// 显示在界面的列名称
        /// <summary>
        /// 显示在界面的列名称
        /// </summary>
        public Dictionary<string, string> DisplayDictonary
        {
            get
            {
                //42是参数录入对应的列
                return ResultViewHelper.GetPkDisplayDictionary("42");
            }
        }

        private bool isAllSelected;
        /// 选中所有表位
        /// <summary>
        /// 选中所有表位
        /// </summary>
        public bool IsAllSelected
        {
            get { return isAllSelected; }
            set
            {
                SetPropertyValue(value, ref isAllSelected, "IsAllSelected");
                string temp = "0";
                if (value)
                {
                    temp = "1";
                }
                for (int i = 0; i < Meters.Count; i++)
                {

                    Meters[i].SetProperty("CHR_CHECKED", temp);
                }
            }
        }

        private DynamicViewModel meterBasicInfo = new DynamicViewModel(0);
        /// 表的基本信息
        /// <summary>
        /// 表的基本信息
        /// </summary>
        public DynamicViewModel FirstMeterToCheck
        {
            get
            {
                return meterBasicInfo;
            }
            set
            {
                SetPropertyValue(value, ref meterBasicInfo, "FirstMeterToCheck");
            }
        }
        #endregion

        #region 数据完整性检查,快速设置等
        /// 如果是由于界面更改所引起的变化,则表的数据相互关联
        /// 这样能够快速输入表的信息
        /// <summary>
        /// 如果是由于界面更改所引起的变化,则表的数据相互关联
        /// 这样能够快速输入表的信息
        /// </summary>
        private bool flagQuick = true;
        /// <summary>
        /// 快速输入标记
        /// </summary>
        public bool FlagQuick
        {
            get
            {
                return flagQuick;
            }
            set
            {
                SetPropertyValue(value, ref flagQuick, "FlagQuick");
            }
        }
        /// <summary>
        /// 表信息发生改变时,将下一个表的相同信息设置为一样的数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MeterInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DynamicViewModel meter = sender as DynamicViewModel;
            if (meter == null)
            {
                return;
            }
            int index = Meters.IndexOf(meter);
            if (e.PropertyName == "CHR_CHECKED")
            {
                RefreshFirstMeter();
                //更新要检标记
                if (this == EquipmentData.MeterGroupInfo)
                {
                    EquipmentData.CheckResults.UpdateYaoJian(index);
                }
                return;
            }
        }
        private int firstMeterIndex = -1;
        /// <summary>
        /// 刷新第一块要检的表
        /// </summary>
        void RefreshFirstMeter()
        {
            for (int i = 0; i < count; i++)
            {
                object obj = Meters[i].GetProperty("CHR_CHECKED");
                if ((obj as string) == "1")
                {
                    if (i != firstMeterIndex)
                    {
                        firstMeterIndex = i;
                        ConvertDbToViewModel(Meters[i]);
                        OnPropertyChanged("FirstMeterToCheck");
                    }
                    return;
                }
            }
            firstMeterIndex = -1;
        }
        /// <summary>
        /// 标的要检标识
        /// </summary>
        public bool[] YaoJian
        {
            get
            {
                bool[] yaoJian = new bool[count];
                for (int i = 0; i < count; i++)
                {
                    object obj = Meters[i].GetProperty("CHR_CHECKED");
                    if ((obj as string) == "1")
                    {
                        yaoJian[i] = true;
                    }
                    else
                    {
                        yaoJian[i] = false;
                    }
                }
                return yaoJian;
            }
            set
            {
                for (int i = 0; i < count; i++)
                {
                    if (i < value.Length)
                    {
                        if (value[i])
                        {
                            Meters[i].SetProperty("CHR_CHECKED", "1");
                        }
                        else
                        {
                            Meters[i].SetProperty("CHR_CHECKED", "0");
                        }
                    }
                }
            }
        }
        #endregion

        #region 加载表信息
        /// <summary>
        /// 从临时数据库加载表信息
        /// </summary>
        private void LoadMetersFromTempDb()
        {
            int countTemp = DALManager.MeterTempDbDal.GetCount(EnumMeterDataDb.TMP_METER_INFO.ToString(), string.Format("avr_device_id='{0}'", EquipmentNo));
            if (countTemp != EquipmentData.Equipment.MeterCount)
            {
                NewMeters();
                EquipmentData.Controller.Index = -1;
                return;
            }
            List<DynamicModel> models = DALManager.MeterTempDbDal.GetList("tmp_meter_info", string.Format("AVR_DEVICE_ID='{0}' order by LNG_BENCH_POINT_NO", EquipmentNo));
            for (int i = 0; i < models.Count; i++)
            {
                object obj = models[i].GetProperty("LNG_BENCH_POINT_NO");
                if (obj is int)
                {
                    int index = (int)obj;
                    if (index <= count && index > 0)
                    {
                        #region 设置表信息
                        for (int j = 0; j < fieldNames.Count; j++)
                        {
                            meters[index - 1].SetProperty(fieldNames[j], models[i].GetProperty(fieldNames[j]));
                        }
                        string pkObj = Meters[index - 1].GetProperty("PK_LNG_METER_ID") as string;
                        if (string.IsNullOrEmpty(pkObj) || pkObj.Length < 8)
                        {
                            Meters[index - 1].SetProperty("PK_LNG_METER_ID", GetUniquenessID8(i + 1).ToString());
                        }
                        //协议编号无效
                        Meters[index - 1].SetProperty("FK_PROTOCOL_ID", 1);
                        #endregion
                    }
                }
            }
            firstMeterIndex = -1;
            RefreshFirstMeter();
        }
        #endregion

        #region 表的唯一编号
        private long longMac = 0;
        /// <summary>
        /// 获取8字节唯一ID：4字节时间戳+3字节主机MAC+1字节自增序列
        /// </summary>
        /// <param name="id">自增序列，只取1字节</param>
        /// <returns>8字节唯一ID</returns>
        public long GetUniquenessID8(int id)
        {
            string strMac = "";
            long lngMac = GetMac(out strMac);

            string s = string.Format("{0:X8}{1:X6}{2:X2}", GetTimeStamp(), ((int)(lngMac)) & 0x00FFFFFF, ((byte)id));
            long n = Convert.ToInt64(s, 16);

            return n;
        }
        /// <summary>
        /// 获取本机MAC地址
        /// </summary>
        /// <param name="MacString">MAC字符串</param>
        /// <returns>MAC值</returns>
        private long GetMac(out string MacString)
        {
            string macAddress = "";
            if (longMac == 0)
            {
                try
                {
                    NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                    foreach (NetworkInterface adapter in nics)
                    {
                        if (!adapter.GetPhysicalAddress().ToString().Equals(""))
                        {
                            macAddress = adapter.GetPhysicalAddress().ToString();
                            longMac = Convert.ToInt64(macAddress, 16);
                            for (int i = 1; i < 6; i++)
                            {
                                macAddress = macAddress.Insert(3 * i - 1, ":");
                            }
                            break;
                        }
                    }

                }
                catch
                {
                }
            }
            MacString = macAddress;
            return longMac;
        }
        /// <summary>
        /// 获得当前时间的4字节时间戳
        /// </summary>
        /// <returns></returns>
        private int GetTimeStamp()
        {
            DateTime timeStamp = new DateTime(1970, 1, 1); //得到1970年的时间戳 
            long a = (DateTime.UtcNow.Ticks - timeStamp.Ticks) / 10000000; //注意这里有时区问题，用now就要减掉8个小时
            int b = (int)a;
            return b;
        }
        /// <summary>
        /// 获取12字节唯一ID：4字节时间戳+4字节主机MAC+2字节进程PID+2字节自增序列
        /// </summary>
        /// <param name="id">自增序列，只取2字节</param>
        /// <returns>12字节唯一ID</returns>
        private long GetUniquenessID12(int id)
        {
            string strMac = "";
            long lngMac = GetMac(out strMac);
            Process curPro = Process.GetCurrentProcess();
            string s = string.Format("{0:X8}{1:X8}{2:X4}{3:X4}", GetTimeStamp(), (int)(lngMac), (short)curPro.Id, ((short)id));
            long n = Convert.ToInt64(s, 16);

            return n;
        }
        #endregion

        #region 所有列的名称
        //PK_LNG_METER_ID	NVARCHAR 	50	唯一编号
        //AVR_DEVICE_ID	NVARCHAR	10	台体编号
        //LNG_BENCH_POINT_NO	INT		表位号
        //AVR_ASSET_NO	NVARCHAR	40	计量编号、资产编号
        //AVR_MADE_NO	NVARCHAR	40	出厂编号
        //AVR_BAR_CODE	NVARCHAR	40	条形码
        //AVR_ADDRESS	   NVARCHAR	20	表通信地址
        //AVR_FACTORY	NVARCHAR	100	制造厂家
        //AVR_METER_MODEL	NVARCHAR	50	表型号
        //AVR_AR_CONSTANT	NVARCHAR	30	表常数
        //有功常数(无功常数)
        //AVR_METER_TYPE	NVARCHAR	50	表类型
        //AVR_AR_CLASS	NVARCHAR	15	等级
        //有功等级(无功等级)
        //AVR_MADE_DATE	NVARCHAR	15	出厂日期
        //AVR_CUSTOMER	NVARCHAR	150	送检单位
        //AVR_CERTIFICATE_NO	NVARCHAR	40	证书编号
        //AVR_METER_NAME	NVARCHAR	100	表名称
        //AVR_WIRING_MODE	NVARCHAR	4	测量方式
        //0=三相三线；1=三相四线
        //AVR_UB	NVARCHAR	15	额定电压（V）
        //AVR_IB	NVARCHAR	15	额定电流
        //IB(IMAX)A
        //AVR_FREQUENCY	NVARCHAR	10	频率。单位HZ
        //CHR_CC_PREVENT_FLAG	CHAR	1	止逆器。
        //0=不经止逆器；1=经止逆器
        //CHR_CT_CONNECTION_FLAG	CHAR	1	互感器。
        //0=不经互感器；1=经互感器
        //AVR_TEST_TYPE	NVARCHAR	20	检定类型
        //DTM_TEST_DATE	DATE		检定日期
        //DTM_VALID_DATE	DATE		计检日期
        //AVR_TEMPERATURE	NVARCHAR	8	温度
        //AVR_HUMIDITY	NVARCHAR	8	湿度
        //AVR_TOTAL_CONCLUSION	NVARCHAR	6	检定结论。Y/N
        //合格Y、不合格N
        //AVR_TEST_PERSON	NVARCHAR	50	检验员
        //AVR_AUDIT_PERSON	NVARCHAR	50	核验员
        //AVR_SUPERVISOR	NVARCHAR	50	主管
        //CHR_CHECKED	CHAR	1	要检此表。1要检，0不检
        //CHR_UPLOAD_FLAG	CHAR	1	数据是否已上网标志。
        //0：未上传,1：已上传
        //AVR_SEAL_1	NVARCHAR	30	铅封号1
        //AVR_SEAL_2	NVARCHAR	30	铅封号2
        //AVR_SEAL_3	NVARCHAR	30	铅封号3
        //AVR_SEAL_4	NVARCHAR	30	铅封号4
        //AVR_SEAL_5	NVARCHAR	30	铅封号5
        //AVR_SOFT_VER	NVARCHAR	255	软件版本号
        //AVR_HARD_VER	NVARCHAR	255	硬件版本号
        //AVR_ARRIVE_BATCH_NO	NVARCHAR	40	到货批次号
        //FK_LNG_SCHEME_ID	LONG		方案唯一编号
        //FK_PROTOCOL_ID	LONG		协议唯一编号
        //AVR_PROTOCOL_NAME	NVARCHAR	50	通讯协议名称
        //AVR_PULSE_TYPE	NVARCHAR	6	脉冲类型（共阴共阳）
        //CHR_RATES_TYPE	CHAR	1	费控类型,0:远程表，1：本地表
        //AVR_TASK_NO	NVARCHAR	40	任务编号
        //AVR_WORK_NO	NVARCHAR	40	工单号
        //AVR_CARR_PROTC_NAME	NVARCHAR	50	载波协议名称
        //AVR_OTHER_1	NVARCHAR	50	备用1
        //AVR_OTHER_2	NVARCHAR	50	备用2
        //AVR_OTHER_3	NVARCHAR	50	备用3
        //AVR_OTHER_4	NVARCHAR	50	备用4
        //AVR_OTHER_5	NVARCHAR	50	备用5
        #endregion

        #region 执行数据库数据与用到的数据之间的转换
        /// 将数据库的数据转换成程序用到的数据
        /// <summary>
        /// 将数据库的数据转换成程序用到的数据
        /// </summary>
        /// <param name="firstMeter">获取的第一块表的数据</param>
        private void ConvertDbToViewModel(DynamicViewModel firstMeter)
        {
            #region 表电流
            string currentString = firstMeter.GetProperty("AVR_IB") as string;
            if (!string.IsNullOrEmpty(currentString))
            {
                string[] currentArray = currentString.Split('(');
                if (currentArray.Length == 2 && !string.IsNullOrEmpty(currentArray[1]) && currentArray[1].Length > 1)
                {
                    FirstMeterToCheck.SetProperty("CurrentRating", currentArray[0]);
                    FirstMeterToCheck.SetProperty("CurrentMax", currentArray[1].Substring(0, currentArray[1].Length - 1));
                }
            }
            #endregion

            #region 费控类型(0:远程表,1:本地表)
            string fkType = firstMeter.GetProperty("CHR_RATES_TYPE") as string;
            if (fkType == "1")
            {
                FirstMeterToCheck.SetProperty("CHR_RATES_TYPE", "本地表");
            }
            else if (fkType == "0")
            {
                FirstMeterToCheck.SetProperty("CHR_RATES_TYPE", "远程表");
            }
            else
            {
                FirstMeterToCheck.SetProperty("CHR_RATES_TYPE", "无费控");
            }
            #endregion

            #region 有无止逆器
            string znq = firstMeter.GetProperty("CHR_CC_PREVENT_FLAG") as string;
            if (znq == "1")
            {
                FirstMeterToCheck.SetProperty("CHR_CC_PREVENT_FLAG", "经止逆器");
            }
            else if (znq == "0")
            {
                FirstMeterToCheck.SetProperty("CHR_CC_PREVENT_FLAG", "不经止逆器");
            }
            #endregion

            #region 互感器类型
            string hgq = firstMeter.GetProperty("CHR_CT_CONNECTION_FLAG") as string;
            if (hgq == "1")
            {
                FirstMeterToCheck.SetProperty("CHR_CT_CONNECTION_FLAG", "互感接入式");
            }
            else if (hgq == "0")
            {
                FirstMeterToCheck.SetProperty("CHR_CT_CONNECTION_FLAG", "直接接入式");
            }
            #endregion

            #region 脉冲类型
            string pulseType = firstMeter.GetProperty("AVR_PULSE_TYPE") as string;
            if (pulseType == "1")
            {
                FirstMeterToCheck.SetProperty("AVR_PULSE_TYPE", "共阳");
            }
            else
            {
                FirstMeterToCheck.SetProperty("AVR_PULSE_TYPE", "共阴");
            }
            #endregion

            #region 接线方式
            string jxfs = firstMeter.GetProperty("AVR_WIRING_MODE") as string;
            FirstMeterToCheck.SetProperty("AVR_WIRING_MODE", CodeDictionary.GetNameLayer2("CLFS", jxfs));
            #endregion

            FirstMeterToCheck.SetProperty("AVR_UB", firstMeter.GetProperty("AVR_UB"));
            FirstMeterToCheck.SetProperty("AVR_FREQUENCY", firstMeter.GetProperty("AVR_FREQUENCY"));
            FirstMeterToCheck.SetProperty("AVR_OTHER_3", firstMeter.GetProperty("AVR_OTHER_3"));
            try
            {
                int schemaId = (int)firstMeter.GetProperty("FK_LNG_SCHEME_ID");
                DynamicViewModel model = EquipmentData.SchemaModels.Schemas.FirstOrDefault(item => ((int)item.GetProperty("ID")) == schemaId);
                if (model != null)
                {
                    EquipmentData.Schema.SchemaId = schemaId;
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage("从表信息加载方案编号出错!", EnumLogSource.数据库存取日志, EnumLevel.Error, e);
            }
        }
        /// 将程序数据转换成数据库用到的数据
        /// <summary>
        /// 将程序数据转换成数据库用到的数据
        /// </summary>
        /// <param name="propertyName">基本信息中的属性名</param>
        /// <param name="propertyValue">值</param>
        /// <returns>转换后的值</returns>
        private string ConvertViewStringlToDbString(string propertyName, string propertyValue)
        {
            string temp = propertyValue;
            #region 根据属性名转换值
            switch (propertyName)
            {
                case "CHR_RATES_TYPE":
                    #region
                    switch (propertyValue)
                    {
                        case "本地表":
                            temp = "1";
                            break;
                        case "远程表":
                            temp = "0";
                            break;
                        case "无费控":
                            temp = "2";
                            break;
                        default:
                            temp = "2";
                            break;
                    }
                    #endregion
                    break;
                case "CHR_CC_PREVENT_FLAG":
                    #region
                    switch (propertyValue)
                    {
                        case "经止逆器":
                            temp = "1";
                            break;
                        case "不经止逆器":
                            temp = "0";
                            break;
                        default:
                            temp = "0";
                            break;
                    }
                    #endregion
                    break;
                case "CHR_CT_CONNECTION_FLAG":
                    #region
                    switch (propertyValue)
                    {
                        case "直接接入式":
                            temp = "0";
                            break;
                        case "互感接入式":
                            temp = "1";
                            break;
                        default:
                            temp = "0";
                            break;
                    }
                    #endregion
                    break;
                case "AVR_PULSE_TYPE":
                    #region
                    switch (propertyValue)
                    {
                        case "共阳":
                            temp = "1";
                            break;
                        case "共阴":
                            temp = "0";
                            break;
                        default:
                            temp = "0";
                            break;
                    }
                    #endregion
                    break;
                case "AVR_WIRING_MODE":
                    temp = CodeDictionary.GetValueLayer2("CLFS", propertyValue);
                    break;
            }
            #endregion
            return temp;
        }
        #endregion

        #region 存储表信息到数据库
        /// 初始化所有表的信息信息
        /// <summary>
        /// 初始化所有表的信息信息
        /// </summary>
        public void NewMeters()
        {
            if (MessageBox.Show("确认要更换新表吗?更换新表操作将会删除当前批次表的检定结论,请确认检定数据已经上传,或者当前检定数据无效再执行更换新表操作", "更换新表", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }
            //要清除的字段名称,清除相关字段的值,生成主键
            string[] fieldsToClear = { "AVR_ASSET_NO", "AVR_MADE_NO", "AVR_BAR_CODE", "AVR_ADDRESS", "AVR_AR_CONSTANT", "AVR_AR_CLASS", "AVR_MADE_DATE", "CHR_CHECKED", "AVR_TOTAL_CONCLUSION", "AVR_CERTIFICATE_NO" };

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < fieldsToClear.Length; j++)
                {
                    Meters[i].SetProperty(fieldsToClear[j], "");
                }
                Meters[i].SetProperty("PK_LNG_METER_ID", GetUniquenessID8(i + 1));
                Meters[i].SetProperty("CHR_CHECKED", "1");
                Meters[i].SetProperty("AVR_DEVICE_ID", EquipmentNo);
            }

            FirstMeterToCheck.SetProperty("CurrentRating", "1.5");
            FirstMeterToCheck.SetProperty("CurrentMax", "6");
            FirstMeterToCheck.SetProperty("CHR_RATES_TYPE", "远程表");
            FirstMeterToCheck.SetProperty("CHR_CC_PREVENT_FLAG", "不经止逆器");
            FirstMeterToCheck.SetProperty("CHR_CT_CONNECTION_FLAG", "互感接入式");
            FirstMeterToCheck.SetProperty("AVR_PULSE_TYPE", "共阴");
            FirstMeterToCheck.SetProperty("AVR_WIRING_MODE", "单相");
            FirstMeterToCheck.SetProperty("AVR_UB", "100");
            FirstMeterToCheck.SetProperty("AVR_FREQUENCY", "50");
            FirstMeterToCheck.SetProperty("AVR_OTHER_3", "首检");

            List<string> tableNames = DALManager.MeterTempDbDal.GetTableNames();
            //临时库中的表信息和检定数据信息
            List<string> sqlDeleteList = new List<string>();
            for (int i = 0; i < tableNames.Count; i++)
            {
                sqlDeleteList.Add(string.Format("delete from {0} where avr_device_id = '{1}'", tableNames[i], EquipmentNo));
            }
            int countDelete = DALManager.MeterTempDbDal.ExecuteOperation(sqlDeleteList);
            LogManager.AddMessage(string.Format("从临时库删除表信息完成,共删除{0}条记录", countDelete), EnumLogSource.数据库存取日志);
            int countAdd = 0;
            for (int i = 0; i < count; i++)
            {
                countAdd += DALManager.MeterTempDbDal.Insert(EnumMeterDataDb.TMP_METER_INFO.ToString(), Meters[i].GetDataSource());
            }
            LogManager.AddMessage(string.Format("从临时库添加表信息完成,共添加{0}条记录", countDelete), EnumLogSource.数据库存取日志);
            //清空临时数据库中的结论
            CheckResultBll.Instance.DeleteResultFromTempDb();

            EquipmentData.MeterGroupInfo = this;
            //删除检定结论
            EquipmentData.CheckResults.ClearAllResult();
        }
        /// 录入表信息
        /// <summary>
        /// 录入表信息
        /// </summary>
        public void UpdateMeter()
        {
            for (int i = 0; i < Meters.Count; i++)
            {
                Meters[i].SetProperty("FK_LNG_SCHEME_ID", EquipmentData.SchemaModels.SelectedSchema.GetProperty("ID"));
            }
            LogManager.AddMessage("开始更新数据库中的表信息...", EnumLogSource.数据库存取日志);
            List<string> fieldsToUpdate = new List<string>();
            for (int i = 0; i < fieldNames.Count; i++)
            {
                if (fieldNames[i] != "PK_LNG_METER_ID")
                {
                    fieldsToUpdate.Add(fieldNames[i]);
                }
            }
            for (int i = 0; i < count; i++)
            {
                int temp = DALManager.MeterTempDbDal.Update(EnumMeterDataDb.TMP_METER_INFO.ToString(), string.Format("PK_LNG_METER_ID = '{0}'", Meters[i].GetProperty("PK_LNG_METER_ID")), Meters[i].GetDataSource(), fieldsToUpdate);
                if (temp != 1)
                {
                    LogManager.AddMessage(string.Format("更新表信息数据失败,表位号:{0}", i + 1), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                }
            }
            LogManager.AddMessage("更新表信息数完成", EnumLogSource.数据库存取日志);
            string errMessage = "";
            if (CheckInputPara(out errMessage))
            {
                ReplaceMeter();

                LogManager.AddMessage("执行保存表信息到数据库完毕.", EnumLogSource.数据库存取日志);
                if (!WcfHelper.Instance.SetMeters())
                {
                    LogManager.AddMessage("下发表信息失败,请检查检定模块服务是否已开启!", EnumLogSource.用户操作日志, EnumLevel.Error);
                }

                if (EquipmentData.LastCheckInfo.CheckIndex == -1)
                {
                    EquipmentData.LastCheckInfo.SchemaId = EquipmentData.Schema.SchemaId;
                    EquipmentData.CheckResults.CheckNodeCurrent = EquipmentData.CheckResults.ResultCollection[0];
                    EquipmentData.LastCheckInfo.CheckIndex = 0;
                }
                UiInterface.CloseWindow("参数录入");
                EquipmentData.NavigateCurrentUi();
            }
            else
            {
                MessageBox.Show(errMessage, "表信息参数不完整", MessageBoxButton.OK, MessageBoxImage.Warning);
                LogManager.AddMessage(errMessage, EnumLogSource.用户操作日志, EnumLevel.Error);
            }
        }
        /// 更换表信息
        /// <summary>
        /// 更换表信息
        /// </summary>
        private void ReplaceMeter()
        {
            for (int i = 0; i < EquipmentData.MeterGroupInfo.count; i++)
            {
                List<string> namesTemp = Meters[i].GetAllProperyName();
                for (int j = 0; j < namesTemp.Count; j++)
                {
                    EquipmentData.MeterGroupInfo.Meters[i].SetProperty(namesTemp[j], Meters[i].GetProperty(namesTemp[j]));
                }
            }

            EquipmentData.CheckResults.UpdateYaoJian();
        }
        /// 表信息合理性验证
        /// <summary>
        /// 表信息合理性验证
        /// </summary>
        /// <param name="errString"></param>
        /// <returns></returns>
        public bool CheckInputPara(out string errString)
        {
            errString = "";
            if (firstMeterIndex == -1)
            {
                errString = "没有表被选中,请选择要检的表!";
                return false;
            }
            #region 基本信息验证
            string[] basicFields = { "CurrentRating", "CurrentMax", "CHR_RATES_TYPE", "CHR_CC_PREVENT_FLAG", "CHR_CT_CONNECTION_FLAG", "AVR_PULSE_TYPE", "AVR_WIRING_MODE", "AVR_UB", "AVR_FREQUENCY", "AVR_OTHER_3" };
            string[] basicParaName = { "额定电流", "最大电流", "费控类型", "有无止逆器", "互感器类型", "脉冲类型", "接线方式", "电压信息", "频率信息", "首检周检类型" };
            for (int i = 0; i < basicFields.Length; i++)
            {
                if (string.IsNullOrEmpty(FirstMeterToCheck.GetProperty(basicFields[i]) as string))
                {
                    errString = string.Format("表信息录入不完整:{0} 未录入!", basicParaName[i]);
                    return false;
                }
            }
            #endregion
            #region 其它信息验证
            string[] otherFields = { "AVR_PROTOCOL_NAME", "AVR_METER_TYPE", "AVR_AR_CONSTANT", "AVR_AR_CLASS" };
            string[] otherNames = { "通讯协议", "表类型", "表常数", "准确度等级", "" };
            for (int i = 0; i < otherFields.Length; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    object obj = Meters[j].GetProperty("CHR_CHECKED");
                    if ((obj as string) == "1")
                    {
                        if (string.IsNullOrEmpty(Meters[j].GetProperty(otherFields[i]) as string))
                        {
                            errString = string.Format("表信息录入不完整:表位{0} 的 {1} 未录入!", j + 1, otherNames[i]);
                            return false;
                        }
                    }
                }
            }
            #endregion

            #region 方案确认
            if (EquipmentData.SchemaModels.SelectedSchema == null)
            {
                return false;
            }
            #endregion

            return true;
        }
        /// 转换数据库信息到检定软件信息
        /// <summary>
        /// 转换数据库信息到检定软件信息
        /// </summary>
        /// <returns></returns>
        public MeterInfo[] GetVerifyMeterInfo()
        {
            MeterInfo[] meterInfos = new MeterInfo[count];
            for (int i = 0; i < count; i++)
            {
                meterInfos[i] = new MeterInfo();
                // 表位号		在表架上所挂位置
                meterInfos[i].Mb_intBno = i + 1;
                // 条形码	
                meterInfos[i].Mb_ChrTxm = Meters[i].GetProperty("AVR_BAR_CODE") as string;
                // 表通信地址
                meterInfos[i].Mb_chrAddr = Meters[i].GetProperty("AVR_ADDRESS") as string;
                // 表型号
                meterInfos[i].Mb_Bxh = Meters[i].GetProperty("AVR_METER_MODEL") as string;
                // 表常数
                meterInfos[i].Mb_chrBcs = Meters[i].GetProperty("AVR_AR_CONSTANT") as string;
                // 表类型
                meterInfos[i].Mb_chrBlx = Meters[i].GetProperty("AVR_METER_TYPE") as string;
                // 表等级		有功（无功）
                meterInfos[i].Mb_chrBdj = Meters[i].GetProperty("AVR_AR_CLASS") as string;
                int temp = 0;
                int.TryParse(Meters[i].GetProperty("AVR_PULSE_TYPE") as string, out temp);
                // 共阴 共阳类型
                meterInfos[i].Mb_gygy = temp;
                temp = 0;
                int.TryParse(Meters[i].GetProperty("AVR_WIRING_MODE") as string, out temp);
                // 测量方式
                meterInfos[i].Mb_intClfs = temp;
                // 电压		XXX（不带单位）
                meterInfos[i].Mb_chrUb = Meters[i].GetProperty("AVR_UB") as string;
                // 电流		Ib(Imax)（不带单位）
                meterInfos[i].Mb_chrIb = Meters[i].GetProperty("AVR_IB") as string;
                // 频率		XX（不带单位）
                meterInfos[i].Mb_chrHz = Meters[i].GetProperty("AVR_FREQUENCY") as string;
                // 止逆器		1-有，0-无
                meterInfos[i].Mb_BlnZnq = (Meters[i].GetProperty("CHR_CC_PREVENT_FLAG") as string == "1") ? true : false;
                // 互感器		1-经互感器
                meterInfos[i].Mb_BlnHgq = (Meters[i].GetProperty("CHR_CT_CONNECTION_FLAG") as string == "0") ? false : true;
                // 软件版本号
                meterInfos[i].Mb_chrSoftVer = Meters[i].GetProperty("AVR_SOFT_VER") as string;
                // 硬件版本号
                meterInfos[i].Mb_chrHardVer = Meters[i].GetProperty("AVR_HARD_VER") as string;
                // 通讯协议名称
                meterInfos[i].AVR_PROTOCOL_NAME = Meters[i].GetProperty("AVR_PROTOCOL_NAME") as string;
                // 载波协议名称
                meterInfos[i].AVR_CARR_PROTC_NAME = Meters[i].GetProperty("AVR_CARR_PROTC_NAME") as string;
                // 费控类型,1本地，0远程
                temp = 0;
                int.TryParse(Meters[i].GetProperty("CHR_RATES_TYPE") as string, out temp);
                meterInfos[i].Mb_intFKType = temp;
                //是否要检
                meterInfos[i].YaoJianYn = (Meters[i].GetProperty("CHR_CHECKED") as string == "1");
                // 使用的规程名称
                meterInfos[i].GuiChengName = "JJG596-2012";
            }
            return meterInfos;
        }
        #endregion
    }
}
