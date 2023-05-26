using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mesurement.UiLayer.ViewModel.ErrorLimit
{
    /// <summary>
    /// 误差限数据模型
    /// </summary>
    public class ErrorLimitViewModel : ViewModelBase
    {
        #region 误差限相关的一些条件
        private SelectCollection<LimitNameModel> wcLimitNames = new SelectCollection<LimitNameModel>();
        /// <summary>
        /// 误差限名称列表
        /// </summary>
        public SelectCollection<LimitNameModel> WcLimitNames
        {
            get { return wcLimitNames; }
            set { SetPropertyValue(value, ref wcLimitNames, "WcLimitNames"); }
        }

        private string limitRule;
        /// <summary>
        /// 误差限参照规程
        /// </summary>
        public string LimitRule
        {
            get { return limitRule; }
            set { SetPropertyValue(value, ref limitRule, "LimitRule"); }
        }
        private string meterClass;
        /// <summary>
        /// 表等级
        /// </summary>
        public string MeterClass
        {
            get { return meterClass; }
            set { SetPropertyValue(value, ref meterClass, "MeterClass"); }
        }
        private string powerComponent;
        /// <summary>
        /// 功率元件
        /// </summary>
        public string PowerComponent
        {
            get { return powerComponent; }
            set { SetPropertyValue(value, ref powerComponent, "PowerComponent"); }
        }
        private string connectType;
        /// <summary>
        /// 接入方式:直接式还是互感式
        /// </summary>
        public string ConnectType
        {
            get { return connectType; }
            set { SetPropertyValue(value, ref connectType, "ConnectType"); }
        }
        #endregion
        /// <summary>
        /// 加载当前误差限
        /// </summary>
        public void LoadErrorLimit()
        {
            string temp = where;
            if(string.IsNullOrEmpty(temp))
            {
                return;
            }
            List<DynamicModel> models = DALManager.WcLimitDal.GetList("wclimit", temp);
            #region 解析
            Dictionary<string, string> dictionaryCurrent = CodeDictionary.GetLayer2("CurrentTimes");
            Dictionary<string, string> dictionaryFactor = CodeDictionary.GetLayer2("PowerFactor");
            foreach (string factor in dictionaryFactor.Keys)
            {
                DynamicViewModel viewModelTemp = ValuesLimit.FirstOrDefault(item => item.GetProperty("PowerFactor") as string == factor);
                if (viewModelTemp == null)
                {
                    continue;
                }
                foreach (string current in dictionaryCurrent.Keys)
                {
                    string valueFactor = CodeDictionary.GetValueLayer2("PowerFactor", factor).TrimStart('0');
                    string valueCurrent = CodeDictionary.GetValueLayer2("CurrentTimes", current).TrimStart('0');
                    DynamicModel modelTemp = models.FirstOrDefault(item => item.GetProperty("GlysID").ToString() == valueFactor && item.GetProperty("CurrentID").ToString() == valueCurrent);
                    string currentString = current.Replace('.', '_').Replace('(', '_').Replace(')', '_');
                    ErrorLimitCell cellTemp = new ErrorLimitCell();
                    if (modelTemp == null)
                    {
                        cellTemp.LimitValue = string.Format("+{0}|-{0}", MeterClass);
                        cellTemp.FlagNoValue = true;
                        cellTemp.ChangeFlag = false;
                    }
                    else
                    {
                        cellTemp.LimitValue = modelTemp.GetProperty("Limit") as string;
                        cellTemp.ChangeFlag = false;
                    }
                    viewModelTemp.SetProperty(currentString, cellTemp);
                }
            }
            #endregion
        }

        public ErrorLimitViewModel()
        {
            InitializeValuesLimit();
            List<DynamicModel> models = DALManager.WcLimitDal.GetList("WcLimitName");
            for (int i = 0; i < models.Count; i++)
            {
                WcLimitNames.ItemsSource.Add(new LimitNameModel()
                {
                    ID = models[i].GetProperty("ID").ToString(),
                    Name = models[i].GetProperty("Name").ToString()
                });
            }
            WcLimitNames.SelectedItem = WcLimitNames.ItemsSource.FirstOrDefault(item => item.ID == "1");
        }

        private bool enableEdit = true;

        public bool EnableEdit
        {
            get { return enableEdit; }
            set { SetPropertyValue(value, ref enableEdit, "EnableEdit"); }
        }
        
        /// <summary>
        /// 初始化误差限
        /// </summary>
        private void InitializeValuesLimit()
        {
            valuesLimit.Clear();
            Dictionary<string, string> dictionaryCurrent = CodeDictionary.GetLayer2("CurrentTimes");
            Dictionary<string, string> dictionaryFactor = CodeDictionary.GetLayer2("PowerFactor");
            foreach (string factor in dictionaryFactor.Keys)
            {
                DynamicViewModel modelTemp = new DynamicViewModel(0);
                modelTemp.SetProperty("PowerFactor", factor);
                foreach (string current in dictionaryCurrent.Keys)
                {
                    ErrorLimitCell limitCell = new ErrorLimitCell();
                    modelTemp.SetProperty(current.Replace('.', '_').Replace('(', '_').Replace(')', '_'), limitCell);
                }
                ValuesLimit.Add(modelTemp);
            }
        }

        private AsyncObservableCollection<DynamicViewModel> valuesLimit = new AsyncObservableCollection<DynamicViewModel>();
        /// <summary>
        /// 误差限的值
        /// </summary>
        public AsyncObservableCollection<DynamicViewModel> ValuesLimit
        {
            get { return valuesLimit; }
            set { SetPropertyValue(value, ref valuesLimit, "ValuesLimit"); }
        }

        /// <summary>
        /// 查询条件
        /// </summary>
        private string where
        {
            get
            {
                string valueRule = CodeDictionary.GetValueLayer2("WcLimitRule", LimitRule);
                string valueClass = CodeDictionary.GetValueLayer2("MeterAccuracyLevel", MeterClass);
                string valueComponent = CodeDictionary.GetValueLayer2("PowerYuanJiang", PowerComponent);
                string valueConnectType = CodeDictionary.GetValueLayer2("CouplingMode", ConnectType);
                if(string.IsNullOrEmpty(valueRule)|| string.IsNullOrEmpty(valueClass) || string.IsNullOrEmpty(valueComponent) || string.IsNullOrEmpty(valueConnectType) ||WcLimitNames.SelectedItem==null)
                {
                    return "";
                }
                return string.Format("wclimitnameid={4} and guichengid={0} and meterlevelid={1} and yjid={2} and hgq={3}", valueRule, valueClass, valueComponent, valueConnectType == "0" ? 0 : -1, WcLimitNames.SelectedItem.ID);
            }
        }

        /// <summary>
        /// 保存误差限值
        /// </summary>
        public void SaveLimitValues()
        {
            List<string> updateList = new List<string>();
            List<string> insertList = new List<string>();
            #region 获取要更新和要插入的sql语句
            Dictionary<string, string> dictionaryCurrent = CodeDictionary.GetLayer2("CurrentTimes");
            Dictionary<string, string> dictionaryFactor = CodeDictionary.GetLayer2("PowerFactor");
            foreach (string factor in dictionaryFactor.Keys)
            {
                DynamicViewModel viewModelTemp = ValuesLimit.FirstOrDefault(item => item.GetProperty("PowerFactor") as string == factor);
                if (viewModelTemp == null)
                {
                    continue;
                }
                foreach (string current in dictionaryCurrent.Keys)
                {
                    string currentString = current.Replace('.', '_').Replace('(', '_').Replace(')', '_');
                    ErrorLimitCell cellTemp = viewModelTemp.GetProperty(currentString) as ErrorLimitCell;
                    if (cellTemp == null || (!cellTemp.ChangeFlag && !cellTemp.FlagNoValue))
                    {
                        continue;
                    }
                    string valueFactor = CodeDictionary.GetValueLayer2("PowerFactor", factor).TrimStart('0');
                    string valueCurrent = CodeDictionary.GetValueLayer2("CurrentTimes", current).TrimStart('0');
                    string whereTemp = string.Format("{0} and glysid={1} and currentid={2}", where, valueFactor, valueCurrent);
                    if (cellTemp.ChangeFlag)
                    {
                        updateList.Add(string.Format("update wclimit set limit = '{0}' where {1}", cellTemp.LimitValue, whereTemp));
                        cellTemp.ChangeFlag = false;
                    }
                    else if (cellTemp.FlagNoValue)
                    {
                        string valueRule = CodeDictionary.GetValueLayer2("WcLimitRule", LimitRule);
                        string valueClass = CodeDictionary.GetValueLayer2("MeterAccuracyLevel", MeterClass);
                        string valueComponent = CodeDictionary.GetValueLayer2("PowerYuanJiang", PowerComponent);
                        string valueConnectType = CodeDictionary.GetValueLayer2("CouplingMode", ConnectType);
                        string stringValues = string.Format("{7},{0},{1},{2},{3},{4},{5},-1,'{6}'", valueRule, valueClass, valueComponent, valueFactor, valueCurrent, valueConnectType == "0" ? 0 : -1, cellTemp.LimitValue, WcLimitNames.SelectedItem.ID);
                        insertList.Add(string.Format("insert into wclimit (wclimitnameid,GuiChengID,MeterLevelID,YjID,GlysID,CurrentID,Hgq,YouGong,Limit) values ({0})", stringValues));
                        cellTemp.FlagNoValue = false;
                    }
                }
            }
            #endregion
            int updateCount = DALManager.WcLimitDal.ExecuteOperation(updateList);
            int insertCount = DALManager.WcLimitDal.ExecuteOperation(insertList);
            if (updateCount + insertCount > 0)
            {
                LogManager.AddMessage(string.Format("误差限保存成功,更新的误差限数量:{0},插入的误差限数量:{1}", updateCount, insertCount), EnumLogSource.数据库存取日志, EnumLevel.Tip);
            }
        }

        public void DeleteLimitName(LimitNameModel model)
        {
            int nameDeleteCount = DALManager.WcLimitDal.Delete("wclimitname", string.Format("id={0}", model.ID));
            int limitDeleteCount = DALManager.WcLimitDal.Delete("wclimit", string.Format("wclimitnameid={0}", model.ID));
            WcLimitNames.ItemsSource.Remove(model);
            LogManager.AddMessage(string.Format("删除误差限名称数量:{0}条,删除误差限数量:{1}条.", nameDeleteCount, limitDeleteCount), EnumLogSource.数据库存取日志, EnumLevel.Tip);
        }
    }
    /// <summary>
    /// 误差限名称模型
    /// </summary>
    public class LimitNameModel : ViewModelBase
    {
        private string id;
        /// <summary>
        /// 误差限名称编号
        /// </summary>
        public string ID
        {
            get { return id; }
            set { SetPropertyValue(value, ref id, "ID"); }
        }
        private string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { SetPropertyValue(value, ref name, "Name"); }
        }
    }
}
