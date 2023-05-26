using Clou.Mesurement.UiLayer.ViewModel.Model;

namespace Clou.Mesurement.UiLayer.ViewModel.CheckInfo
{
    /// 检定点分类
    /// <summary>
    /// 检定点分类
    /// </summary>
    public class CategoryViewModel : ViewModelBase
    {
        private int level = 0;

        public int Level
        {
            get { return level; }
            set { SetPropertyValue(value, ref level, "Level"); }
        }

        private bool isCurrent;

        public bool IsCurrent
        {
            get { return isCurrent; }
            set { SetPropertyValue(value, ref isCurrent, "IsCurrent"); }
        }

        private string name = "";
        /// 名称
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { SetPropertyValue(value, ref name, "Name"); }
        }
        private string categoryNo;
        /// 检定大项编号
        /// <summary>
        /// 检定大项编号
        /// </summary>
        public string CategoryNo
        {
            get { return categoryNo; }
            set { SetPropertyValue(value, ref categoryNo, "CategoryNo"); }
        }

        private bool isSelected;
        /// 是否选中
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                SetPropertyValue(value, ref isSelected, "IsSelected");
                for (int i = 0; i < CheckNodes.Count; i++)
                {
                    CheckNodes[i].IsSelected = value;
                }
            }
        }

        /// 不合格数量
        /// <summary>
        /// 不合格数量
        /// </summary>
        public int FailCount
        {
            get
            {
                int failCount = 0;
                bool[] yaoJian = EquipmentData.MeterGroupInfo.YaoJian;
                for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
                {
                    if (yaoJian[i])
                    {
                        MeterResultUnit resultUnit = ResultSummary.GetProperty(string.Format("表位{0}", i + 1)) as MeterResultUnit;
                        if(resultUnit!=null && resultUnit.Result == "不合格")
                        {
                            failCount++;
                            continue;
                        }
                    }
                }
                return failCount;
            }
        }

        /// 测试通过
        /// <summary>
        /// 测试通过
        /// </summary>
        public bool TestPass
        {
            get
            {
                for (int i = 0; i < CheckNodes.Count; i++)
                {
                    if (!CheckNodes[i].TestPass)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private AsyncObservableCollection<CheckNodeViewModel> checkNodes = new AsyncObservableCollection<CheckNodeViewModel>();
        /// 检定点列表
        /// <summary>
        /// 检定点列表
        /// </summary>
        public AsyncObservableCollection<CheckNodeViewModel> CheckNodes
        {
            get { return checkNodes; }
            set { SetPropertyValue(value, ref checkNodes, "CheckNodes"); }
        }

        private DynamicViewModel resultSummary = new DynamicViewModel(0);
        /// 表的结论总览
        /// <summary>
        /// 表的结论总览
        /// </summary>
        public DynamicViewModel ResultSummary
        {
            get { return resultSummary; }
            set { SetPropertyValue(value, ref resultSummary, "ResultSummary"); }
        }
        /// 更新结论总览
        /// <summary>
        /// 更新结论总览
        /// </summary>
        public void RefreshResultSummary()
        {
            int meterCount = EquipmentData.Equipment.MeterCount;
            bool[] yaoJian = EquipmentData.MeterGroupInfo.YaoJian;
            #region 更新结论
            for (int j = 0; j < meterCount; j++)
            {
                string temp = "合格";
                for (int i = 0; i < CheckNodes.Count; i++)
                {
                    string temp1 = CheckNodes[i].CheckResults[j].GetProperty("结论") as string;
                    if (temp1 == "不合格")
                    {
                        temp = "不合格";
                    }
                    else if (temp1 != "合格" && temp != "不合格")
                    {
                        temp = "";
                    }
                }
                MeterResultUnit resultUnit = ResultSummary.GetProperty(string.Format("表位{0}", j + 1)) as MeterResultUnit;
                if (resultUnit == null)
                {
                    resultUnit = new MeterResultUnit();
                    ResultSummary.SetProperty(string.Format("表位{0}", j + 1), resultUnit);
                }
                resultUnit.Result = temp;
                resultUnit.ResultValue = temp;
            }
            #endregion
            OnPropertyChanged("TestPass");
            OnPropertyChanged("FailCount");
        }
    }
}
