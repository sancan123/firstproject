using System.Collections.Generic;
using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.ViewModel.Model;
using System;
using Mesurement.UiLayer.Utility.Log;
using System.Windows;
using Mesurement.UiLayer.Utility;

namespace Mesurement.UiLayer.ViewModel.Log
{
    /// <summary>
    /// 报文日志
    /// </summary>
    public class FrameLogViewModel : ViewModelBase
    {
        public FrameLogViewModel()
        {
            InitializeModules();
            PagerModel.EventUpdateData += PagerModel_EventUpdateData;

            SearchLog();
        }

        #region 查询条件
        private SearchModule portNoModule;
        /// <summary>
        /// 端口号查询条件
        /// </summary>
        public SearchModule PortNoModule
        {
            get { return portNoModule; }
            set { SetPropertyValue(value, ref portNoModule, "PortNoModule"); }
        }

        private SearchModule deviceNameModule;
        /// <summary>
        /// 设备名查询条件
        /// </summary>
        public SearchModule DeviceNameModule
        {
            get { return deviceNameModule; }
            set { SetPropertyValue(value, ref deviceNameModule, "DeviceNameModule"); }
        }
        /// <summary>
        /// 初始化查询条件
        /// </summary>
        private void InitializeModules()
        {
            PortNoModule = new SearchModule { Header = Application.Current.FindResource("端口号") as string };
            for (int i = 0; i < 64; i++)
            {
                PortNoModule.ItemsSource.Add(new SearchModule.SearchCondition
                {
                    TextCondition = (i + 1).ToString()
                });
            }
            string[] arrayDevice = new string[] { "电能表", "标准表", "功率源", "误差板", "时基源", "多功能板" };
            DeviceNameModule = new SearchModule
            {
                Header = Application.Current.FindResource("设备名称") as string
            };
            for (int i = 0; i < arrayDevice.Length; i++)
            {
                DeviceNameModule.ItemsSource.Add(new SearchModule.SearchCondition
                {
                    TextCondition = arrayDevice[i]
                });
            }
        }

        private bool timeSelected;
        /// <summary>
        /// 是否添加时间条件
        /// </summary>
        public bool TimeSelected
        {
            get { return timeSelected; }
            set { SetPropertyValue(value, ref timeSelected, "TimeSelected"); }
        }
        private bool startSelected;
        /// <summary>
        /// 是否开始时间条件
        /// </summary>
        public bool StartSelected
        {
            get { return startSelected; }
            set { SetPropertyValue(value, ref startSelected, "StartSelected"); }
        }

        private DateTime? timeStart;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? TimeStart
        {
            get { return timeStart; }
            set { SetPropertyValue(value, ref timeStart, "TimeStart"); }
        }
        private bool endSelected;
        /// <summary>
        /// 是否添加结束时间
        /// </summary>
        public bool EndSelected
        {
            get { return endSelected; }
            set { SetPropertyValue(value, ref endSelected, "EndSelected"); }
        }

        private DateTime? timeEnd;
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? TimeEnd
        {
            get { return timeEnd; }
            set { SetPropertyValue(value, ref timeEnd, "TimeEnd"); }
        }

        private bool sendTextSelected;
        /// <summary>
        /// 是否发送报文内容过滤
        /// </summary>
        public bool SendTextSelected
        {
            get { return sendTextSelected; }
            set { SetPropertyValue(value, ref sendTextSelected, "SendTextSelected"); }
        }

        private bool receiveTextSelected;
        /// <summary>
        /// 是否接收报文内容过滤
        /// </summary>
        public bool ReceiveTextSelected
        {
            get { return receiveTextSelected; }
            set { SetPropertyValue(value, ref receiveTextSelected, "ReceiveTextSelected"); }
        }

        private string sendSearchText;
        /// <summary>
        /// 发送报文内容
        /// </summary>
        public string SendSearchText
        {
            get { return sendSearchText; }
            set { SetPropertyValue(value, ref sendSearchText, "SendSearchText"); }
        }

        private string receiveSearchText;
        /// <summary>
        /// 接收报文内容
        /// </summary>
        public string ReceiveSearchText
        {
            get { return receiveSearchText; }
            set { SetPropertyValue(value, ref receiveSearchText, "ReceiveSearchText"); }
        }
        #endregion

        #region 获取查询条件的Sql语句
        /// <summary>
        /// 获取总的查询条件
        /// </summary>
        private string where
        {
            get
            {
                List<string> conditionList = new List<string>() { GetPortNoCondition(), GetDeviceNameCondition(), GetTimeCondition(), GetTextCondition() };
                List<string> tempList = new List<string>();
                for (int i = 0; i < conditionList.Count; i++)
                {
                    if (!string.IsNullOrEmpty(conditionList[i]))
                    {
                        tempList.Add(string.Format("({0})",conditionList[i]));
                    }
                }
                return string.Join(" and ", tempList);
            }
        }
        /// <summary>
        /// 获取设备名称查询字符串
        /// </summary>
        /// <returns></returns>
        private string GetDeviceNameCondition()
        {
            List<string> conditionList = new List<string>();
            if (DeviceNameModule.IsSelected)
            {
                for (int i = 0; i < DeviceNameModule.ItemsSource.Count; i++)
                {
                    if (DeviceNameModule.ItemsSource[i].IsSelected)
                    {
                        switch (DeviceNameModule.ItemsSource[i].TextCondition)
                        {
                            //MeterProtocol
                            case "电能表":
                                conditionList.Add("chrEquipName like '%MeterProtocol%'");
                                break;
                            //CL311
                            case "标准表":
                                conditionList.Add("chrEquipName like '%CL311%'");
                                break;
                            //CL309,CL303,CL101,
                            case "功率源":
                                conditionList.Add("chrEquipName like '%CL309%' or chrEquipName like '%CL303%' or chrEquipName like '%CL101%' ");
                                break;
                            //CL188
                            case "误差板":
                                conditionList.Add("chrEquipName like '%CL188%'");
                                break;
                            //CL191
                            case "时基源":
                                conditionList.Add("chrEquipName like '%CL191%'");
                                break;
                            //2029
                            case "多功能板":
                                conditionList.Add("chrEquipName like '%2029%'");
                                break;
                        }
                    }
                }
            }
            return string.Join(" or ", conditionList);
        }
        /// <summary>
        /// 获取端口号查询条件
        /// </summary>
        /// <returns></returns>
        private string GetPortNoCondition()
        {
            List<string> conditionList = new List<string>();
            if (PortNoModule.IsSelected)
            {
                for (int i = 0; i < PortNoModule.ItemsSource.Count; i++)
                {
                    if (PortNoModule.ItemsSource[i].IsSelected)
                    {
                        conditionList.Add(string.Format("chrPortNo like '%[_]{0}'", PortNoModule.ItemsSource[i].TextCondition));
                    }
                }
            }
            return string.Join(" or ", conditionList);
        }
        /// <summary>
        /// 获取日期限制条件
        /// </summary>
        /// <returns></returns>
        private string GetTimeCondition()
        {
            if (!TimeSelected)
            {
                return "";
            }
            List<string> conditionList = new List<string>();
            if (StartSelected)
            {
                if (TimeStart != null)
                {
                    conditionList.Add(string.Format("chrSTime > '{0}'", TimeStart.Value.ToString("yyyy/MM/dd HH:mm:ss")));
                }
            }
            if (EndSelected)
            {
                if (TimeEnd != null)
                {
                    conditionList.Add(string.Format("chrSTime < '{0}'", TimeEnd.Value.ToString("yyyy/MM/dd HH:mm:ss")));
                }
            }
            return string.Join(" and ", conditionList);
        }
        /// <summary>
        /// 内容查询条件
        /// </summary>
        /// <returns></returns>
        private string GetTextCondition()
        {
            List<string> listCondition = new List<string>();
            if (ReceiveTextSelected)
            {
                listCondition.Add(string.Format("chrSFrame like '%{0}%'", ReceiveSearchText));
            }
            if (SendTextSelected)
            {
                listCondition.Add(string.Format("chrSFrame like '%{0}%'", SendSearchText));
            }
            return string.Join(" and ", listCondition);
        }
        #endregion

        #region 分页控件
        private DataPagerViewModel pagerModel = new DataPagerViewModel();
        /// <summary>
        /// 分页控件
        /// </summary>
        public DataPagerViewModel PagerModel
        {
            get { return pagerModel; }
            set { SetPropertyValue(value, ref pagerModel, "PagerModel"); }
        }

        void PagerModel_EventUpdateData(object sender, EventArgs e)
        {
            UpdateLog();
        }
        #endregion

        #region 查询和删除报文
        private AsyncObservableCollection<DynamicViewModel> frameModels = new AsyncObservableCollection<DynamicViewModel>();
        /// <summary>
        /// 查询到的报文
        /// </summary>
        public AsyncObservableCollection<DynamicViewModel> FrameModels
        {
            get { return frameModels; }
            set { SetPropertyValue(value, ref frameModels, "FrameModels"); }
        }
        /// <summary>
        /// 更新表格内容
        /// </summary>
        private void UpdateLog()
        {
            FrameModels.Clear();
            List<DynamicModel> models = DALManager.LogDal.GetPage("FrameDeviceLog", "chrSTime", PagerModel.PageSize, PagerModel.PageIndex, where, false);
            for (int i = 0; i < models.Count; i++)
            {
                DynamicViewModel modelTemp = new DynamicViewModel(models[i], i);
                modelTemp.SetProperty("PortNum", GetPortNum(models[i].GetProperty("chrPortNo") as string));
                modelTemp.SetProperty("DeviceName", GetEquipmentName(models[i].GetProperty("chrEquipName") as string));
                FrameModels.Add(modelTemp);
            }
        }
        /// <summary>
        /// 转换端口编号
        /// </summary>
        /// <param name="chrPortNo"></param>
        /// <returns></returns>
        private string GetPortNum(string chrPortNo)
        {
            if (string.IsNullOrEmpty(chrPortNo) || chrPortNo.Split('_').Length != 3)
            {
                return "其它";
            }
            else
            {
                return chrPortNo.Split('_')[2];
            }
        }
        /// <summary>
        /// 转换设备名称
        /// </summary>
        /// <param name="chrEquipName"></param>
        /// <returns></returns>
        private string GetEquipmentName(string chrEquipName)
        {
            string equipmentName = "其它";
            if (chrEquipName.IndexOf("MeterProtocol") >= 0)
            {
                equipmentName = "电能表";
            }
            else if (chrEquipName.IndexOf("CL311") >= 0)
            {
                equipmentName = "标准表";
            }
            else if (chrEquipName.IndexOf("CL309") >= 0 || chrEquipName.IndexOf("CL303") >= 0 || chrEquipName.IndexOf("CL101") >= 0)
            {
                equipmentName = "功率源";
            }
            else if (chrEquipName.IndexOf("CL188") >= 0)
            {
                equipmentName = "误差板";
            }
            else if (chrEquipName.IndexOf("CL191") >= 0)
            {
                equipmentName = "时基源";
            }
            else if (chrEquipName.IndexOf("2029") >= 0)
            {
                equipmentName = "多功能板";
            }
            return equipmentName;
        }
        /// <summary>
        /// 供外部调用,执行查询
        /// </summary>
        public void SearchLog()
        {
            PagerModel.Total = DALManager.LogDal.GetCount("FrameDeviceLog", where);
        }
        /// <summary>
        /// 删除台体设备报文(不删除电能表操作报文)
        /// </summary>
        public void DeleteLog()
        {
            if(MessageBox.Show("确认要删除台体设备报文吗?删除台体设备报文(不删除电能表操作报文)", "删除报文",MessageBoxButton.YesNo,MessageBoxImage.Warning)==MessageBoxResult.Yes)
            {
                TaskManager.AddDataBaseAction(() =>
                {
                    LogManager.AddMessage("开始删除设备报文,该操作可能需要较长时间,请等待!!!", EnumLogSource.数据库存取日志);
                    int count = DALManager.LogDal.Delete("FrameDeviceLog", "chrEquipName not like '%MeterProtocol%'");
                    LogManager.AddMessage(string.Format("删除报文操作成功,共删除了{0}条报文.", count), EnumLogSource.数据库存取日志,EnumLevel.Tip);
                    SearchLog();
                });
            }
        }
        #endregion

        /// <summary>
        /// 释放内容
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            FrameModels.Clear();
            PagerModel.EventUpdateData -= PagerModel_EventUpdateData;
            base.Dispose(disposing);
        }
    }
}
