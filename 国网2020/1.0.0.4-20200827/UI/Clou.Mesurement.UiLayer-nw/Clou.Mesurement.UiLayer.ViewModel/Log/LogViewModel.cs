using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.Utility;
using Mesurement.UiLayer.ViewModel.Model;
using System;
using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.DAL.Config;

namespace Mesurement.UiLayer.ViewModel.Log
{
    /// 日志视图模型
    /// <summary>
    /// 日志视图模型
    /// </summary>
    public class LogViewModel : ViewModelBase
    {
        private static LogViewModel instance;
        public static LogViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LogViewModel();
                }
                return instance;
            }
        }

        public void Initialize()
        { }

        /// 添加新的日志数据
        /// <summary>
        /// 添加新的日志数据
        /// </summary>
        /// <param name="model"></param>
        public void AddLogModel(LogModel model)
        {
            switch (model.Level)
            {
                case EnumLevel.ErrorSpeech:
                case EnumLevel.Error:
                case EnumLevel.Tip:
                    TipMessage = model.Message;
                    break;
            }
            switch (model.Level)
            {
                case EnumLevel.ErrorSpeech:
                case EnumLevel.InformationSpeech:
                case EnumLevel.WarningSpeech:
                    if (ConfigHelper.Instance.OpenVoice)
                    {
                        SpeechClass.Instance.Speak(model);
                    }
                    int temp = (int)model.Level;
                    model.Level = (EnumLevel)(temp - 90);
                    break;
            }
            switch (model.LogSource)
            {
                case EnumLogSource.检定业务日志:
                    if (EquipmentData.Controller.NewArrived)
                    {
                        EquipmentData.Controller.NewArrived = false;
                    }
                    EquipmentData.Controller.NewArrived = true;
                    break;
            }
            LogsCheckLogic.Add(new LogUnitViewModel(model));
            SaveLog(model);
        }

        //用户操作日志
        //数据库存取日志
        //业务逻辑日志
        //检定业务日志
        //设备操作日志
        private LogCollection logsUserOperation = new LogCollection();
        /// 用户日志
        /// <summary>
        /// 用户日志
        /// </summary>
        public LogCollection LogsUserOperation
        {
            get { return logsUserOperation; }
            set { SetPropertyValue(value, ref logsUserOperation, "LogsUserOperation"); }
        }

        private LogCollection logsDatabase = new LogCollection();
        /// 数据库存取日志
        /// <summary>
        /// 数据库存取日志
        /// </summary>
        public LogCollection LogsDatabase
        {
            get { return logsDatabase; }
            set { SetPropertyValue(value, ref logsDatabase, "LogsDatabase"); }
        }

        private LogCollection logsCheckLogic = new LogCollection();
        /// 检定业务日志
        /// <summary>
        /// 检定业务日志
        /// </summary>
        public LogCollection LogsCheckLogic
        {
            get { return logsCheckLogic; }
            set { SetPropertyValue(value, ref logsCheckLogic, "LogsCheckLogic"); }
        }

        private LogCollection logsDevice = new LogCollection();
        /// 设备操作日志
        /// <summary>
        /// 设备操作日志
        /// </summary>
        public LogCollection LogsDevice
        {
            get { return logsDevice; }
            set { SetPropertyValue(value, ref logsDevice, "LogsDevice"); }
        }
        /// 日志的类,内部封装了一个定时器,如果日志的时间操作定时器的时间就会将日志移除
        /// <summary>
        /// 日志的类,内部封装了一个定时器,如果日志的时间操作定时器的时间就会将日志移除
        /// </summary>
        public class LogCollection : AsyncObservableCollection<LogUnitViewModel>
        {
            /// <summary>
            /// 当日志数量过大被删除时,会保留最近的日志数量
            /// </summary>
            private int minLogCount = 20;
            /// <summary>
            ///当日志数量过大时会执行自动删除
            /// </summary>
            private int maxLogCount = 200;
            System.Timers.Timer timer = new System.Timers.Timer(60000);
            /// <summary>
            /// 
            /// </summary>
            /// <param name="maxCount"></param>
            public LogCollection(int maxCount = 200)
            {
                maxLogCount = maxCount;
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }

            private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                if (Items != null && Items.Count > maxLogCount)
                {
                    while (Count > minLogCount)
                    {
                        RemoveAt(0);
                    }
                }
            }
        }

        private void SaveLog(LogModel model)
        {
            TaskManager.AddSaveLogAction(() =>
                {
                    switch (model.LogSource)
                    {
                        case EnumLogSource.用户操作日志:
                        case EnumLogSource.数据库存取日志:
                        case EnumLogSource.检定业务日志:
                        case EnumLogSource.设备操作日志:
                            DynamicModel modelTemp = new DynamicModel();
                            modelTemp.SetProperty("AVR_DEVICE_ID", EquipmentData.Equipment.ID);
                            modelTemp.SetProperty("AVR_TYPE", (int)model.Level);
                            modelTemp.SetProperty("AVR_LOG", model.Message.Replace("'", "_"));
                            modelTemp.SetProperty("AVR_WRITE_TIME", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            modelTemp.SetProperty("AVR_SOURCE", (int)model.LogSource);
                            DALManager.LogDal.Insert("RUN_LOG", modelTemp);
                            break;
                    }
                });
        }

        private string tipMessage;
        /// <summary>
        /// 提示信息
        /// </summary>
        public string TipMessage
        {
            get { return tipMessage; }
            set
            {
                tipMessage = value;
                OnPropertyChanged("TipMessage");
            }
        }
    }
}
