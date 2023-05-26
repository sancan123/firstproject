﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.ViewModel.Model;
using Mesurement.UiLayer.ViewModel.Schema.Error;
using Mesurement.UiLayer.Utility.Log;

namespace Mesurement.UiLayer.ViewModel.Schema
{

    /// 检定方案视图模型
    /// <summary>
    /// 检定方案视图模型
    /// </summary>
    public class SchemaViewModel : SchemaNodeViewModel
    {
        public SchemaViewModel()
        {
            PropertyChanged += (obj, e) =>
            {
                if (e.PropertyName == "ParaNo")
                {
                    ParaInfo.ParaNo = ParaNo;
                    ParaValuesConvert();
                }
            };
        }
        public SchemaViewModel(int idSchema)
        {
            LoadSchema(idSchema);
            PropertyChanged += (obj, e) =>
            {
                if (e.PropertyName == "ParaNo")
                {
                    ParaInfo.ParaNo = ParaNo;
                    ParaValuesConvert();
                }
            };
        }

        public void LoadSchema(int idSchema)
        {
            Children.Clear();
            if (SchemaId != idSchema)
            {
                SchemaId = idSchema;
            }
            DynamicModel model = DALManager.ApplicationDbDal.GetByID("schema_info", string.Format("id={0}", idSchema));
            if (model == null)
            {
                LogManager.AddMessage(string.Format("未能加载编号为 {0} 的方案.", idSchema), EnumLogSource.用户操作日志, EnumLevel.WarningSpeech);
                return;
            }
            Name = model.GetProperty("SCHEMA_NAME") as string;
            LoadParaValues();
            LogManager.AddMessage(string.Format("当前方案:{0}", Name), EnumLogSource.用户操作日志, EnumLevel.InformationSpeech);
        }
        private AsyncObservableCollection<DynamicViewModel> paraValues = new AsyncObservableCollection<DynamicViewModel>();
        /// 参数值
        /// <summary>
        /// 参数值
        /// </summary>
        public AsyncObservableCollection<DynamicViewModel> ParaValues
        {
            get { return paraValues; }
            set { SetPropertyValue(value, ref paraValues, "ParaValues"); }
        }

        private ParaInfoViewModel paraInfo = new ParaInfoViewModel();
        /// 检定点参数信息
        /// <summary>
        /// 检定点参数信息
        /// </summary>
        public ParaInfoViewModel ParaInfo
        {
            get { return paraInfo; }
            set { SetPropertyValue(value, ref paraInfo, "ParaInfo"); }
        }

        #region 参数值的配置
        private AsyncObservableCollection<DynamicViewModel> paraValuesView = new AsyncObservableCollection<DynamicViewModel>();
        /// 参数值
        /// <summary>
        /// 参数值
        /// </summary>
        public AsyncObservableCollection<DynamicViewModel> ParaValuesView
        {
            get { return paraValuesView; }
            set { SetPropertyValue(value, ref paraValuesView, "ParaValuesView"); }
        }
        /// 加载方案
        /// <summary>
        /// 加载方案
        /// </summary>
        public void LoadParaValues()
        {
            ParaValues.Clear();
            List<DynamicModel> models = DALManager.ApplicationDbDal.GetList(EnumAppDbTable.SCHEMA_PARA_VALUE.ToString(), string.Format("SCHEMA_ID={0} and (valid_flag <> '0' or valid_flag is null) order by para_index", SchemaId));
            for (int i = 0; i < models.Count; i++)
            {
                ParaValues.Add(new DynamicViewModel(models[i], 0));
            }
            InitialSchemaTree();
        }

        #region 增删改查操作
        /// 保存参数配置
        /// <summary>
        /// 保存参数配置
        /// </summary>
        public void SaveParaValue()
        {
            RefreshPointCount();
            if (SelectedNode != null && ParaNo == SelectedNode.ParaNo)
            {
                SelectedNode.ParaValuesCurrent = ParaValuesConvertBack();
            }
            List<SchemaNodeViewModel> nodesTerminal = GetTerminalNodes();
            List<DynamicModel> models = new List<DynamicModel>();
            for (int i = 0; i < nodesTerminal.Count; i++)
            {
                models.AddRange(nodesTerminal[i].ParaValuesCurrent);
            }
            for (int i = 0; i < models.Count; i++)
            {
                string PARA_NO = models[i].GetProperty("PARA_NO") as string;
                string PARA_INDEX = (i + 1).ToString("D3");
                models[i].SetProperty("PARA_INDEX", PARA_INDEX);
                models[i].SetProperty("SCHEMA_ID", SchemaId);
                if (PARA_NO == "14002" || PARA_NO == "04003")
                {
                    string dataFlag = models[i].GetProperty("PARA_VALUE").ToString().Split('|')[0];
                    models[i].SetProperty("PARA_KEY", PARA_NO + "_" + dataFlag);
                    //models[i].SetProperty("PARA_KEY", PARA_NO + "_" + PARA_INDEX);
                }
            }
            //for (int i = 0; i < models.Count; i++)   -old
            //{
            //    models[i].SetProperty("PARA_INDEX", (i + 1).ToString("D3"));
            //    models[i].SetProperty("SCHEMA_ID", SchemaId);
            //}
            int deleteCount = DALManager.ApplicationDbDal.Delete(EnumAppDbTable.SCHEMA_PARA_VALUE.ToString(), string.Format("schema_id={0}", SchemaId));
            LogManager.AddMessage(string.Format("删除方案{0}的所有检定点,共删除{1}条记录", Name, deleteCount));
            int insertCount = DALManager.ApplicationDbDal.Insert(EnumAppDbTable.SCHEMA_PARA_VALUE.ToString(), models);
            LoadParaValues();
            LogManager.AddMessage(string.Format("插入方案{0}的所有检定点,共插入{1}条记录", Name, insertCount), EnumLogSource.用户操作日志, EnumLevel.Tip);
        }

        public void AddNewParaValue()
        {
            List<string> propertyNames = new List<string>();
            for (int i = 0; i < ParaInfo.CheckParas.Count; i++)
            {
                propertyNames.Add(paraInfo.CheckParas[i].ParaDisplayName);
            }
            DynamicViewModel viewModel = new DynamicViewModel(propertyNames, 0);
            viewModel.SetProperty("IsSelected", true);
            for (int i = 0; i < propertyNames.Count; i++)
            {
                viewModel.SetProperty(propertyNames[i], ParaInfo.CheckParas[i].DefaultValue);
            }
            ParaValuesView.Add(viewModel);
        }
        #endregion
        #endregion

        #region 加载方案树
        /// <summary>
        /// 初始化方案树
        /// </summary>
        /// <param name="listParaNo"></param>
        public void InitialSchemaTree()
        {
            Children.Clear();
            for (int i = 0; i < ParaValues.Count; i++)
            {
                string noTemp = ParaValues[i].GetProperty("PARA_NO") as string;
                if (string.IsNullOrEmpty(noTemp))
                {
                    continue;
                }

                SchemaNodeViewModel nodeParent = GetLastNode(noTemp);
                //如果方案编号不符合规则:长度为2+3*i,丢弃这个点
                if (nodeParent == null)
                {
                    continue;
                }
                nodeParent.IsTerminal = true;
                nodeParent.ParaValuesCurrent.Add(ParaValues[i].GetDataSource());
            }
            RefreshPointCount();
        }
        private SchemaNodeViewModel GetLastNode(string noTemp)
        {
            //长度规则:2+3*i
            if (noTemp == null || noTemp.Length < 2 || ((noTemp.Length - 2) % 3 > 0))
            {
                return null;
            }
            List<string> noList = GetNoList(noTemp);
            #region 添加到现有节点或者获取现有节点
            SchemaNodeViewModel nodeCurrent = GetExistLastNode();
            int indexTemp = -1;
            #region 遍历每一层的编号
            while (nodeCurrent != null)
            {
                indexTemp = noList.IndexOf(nodeCurrent.ParaNo);
                if (indexTemp >= 0)
                {
                    break;
                }
                else
                {
                    nodeCurrent = nodeCurrent.ParentNode;
                }
            }
            #endregion
            #region 如果最后一个节点不存在当前的任何一个编号则创建一个新的顶层节点
            if (nodeCurrent == null)
            {
                nodeCurrent = new SchemaNodeViewModel
                {
                    ParaNo = noTemp.Substring(0, 2),
                    Name = SchemaFramework.GetItemName(noTemp.Substring(0, 2)),
                    Level = 1,
                };
                Children.Add(nodeCurrent);
                indexTemp = 0;
            }
            #endregion
            #region 在要插入的节点循环插入各层节点
            for (; indexTemp + 1 < noList.Count; indexTemp++)
            {
                SchemaNodeViewModel nodeNew = new SchemaNodeViewModel
                {
                    ParaNo = noList[indexTemp + 1],
                    Name = SchemaFramework.GetItemName(noList[indexTemp + 1]),
                    Level = indexTemp + 2,
                };
                nodeCurrent.Children.Add(nodeNew);
                nodeNew.ParentNode = nodeCurrent;
                nodeCurrent = nodeNew;
            }
            #endregion
            #endregion
            return nodeCurrent;
        }
        /// <summary>
        /// 获取当前的最后一个节点
        /// </summary>
        /// <returns></returns>
        private SchemaNodeViewModel GetExistLastNode()
        {
            if (Children.Count == 0)
            {
                return null;
            }
            SchemaNodeViewModel nodeRoot = Children[Children.Count - 1];
            while (nodeRoot.Children.Count > 0)
            {
                nodeRoot = nodeRoot.Children[nodeRoot.Children.Count - 1];
            }
            return nodeRoot;
        }
        /// <summary>
        /// 更新方案概览,每个节点检定点的数量
        /// </summary>
        public void RefreshPointCount()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                GetPointCountPerNode(Children[i]);
            }

            #region 刷新获取每一层的节点列表
            levelDictinary.Clear();
            int levelTemp = 1;
            while (true)
            {
                List<SchemaNodeViewModel> levelNodes = GetAllNodes(levelTemp);
                if (levelNodes.Count > 0)
                {
                    levelDictinary.Add(levelTemp, levelNodes);
                }
                else
                {
                    break;
                }
                levelTemp++;
            }
            #endregion
        }
        /// <summary>
        /// 递归获取每个分支上的子节点数量
        /// </summary>
        /// <param name="nodeTemp"></param>
        private int GetPointCountPerNode(SchemaNodeViewModel nodeTemp)
        {
            if (nodeTemp.IsTerminal)
            {
                nodeTemp.PointCount = nodeTemp.ParaValuesCurrent.Count;
            }
            else
            {
                int sumTemp = 0;
                for (int i = 0; i < nodeTemp.Children.Count; i++)
                {
                    sumTemp = sumTemp + GetPointCountPerNode(nodeTemp.Children[i]);
                }
                nodeTemp.PointCount = sumTemp;
            }

            //如果检定点数量为0,则将该节点删除
            if (nodeTemp.PointCount == 0)
            {
                if (nodeTemp.ParentNode == null)
                {
                    Children.Remove(nodeTemp);
                }
                else
                {
                    nodeTemp.ParentNode.Children.Remove(nodeTemp);
                }
            }

            return nodeTemp.PointCount;
        }
        /// <summary>
        /// 根据获取已经存在的对应的节点,如果不存在则创建一个节点并返回
        /// </summary>
        /// <param name="noTemp"></param>
        /// <returns></returns>
        private SchemaNodeViewModel GetFirstNode(string noTemp)
        {
            //长度规则:2+3*i
            if (noTemp == null || noTemp.Length < 2 || ((noTemp.Length - 2) % 3 > 0))
            {
                return null;
            }
            List<string> noList = GetNoList(noTemp);
            int levelTemp = 1;
            SchemaNodeViewModel nodeCurrent = null;
            #region 从根节点找起,一步步往上匹配
            List<SchemaNodeViewModel> nodes = null;
            int i = noList.Count - 1;
            for (; i >= 0; i--)
            {
                levelTemp = i + 1;
                if (levelDictinary.ContainsKey(levelTemp))
                {
                    nodes = levelDictinary[levelTemp];
                    int indexTemp = nodes.FindIndex(item => item.ParaNo == noList[i]);
                    if (indexTemp >= 0)
                    {
                        nodeCurrent = nodes[indexTemp];
                        break;
                    }
                }
            }
            #endregion
            #region 找到的顶层节点一步步往下插入
            if (nodeCurrent == null)
            {
                i = 0;
            }
            else
            {
                i = i + 1;
            }
            for (; i < noList.Count; i++)
            {
                nodeCurrent = InsertNodeInLevel(nodeCurrent, noList[i]);
            }
            #endregion
            if (nodeCurrent != null)
            {
                nodeCurrent.IsTerminal = true;
            }
            return nodeCurrent;
        }
        /// <summary>
        /// 是否存在检定点编号
        /// </summary>
        /// <param name="noTemp"></param>
        /// <returns></returns>
        public bool ExistNode(string noTemp)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (ExistNode(noTemp, Children[i]))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 是否存在编号
        /// </summary>
        /// <param name="noTemp"></param>
        /// <param name=""></param>
        /// <returns></returns>
        private bool ExistNode(string noTemp, SchemaNodeViewModel nodeTemp)
        {
            if (nodeTemp.ParaNo == noTemp)
            {
                return true;
            }
            for (int i = 0; i < nodeTemp.Children.Count; i++)
            {
                if (ExistNode(noTemp, nodeTemp.Children[i]))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 编号必须为数字
        /// </summary>
        /// <param name="levelTemp"></param>
        /// <param name="noTemp"></param>
        private SchemaNodeViewModel InsertNodeInLevel(SchemaNodeViewModel nodeParent, string noTemp)
        {
            SchemaNodeViewModel nodeNew = new SchemaNodeViewModel
            {
                ParaNo = noTemp,
                Name = SchemaFramework.GetItemName(noTemp)
            };
            string intNo = SchemaFramework.GetSortNo(noTemp);//int.Parse(noTemp);
            AsyncObservableCollection<SchemaNodeViewModel> nodes = Children;
            nodeNew.Level = 1;
            nodeNew.ParentNode = null;
            if (nodeParent != null)
            {
                nodes = nodeParent.Children;
                nodeNew.Level = nodeParent.Level + 1;
                nodeNew.ParentNode = nodeParent;
            }
            bool flagAdd = false;
            for (int i = 0; i < nodes.Count; i++)
            {//TODO:排序
                string intTemp = SchemaFramework.GetSortNo(nodes[i].ParaNo);
                if (intTemp == null) continue;
                if (intTemp.CompareTo(intNo) > 0)
                {
                    nodes.Insert(i, nodeNew);
                    flagAdd = true;
                    break;
                }
            }
            if (!flagAdd)
            {
                nodes.Add(nodeNew);
            }
            return nodeNew;
        }
        /// <summary>
        /// 每一层节点的列表
        /// </summary>
        private Dictionary<int, List<SchemaNodeViewModel>> levelDictinary = new Dictionary<int, List<SchemaNodeViewModel>>();
        /// <summary>
        /// 获取各层的编号列表
        /// </summary>
        /// <param name="noTemp"></param>
        /// <returns></returns>
        private List<string> GetNoList(string noTemp)
        {
            List<string> noList = new List<string>();
            int stringLength = noTemp.Length;
            noList.Add(noTemp.Substring(0, 2));
            int level = 0;
            while (2 + 3 * level < stringLength)
            {
                level = level + 1;
                noList.Add(noTemp.Substring(0, 2 + 3 * level));
            }
            return noList;
        }
        /// <summary>
        /// 获取某一层所有的子节点
        /// </summary>
        /// <param name="levelTemp"></param>
        /// <returns></returns>
        private List<SchemaNodeViewModel> GetAllNodes(int levelTemp)
        {
            List<SchemaNodeViewModel> nodes = new List<SchemaNodeViewModel>();
            for (int i = 0; i < Children.Count; i++)
            {
                nodes.AddRange(GetNodesPerLevel(levelTemp, Children[i]));
            }
            return nodes;
        }

        /// <summary>
        /// 获取每一层的所有节点
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private List<SchemaNodeViewModel> GetNodesPerLevel(int levelTemp, SchemaNodeViewModel nodeTemp)
        {
            List<SchemaNodeViewModel> nodeList = new List<SchemaNodeViewModel>();
            if (nodeTemp.Level == levelTemp)
            {
                nodeList.Add(nodeTemp);
            }
            else if (nodeTemp.Level < levelTemp)
            {
                for (int i = 0; i < nodeTemp.Children.Count; i++)
                {
                    nodeList.AddRange(GetNodesPerLevel(levelTemp, nodeTemp.Children[i]));
                }
            }
            return nodeList;
        }
        #endregion

        /// <summary>
        /// 更新误差点
        /// </summary>
        /// <param name="model">要更新的误差点</param>
        public void UpdateErrorPoint(ErrorModel model)
        {
            if (model.FlagRemove)
            {
                string errorValue = string.Format("基本误差|{0}|{1}|{2}|{3}|", model.FangXiang, model.Component, model.Factor, model.Current);
                for (int i = 0; i < SelectedNode.ParaValuesCurrent.Count; )
                {
                    string paraValueTemp = SelectedNode.ParaValuesCurrent[i].GetProperty("PARA_VALUE") as string;
                    if ((!string.IsNullOrEmpty(paraValueTemp)) && paraValueTemp.Contains(errorValue))
                    {
                        SelectedNode.ParaValuesCurrent.RemoveAt(i);
                        ParaValuesConvert();
                        continue;
                    }
                    i++;
                }
            }
            else
            {
                //误差试验类型|功率方向|功率元件|功率因数|电流倍数|添加谐波|逆相序|相对Ib误差圈数
                DynamicViewModel dynamicViewModel = new DynamicViewModel(0);
                dynamicViewModel.SetProperty("误差试验类型", "基本误差");
                dynamicViewModel.SetProperty("功率方向", model.FangXiang);
                dynamicViewModel.SetProperty("功率元件", model.Component);
                dynamicViewModel.SetProperty("功率因数", model.Factor);
                dynamicViewModel.SetProperty("电流倍数", model.Current);
                dynamicViewModel.SetProperty("添加谐波", "否");
                dynamicViewModel.SetProperty("逆相序", "否");
                dynamicViewModel.SetProperty("误差圈数(Ib)", model.LapCountIb);
                dynamicViewModel.SetProperty("误差限倍数(%)", model.GuichengMulti);
                dynamicViewModel.SetProperty("IsSelected", true);
                DynamicModel modelTemp = ParaValueConvertBack(dynamicViewModel);
                if (modelTemp == null) return;

                #region 排序
                IEnumerable<string> valuesEnumrable = from item in SelectedNode.ParaValuesCurrent select GetErrorSortString(item.GetProperty("PARA_KEY") as string);
                int indexCurrent = 0;
                string valueCurrent = modelTemp.GetProperty("PARA_KEY") as string;
                string valueSort = GetErrorSortString(valueCurrent);
                if (valuesEnumrable != null && valuesEnumrable.Count() > 0)
                {
                    for (; indexCurrent < valuesEnumrable.Count(); indexCurrent++)
                    {
                        if (string.Compare(valueSort, valuesEnumrable.ElementAt(indexCurrent)) < 0)
                        {
                            ParaValuesView.Insert(indexCurrent, dynamicViewModel);
                            SelectedNode.ParaValuesCurrent.Insert(indexCurrent, modelTemp);
                            SelectedNode.PointCount = SelectedNode.ParaValuesCurrent.Count;
                            return;
                        }
                    }
                }
                ParaValuesView.Add(dynamicViewModel);
                SelectedNode.ParaValuesCurrent.Add(modelTemp);
                SelectedNode.PointCount = SelectedNode.ParaValuesCurrent.Count;
                #endregion
            }
            RefreshPointCount();
        }
        public SchemaNodeViewModel SelectedNode { get; set; }
        /// <summary>
        /// 将当前的视图转换成方案配置信息
        /// </summary>
        /// <returns></returns>
        public List<DynamicModel> ParaValuesConvertBack()
        {
            List<DynamicModel> models = new List<DynamicModel>();
            if (SelectedNode == null)
            {
                return new List<DynamicModel>();
            }
            SelectedNode.ParaValuesCurrent.Clear();
            for (int i = 0; i < ParaValuesView.Count; i++)
            {
                DynamicModel modelTemp = ParaValueConvertBack(ParaValuesView[i]);
                //add by wzs 
                //if (SelectedNode.ParaNo == "02011")//初始化固有误差情况下.在PARA_VALUE_NO和PARA_KEY后加01
                //{
                //    modelTemp.SetProperty("PARA_VALUE_NO", modelTemp.GetProperty("PARA_VALUE_NO") + "01");
                //    modelTemp.SetProperty("PARA_KEY", modelTemp.GetProperty("PARA_KEY") + "01");     
                //}

                models.Add(modelTemp);
            }
            //add by wzs 
            //if (SelectedNode.ParaNo == "02011")//初始化固有误差情况下，再加一次误差点
            //{
            //    for (int i = ParaValuesView.Count-1; i >=0; i--)
            //    {
            //        DynamicModel modelTemp = ParaValueConvertBack(ParaValuesView[i]);
            //        modelTemp.SetProperty("PARA_VALUE_NO", modelTemp.GetProperty("PARA_VALUE_NO") + "02");
            //        modelTemp.SetProperty("PARA_KEY", modelTemp.GetProperty("PARA_KEY") + "02");

            //        //modelTemp.
            //        models.Add(modelTemp);
            //    }

            //}


            return models;
        }
        /// <summary>
        /// 将视图模型转换成结论模型
        /// </summary>
        /// <param name="viewModelTemp"></param>
        /// <returns></returns>
        private DynamicModel ParaValueConvertBack(DynamicViewModel viewModelTemp)
        {
            #region 获取para_value和para_value_no
            string validFlag = "0";
            if (viewModelTemp.GetProperty("IsSelected") is bool)
            {
                if ((bool)(viewModelTemp.GetProperty("IsSelected")))
                {
                    validFlag = "1";
                }
            }
            List<string> propertyNames = viewModelTemp.GetAllProperyName();
            propertyNames.Remove("IsSelected");
            List<string> valueList = new List<string>();
            List<string> codeList = new List<string>();
            string paraName = ParaInfo.ItemName;
            if (paraName == "")
            {
                paraName = ParaInfo.CategoryName;
            }
            string basicName = paraName;
            if (!ParaInfo.ContainProjectName)
            {
                paraName = "";
            }
            string paraKey = ParaInfo.ParaNo;
            string paraCodeString = "";
            #region 参数
            for (int j = 0; j < propertyNames.Count; j++)
            {
                string temp = viewModelTemp.GetProperty(propertyNames[j]) as string;
                string codeTemp = "";
                valueList.Add(temp);
                if (ParaInfo.CheckParas.Count > j)
                {
                    codeTemp = CodeDictionary.GetValueLayer2(ParaInfo.CheckParas[j].ParaEnumType, temp);
                    if (ParaInfo.CheckParas[j].IsNameMember)
                    {
                        paraName = paraName + "_" + temp;
                    }
                    if (ParaInfo.CheckParas[j].IsKeyMember)
                    {
                        paraCodeString = paraCodeString + codeTemp;
                    }
                }
                codeList.Add(codeTemp);
            }
            #endregion
            if (!string.IsNullOrEmpty(paraCodeString))
            {
                paraKey = paraKey + "_" + paraCodeString;
            }
            #endregion
            string paraValue = string.Join("|", valueList);
            string paraValueNo = string.Join("|", codeList);

            DynamicModel modelTemp = new DynamicModel();
            modelTemp.SetProperty("PARA_NO", ParaNo);
            modelTemp.SetProperty("PARA_VALUE", paraValue);
            modelTemp.SetProperty("PARA_VALUE_NO", paraKey);
            modelTemp.SetProperty("PARA_KEY", paraKey);
            if (string.IsNullOrEmpty(paraName))
            {
                paraName = basicName;
            }
            modelTemp.SetProperty("PARA_NAME", paraName.TrimStart('_'));
            modelTemp.SetProperty("VALID_FLAG", validFlag);
            modelTemp.SetProperty("SCHEMA_ID", SchemaId);
            return modelTemp;
        }

        /// 加载当前选定检定点参数视图
        /// <summary>
        /// 加载当前选定检定点参数视图
        /// </summary>
        public void ParaValuesConvert()
        {
            if (SelectedNode == null)
            {
                return;
            }
            ParaValuesView.Clear();
            List<DynamicModel> models = SelectedNode.ParaValuesCurrent;
            IEnumerable<string> displayNames = from item in ParaInfo.CheckParas select item.ParaDisplayName;
            IEnumerable<string> enumCodes = from item in ParaInfo.CheckParas select item.ParaEnumType;
            IEnumerable<bool> keyRules = from item in ParaInfo.CheckParas select item.IsKeyMember;
            for (int i = 0; i < models.Count; i++)
            {
                DynamicViewModel dynamicViewModel = new DynamicViewModel(displayNames.ToList(), i);
                string stringParaValue = models.ElementAt(i).GetProperty("PARA_VALUE") as string;
                if (stringParaValue == null)
                {
                    stringParaValue = "";
                }
                string[] arrayParaValue = stringParaValue.Split('|');
                for (int j = 0; j < displayNames.Count(); j++)
                {
                    if (arrayParaValue.Length > j)
                    {
                        dynamicViewModel.SetProperty(displayNames.ElementAt(j), arrayParaValue[j]);
                    }
                    else
                    {
                        dynamicViewModel.SetProperty(displayNames.ElementAt(j), "");
                    }
                }
                bool isChecked = false;
                if (models[i].GetProperty("VALID_FLAG") as string == "1")
                {
                    isChecked = true;
                }
                dynamicViewModel.SetProperty("IsSelected", isChecked);
                ParaValuesView.Add(dynamicViewModel);
            }
        }
        /// <summary>
        /// 添加检定点
        /// </summary>
        /// <param name="noTemp"></param>
        public SchemaNodeViewModel AddParaNode(string noTemp)
        {
            string itemName = SchemaFramework.GetItemName(noTemp);
            if (string.IsNullOrEmpty(itemName))
            {
                return null;
            }
            SchemaNodeViewModel nodeTemp = GetFirstNode(noTemp);
            ParaNo = noTemp;
            SelectedNode = nodeTemp;
            ParaValuesConvert();
            AddNewParaValue();
            SelectedNode.ParaValuesCurrent = ParaValuesConvertBack();
            RefreshPointCount();
            return nodeTemp;
        }
        /// <summary>
        /// 恢复默认排序
        /// </summary>
        public void SortDefault()
        {
            ParaValues.Clear();
            List<DynamicModel> models = new List<DynamicModel>();
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].ParaValuesCurrent.Count > 0)
                {
                    models.AddRange(Children[i].ParaValuesCurrent);
                }
                else
                {
                    for (int j = 0; j < Children[i].Children.Count; j++)
                    {
                        for (int k = 0; k < Children[i].Children[j].PointCount; k++)
                        {
                            if (Children[i].Children[j].PointCount < 2)
                            {
                                models.Add(Children[i].Children[j].ParaValuesCurrent[0]);
                            }
                            else
                            {
                                models.Add(Children[i].Children[j].Children[k].ParaValuesCurrent[0]);
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < models.Count; i++)
            {
                ParaValues.Add(new DynamicViewModel(models[i], i));
            }
            ParaValues.Sort(item => GetSortString(item));
            InitialSchemaTree();
        }
        /// <summary>
        /// 获取排序字符串
        /// </summary>
        /// <param name="viewModelTemp">要排序的模型</param>
        private string GetSortString(DynamicViewModel viewModelTemp)
        {
            string keyString = viewModelTemp.GetProperty("PARA_KEY") as string;
            string para_no = viewModelTemp.GetProperty("PARA_NO") as string;
            DynamicModel modelFormat = SchemaFramework.GetParaFormat(para_no);
            string sort_no = "999";
            if (modelFormat != null)
            {
                sort_no = modelFormat.GetProperty("DEFAULT_SORT_NO") as string;
            }

            string sortKeyString = sort_no + "_" + keyString;
            //如果是基本误差
            if (keyString != null && (keyString.StartsWith("02001")))
            {
                return GetErrorSortString(sortKeyString);
            }
            return sortKeyString;
        }
        private string GetErrorSortString(string keyString)
        {
            if (keyString == null)
            {
                return "";
            }
            string[] arrayTemp = keyString.Split('_');
            if (arrayTemp.Length == 3)
            {
                //数据格式:排序号|误差试验类型|功率方向|功率元件|功率因数|电流倍数|添加谐波|逆相序
                string strPara = arrayTemp[2];
                string currentString = strPara.Substring(5, 2);
                strPara = strPara.Remove(5, 2);
                strPara = strPara.Insert(3, currentString);
                return arrayTemp[0] + "_" + arrayTemp[1] + "_" + strPara;
            }
            else if (arrayTemp.Length == 2)
            {
                //数据格式:误差试验类型|功率方向|功率元件|功率因数|电流倍数|添加谐波|逆相序
                string strPara = arrayTemp[1];
                string currentString = strPara.Substring(5, 2);
                strPara = strPara.Remove(5, 2);
                strPara = strPara.Insert(3, currentString);
                return arrayTemp[0] + "_" + strPara;
            }
            return keyString;
        }
        /// <summary>
        /// 移动检定点
        /// </summary>
        /// <param name="nodeSource"></param>
        /// <param name="nodeDest"></param>
        public void MoveNode(SchemaNodeViewModel nodeSource, SchemaNodeViewModel nodeDest)
        {
            if (nodeDest == null || nodeSource == null || nodeSource.Equals(nodeDest))
            {
                return;
            }
            //终端节点,及包含了检定点的节点
            List<SchemaNodeViewModel> nodesTerminalSource = nodeSource.GetTerminalNodes();
            List<SchemaNodeViewModel> nodesTerminalDest = nodeDest.GetTerminalNodes();
            List<SchemaNodeViewModel> nodesTerminalAll = GetTerminalNodes();
            if (nodesTerminalDest.Count == 0 || nodesTerminalSource.Count == 0)
            {
                return;
            }
            for (int i = 0; i < nodesTerminalSource.Count; i++)
            {
                nodesTerminalAll.Remove(nodesTerminalSource[i]);
            }
            int insertIndex = nodesTerminalAll.IndexOf(nodesTerminalDest[0]);
            if (insertIndex >= 0)
            {
                for (int i = 0; i < nodesTerminalSource.Count; i++)
                {
                    nodesTerminalAll.Insert(insertIndex, nodesTerminalSource[i]);
                    insertIndex++;
                }
            }

            ParaValues.Clear();
            List<DynamicModel> models = new List<DynamicModel>();
            for (int i = 0; i < nodesTerminalAll.Count; i++)
            {
                if (nodesTerminalAll[i].ParaValuesCurrent.Count > 0)
                {
                    models.AddRange(nodesTerminalAll[i].ParaValuesCurrent);
                }
            }
            for (int i = 0; i < models.Count; i++)
            {
                ParaValues.Add(new DynamicViewModel(models[i], i));
            }
            InitialSchemaTree();
        }

        /// <summary>
        /// 获取最底层的检定点
        /// </summary>
        /// <param name="nodeTemp"></param>
        /// <returns></returns>
        private List<DynamicModel> GetParaValues(SchemaNodeViewModel nodeTemp)
        {
            List<DynamicModel> modelsTemp = new List<DynamicModel>();
            List<SchemaNodeViewModel> nodesTerminal = nodeTemp.GetTerminalNodes();
            for (int i = 0; i < nodesTerminal.Count; i++)
            {
                modelsTemp.AddRange(nodesTerminal[i].ParaValuesCurrent);
            }
            return modelsTemp;
        }
    }
}
