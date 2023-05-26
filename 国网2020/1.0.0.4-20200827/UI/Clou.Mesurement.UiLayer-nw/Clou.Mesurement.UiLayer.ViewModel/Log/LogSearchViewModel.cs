using System.Collections.Generic;
using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.ViewModel.Model;
using System;
using Mesurement.UiLayer.Utility.Log;
using System.Windows;
using Mesurement.UiLayer.Utility;

namespace Mesurement.UiLayer.ViewModel.Log
{
    /// 日志视图模型
    /// <summary>
    /// 日志视图模型
    /// </summary>
    public class LogSearchViewModel : ViewModelBase
    {
        public LogSearchViewModel()
        {
            LogSources.Add(new EnumSourceClass { LogSource = EnumLogSource.用户操作日志, IsSelected = false });
            LogSources.Add(new EnumSourceClass { LogSource = EnumLogSource.检定业务日志, IsSelected = false });
            LogSources.Add(new EnumSourceClass { LogSource = EnumLogSource.设备操作日志, IsSelected = false });
            LogSources.Add(new EnumSourceClass { LogSource = EnumLogSource.数据库存取日志, IsSelected = false });

            LogLevels.Add(new EnumLevelClass { LogLevel = EnumLevel.Information, IsSelected = false });
            LogLevels.Add(new EnumLevelClass { LogLevel = EnumLevel.Warning, IsSelected = false });
            LogLevels.Add(new EnumLevelClass { LogLevel = EnumLevel.Error, IsSelected = false });

            PagerModel.EventUpdateData += PagerModel_EventUpdateData;

            SearchLog();
        }

        void PagerModel_EventUpdateData(object sender, EventArgs e)
        {
            UpdateLog();
        }

        private DataPagerViewModel pagerModel = new DataPagerViewModel();

        public DataPagerViewModel PagerModel
        {
            get { return pagerModel; }
            set { SetPropertyValue(value, ref pagerModel, "PagerModel"); }
        }

        private bool sourceSelected;

        public bool SourceSelected
        {
            get { return sourceSelected; }
            set { SetPropertyValue(value, ref sourceSelected, "SourceSelected"); }
        }

        private bool levelSelected;

        public bool LevelSelected
        {
            get { return levelSelected; }
            set { SetPropertyValue(value, ref levelSelected, "LevelSelected"); }
        }

        private AsyncObservableCollection<EnumSourceClass> logSources = new AsyncObservableCollection<EnumSourceClass>();
        public AsyncObservableCollection<EnumSourceClass> LogSources
        {
            get { return logSources; }
            set { SetPropertyValue(value, ref logSources, "LogSources"); }
        }

        private AsyncObservableCollection<EnumLevelClass> logLevels = new AsyncObservableCollection<EnumLevelClass>();
        public AsyncObservableCollection<EnumLevelClass> LogLevels
        {
            get { return logLevels; }
            set { SetPropertyValue(value, ref logLevels, "LogLevels"); }
        }

        private AsyncObservableCollection<LogUnitViewModel> logModels = new AsyncObservableCollection<LogUnitViewModel>();

        public AsyncObservableCollection<LogUnitViewModel> LogModels
        {
            get { return logModels; }
            set { SetPropertyValue(value, ref logModels, "LogModels"); }
        }

        private bool timeSelected;

        public bool TimeSelected
        {
            get { return timeSelected; }
            set { SetPropertyValue(value, ref timeSelected, "TimeSelected"); }
        }
        private bool startSelected;

        public bool StartSelected
        {
            get { return startSelected; }
            set { SetPropertyValue(value, ref startSelected, "StartSelected"); }
        }

        private DateTime? timeStart;

        public DateTime? TimeStart
        {
            get { return timeStart; }
            set { SetPropertyValue(value, ref timeStart, "TimeStart"); }
        }
        private bool endSelected;

        public bool EndSelected
        {
            get { return endSelected; }
            set { SetPropertyValue(value, ref endSelected, "EndSelected"); }
        }

        private DateTime? timeEnd;

        public DateTime? TimeEnd
        {
            get { return timeEnd; }
            set { SetPropertyValue(value, ref timeEnd, "TimeEnd"); }
        }

        private bool textSelected;

        public bool TextSelected
        {
            get { return textSelected; }
            set { SetPropertyValue(value, ref textSelected, "TextSelected"); }
        }

        private string searchText;

        public string SearchText
        {
            get { return searchText; }
            set { SetPropertyValue(value, ref searchText, "SearchText"); }
        }

        private string where
        {
            get
            {
                List<string> conditionList = new List<string>();
                if (SourceSelected)
                {
                    for (int i = 0; i < LogSources.Count; i++)
                    {
                        if (LogSources[i].IsSelected)
                        {
                            conditionList.Add(string.Format("avr_source = '{0}'", (int)(LogSources[i].LogSource)));
                        }
                    }
                }
                if (LevelSelected)
                {
                    for (int i = 0; i < LogLevels.Count; i++)
                    {
                        if (LogLevels[i].IsSelected)
                        {
                            conditionList.Add(string.Format("avr_type = '{0}'", (int)(LogLevels[i].LogLevel)));
                        }
                    }
                }
                if (StartSelected)
                {
                    if (TimeStart != null)
                    {
                        conditionList.Add(string.Format("avr_write_time > '{0}'", TimeStart.Value.ToString("yyyy-MM-dd HH-mm-ss")));
                    }
                }
                if (EndSelected)
                {
                    if (TimeEnd != null)
                    {
                        conditionList.Add(string.Format("avr_write_time < '{0}'", TimeEnd.Value.ToString("yyyy-MM-dd HH-mm-ss")));
                    }
                }
                if (TextSelected)
                {
                    conditionList.Add(string.Format("avr_log like '%{0}%'", SearchText));
                }
                if (conditionList.Count > 0)
                {
                    return string.Join(" or ", conditionList);
                }
                else
                {
                    return "";
                }
            }
        }

        private void UpdateLog()
        {
            LogModels.Clear();
            List<DynamicModel> models = DALManager.LogDal.GetPage("RUN_LOG", "AVR_WRITE_TIME", PagerModel.PageSize, PagerModel.PageIndex, where, false);
            for (int i = 0; i < models.Count; i++)
            {
                EnumLogSource sourceTemp = EnumLogSource.用户操作日志;
                Enum.TryParse(models[i].GetProperty("AVR_SOURCE") as string, out sourceTemp);
                EnumLevel levelTemp = EnumLevel.Error;
                Enum.TryParse(models[i].GetProperty("AVR_TYPE") as string, out levelTemp);
                LogModel model = new LogModel(models[i].GetProperty("AVR_LOG") as string, sourceTemp, levelTemp);
                model.Time = models[i].GetProperty("AVR_WRITE_TIME") as string;

                LogModels.Add(new LogUnitViewModel(model));
            }
        }

        public void SearchLog()
        {
            PagerModel.Total = DALManager.LogDal.GetCount("RUN_LOG", where);
        }
        public class EnumSourceClass : ViewModelBase
        {
            private bool isSelected;

            public bool IsSelected
            {
                get { return isSelected; }
                set { SetPropertyValue(value, ref isSelected, "IsSelected"); }
            }

            private EnumLogSource logSource;

            public EnumLogSource LogSource
            {
                get { return logSource; }
                set { SetPropertyValue(value, ref logSource, "LogSource"); }
            }

        }
        public class EnumLevelClass : ViewModelBase
        {
            private bool isSelected;

            public bool IsSelected
            {
                get { return isSelected; }
                set { SetPropertyValue(value, ref isSelected, "IsSelected"); }
            }

            private EnumLevel logLevel;

            public EnumLevel LogLevel
            {
                get { return logLevel; }
                set { SetPropertyValue(value, ref logLevel, "LogLevel"); }
            }
        }

        protected override void Dispose(bool disposing)
        {
            LogSources.Clear();
            LogLevels.Clear();
            LogModels.Clear();
            LogSources = null;
            LogLevels = null;
            LogModels = null;
            PagerModel.EventUpdateData -= PagerModel_EventUpdateData;
            base.Dispose(disposing);
        }
        /// <summary>
        /// 删除十天前的报文
        /// </summary>
        public void DeleteLog()
        {
            if (MessageBox.Show("确认要删除10天以前的日志吗?", "删除日志", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                TaskManager.AddDataBaseAction(() =>
                {
                    LogManager.AddMessage("开始删除最近十天的软件运行日志,该操作可能需要较长时间,请等待!!!", EnumLogSource.数据库存取日志);
                    string timeString = DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd hh:mm:ss");
                    int count = DALManager.LogDal.Delete("RUN_LOG", string.Format("avr_write_time < '{0}'", timeString));
                    LogManager.AddMessage(string.Format("删除日志操作成功,共删除了{0}条日志.", count), EnumLogSource.数据库存取日志, EnumLevel.Tip);
                    SearchLog();
                });
            }
        }
    }
}
