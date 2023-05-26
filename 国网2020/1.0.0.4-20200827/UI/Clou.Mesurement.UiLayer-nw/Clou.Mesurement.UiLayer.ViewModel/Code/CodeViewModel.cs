using Clou.Mesurement.UiLayer.DAL;
using Clou.Mesurement.UiLayer.Utility.Log;
using Clou.Mesurement.UiLayer.ViewModel.Model;
using System.Collections.Generic;

namespace Clou.Mesurement.UiLayer.ViewModel.Code
{
    /// 编码视图模型
    /// <summary>
    /// 编码视图模型
    /// </summary>
    public class CodeViewModel : ViewModelBase
    {
        private string[] arrayCategoryName = { "软件配置", "配置数据源", "检定方案", "检定参数类", "错误编码", "数据库配置", "编码类型7", "编码类型8", "未定义类型" };

        private CodeTreeNode treeCode = new CodeTreeNode();

        public CodeTreeNode TreeCode
        {
            get { return treeCode; }
            set { SetPropertyValue(value, ref treeCode, "TreeCode"); }
        }

        private AsyncObservableCollection<CodeUnit> units = new AsyncObservableCollection<CodeUnit>();

        public AsyncObservableCollection<CodeUnit> Units
        {
            get { return units; }
            set { units = value; }
        }

        private CodeTreeNode currentNode;

        public CodeTreeNode CurrentNode
        {
            get { return currentNode; }
            set
            {
                SetPropertyValue(value, ref currentNode, "CurrentNode");
                LoadCurrentConfig();
            }
        }

        public void Initialize()
        {
            TreeCode.Children.Clear();
            for (int i = 0; i < arrayCategoryName.Length; i++)
            {
                string categoryName = arrayCategoryName[i];
                CodeTreeNode node = new CodeTreeNode
                    {
                        CodeDisplay = categoryName,
                    };
                string temp = "1";
                temp = temp.PadLeft(i + 1, '_');
                string where = string.Format("enum_category like '{0}%'", temp);
                if (i == 8)
                {
                    where = "enum_category = '00000000'";
                }
                List<DynamicModel> models = DALManager.ApplicationDbDal.GetList(EnumAppDbTable.CODE_SUMMARY.ToString(), where);

                foreach (DynamicModel model in models)
                {
                    node.Children.Add(
                    new CodeTreeNode
                        {
                            ID = (int)(model.GetProperty("ID")),
                            CodeName = model.GetProperty("ENUM_TYPE") as string,
                            CodeDisplay = model.GetProperty("CHINESE_NAME") as string,
                            CodeCategory = model.GetProperty("ENUM_CATEGORY") as string,
                            ChangeFlag = false
                        }
                  );
                }
                TreeCode.Children.Add(node);
            }
        }

        public void LoadCurrentConfig()
        {
            Units.Clear();
            if (currentNode == null)
            {
                return;
            }
            List<DynamicModel> models = DALManager.ApplicationDbDal.GetList(EnumAppDbTable.CODE_DETAIL.ToString(), string.Format("ENUM_TYPE='{0}' order by ENUM_VALUE", currentNode.CodeName));
            for (int i = 0; i < models.Count; i++)
            {
                Units.Add(new CodeUnit
                {
                    ID = (int)models[i].GetProperty("ID"),
                    CodeName = models[i].GetProperty("ENUM_NAME") as string,
                    CodeValue = models[i].GetProperty("ENUM_VALUE") as string,
                    Category = models[i].GetProperty("ENUM_TYPE") as string,
                    IsValid= (models[i].GetProperty("VALID_FLAG") as string)!="0",
                    ChangeFlag = false
                });
            }
        }
        public CodeViewModel()
        {
            Initialize();
        }

        public void DeleteCodeValue(CodeUnit unit)
        {
            Units.Remove(unit);
            if (unit.ID != 0)
            {
                int result = DALManager.ApplicationDbDal.Delete(EnumAppDbTable.CODE_DETAIL.ToString(), string.Format("ID={0}", unit.ID));
                if (result>0)
                {
                    LogManager.AddMessage(string.Format("编码:{0} 删除成功!", unit.CodeName), EnumLogSource.数据库存取日志);
                }
                else
                {
                    LogManager.AddMessage(string.Format("编码:{0} 删除失败!", unit.CodeName), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                }
            }
        }
        public void DeleteCode()
        {
        }
        public void AddCodeValue()
        {
            Units.Add(new CodeUnit
            {
                CodeName = "添加新项",
                CodeValue = "添加新值",
                Category = CurrentNode.CodeName,
                ID = 0
            });
        }
        public void SaveCodeValue()
        {
            for (int i = 0; i < Units.Count; i++)
            {
                if (Units[i].ChangeFlag)
                {
                    DynamicModel model = new DynamicModel();
                    model.SetProperty("ENUM_NAME", Units[i].CodeName);
                    model.SetProperty("ENUM_VALUE", Units[i].CodeValue);
                    model.SetProperty("ENUM_TYPE", Units[i].Category);
                    model.SetProperty("VALID_FLAG", Units[i].IsValid ? "1" : "0");
                    int result = 0;
                    if (Units[i].ID != 0)
                    {
                        result = DALManager.ApplicationDbDal.Update(EnumAppDbTable.CODE_DETAIL.ToString(), string.Format("ID={0}", Units[i].ID), model, new List<string> { "ENUM_NAME", "ENUM_VALUE", "ENUM_TYPE", "VALID_FLAG" });
                        if (result > 0)
                        {
                            LogManager.AddMessage(string.Format("编码:{0} 更新成功!", Units[i].CodeName), EnumLogSource.数据库存取日志);
                            Units[i].ChangeFlag = false;
                        }
                        else
                        {
                            LogManager.AddMessage(string.Format("编码:{0} 更新失败!", Units[i].CodeName), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                        }
                    }
                    else
                    {
                        result = DALManager.ApplicationDbDal.Insert(EnumAppDbTable.CODE_DETAIL.ToString(), model);
                        if (result > 0)
                        {
                            LogManager.AddMessage(string.Format("编码:{0} 插入成功!", Units[i].CodeName), EnumLogSource.数据库存取日志);
                            Units[i].ChangeFlag = false;
                        }
                        else
                        {
                            LogManager.AddMessage(string.Format("编码:{0} 插入失败!", Units[i].CodeName), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                        }
                    }

                    CodeDictionary.LoadCode(Units[i].Category, true);
                }
            }
        }
        public void SaveCode(CodeTreeNode treeNode)
        {
            if (treeNode == null) return;
            for (int i = 0; i < treeNode.Children.Count; i++)
            {
                if (treeNode.Children[i].ChangeFlag)
                {
                    DynamicModel model = new DynamicModel();
                    model.SetProperty("CHINESE_NAME", treeNode.Children[i].CodeDisplay);
                    model.SetProperty("ENUM_CATEGORY", treeNode.Children[i].CodeCategory);
                    model.SetProperty("ENUM_TYPE", treeNode.Children[i].CodeName); int result = 0;
                    if (treeNode.Children[i].ID != 0)
                    {
                        result = DALManager.ApplicationDbDal.Update(EnumAppDbTable.CODE_SUMMARY.ToString(), string.Format("ID={0}", treeNode.Children[i].ID), model, new List<string> { "CHINESE_NAME", "ENUM_CATEGORY" });
                        if (result > 0)
                        {
                            LogManager.AddMessage(string.Format("编码:{0} 更新成功!", treeNode.Children[i].CodeDisplay), EnumLogSource.数据库存取日志);
                            treeNode.Children[i].ChangeFlag = false;
                        }
                        else
                        {
                            LogManager.AddMessage(string.Format("编码:{0} 更新失败!", treeNode.Children[i].CodeDisplay), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                        }
                    }
                    else
                    {
                        result = DALManager.ApplicationDbDal.Insert(EnumAppDbTable.CODE_SUMMARY.ToString(), model);
                        if (result > 0)
                        {
                            LogManager.AddMessage(string.Format("编码:{0} 插入成功!", treeNode.Children[i].CodeDisplay), EnumLogSource.数据库存取日志);
                            treeNode.Children[i].ChangeFlag = false;
                        }
                        else
                        {
                            LogManager.AddMessage(string.Format("编码:{0} 插入失败!", treeNode.Children[i].CodeDisplay), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                        }
                    }
                    CodeDictionary.LoadCode(treeNode.Children[i].CodeName,true);
                }
            }
        }
        public void AddCode(CodeTreeNode treeNode)
        {
            if (treeNode == null) return;
            int temp = TreeCode.Children.IndexOf(treeNode);
            string category = "1".PadLeft(temp + 1, '0').PadRight(8, '0');
            treeNode.Children.Add(new CodeTreeNode
            {
                ID = 0,
                CodeCategory = category,
                CodeName = "添加新值",
                CodeDisplay = "添加新值"
            });
        }
        protected override void Dispose(bool disposing)
        {
            Units.Clear();
            for (int i = 0; i < TreeCode.Children.Count; i++)
            {
                TreeCode.Children[i].Children.Clear();
            }
            TreeCode.Children.Clear();
            base.Dispose(disposing);
        }
    }
}
