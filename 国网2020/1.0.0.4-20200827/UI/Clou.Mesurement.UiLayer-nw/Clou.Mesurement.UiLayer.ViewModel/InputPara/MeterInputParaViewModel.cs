using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.ViewModel.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Mesurement.UiLayer.VerifyService;
using Mesurement.UiLayer.ViewModel.WcfService;
using Mesurement.UiLayer.Utility;

namespace Mesurement.UiLayer.ViewModel.InputPara
{
    /// <summary>
    /// 表信息录入数据模型
    /// </summary>
    public class MeterInputParaViewModel : ViewModelBase
    {
        /// <summary>
        /// 参数录入时的构造函数,从内存中加载表信息,由于是在xaml页面中构造,所有没有参数
        /// </summary>
        public MeterInputParaViewModel()
        {
            Initial();
            for (int i = 0; i < ParasModel.AllUnits.Count; i++)
            {
                for (int j = 0; j < Meters.Count; j++)
                {
                    string fieldName = ParasModel.AllUnits[i].FieldName;
                    object objTemp = EquipmentData.MeterGroupInfo.Meters[j].GetProperty(fieldName);
                    Meters[j].SetProperty(fieldName, objTemp);
                }
            }
            RefreshFirstMeterInfo();
        }
        /// <summary>
        /// 程序启动时的构造函数,从数据库加载
        /// </summary>
        /// <param name="isCurrent"></param>
        public MeterInputParaViewModel(bool isCurrent)
        {
            Initial();
            LoadMetersFromTempDb();
            //如果表位数与当前表位数不符,执行换新表
            if (Meters.Count != EquipmentData.Equipment.MeterCount)
            {
                NewMeters();
            }
        }
        /// <summary>
        /// 空方法,用于初始化表信息
        /// </summary>
        public void Initialize()
        { }
        private InputParaViewModel parasModel = new InputParaViewModel();
        /// <summary>
        /// 表信息录入相关的数据模型
        /// </summary>
        public InputParaViewModel ParasModel
        {
            get { return parasModel; }
            set { SetPropertyValue(value, ref parasModel, "ParasModel"); }
        }
        private AsyncObservableCollection<DynamicViewModel> meters = new AsyncObservableCollection<DynamicViewModel>();
        /// <summary>
        /// 表信息集合
        /// </summary>
        public AsyncObservableCollection<DynamicViewModel> Meters
        {
            get { return meters; }
            set { SetPropertyValue(value, ref meters, "Meters"); }
        }

        private DynamicViewModel firstMeter = new DynamicViewModel(0);
        /// <summary>
        /// 表位基本信息
        /// </summary>
        public DynamicViewModel FirstMeter
        {
            get { return firstMeter; }
            set { SetPropertyValue(value, ref firstMeter, "FirstMeter"); }
        }
        /// <summary>
        /// 表位是否要检
        /// </summary>
        public bool[] YaoJian
        {
            get
            {
                bool[] arrayTemp = new bool[EquipmentData.Equipment.MeterCount];
                for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
                {
                    arrayTemp[i] = Meters[i].GetProperty("CHR_CHECKED") as string == "1";
                }
                return arrayTemp;
            }
        }
        /// <summary>
        /// 初始化表信息
        /// </summary>
        private void Initial()
        {
            int meterCount = EquipmentData.Equipment.MeterCount;
            #region 赋初值
            for (int i = 0; i < meterCount; i++)
            {
                DynamicViewModel viewModel = null;
                if (i >= Meters.Count)
                {
                    viewModel = new DynamicViewModel(i + 1);
                    meters.Add(viewModel);
                }
                else
                {
                    viewModel = Meters[i];
                }
                //设置默认值
                for (int j = 0; j < ParasModel.AllUnits.Count; j++)
                {
                    InputParaUnit paraUnit = ParasModel.AllUnits[j];
                    if (paraUnit.FieldName == "AVR_DEVICE_ID")
                    {
                        viewModel.SetProperty("AVR_DEVICE_ID", EquipmentData.Equipment.ID);
                    }
                    else if (paraUnit.FieldName == "LNG_BENCH_POINT_NO")
                    {
                        viewModel.SetProperty("LNG_BENCH_POINT_NO", i + 1);
                    }
                    else if (paraUnit.FieldName == "DTM_TEST_DATE")
                    {
                        viewModel.SetProperty("DTM_TEST_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        if (paraUnit.IsNewValue)
                        {
                            if (!string.IsNullOrEmpty(paraUnit.DefaultValue))
                            {
                                viewModel.SetProperty(paraUnit.FieldName, paraUnit.DefaultValue);
                            }
                            else
                            {
                                viewModel.SetProperty(paraUnit.FieldName, "");
                            }
                        }
                    }
                }
            }
            #endregion

            for (int i = 0; i < Meters.Count; i++)
            {
                Meters[i].PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "CHR_CHECKED")
                    {
                        RefreshFirstMeterInfo();
                    }
                };
            }
        }

        /// <summary>
        /// 从临时数据库加载表信息
        /// </summary>
        private void LoadMetersFromTempDb()
        {
            List<DynamicModel> models = DALManager.MeterTempDbDal.GetList("tmp_meter_info", string.Format("AVR_DEVICE_ID='{0}' order by LNG_BENCH_POINT_NO", EquipmentData.Equipment.ID));
            for (int i = 0; i < models.Count; i++)
            {
                object obj = models[i].GetProperty("LNG_BENCH_POINT_NO");
                if (obj is int)
                {
                    int index = (int)obj;
                    if (index <= Meters.Count && index > 0)
                    {
                        //Meters[index - 1] = new DynamicViewModel(models[i], index);
                        for (int j = 0; j < ParasModel.AllUnits.Count; j++)
                        {
                            InputParaUnit paraUnitTemp = ParasModel.AllUnits[j];
                            if (paraUnitTemp.ValueType == InputParaUnit.EnumValueType.编码值)
                            {
                                Meters[index - 1].SetProperty(paraUnitTemp.FieldName, CodeDictionary.GetNameLayer2(paraUnitTemp.CodeType, models[i].GetProperty(paraUnitTemp.FieldName) as string));
                            }
                            else
                            {
                                Meters[index - 1].SetProperty(paraUnitTemp.FieldName, models[i].GetProperty(paraUnitTemp.FieldName));
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < Meters.Count; i++)
            {
                string pkObj = Meters[i].GetProperty("PK_LNG_METER_ID") as string;
                if (string.IsNullOrEmpty(pkObj) || pkObj.Length < 8)
                {
                    Meters[i].SetProperty("PK_LNG_METER_ID", GetUniquenessID8(i + 1).ToString());
                }
            }
            RefreshFirstMeterInfo();
        }
        /// <summary>
        /// 验证数据是否完整
        /// </summary>
        /// <param name="stringError"></param>
        /// <returns></returns>
        public bool CheckInfoCompleted(out string stringError)
        {
            stringError = "";
            if (EquipmentData.SchemaModels.SelectedSchema == null)
            {
                stringError = "检定方案不能为空,请指定当前检定方案!";
                return false;
            }
            bool[] yaojian = YaoJian;
            bool flagHaveYaojian = false;
            for (int i = 0; i < yaojian.Length; i++)
            {
                if (yaojian[i])
                {
                    flagHaveYaojian = true;
                    break;
                }
            }
            if (!flagHaveYaojian)
            {
                stringError = "请至少选择一块要检的表";
                return false;
            }
            for (int j = 0; j < Meters.Count; j++)
            {
                if (!yaojian[j])
                {
                    continue;
                }
                for (int i = 0; i < ParasModel.AllUnits.Count; i++)
                {
                    if (ParasModel.AllUnits[i].IsDisplayMember && ParasModel.AllUnits[i].IsNecessary)
                    {
                        if (Meters[j].GetProperty(ParasModel.AllUnits[i].FieldName) == null || string.IsNullOrEmpty(Meters[j].GetProperty(ParasModel.AllUnits[i].FieldName).ToString()))
                        {
                            stringError = string.Format("表位{0}缺少信息: {1}", j + 1, ParasModel.AllUnits[i].DisplayName);
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 更新第一块要检表信息
        /// </summary>
        private void RefreshFirstMeterInfo()
        {
            bool[] yaojian = YaoJian;
            for (int i = 0; i < yaojian.Length; i++)
            {
                if (yaojian[i])
                {
                    FirstMeter = Meters[i];
                    break;
                }
            }
        }
        /// <summary>
        /// 保存表信息
        /// </summary>
        private void SaveMeterInfo()
        {
            #region 转换显示数据为数据库数据
            List<DynamicModel> models = new List<DynamicModel>();
            for (int i = 0; i < Meters.Count; i++)
            {
                DynamicModel modelTemp = new DynamicModel();

                for (int j = 0; j < ParasModel.AllUnits.Count; j++)
                {
                    InputParaUnit paraUnitTemp = ParasModel.AllUnits[j];
                    if (paraUnitTemp.ValueType == InputParaUnit.EnumValueType.编码值)
                    {
                        modelTemp.SetProperty(paraUnitTemp.FieldName, CodeDictionary.GetValueLayer2(paraUnitTemp.CodeType, Meters[i].GetProperty(paraUnitTemp.FieldName) as string));
                    }
                    else
                    {
                        modelTemp.SetProperty(paraUnitTemp.FieldName, Meters[i].GetProperty(paraUnitTemp.FieldName));
                    }
                }
                modelTemp.SetProperty("FK_LNG_SCHEME_ID", EquipmentData.Schema.SchemaId);
                if(i==0)//获取测量方式
                {
                    EquipmentData.Equipment.EquipmentType = GetMeterInfoName(int.Parse(modelTemp.GetProperty("AVR_WIRING_MODE").ToString()), "AVR_WIRING_MODE");
              
                }
                models.Add(modelTemp);
            }
            #endregion
            #region 获取当前表数量
            List<string> pkList = new List<string>();
            for (int i = 0; i < Meters.Count; i++)
            {
                pkList.Add(string.Format("PK_LNG_METER_ID = '{0}'", Meters[i].GetProperty("PK_LNG_METER_ID")));
            }
            string pkWhere = string.Join(" or ", pkList);
            #endregion
            int countInDb = DALManager.MeterTempDbDal.GetCount("tmp_meter_info", pkWhere);
            #region 插入新数据
            if (countInDb != Meters.Count)
            {
                int deleteCount = DALManager.MeterTempDbDal.Delete("tmp_meter_info", string.Format("avr_device_id='{0}'", EquipmentData.Equipment.ID));
                LogManager.AddMessage(string.Format("数据库中表数量:{1}块 与当前录入表数量:{2}块 不一致,删除表信息,共删除{0}条表信息数据.", deleteCount, countInDb, Meters.Count), EnumLogSource.数据库存取日志);
                int insertCount = DALManager.MeterTempDbDal.Insert("tmp_meter_info", models);
                LogManager.AddMessage(string.Format("更新表信息,共插入{0}条表信息数据.", insertCount), EnumLogSource.数据库存取日志);
                return;
            }
            #endregion
            #region 更新现有信息
            List<string> fieldNames = new List<string>();
            var namesTemp = from item in ParasModel.AllUnits select item.FieldName;
            fieldNames = namesTemp.ToList();
            fieldNames.Remove("PK_LNG_METER_ID");
            int updateCount = DALManager.MeterTempDbDal.Update("tmp_meter_info", "PK_LNG_METER_ID", models, fieldNames);
            LogManager.AddMessage(string.Format("更新表信息,共更新{0}条表信息数据.", updateCount), EnumLogSource.数据库存取日志);
            #endregion
        }
        /// <summary>
        /// 更新表信息
        /// </summary>
        public void UpdateMeterInfo()
        {
            string errorString = "";
            if (!CheckInfoCompleted(out errorString))
            {
                MessageBox.Show(errorString, "表信息不完整");
                return;
            }
            SaveMeterInfo();
            EquipmentData.MeterGroupInfo.LoadMetersFromTempDb();
            EquipmentData.CheckResults.RefreshYaojian();
            TaskManager.AddWcfAction(() =>
            {
                if (!WcfHelper.Instance.SetMeters())
                {
                    LogManager.AddMessage("下发表信息失败,请检查检定模块服务是否已开启!", EnumLogSource.用户操作日志, EnumLevel.Error);
                }
            });

            if (EquipmentData.Controller.Index == -1)
            {
                EquipmentData.LastCheckInfo.SchemaId = EquipmentData.Schema.SchemaId;
                EquipmentData.LastCheckInfo.CheckIndex = 0;
                EquipmentData.Controller.Index = 0;
            }
            UiInterface.CloseWindow("参数录入");
            EquipmentData.NavigateCurrentUi();

           
        }
        /// <summary>
        /// 换新表
        /// </summary>
        public void NewMeters()
        {
            if (MessageBox.Show("确认要更换新表吗?更换新表操作将会删除当前批次表的检定结论,请确认检定数据已经上传,或者当前检定数据无效再执行更换新表操作", "更换新表", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                EquipmentData.Controller.Index = -1;
                Initial();
                for (int i = 0; i < Meters.Count; i++)
                {
                    Meters[i].SetProperty("PK_LNG_METER_ID", GetUniquenessID8(i + 1).ToString());
                }
                SaveMeterInfo();
                LoadMetersFromTempDb();

                //清空临时数据库中的结论
                CheckResultBll.Instance.DeleteResultFromTempDb();
                //删除检定结论
                EquipmentData.CheckResults.ClearAllResult();

                EquipmentData.NavigateCurrentUi();
            }
        }
        /// <summary>
        /// 转换数据库信息并将数据发送到检定服务
        /// </summary>
        /// <returns></returns>
        public MeterInfo[] GetVerifyMeterInfo()
        {
            bool[] yaojianTemp = YaoJian;
            MeterInfo[] meterInfos = new MeterInfo[Meters.Count];
            for (int i = 0; i < Meters.Count; i++)
            {
                meterInfos[i] = new MeterInfo();
                // 表位号		在表架上所挂位置
                meterInfos[i].Mb_intBno = i + 1;
                // 条形码	
                meterInfos[i].Mb_ChrTxm = GetMeterInfo(i, "AVR_BAR_CODE");
                // 表通信地址
                meterInfos[i].Mb_chrAddr = GetMeterInfo(i, "AVR_ADDRESS");
                // 表MAC通信地址 
                meterInfos[i].AVR_OTHER_2 = GetMeterInfo(i, "AVR_OTHER_2");
                // 表号
                meterInfos[i]._Mb_MeterNo = GetMeterInfo(i, "AVR_OTHER_1");
                // 表型号
                meterInfos[i].Mb_Bxh = GetMeterInfo(i, "AVR_METER_MODEL"); 
                // 表常数
                meterInfos[i].Mb_chrBcs = GetMeterInfo(i, "AVR_AR_CONSTANT"); 
                // 表类型
                meterInfos[i].Mb_chrBlx = GetMeterInfo(i, "AVR_METER_TYPE"); 
                // 表等级		有功（无功）
                meterInfos[i].Mb_chrBdj = GetMeterInfo(i, "AVR_AR_CLASS"); 
                int temp = 0;
                int.TryParse(GetMeterInfo(i, "AVR_PULSE_TYPE"), out temp);
                // 共阴 共阳类型
                meterInfos[i].Mb_gygy = temp;
                temp = 0;
                int.TryParse(GetMeterInfo(i, "AVR_WIRING_MODE"), out temp);
                // 测量方式
                meterInfos[i].Mb_intClfs = temp;
                // 电压		XXX（不带单位）
                meterInfos[i].Mb_chrUb = GetMeterInfo(i, "AVR_UB"); 
                // 电流		Ib(Imax)（不带单位）
                meterInfos[i].Mb_chrIb = GetMeterInfo(i, "AVR_IB");
                // 频率		XX（不带单位）
                meterInfos[i].Mb_chrHz = GetMeterInfo(i, "AVR_FREQUENCY"); 
                // 止逆器		1-有，0-无
                meterInfos[i].Mb_BlnZnq = (GetMeterInfo(i, "CHR_CC_PREVENT_FLAG") == "1") ? true : false;
                // 互感器		1-经互感器
                meterInfos[i].Mb_BlnHgq = (GetMeterInfo(i, "CHR_CT_CONNECTION_FLAG") == "0") ? false : true;
                // 软件版本号
                meterInfos[i].Mb_chrSoftVer = GetMeterInfo(i, "AVR_SOFT_VER"); 
                // 硬件版本号
                meterInfos[i].Mb_chrHardVer = GetMeterInfo(i, "AVR_HARD_VER"); 
                // 通讯协议名称
                meterInfos[i].AVR_PROTOCOL_NAME = GetMeterInfo(i, "AVR_PROTOCOL_NAME"); 
                // 载波协议名称
                meterInfos[i].AVR_CARR_PROTC_NAME = GetMeterInfo(i, "AVR_CARR_PROTC_NAME"); 
                // 费控类型,1本地，0远程
                temp = 0;
                int.TryParse(GetMeterInfo(i, "CHR_RATES_TYPE"), out temp);
                meterInfos[i].Mb_intFKType = temp;
                //是否要检
                meterInfos[i].YaoJianYn = yaojianTemp[i];
                // 使用的规程名称
                meterInfos[i].GuiChengName = GetMeterInfo(i, "AVR_OTHER_5");//"JJG596-2012";
               
                //备用3
                meterInfos[i].AVR_OTHER_3 = GetMeterInfo(i, "AVR_OTHER_3");
                //备用4
                meterInfos[i].AVR_OTHER_4 = GetMeterInfo(i, "AVR_OTHER_4");

                //表唯一ID
                meterInfos[i].MB_ID = GetMeterInfo(i, "PK_LNG_METER_ID");
            }
            return meterInfos;
        }

        #region 表的唯一编号
        private long longMac = 0;
        /// <summary>
        /// 获取8字节唯一ID：4字节时间戳+3字节主机MAC+1字节自增序列
        /// </summary>
        /// <param name="id">自增序列，只取1字节</param>
        /// <returns>8字节唯一ID</returns>
        private long GetUniquenessID8(int id)
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

        /// <summary>
        /// 获取表信息
        /// </summary>
        /// <param name="index">表序号,从0开始</param>
        /// <param name="fieldName">表字段名称</param>
        /// <returns></returns>
        public string GetMeterInfo(int index, string fieldName)
        {
            InputParaUnit paraUnit = ParasModel.AllUnits.FirstOrDefault(item => item.FieldName == fieldName);
            if (paraUnit != null && index >= 0 && index < Meters.Count)
            {
                string resultTemp = Meters[index].GetProperty(fieldName) as string;
                if (paraUnit.ValueType == InputParaUnit.EnumValueType.编码值)
                {
                    resultTemp = CodeDictionary.GetValueLayer2(paraUnit.CodeType, resultTemp);
                }
                return resultTemp;
            }
            else
            { return ""; }
        }

        /// <summary>
        /// 获取表中文名称
        /// </summary>
        /// <param name="index">表序号,从0开始</param>
        /// <param name="fieldName">表字段名称</param>
        /// <returns></returns>
        public string GetMeterInfoName(int index, string fieldName)
        {
            InputParaUnit paraUnit = ParasModel.AllUnits.FirstOrDefault(item => item.FieldName == fieldName);
            if (paraUnit != null && index >= 0 && index < Meters.Count)
            {
                string resultTemp = Meters[index].GetProperty(fieldName) as string;
               
                return resultTemp;
            }
            else
            { return ""; }
        }
    }
}
