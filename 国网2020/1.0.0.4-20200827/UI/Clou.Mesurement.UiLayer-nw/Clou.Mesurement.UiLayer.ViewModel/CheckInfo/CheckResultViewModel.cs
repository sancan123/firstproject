using System.Collections.Generic;
using Mesurement.UiLayer.ViewModel.Model;
using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.Utility.Log;
using System.Threading.Tasks;
using Mesurement.UiLayer.Utility;
using Mesurement.UiLayer.ViewModel.Time;
using Mesurement.UiLayer.ViewModel.Schema;
using System.Linq;
using System;

namespace Mesurement.UiLayer.ViewModel.CheckInfo
{
    /// <summary>
    /// 检定结论视图模型
    /// </summary>
    public class CheckResultViewModel : ViewModelBase
    {
        public CheckResultViewModel()
        {
            DetailResultView.Clear();
            for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
            {
                DetailResultView.Add(new DynamicViewModel(i + 1));
            }
        }

        private CheckNodeViewModel checkNodeCurrent;
        /// <summary>
        /// 当前选中的检定点
        /// </summary>
        public CheckNodeViewModel CheckNodeCurrent
        {
            get { return checkNodeCurrent; }
            set
            {
                bool flagTemp = false;
                if (checkNodeCurrent == null || checkNodeCurrent.ParaNo != value.ParaNo)
                {
                    flagTemp = true;
                }
                FlagLoadColumn = true;
                SetPropertyValue(value, ref checkNodeCurrent, "CheckNodeCurrent");
                if (flagTemp)
                {
                    LoadViewColumn();
                }
                else
                {
                    RefreshDetailResult();
                }
            }
        }

        private AsyncObservableCollection<CheckNodeViewModel> resultCollection = new AsyncObservableCollection<CheckNodeViewModel>();
        /// <summary>
        /// 检定结论集合
        /// </summary>
        public AsyncObservableCollection<CheckNodeViewModel> ResultCollection
        {
            get { return resultCollection; }
            set { resultCollection = value; }
        }
        #region 初始化检定结论
        /// 初始化检定结论
        /// <summary>
        /// 初始化检定结论
        /// </summary>
        /// <param name="schemaId">方案编号</param>
        public void InitialResult()
        {
            ResultCollection.Clear();
            Categories.Clear();

            for (int i = 0; i < EquipmentData.Schema.Children.Count; i++)
            {
                Categories.Add(GetResultNode(EquipmentData.Schema.Children[i]));
            }
            for (int i = 0; i < Categories.Count; i++)
            {
                for (int j = 0; j < Categories[i].Children.Count; j++)
                {
                    Categories[i].Children[j].CompressNode();
                }
            }
            #region 对第二层的节点,如果只有一个子元素,取消第二层节点
            #endregion

            #region 加载检定结论
            TaskManager.AddDataBaseAction(() =>
            {
                for (int i = 0; i < ResultCollection.Count; i++)
                {
                    CheckResultBll.Instance.LoadCheckResult(ResultCollection[i]);
                    ResultCollection[i].RefreshResultSummary();
                }
                for (int i = 0; i < Categories.Count; i++)
                {
                    UpdateResultSummaryDown(Categories[i]);
                }
                //初始化时间统计
                TimeMonitor.Instance.Initialize();
            });
            #endregion
        }

        /// <summary>
        /// 初始化方案节点对应的结论节点
        /// </summary>
        /// <param name="schemaNode"></param>
        /// <returns></returns>
        public CheckNodeViewModel GetResultNode(SchemaNodeViewModel schemaNode)
        {
            #region 方案相关信息
            CheckNodeViewModel categoryModel = new CheckNodeViewModel
            {
                IsSelected = schemaNode.IsSelected,
                Name = schemaNode.Name,
                ParaNo = schemaNode.ParaNo,
                Level = schemaNode.Level
            };
            #endregion
            #region 如果为根节点则加载所有表位的详细信息
            for (int i = 0; i < schemaNode.ParaValuesCurrent.Count; i++)
            {
                CheckNodeViewModel itemModel = new CheckNodeViewModel
                {
                    IsSelected = schemaNode.IsSelected,
                    Name = schemaNode.ParaValuesCurrent[i].GetProperty("PARA_NAME") as string,
                    ParaNo = schemaNode.ParaValuesCurrent[i].GetProperty("PARA_NO") as string,
                    ItemKey = schemaNode.ParaValuesCurrent[i].GetProperty("PARA_KEY") as string,
                    Level = schemaNode.Level + 1
                };
                //初始化详细结论
                itemModel.InitializeCheckResults();
                //设置父节点
                itemModel.Parent = categoryModel;
                //添加到总结论集合,方便使用
                ResultCollection.Add(itemModel);
                categoryModel.Children.Add(itemModel);
            }
            #endregion
            #region 对子节点递归
            for (int i = 0; i < schemaNode.Children.Count; i++)
            {
                CheckNodeViewModel nodeChild = GetResultNode(schemaNode.Children[i]);
                nodeChild.Parent = categoryModel;
                categoryModel.Children.Add(nodeChild);
            }
            #endregion
            return categoryModel;
        }
        #endregion
        /// <summary>
        /// 清除当前点的检定结论
        /// </summary>
        public void ResetCurrentResult()
        {
            if (ResultCollection.Count <= EquipmentData.Controller.Index || EquipmentData.Controller.Index < 0)
            {
                return;
            }

            bool[] yaojianTemp = EquipmentData.MeterGroupInfo.YaoJian;
            List<string> listNames = ResultCollection[EquipmentData.Controller.Index].CheckResults[0].GetAllProperyName();
            #region 更新详细结论
            for (int i = 0; i < CheckNodeCurrent.CheckResults.Count; i++)
            {
                //只更新要检表的结论
                if (yaojianTemp[i])
                {
                    for (int j = 0; j < listNames.Count; j++)
                    {
                        if (listNames[j] != "要检")
                        {
                            CheckNodeCurrent.CheckResults[i].SetProperty(listNames[j], "");
                        }
                    }
                }
            }
            #endregion
            RefreshDetailResult();
            CheckNodeCurrent.RefreshResultSummary();
            UpdateResultSummaryUp(CheckNodeCurrent);
        }
        /// <summary>
        /// 清除所有的结论
        /// </summary>
        public void ClearAllResult()
        {
            bool[] yaojianTemp = EquipmentData.MeterGroupInfo.YaoJian;
            for (int j = 0; j < ResultCollection.Count; j++)
            {
                List<string> listNames = ResultCollection[j].CheckResults[0].GetAllProperyName();
                #region 更新详细结论
                for (int i = 0; i < ResultCollection[j].CheckResults.Count; i++)
                {
                    //只更新要检表的结论
                    if (yaojianTemp[i])
                    {
                        for (int k = 0; k < listNames.Count; k++)
                        {
                            if (listNames[k] != "要检")
                            {
                                ResultCollection[j].CheckResults[i].SetProperty(listNames[k], "");
                            }
                        }
                    }
                }
                #endregion
                ResultCollection[j].RefreshResultSummary();
            }
            for (int i = 0; i < Categories.Count; i++)
            {
                UpdateResultSummaryDown(Categories[i]);
            }
        }
        /// <summary>
        /// 更新检定结论
        /// </summary>
        /// <param name="itemKey"></param>
        /// <param name="columnName"></param>
        /// <param name="arrayResult"></param>
        public void UpdateCheckResult(string itemKey, string columnName, string[] arrayResult)
        {
            bool[] yaoJianTemp = EquipmentData.MeterGroupInfo.YaoJian;

            

            #region 更新表号
            List<string> sqlList1 = new List<string>();
            if (columnName == "表号")
            {
                for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
                {
                    if (yaoJianTemp[i] && !string.IsNullOrEmpty(arrayResult[i]))
                    {
                        EquipmentData.MeterGroupInfo.Meters[i].SetProperty("AVR_OTHER_1", arrayResult[i]);
                        sqlList1.Add(string.Format("update tmp_meter_info set AVR_OTHER_1 ='{0}' where PK_LNG_METER_ID  = '{1}'", arrayResult[i], EquipmentData.MeterGroupInfo.Meters[i].GetProperty("PK_LNG_METER_ID")));
                    }
                }
                int countTemp = DALManager.MeterTempDbDal.ExecuteOperation(sqlList1);
                LogManager.AddMessage(string.Format("更新数据库中表号信息完成,共更新{0}条", countTemp), EnumLogSource.数据库存取日志);
                return;
            }
            #endregion

            //是否传来的数据信息为表地址数据,如果是则更新表地址
            //08005:通信测试,08001:自动探测表地址
            bool flagUpdateMeterAddress = (itemKey == "00002" ) && columnName == "检定数据";

            CheckNodeViewModel nodeTemp = GetResultNode(itemKey);
            if (nodeTemp == null || nodeTemp.CheckResults.Count < 0)
            {
                LogManager.AddMessage(string.Format("未找到检定点编号{0}对应的检定点编号", itemKey), EnumLogSource.检定业务日志, EnumLevel.Warning);
                return;
            }
            List<string> listNames = nodeTemp.CheckResults[0].GetAllProperyName();
            if (!listNames.Contains(columnName))
            {
                LogManager.AddMessage(string.Format("不识别的检定结论:{0}", columnName), EnumLogSource.检定业务日志, EnumLevel.Warning);
                return;
            }
            #region 更新详细结论
            for (int i = 0; i < nodeTemp.CheckResults.Count; i++)
            {
                if (arrayResult.Length > i)
                {
                    //只更新要检表的结论
                    if (yaoJianTemp[i])
                    {
                        nodeTemp.CheckResults[i].SetProperty(columnName, arrayResult[i]);
                        //如果需要更新表地址,并且地址不为空
                        if (flagUpdateMeterAddress && !string.IsNullOrEmpty(arrayResult[i]))
                        {
                            EquipmentData.MeterGroupInfo.Meters[i].SetProperty("AVR_ADDRESS", arrayResult[i]);
                        }
                    }
                }
            }
            if (columnName == "结论")
            {
                nodeTemp.RefreshResultSummary();
                UpdateResultSummaryUp(nodeTemp);
                CheckResultBll.Instance.SaveCheckResult(nodeTemp);

                int indexTemp = ResultCollection.IndexOf(nodeTemp);
                TimeMonitor.Instance.ActiveCurrentItem(indexTemp, true);

                #region 更新总结论

                List<string> sqlSumRetList = new List<string>();
                string[] strResult = new string[] { "合格" };

                for (int i = 0; i < nodeTemp.CheckResults.Count; i++)
                {
                    if (arrayResult.Length > i)
                    {
                        //只更新要检表的结论
                        if (yaoJianTemp[i])
                        {
                            if (!string.IsNullOrEmpty(arrayResult[i]))
                            {
                                string meterId = EquipmentData.MeterGroupInfo.Meters[i].GetProperty("PK_LNG_METER_ID") as string;
                                string meterSumResult = EquipmentData.MeterGroupInfo.Meters[i].GetProperty("AVR_TOTAL_CONCLUSION") as string;
                                string strSQL = string.Format("FK_LNG_METER_ID  = '{0}'", meterId);
                                List<DynamicModel> models = DALManager.MeterTempDbDal.GetList("TMP_METER_RATES_CONTROL", strSQL);

                                if (models.Count > 0)
                                {
                                    strResult = new string[models.Count];
                                    for (int j = 0; j < models.Count; j++)
                                    {
                                        strResult[j] = models[j].GetProperty("AVR_CONCLUSION") as string;
                                    }
                                }

                                if (Array.IndexOf(strResult, "不合格") > -1)
                                {
                                    sqlSumRetList.Add(string.Format("update tmp_meter_info set AVR_TOTAL_CONCLUSION ='{0}' where PK_LNG_METER_ID  = '{1}'", "不合格", meterId));
                                    EquipmentData.MeterGroupInfo.Meters[i].SetProperty("AVR_TOTAL_CONCLUSION", "不合格");

                                }
                                else
                                {
                                    sqlSumRetList.Add(string.Format("update tmp_meter_info set AVR_TOTAL_CONCLUSION ='{0}' where PK_LNG_METER_ID  = '{1}'", "合格", meterId));
                                    EquipmentData.MeterGroupInfo.Meters[i].SetProperty("AVR_TOTAL_CONCLUSION", "合格");
                                }
                            }
                        }
                    }
                }
                int countTemp = DALManager.MeterTempDbDal.ExecuteOperation(sqlSumRetList);
                LogManager.AddMessage(string.Format("更新数据库中总结论信息完成,共更新{0}条", countTemp), EnumLogSource.数据库存取日志);
            }
                #endregion

            #endregion

            RefreshDetailResult();
           
            #region 更新表地址
            List<string> sqlList = new List<string>();
            if (flagUpdateMeterAddress)
            {
                for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
                {
                    if (yaoJianTemp[i] && !string.IsNullOrEmpty(arrayResult[i]))
                    {
                        sqlList.Add(string.Format("update tmp_meter_info set AVR_ADDRESS ='{0}' where PK_LNG_METER_ID  = '{1}'", arrayResult[i], EquipmentData.MeterGroupInfo.Meters[i].GetProperty("PK_LNG_METER_ID")));
                    }
                }
                int countTemp = DALManager.MeterTempDbDal.ExecuteOperation(sqlList);
                LogManager.AddMessage(string.Format("更新数据库中表地址信息完成,共更新{0}条", countTemp), EnumLogSource.数据库存取日志);
            }
            #endregion
        }


        #region 获取检定数据
        public void GetDataValue(string AVR_PROJECT_NAME,out string[] arrayResult)
        {
            arrayResult = new string[EquipmentData.Equipment.MeterCount];
            string[] result = new string[EquipmentData.Equipment.MeterCount];
            bool[] yaoJianTemp = EquipmentData.MeterGroupInfo.YaoJian;
            string sql = "";
            for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
            {
                if (yaoJianTemp[i] )
                {
                    sql = string.Format("select AVR_VALUE from TMP_METER_COMMUNICATION where PK_LNG_METER_ID  = '{0}' and AVR_PROJECT_NAME = '{1}'", EquipmentData.MeterGroupInfo.Meters[i].GetProperty("PK_LNG_METER_ID"), AVR_PROJECT_NAME);
                    result[i] = DALManager.MeterTempDbDal.GetData(AVR_PROJECT_NAME);
                }
            }
            arrayResult = result;        
        }










        #endregion





        #region 刷新结论总览,从上往下和从下往上两个方法
        public void UpdateResultSummaryDown(CheckNodeViewModel nodeTop)
        {
            if (nodeTop.CheckResults.Count > 0)
            {
                return;
            }
            else
            {
                nodeTop.RefreshResultSummary();
                for (int i = 0; i < nodeTop.Children.Count; i++)
                {
                    nodeTop.Children[i].RefreshResultSummary();
                }
            }
        }
        public void UpdateResultSummaryUp(CheckNodeViewModel nodeBottom)
        {
            CheckNodeViewModel nodeParent = nodeBottom.Parent;
            while (nodeParent != null)
            {
                if (nodeParent != null)
                {
                    nodeParent.RefreshResultSummary();
                }
                nodeParent = nodeParent.Parent;
            }
        }
        #endregion

        private CheckNodeViewModel GetResultNode(string itemKey)
        {
            int indexTemp = EquipmentData.Controller.Index;
            CheckNodeViewModel nodeResult = null;
            for (; indexTemp >= 0; indexTemp--)
                if (indexTemp < ResultCollection.Count)
                {
                    CheckNodeViewModel nodeTemp = ResultCollection[indexTemp];
                    if (nodeTemp.ItemKey == itemKey && nodeTemp.IsSelected)
                    {
                        nodeResult = nodeTemp;
                        break;
                    }
                }
            return nodeResult;
        }
        /// <summary>
        /// 更新表位要检状态
        /// </summary>
        public void UpdateYaoJian()
        {
            for (int i = 0; i < ResultCollection.Count; i++)
            {
                UpdateYaoJian(i);
            }
        }

        /// <summary>
        /// 更新要检标记,序号从0开始
        /// </summary>
        /// <param name="meterIndex"></param>
        public void UpdateYaoJian(int meterIndex)
        {
            bool[] yaojianTemp = EquipmentData.MeterGroupInfo.YaoJian;
            for (int j = 0; j < ResultCollection.Count; j++)
            {
                if (ResultCollection.Count > j && ResultCollection[j].CheckResults.Count > meterIndex)
                {
                    ResultCollection[j].CheckResults[meterIndex].SetProperty("要检", yaojianTemp[meterIndex]);
                }
            }
        }

        private AsyncObservableCollection<CheckNodeViewModel> categories = new AsyncObservableCollection<CheckNodeViewModel>();
        /// <summary>
        /// 检定大类列表
        /// </summary>
        public AsyncObservableCollection<CheckNodeViewModel> Categories
        {
            get { return categories; }
            set { SetPropertyValue(value, ref categories, "Categories"); }
        }

        #region 更新主界面检定数据
        private AsyncObservableCollection<DynamicViewModel> detailResultView = new AsyncObservableCollection<DynamicViewModel>();
        /// <summary>
        /// 当前显示的检定点
        /// 将界面显示的检定点结论模型固定,不重新设置值,这样可以大大提高界面绑定数据的速度
        /// </summary>
        public AsyncObservableCollection<DynamicViewModel> DetailResultView
        {
            get { return detailResultView; }
            set { SetPropertyValue(value, ref detailResultView, "DetailResultView"); }
        }
        /// <summary>
        /// 更改显示结论的列
        /// </summary>
        private void LoadViewColumn()
        {
            for (int i = 0; i < DetailResultView.Count; i++)
            {
                List<string> propertyNames = detailResultView[i].GetAllProperyName();
                for (int j = 0; j < propertyNames.Count; j++)
                {
                    if (propertyNames[j] != "要检")
                    {
                        DetailResultView[i].RemoveProperty(propertyNames[j]);
                    }
                }
                if (CheckNodeCurrent.CheckResults.Count <= i)
                {
                    continue;
                }
                propertyNames = CheckNodeCurrent.CheckResults[i].GetAllProperyName();
                for (int j = 0; j < propertyNames.Count; j++)
                {
                    DetailResultView[i].SetProperty(propertyNames[j], CheckNodeCurrent.CheckResults[i].GetProperty(propertyNames[j]));
                }
            }
            FlagLoadColumn = false;
        }
        /// <summary>
        /// 更新当前显示的数据
        /// </summary>
        private void RefreshDetailResult()
        {
            Parallel.For(0, DetailResultView.Count, (i) =>
                {
                    try
                    {
                        List<string> propertyNames = CheckNodeCurrent.CheckResults[i].GetAllProperyName();
                        for (int j = 0; j < propertyNames.Count; j++)
                        {
                            detailResultView[i].SetProperty(propertyNames[j], CheckNodeCurrent.CheckResults[i].GetProperty(propertyNames[j]));
                        }
                    }
                    catch
                    {
                    }
                });
        }
        private bool flagLoadColumn;
        /// <summary>
        /// 列加载完毕标记
        /// 如果检定结论视图发生了变化,值会变成true,界面加载完毕以后值会变为false,该标记用于防止界面加载过于频繁影响速度
        /// </summary>
        public bool FlagLoadColumn
        {
            get { return flagLoadColumn; }
            set
            {
                SetPropertyValue(value, ref flagLoadColumn, "FlagLoadColumn");
            }
        }
        #endregion
        /// <summary>
        /// 更新详细结论中的要检标记
        /// </summary>
        public void RefreshYaojian()
        {
            bool[] yaojian = EquipmentData.MeterGroupInfo.YaoJian;
            for (int i = 0; i < ResultCollection.Count; i++)
            {
                for (int j = 0; j < EquipmentData.Equipment.MeterCount; j++)
                {
                    if (yaojian.Length > j)
                    {
                        ResultCollection[i].CheckResults[j].SetProperty("要检", yaojian[j]);
                    }
                }
            }
            //更新界面显示
            for (int j = 0; j < EquipmentData.Equipment.MeterCount; j++)
            {
                if (yaojian.Length > j)
                {
                    detailResultView[j].SetProperty("要检", yaojian[j]);
                }
            }
        }
    }
}
