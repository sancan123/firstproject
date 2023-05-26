using System.Collections.Generic;
using Mesurement.UiLayer.DAL.Config;
using Mesurement.UiLayer.ViewModel.Model;
using Mesurement.UiLayer.ViewModel.WcfService;
using Mesurement.UiLayer.Utility.Log;
using System;
using System.Reflection;
using Mesurement.UiLayer.Utility;

namespace Mesurement.UiLayer.ViewModel.Device
{
    public class DeviceViewModel : ViewModelBase
    {
        private bool isBusy;
        /// 正在忙碌
        /// <summary>
        /// 正在忙碌
        /// </summary>
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetPropertyValue(value, ref isBusy, "IsBusy"); }
        }

        private string userText;

        public string UserText
        {
            get { return userText; }
            set { SetPropertyValue(value, ref userText, "UserText"); }
        }


        #region 设备状态
        private bool isReady;
        /// 设备就绪,连接以后就可以下发开始检定命令了
        /// <summary>
        /// 设备就绪,连接以后就可以下发开始检定命令了
        /// </summary>
        public bool IsReady
        {
            get { return isReady; }
            set
            {
                SetPropertyValue(value, ref isReady, "IsReady");
                EquipmentData.Controller.OnPropertyChanged("IsEnable");
            }
        }

        private bool? isConnected = null;
        /// <summary>
        /// 台体设备连接正常
        /// </summary>
        public bool? IsConnected
        {
            get { return isConnected; }
            set
            {
                SetPropertyValue(value, ref isConnected, "IsConnected");
            }
        }
        #endregion

        #region 表位信息
        private bool selectAll;
        public bool SelectAll
        {
            get { return selectAll; }
            set
            {
                SetPropertyValue(value, ref selectAll, "SelectAll");
                for (int i = 0; i < MeterUnits.Count; i++)
                {
                    MeterUnits[i].IsSelected = value;
                }
            }
        }

        private AsyncObservableCollection<MeterUnitViewModel> meterUnits = new AsyncObservableCollection<MeterUnitViewModel>();
        public AsyncObservableCollection<MeterUnitViewModel> MeterUnits
        {
            get { return meterUnits; }
            set { SetPropertyValue(value, ref meterUnits, "MeterUnits"); }
        }
        #endregion

        #region 配置相关
        private AsyncObservableCollection<DevicePortClass> devices = new AsyncObservableCollection<DevicePortClass>();
        /// 设备列表
        /// <summary>
        /// 设备列表
        /// </summary>
        public AsyncObservableCollection<DevicePortClass> Devices
        {
            get { return devices; }
            set { SetPropertyValue(value, ref devices, "Devices"); }
        }

        private AsyncObservableCollection<string> relays = new AsyncObservableCollection<string>();
        public AsyncObservableCollection<string> Relays
        {
            get { return relays; }
            set { relays = value; }
        }

        private AsyncObservableCollection<string> meterPorts = new AsyncObservableCollection<string>();
        /// 表位端口配置字符串
        /// <summary>
        /// 表位端口配置字符串
        /// </summary>
        public AsyncObservableCollection<string> MeterPorts
        {
            get { return meterPorts; }
            set { meterPorts = value; }
        }
        #endregion

        ///标准表 = 1001,
        ///功率源 = 1002,
        ///载波模块 = 1003,
        ///多功能板 = 1004,
        ///耐压仪 = 1005,
        ///功耗板 = 1006,
        ///误差板 = 1007,
        ///标准时钟源 = 1008,
        ///电流互感器 = 1009,
        ///功耗仪 = 1010,
        ///电机控制板 = 1011,
        ///电机压接板 = 1012,
        ///远程控制板 = 1013,
        ///温湿度采集板 = 1014,
        public void LoadDevices()
        {
            Devices.Clear();
            MeterUnits.Clear();
            Relays.Clear();
            #region 仪表设备
            int DevIndex = 0;
            for (int i = 1001; i < 1024; i++)
            {
                if (i == 1020 || i == 1021) continue;

                EnumConfigId enumConfigId = (EnumConfigId)i;
                List<string> listTemp = ConfigHelper.Instance.GetConfig(enumConfigId);
                if (listTemp.Count > 0)
                {
                    for (int j = 0; j < listTemp.Count; j++)
                    {
                        DevicePortClass device = new DevicePortClass(listTemp[j]);
                        device.DeviceType = enumConfigId;
                        device.Index = j;
                        device.Count = listTemp.Count;
                        Devices.Add(device);
                        DevIndex++;
                    }
                }
            }
            //List<string> listTemp2 = ConfigHelper.Instance.GetConfig(EnumConfigId.波形控制器);
            //if (listTemp2.Count > 0)
            //{
            //    for (int j = 0; j < listTemp2.Count; j++)
            //    {
            //        DevicePortClass device = new DevicePortClass(listTemp2[j]);
            //        device.DeviceType = EnumConfigId.波形控制器;
            //        device.Index = j;
            //        device.Count = listTemp2.Count;
            //        //device.DriverName = "E_CL2050";
            //        //device.ClassName = "CL2050";
            //        Devices.Add(device);
            //    }
            //}
            #endregion
            #region 表位状态

            bool[] BlnMeterYaojian = new bool[EquipmentData.Equipment.MeterCount];
            BlnMeterYaojian = EquipmentData.MeterGroupInfo.YaoJian;
            for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
            {

                MeterUnits.Add(new MeterUnitViewModel()
                {
                    TextScreen = string.Format("--{0}--", (i + 1).ToString("D2")),
                    IsSelected = BlnMeterYaojian[i]
                });
            }
            #endregion
            //#region 继电器配置
            //List<string> listRelay = ConfigHelper.Instance.GetConfig(EnumConfigId.继电器配置);
            //for (int i = 0; i < listRelay.Count; i++)
            //{
            //    Relays.Add(listRelay[i]);
            //}
            //#endregion
            #region 表位端口
            List<string> meterList = ConfigHelper.Instance.GetConfig(EnumConfigId.RS485);
            MeterPorts = new AsyncObservableCollection<string>(meterList);
            ServersViewModel.Instance.LoadMeterPort();
            #endregion
            LogManager.AddMessage("设备端口参数加载成功.", EnumLogSource.设备操作日志);
        }

        /// <summary>
        /// 解析设备操作命令
        /// </summary>
        /// <param name="deviceCommand">格式:{设备名}|{方法名}|{参数1}|{参数2}|{参数3}...</param>
        public override void CommandFactoryMethod(string deviceCommand)
        {
            TaskManager.AddWcfAction(() =>
            {
                string[] arrayCommand = deviceCommand.Split('|');
                #region 合法性校验
                if (string.IsNullOrEmpty(arrayCommand[0]))
                {
                    LogManager.AddMessage(string.Format("设备控制方法不能为空:{0}", arrayCommand[0]), EnumLogSource.用户操作日志, EnumLevel.Warning);
                    return;
                }
                #endregion
                #region 获取方法
                try
                {
                    Type[] typeArray = Type.EmptyTypes;
                    if (arrayCommand.Length > 1)
                    {
                        typeArray = new Type[arrayCommand.Length - 1];
                        for (int i = 1; i < arrayCommand.Length; i++)
                        {
                            typeArray[i - 1] = typeof(string);
                        }
                    }
                    MethodInfo method = GetType().GetMethod(arrayCommand[0], typeArray);
                    if (method == null)
                    {
                        LogManager.AddMessage(string.Format("没有找到方法:{0}", deviceCommand), EnumLogSource.设备操作日志, EnumLevel.Warning);
                        return;
                    }
                    try
                    {
                        object[] arrayParams = null;
                        if (arrayCommand.Length > 1)
                        {
                            arrayParams = new object[arrayCommand.Length - 1];
                            for (int j = 0; j < arrayParams.Length; j++)
                            {
                                arrayParams[j] = arrayCommand[1 + j];
                            }
                        }
                        IsBusy = true;
                        object objReturn = method.Invoke(this, arrayParams);
                        IsBusy = false;
                        if (objReturn is int)
                        {
                            int resultTemp = (int)objReturn;
                            if (resultTemp == 0)
                            {
                                LogManager.AddMessage(string.Format("调用台体操作方法{0}成功", deviceCommand), EnumLogSource.设备操作日志);
                            }
                            else
                            {
                                LogManager.AddMessage(string.Format("调用台体操作方法{0}失败,返回值:{1}", deviceCommand, objReturn), EnumLogSource.设备操作日志, EnumLevel.Warning);
                            }
                        }
                    }
                    catch (Exception methodEx)
                    {
                        IsBusy = false;
                        LogManager.AddMessage(string.Format("调用方法出现异常:{0}:{1}", deviceCommand, methodEx.Message), EnumLogSource.设备操作日志, EnumLevel.Warning, methodEx);
                    }
                }
                catch (AmbiguousMatchException e)
                {
                    IsBusy = false;
                    LogManager.AddMessage(string.Format("找到不止一个具有指定名称的方法:{0}", deviceCommand), EnumLogSource.设备操作日志, EnumLevel.Warning, e);
                }
                #endregion
            });
        }

        public void UserInvoke()
        {
            if (string.IsNullOrEmpty(UserText))
            {
                return;
            }
            WcfHelper.Instance.DeviceControl(UserText, null);
        }

        #region 调用设备驱动方法
        /// 下发初始化连接命令
        /// <summary>
        /// 下发初始化连接命令
        /// </summary>
        public void InitSetting()
        {
            //检定服务的客户端是否开启,如果没有连接成功则建立连接
            if (!WcfHelper.Instance.ClientIsReady)
            {
                if (!WcfHelper.Instance.InitialControlClient())
                {
                    return;
                }
            }
            #region 下发命令
            List<string> deviceStringList = new List<string>();
            for (int i = 0; i < devices.Count; i++)
            {
                deviceStringList.Add(devices[i].ToString());
            }
            deviceStringList.AddRange(Relays);
            deviceStringList.AddRange(MeterPorts);
            WcfHelper.Instance.InitDevice(deviceStringList.ToArray());
            #endregion
        }
        /// 连接设备
        /// <summary>
        /// 连接设备
        /// </summary>
        public void Link()
        {
            object[] paras = null;
            if (WcfHelper.Instance.DeviceControl("Link", paras) != 0)
            {
                EquipmentData.DeviceManager.IsConnected = false;
            }
            else
            {
                EquipmentData.DeviceManager.IsConnected = true;
            }
            IsReady = true;
        }
        /// 断开设备连接
        /// <summary>
        /// 断开设备连接
        /// </summary>
        public void UnLink()
        {
            object[] paras = null;
            if (IsReady)
            {
                if (WcfHelper.Instance.ClientIsReady)
                {
                    WcfHelper.Instance.DeviceControl("UnLink", paras);
                }
            }
            IsReady = false;
        }
        #region 功率源
        public void PowerOnFree(double Ua, double Ub, double Uc, double Ia, double Ib, double Ic, double PhiUa, double PhiUb, double PhiUc, double PhiIa, double PhiIb, double PhiIc, float Hz)
        {
            object[] paras = new object[] { Ua, Ub, Uc, Ia, Ib, Ic, PhiUa, PhiUb, PhiUc, PhiIa, PhiIb, PhiIc, Hz };
            WcfHelper.Instance.DeviceControl("PowerOnFree", paras);
        }
        public int PowerOnOnlyVoltage()
        {
            string ubString = EquipmentData.MeterGroupInfo.FirstMeter.GetProperty("AVR_UB") as string;
            float floatTemp = 0;
            if (float.TryParse(ubString, out floatTemp))
            {
                int ControlType = 0;
                object[] paras1 = new object[] { string.Join("|", EquipmentData.MeterGroupInfo.YaoJian), ControlType };
                TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("SetLoadRelayControl", paras1));

                object[] paras2 = new object[] { floatTemp, 0F, 1,1, "1.0", true, false };
                TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("PowerOn", paras2));
            }
            return 0;
        }
        public int PowerOff()
        {
            object[] paras = null;
            return WcfHelper.Instance.DeviceControl("PowerOff", paras);
        }
        #endregion
        #region 标准表
        public int ReadStdMeterConst()
        {
            long temp = 0;
            object[] paras = new object[] { temp };
            return WcfHelper.Instance.DeviceControl("ReadStdMeterConst", paras);
        }
        public int ReadStdInfo()
        {
            object[] paras = null;
            return WcfHelper.Instance.DeviceControl("ReadStdInfo", paras);
        }
        #endregion
        #region 时基源
        public void ReadGpsTime()
        {
            object[] paras = null;
            WcfHelper.Instance.DeviceControl("ReadGpsTime", paras);
        }
        #endregion
        #region 耐压仪
        //public void ReadGpsTime()
        //{
        //    WcfHelper.Instance.DeviceControl("ReadGpsTime", null);
        //}
        #endregion
        #region 多功能板
        /// <summary>
        /// 设置台体供电继电器
        /// </summary>
        /// <param name="powerType">耐压供电=1、载波供电=2、普通供电=3、一回路=4、二回路=5</param>
        /// <param name="isCoupling">是否互感器</param>
        public void SetPowerSupplyType(string powerType, string isCoupling)
        {
            int typeTemp = 3;
            bool couplingTemp = false;
            if (int.TryParse(powerType, out typeTemp) && bool.TryParse(isCoupling, out couplingTemp))
            {
                object[] paras = new object[] { typeTemp, couplingTemp };
                WcfHelper.Instance.DeviceControl("SetPowerSupplyType", paras);
            }
        }
        private int lightType;

        public int LightType
        {
            get { return lightType; }
            set { SetPropertyValue(value, ref lightType, "LightType"); }
        }

        /// 控制三色彩灯
        /// <summary>
        /// 控制三色彩灯
        /// </summary>
        /// <param name="stringID">灯类型 18红、19黄、20绿</param>
        /// <param name="type">等于0时灭、1时正常、2时闪烁</param>
        public void SetEquipmentThreeColor(string stringID, string stringType)
        {
            int iId = 0;
            int iType = 0;
            if (int.TryParse(stringID, out iId) && int.TryParse(stringType, out iType))
            {
                object[] paras = null;
                if (iType == 0)
                {
                    paras = new object[] { iId, iType };
                }
                else
                {
                    paras = new object[] { iId, LightType + 1 };
                }
                WcfHelper.Instance.DeviceControl("SetEquipmentThreeColor", paras);
            }
        }
        #endregion
        #region 误差板
        public void SetMeterOnOff(string CtrType)
        {

            bool[] arrayIsolation = new bool[MeterUnits.Count];
            if (CtrType.ToUpper() == "ALL")
            {
                for (int i = 0; i < MeterUnits.Count; i++)
                {
                    arrayIsolation[i] = true;
                }
            }
            else
            {
                for (int i = 0; i < MeterUnits.Count; i++)
                {
                    arrayIsolation[i] = !MeterUnits[i].IsSelected;
                }
            }
            object[] paras = new object[] { string.Join("|", arrayIsolation) };
            WcfHelper.Instance.DeviceControl("SetMeterOnOff", paras);
        }
        public void EquipmentPressA(string strPress)
        {
            bool[] arrayIsolation = new bool[MeterUnits.Count];
            for (int i = 0; i < MeterUnits.Count; i++)
            {
                arrayIsolation[i] = MeterUnits[i].IsSelected;
            }
            bool isPress = bool.Parse(strPress);
            bool[] results = new bool[MeterUnits.Count];
            string[] resultDescriptions = new string[MeterUnits.Count];
            object[] paras = new object[] { isPress, string.Join("|", arrayIsolation), string.Join("|", results), string.Join("|", resultDescriptions) };
            WcfHelper.Instance.DeviceControl("EquipmentPressA", paras);
        }
        public void ReadWcb()
        {
            object[] paras = null;
            WcfHelper.Instance.DeviceControl("ReadWcb", paras);
        }
        #endregion
        #region 加密机
        public void SouthLink(string szType, string cHostIp, int uiPort, int timeout)
        {
            object[] paras = new object[] { szType, cHostIp, uiPort, timeout };
            WcfHelper.Instance.DeviceControl("SouthLink", paras);
        }
        public void SouthCloseDevice()
        {
            object[] paras = null;
            WcfHelper.Instance.DeviceControl("SouthCloseDevice", paras);
        }
        public void SouthIdentityAuthentication(int Flag, string PutDiv, out string OutRand, out string OutEndata)
        {
            OutRand = "";
            OutEndata = "";
            object[] paras = new object[] { Flag, PutDiv, OutRand, OutEndata };
            WcfHelper.Instance.DeviceControl("SouthIdentityAuthentication", paras);
        }
        public void SouthUserControl(int Flag, string PutRand, string PutDiv, string PutEsamNo,
string PutData, out string OutEndataout)
        {
            OutEndataout = "";
            object[] paras = new object[] { Flag,  PutRand,  PutDiv,  PutEsamNo,
 PutData,   OutEndataout};
            WcfHelper.Instance.DeviceControl("SouthUserControl", paras);
        }
        public void SouthParameterUpdate(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, out string OutData)
        {
            OutData = "";
            object[] paras = new object[] { Flag, PutRand, PutDiv, PutApdu, PutData, OutData };
            WcfHelper.Instance.DeviceControl("SouthParameterUpdate", paras);
        }
        public void SouthPrice1Update(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, out string OutData)
        {
            OutData = "";
            object[] paras = new object[] { Flag, PutRand, PutDiv, PutApdu, PutData, OutData };
            WcfHelper.Instance.DeviceControl("SouthPrice1Update", paras);
        }
        public void SouthPrice2Update(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, out string OutData)
        {
            OutData = "";
            object[] paras = new object[] { Flag, PutRand, PutDiv, PutApdu, PutData, OutData };
            WcfHelper.Instance.DeviceControl("SouthPrice2Update", paras);
        }
        public void SouthParameterElseUpdate(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, out string OutEndata)
        {
            OutEndata = "";
            object[] paras = new object[] { Flag, PutRand, PutDiv, PutApdu, PutData, OutEndata };
            WcfHelper.Instance.DeviceControl("SouthParameterElseUpdate", paras);
        }
        public void SouthIncreasePurse(int Flag, string PutRand, string PutDiv, string PutData, out string OutData)
        {
            OutData = "";
            object[] paras = new object[] { Flag, PutRand, PutDiv, PutData, OutData };
            WcfHelper.Instance.DeviceControl("SouthIncreasePurse", paras);
        }
        public void SouthInitPurse(int Flag, string PutRand, string PutDiv, string PutData, out string OutData)
        {
            OutData = "";
            object[] paras = new object[] { Flag, PutRand, PutDiv, PutData, OutData };
            WcfHelper.Instance.DeviceControl("SouthInitPurse", paras);
        }
        public void SouthKeyUpdateV2(int PutKeySum, string PutKeyState, string PutKeyId, string PutRand, string PutDiv, string PutEsamNo, out string OutData)
        {
            OutData = "";
            object[] paras = new object[] { PutKeySum, PutKeyState, PutKeyId, PutRand, PutDiv, PutEsamNo, OutData };
            WcfHelper.Instance.DeviceControl("SouthKeyUpdateV2", paras);
        }
        public void SouthDataClear1(int Flag, string PutRand, string PutDiv, string PutData, out string OutData)
        {
            OutData = "";
            object[] paras = new object[] { Flag, PutRand, PutDiv, PutData, OutData };
            WcfHelper.Instance.DeviceControl("SouthDataClear1", paras);
        }
        public void SouthInfraredRand(out string OutRand1)
        {
            OutRand1 = "";
            object[] paras = new object[] { OutRand1 };
            WcfHelper.Instance.DeviceControl("SouthInfraredRand", paras);
        }
        public void SouthInfraredAuth(int Flag, string PutDiv, string PutEsamNo, string PutRand1, string PutRand1Endata, string PutRand2, out string OutRand2Endata)
        {
            OutRand2Endata = "";
            object[] paras = new object[] { Flag, PutDiv, PutEsamNo, PutRand1, PutRand1Endata, PutRand2, OutRand2Endata };
            WcfHelper.Instance.DeviceControl("SouthInfraredAuth", paras);
        }
        public void SouthMacCheck(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, string PutMac)
        {
            object[] paras = new object[] { Flag, PutRand, PutDiv, PutApdu, PutData, PutMac };
            WcfHelper.Instance.DeviceControl("SouthMacCheck", paras);
        }
        public void SouthMacWrite(int Flag, string PutRand, string PutDiv, string PutEsamNo, string PutFileID, string PutDataBegin, string PutData, out string OutData)
        {
            OutData = "";
            object[] paras = new object[] { Flag, PutRand, PutDiv, PutEsamNo, PutFileID, PutDataBegin, PutData, OutData };
            WcfHelper.Instance.DeviceControl("SouthMacWrite", paras);
        }
        public void SouthEncMacWrite(int Flag, string PutRand, string PutDiv, string PutEsamNo, string PutFileID, string PutDataBegin, string PutData, out string OutData)
        {
            OutData = "";
            object[] paras = new object[] { Flag, PutRand, PutDiv, PutEsamNo, PutFileID, PutDataBegin, PutData, OutData };
            WcfHelper.Instance.DeviceControl("SouthEncMacWrite", paras);
        }
        public void SouthEncForCompare(string PutKeyid, string PutDiv, string PutData, out string OutData)
        {
            OutData = "";
            object[] paras = new object[] { PutKeyid, PutDiv, PutData, OutData };
            WcfHelper.Instance.DeviceControl("SouthEncForCompare", paras);
        }
        public void SouthDecreasePurse(int Flag, string PutRand, string PutDiv, string PutData, out string OutEndata)
        {
            OutEndata = "";
            object[] paras = new object[] { Flag, PutRand, PutDiv, PutData, OutEndata };
            WcfHelper.Instance.DeviceControl("SouthDecreasePurse", paras);
        }
        public void SouthSwitchChargeMode(int Flag, string PutRand, string PutDiv, string PutData,
out string OutData)
        {
            OutData = "";
            object[] paras = new object[] { Flag,  PutRand,  PutDiv,  PutData,
  OutData };
            WcfHelper.Instance.DeviceControl("SouthSwitchChargeMode", paras);
        }
        #endregion

        #region 读卡器
        public void WINAPI_OpenDevice()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("WINAPI_OpenDevice", paras));
        }
        public void WINAPI_ReadUserCardNum(out string UserCardNum)
        {
            UserCardNum = "";
            object[] paras = new object[] { UserCardNum };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("WINAPI_ReadUserCardNum", paras));
        }
        public void WINAPI_ReadParamPresetCardNum(out string ParamPresetCardNum)
        {
            ParamPresetCardNum = "";
            object[] paras = new object[] { ParamPresetCardNum };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("WINAPI_ReadParamPresetCardNum", paras));
        }
        public void WINAPI_CloseDevice()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("WINAPI_CloseDevice", paras));
        }
        public void WINAPI_ReadParamPresetCard(out string fileParam, out string fileMoney, out string filePrice1, out string filePrice2, out string cardNum)
        {
            fileParam = "";
            fileMoney = "";
            filePrice1 = "";
            filePrice2 = "";
            cardNum = "";

            object[] paras = new object[] { fileParam, fileMoney, filePrice1, filePrice2, cardNum };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("WINAPI_ReadParamPresetCard", paras));
        }
        public void WINAPI_MakeParamPresetCard(string fileParam, string fileMoney, string filePrice1, string filePrice2)
        {
            object[] paras = new object[] { fileParam, fileMoney, filePrice1, filePrice2 };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("WINAPI_MakeParamPresetCard", paras));
        }
        public void WINAPI_ReadUserCard(out string fileParam, out string fileMoney, out string filePrice1, out string filePrice2, out string fileReply, out string enfileControl, out string cardNum)
        {
            fileParam = "";
            fileMoney = "";
            filePrice1 = "";
            filePrice2 = "";
            fileReply = "";
            enfileControl = "";
            cardNum = "";

            object[] paras = new object[] { fileParam, fileMoney, filePrice1, filePrice2, fileReply, enfileControl, cardNum };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("WINAPI_ReadUserCard", paras));
        }
        public void WINAPI_MakeUserCard(string fileParam, string fileMoney, string filePrice1, string filePrice2, string fileControl)
        {

            object[] paras = new object[] { fileParam, fileMoney, filePrice1, filePrice2, fileControl };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("WINAPI_MakeUserCard", paras));
        }
        #endregion

        #region 2050
        public void StartHarmonious(string strSean)
        {
            object[] paras = null;
            int sean = 0;
            if (int.TryParse(strSean, out sean))
            {
                string invokeName = "StopHarmonious";
                if (sean == 1)
                {
                    invokeName = "StartEvenHarmonious";
                }
                else if (sean == 2)
                {
                    invokeName = "StartOddHarmonious";
                }
                else if (sean == 3)
                {
                    invokeName = "StartNextHarmonious";
                }
                WcfHelper.Instance.DeviceControl(invokeName, paras);
            }
        }
        public void CheckExceptionState()
        {
            int extmp = 0;
            object[] paras = new object[] { extmp };
            WcfHelper.Instance.DeviceControl("CheckExceptionState", paras);
        }
        #endregion

        #region 翻转机械杆
        public void ReversalMachineryPole(string CtrFlag, string CtrAngularVelocity, string CtrTravel)
        {
            int intCtrFlag = 0;
            ushort ustCtrAngularVelocity = 0;
            int intCtrTravel = 0;
            if (int.TryParse(CtrFlag, out intCtrFlag) && ushort.TryParse(CtrAngularVelocity, out ustCtrAngularVelocity) && int.TryParse(CtrTravel, out intCtrTravel))
            {
                object[] paras = new object[] { intCtrFlag, ustCtrAngularVelocity, intCtrTravel };
                WcfHelper.Instance.DeviceControl("ReversalMachineryPole", paras);
            }
        }
        #endregion

        #region 反转电机
        public void EquipmentReversalA(string strPress)
        {
            bool isPress = bool.Parse(strPress);
            bool[] results = new bool[MeterUnits.Count];
            string[] resultDescriptions = new string[MeterUnits.Count];
            object[] paras = new object[] { isPress, string.Join("|", results), string.Join("|", resultDescriptions) };
            WcfHelper.Instance.DeviceControl("EquipmentReversalA", paras);
        }
        #endregion

        #region 远程上电
        public void RemoteControlOnOrOff(string strPress)
        {
            bool isPress = bool.Parse(strPress);
            object[] paras = new object[] { isPress };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("RemoteControlOnOrOff", paras));
        }
        #endregion

        #region 南网统一接口
        /// <summary>
        /// 检定装置参数配置界面
        /// </summary>
        public void ShowDriverConfig()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("ShowDriverConfig", paras));

        }

        /// <summary>
        /// 读写卡器参数配置
        /// </summary>
        public void ShowCardReaderConfig()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("ShowCardReaderConfig", paras));
        }
        #endregion

        #region 南网一类参数操作
        /// <summary>
        /// 读报警金额1
        /// </summary>
        public void ReadAlertingMoney1()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("ReadAlertingMoney1", paras));
        }

        /// <summary>
        /// 设置报警金额1
        /// </summary>
        /// <param name="AlertingMoney1"></param>
        public void SetAlertingMoney1(string AlertingMoney1)
        {
            object[] paras = new object[] { AlertingMoney1 };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("SetAlertingMoney1", paras));
        }

        /// <summary>
        /// 读报警金额2
        /// </summary>
        public void ReadAlertingMoney2()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("ReadAlertingMoney2", paras));
        }

        /// <summary>
        /// 设置报警金额2
        /// </summary>
        /// <param name="AlertingMoney2"></param>
        public void SetAlertingMoney2(string AlertingMoney2)
        {
            object[] paras = new object[] { AlertingMoney2 };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("SetAlertingMoney2", paras));
        }

        /// <summary>
        /// 读电流互感器变比
        /// </summary>
        public void ReadCurrentScale()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("ReadCurrentScale", paras));
        }

        /// <summary>
        /// 设置电流互感器变比
        /// </summary>
        /// <param name="CurrentScale"></param>
        public void SetCurrentScale(string CurrentScale)
        {
            object[] paras = new object[] { CurrentScale };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("SetCurrentScale", paras));
        }

        /// <summary>
        /// 读电压互感器变比
        /// </summary>
        public void ReadVoltageScale()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("ReadVoltageScale", paras));
        }

        /// <summary>
        /// 设置电压互感器变比
        /// </summary>
        /// <param name="AlertingMoney2"></param>
        public void SetVoltageScale(string VoltageScale)
        {
            object[] paras = new object[] { VoltageScale };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("SetVoltageScale", paras));
        }

        /// <summary>
        /// 读身份认证时效
        /// </summary>
        public void ReadIdentityTime()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("ReadIdentityTime", paras));
        }

        /// <summary>
        /// 设置身份认证时效
        /// </summary>
        /// <param name="AlertingMoney2"></param>
        public void SetIdentityTime(string IdentityTime)
        {
            object[] paras = new object[] { IdentityTime };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("SetIdentityTime", paras));
        }

        /// <summary>
        /// 读费率
        /// </summary>
        public void ReaderFl()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("ReaderFl", paras));
        }

        /// <summary>
        /// 设置费率
        /// </summary>
        /// <param name="strPara"></param>
        public void SetFl(string strPara)
        {
            object[] paras = new object[] { strPara };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("SetFl", paras));
        }

        /// <summary>
        /// 读第1阶梯表
        /// </summary>
        public void ReaderJtBy1()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("ReaderJtBy1", paras));
        }

        /// <summary>
        /// 设置第1阶梯表
        /// </summary>
        /// <param name="strPara"></param>
        public void SetJtBy1(string strPara)
        {
            object[] paras = new object[] { strPara };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("SetJtBy1", paras));
        }

        /// <summary>
        /// 读第2阶梯表
        /// </summary>
        public void ReaderJtBy2()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("ReaderJtBy2", paras));
        }

        /// <summary>
        /// 设置第2阶梯表
        /// </summary>
        /// <param name="strPara"></param>
        public void SetJtBy2(string strPara)
        {
            object[] paras = new object[] { strPara };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("SetJtBy2", paras));
        }

        public void SetInitPurse(string strMoney)
        {
            object[] paras = new object[] { strMoney };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("SetInitPurse", paras));
        }

        public void ReaderChangFlTime()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("ReaderChangFlTime", paras));
        }

        public void SetChangFlTime(string ChangTime)
        {
            object[] paras = new object[] { ChangTime };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("SetChangFlTime", paras));
        }

        public void ReaderChangJtTime()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("ReaderChangJtTime", paras));
        }

        public void SetChangJtTime(string ChangTime)
        {
            object[] paras = new object[] { ChangTime };
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("SetChangJtTime", paras));
        }

        #endregion

        #region 快捷命令
        public void Timing()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("Timing", paras));
        }

        public void BreakRelay()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("BreakRelay", paras));
        }

        public void DirectRemoteControl()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("DirectRemoteControl", paras));
        }

        public void CloseRelay()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("CloseRelay", paras));
        }

        public void EnPower()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("EnPower", paras));
        }

        public void RelieveEnPower()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("RelieveEnPower", paras));
        }

        public void ChangeLocalMode()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("ChangeLocalMode", paras));
        }

        public void ChangeRemoteModel()
        {
            object[] paras = null;
            TaskManager.AddWcfAction(() => WcfHelper.Instance.DeviceControl("ChangeRemoteModel", paras));
        }
        #endregion

        #endregion
    }
}
