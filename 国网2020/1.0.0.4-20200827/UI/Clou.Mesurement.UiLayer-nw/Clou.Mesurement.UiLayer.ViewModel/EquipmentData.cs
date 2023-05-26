using Mesurement.UiLayer.ViewModel.Schema;
using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.ViewModel.CheckInfo;
using Mesurement.UiLayer.ViewModel.CheckController;
using Mesurement.UiLayer.ViewModel.Device;
using System.Linq;
using Mesurement.UiLayer.ViewModel.Monitor;
using System.ComponentModel;
using Mesurement.UiLayer.ViewModel.InputPara;

namespace Mesurement.UiLayer.ViewModel
{
    /// 检定数据中心
    /// <summary>
    /// 检定数据中心
    /// </summary>
    public class EquipmentData
    {
        private static MeterInputParaViewModel meterGroupInfo;
        /// 表信息数据集合
        /// <summary>
        /// 表信息数据集合
        /// </summary>
        public static MeterInputParaViewModel MeterGroupInfo
        {
            get
            {
                if (meterGroupInfo == null)
                {
                    meterGroupInfo = new MeterInputParaViewModel(true);
                }
                return meterGroupInfo;
            }
            set
            {
                meterGroupInfo = value;
            }
        }
        private static SchemaOperationViewModel schemaModels;
        /// <summary>
        /// 方案列表
        /// </summary>
        public static SchemaOperationViewModel SchemaModels
        {
            get
            {
                if (schemaModels == null)
                {
                    schemaModels = new SchemaOperationViewModel();
                    SchemaModels.PropertyChanged += SchemaModels_PropertyChanged;
                }
                return schemaModels;
            }
        }
        /// 更改检定方案时的事件
        /// <summary>
        /// 更改检定方案时的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SchemaModels_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedSchema")
            {
                if(SchemaModels.SelectedSchema==null)
                {
                    return;
                }
                int schemaId = (int)SchemaModels.SelectedSchema.GetProperty("ID");
                if (Schema == null)
                {
                    schema = new SchemaViewModel(schemaId);
                }
                else
                {
                    Schema.LoadSchema(schemaId);
                }
                Equipment.TextLogin = "软件登录中,请等待:初始化检定结论信息...";
                CheckResults.InitialResult();
            }
        }


        private static SchemaViewModel schema;
        /// 检定方案
        /// <summary>
        /// 检定方案
        /// </summary>
        public static SchemaViewModel Schema
        {
            get
            {
                if(schema==null)
                {
                    schema = new SchemaViewModel();
                }
                return schema;
            }
        }
        private static CheckResultViewModel checkResults;
        /// 检定结论
        /// <summary>
        /// 检定结论
        /// </summary>
        public static CheckResultViewModel CheckResults
        {
            get
            {
                if (checkResults == null)
                {
                    checkResults = new CheckResultViewModel();
                }
                return checkResults;
            }
        }
        private static LastCheckInfoViewModel lastCheckInfo;
        /// 检定软件退出时的软件信息
        /// <summary>
        /// 检定软件退出时的软件信息
        /// </summary>
        public static LastCheckInfoViewModel LastCheckInfo
        {
            get
            {
                if (lastCheckInfo == null)
                {
                    lastCheckInfo = new LastCheckInfoViewModel();
                }
                return lastCheckInfo;
            }
        }

        private static EquipmentViewModel equipment;
        /// 台体信息
        /// <summary>
        /// 台体信息
        /// </summary>
        public static EquipmentViewModel Equipment
        {
            get
            {
                if (equipment == null)
                {
                    equipment = new EquipmentViewModel();
                }
                return equipment;
            }
        }

        private static StdInfoViewModel stdInfo;

        public static StdInfoViewModel StdInfo
        {
            get
            {
                if (stdInfo == null)
                {
                    stdInfo = new StdInfoViewModel();
                }
                return stdInfo;
            }
        }

        /// 初始化检定数据
        /// <summary>
        /// 初始化检定数据
        /// </summary>
        public static void Initialize()
        {
            Equipment.TextLogin = "软件登录中,请等待:正在加载表信息...";
            MeterGroupInfo.Initialize();
            Equipment.TextLogin = "软件登录中,请等待:正在加载设备信息...";
            DeviceManager.LoadDevices();
            //加载检定初始数据
            LastCheckInfo.LoadLastCheckInfo();
            Equipment.TextLogin = "软件登录中,请等待:正在加载方案信息...";
            //加载检定方案
            SchemaModels.SelectedSchema = SchemaModels.Schemas.FirstOrDefault(item => (int)item.GetProperty("ID") == LastCheckInfo.SchemaId);
            Controller.Index = LastCheckInfo.CheckIndex;
        }
        /// 导航到当前的默认界面
        /// <summary>
        /// 导航到当前的默认界面
        /// </summary>
        public static void NavigateCurrentUi()
        {
            //根据检定点序号来加载不同检定界面
            if (LastCheckInfo.CheckIndex == -1)     //参数录入
            {
                UiInterface.ChangeUi("参数录入", "ViewInputPara");
            }
            else if (LastCheckInfo.CheckIndex == -3)        //审核存盘
            {
            }
            else                    //检定界面
            {
                string errorString = "";
                if (MeterGroupInfo.CheckInfoCompleted(out errorString))
                {
                    UiInterface.ChangeUi("结论总览", "ViewResultSummary");
                    //UiInterface.ChangeUi("电能表报文记录", "ViewLiveMeterFrame");
                    UiInterface.ChangeUi("运行日志", "ViewLog");
                    //UiInterface.ChangeUi("串口服务器", "ViewComServer");
                    UiInterface.ChangeUi("标准表信息", "ViewStd");
                    //UiInterface.ChangeUi("误差板数据", "ViewErrorBoard");
                }
                else
                {
                    UiInterface.ChangeUi("参数录入", "ViewInputPara");
                    LogManager.AddMessage(errorString, EnumLogSource.用户操作日志, EnumLevel.Warning);
                }
            }
        }
        private static ControllerViewModel controller;

        public static ControllerViewModel Controller
        {
            get
            {
                if (controller == null)
                {
                    controller = new ControllerViewModel();
                }
                return controller;
            }
        }
        private static DeviceViewModel deviceManager;
        public static DeviceViewModel DeviceManager
        {
            get
            {
                if (deviceManager == null)
                {
                    deviceManager = new DeviceViewModel();
                }
                return deviceManager;
            }
        }
    }
}
