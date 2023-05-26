using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.ViewModel.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Mesurement.UiLayer.ViewModel.CodeTree
{
    /// <summary>
    /// 编码的树形节点
    /// </summary>
    public class CodeTreeNode : ViewModelBase
    {
        public CodeTreeNode()
        {
            PropertyChanged += CodeTreeNode_PropertyChanged;
        }

        private void CodeTreeNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Parent" && e.PropertyName!="ID" && e.PropertyName != "FlagChanged")
            {
                FlagChanged = true;
            }
        }

        private DynamicModel GetModel()
        {
            DynamicModel modelTemp = new DynamicModel();
            modelTemp.SetProperty("ID", ID);
            modelTemp.SetProperty("CODE_CN_NAME", CODE_NAME);
            modelTemp.SetProperty("CODE_VALUE", CODE_VALUE);
            modelTemp.SetProperty("CODE_EN_NAME", CODE_TYPE);
            modelTemp.SetProperty("VALID_FLAG", VALID_FLAG ? "1" : "0");
            modelTemp.SetProperty("CODE_LEVEL", CODE_LEVEL.ToString());
            modelTemp.SetProperty("CODE_CATEGORY", CODE_CATEGORY);
            modelTemp.SetProperty("CODE_PARENT", CODE_PARENT);
            modelTemp.SetProperty("CODE_PERMISSION", ((int)CodePermission).ToString());

            return modelTemp;
        }

        public CodeTreeNode(DynamicModel modelTemp)
        {
            ID = (int)(modelTemp.GetProperty("ID"));
            CODE_NAME = modelTemp.GetProperty("CODE_CN_NAME") as string;
            CODE_VALUE = modelTemp.GetProperty("CODE_VALUE") as string;
            CODE_TYPE = modelTemp.GetProperty("CODE_EN_NAME") as string;
            string validFlag = modelTemp.GetProperty("VALID_FLAG") as string;
            if (validFlag == "0")
            {
                VALID_FLAG = false;
            }
            else
            {
                VALID_FLAG = true;
            }
            string levelTemp = modelTemp.GetProperty("CODE_LEVEL") as string;
            if (!int.TryParse(levelTemp, out code_level))
            {
                CODE_LEVEL = 1;
            }
            CODE_CATEGORY = modelTemp.GetProperty("CODE_CATEGORY") as string;
            CODE_PARENT = modelTemp.GetProperty("CODE_PARENT") as string;

            string strPermission = modelTemp.GetProperty("CODE_PERMISSION") as string;
            int intTemp = 20;
            if (int.TryParse(strPermission, out intTemp))
            {
                CodePermission = (EnumPermission)intTemp;
            }

            PropertyChanged += CodeTreeNode_PropertyChanged;
        }

        private int id = 0;
        /// <summary>
        /// 编码在数据库中的编号,新添加的编号为0
        /// </summary>
        public int ID
        {
            get { return id; }
            set { SetPropertyValue(value, ref id, "ID"); }
        }
        private string code_type;
        /// <summary>
        /// 编码英文名称
        /// </summary>
        public string CODE_TYPE
        {
            get { return code_type; }
            set { SetPropertyValue(value, ref code_type, "CODE_TYPE"); }
        }
        private string code_name;
        /// <summary>
        /// 编码中文名称
        /// </summary>
        public string CODE_NAME
        {
            get { return code_name; }
            set { SetPropertyValue(value, ref code_name, "CODE_NAME"); }
        }
        private string code_value;
        /// <summary>
        /// 编码值
        /// </summary>
        public string CODE_VALUE
        {
            get { return code_value; }
            set { SetPropertyValue(value, ref code_value, "CODE_VALUE"); }
        }
        private int code_level;
        /// <summary>
        /// 编码层数
        /// </summary>
        public int CODE_LEVEL
        {
            get { return code_level; }
            set { SetPropertyValue(value, ref code_level, "CODE_LEVEL"); }
        }

        private EnumPermission codePermission=EnumPermission.仅超级用户可操作;
        /// <summary>
        /// 编码修改权限
        /// </summary>
        public EnumPermission CodePermission
        {
            get { return codePermission; }
            set { SetPropertyValue(value, ref codePermission, "CodePermission"); }
        }

        private string code_parent;
        /// <summary>
        /// 父节点英文名称
        /// </summary>
        public string CODE_PARENT
        {
            get { return code_parent; }
            set { SetPropertyValue(value, ref code_parent, "CODE_PARENT"); }
        }
        private string code_category;
        /// <summary>
        /// 编码类别
        /// </summary>
        public string CODE_CATEGORY
        {
            get { return code_category; }
            set { SetPropertyValue(value, ref code_category, "CODE_CATEGORY"); }
        }

        private bool valid_flag;
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool VALID_FLAG
        {
            get { return valid_flag; }
            set { SetPropertyValue(value, ref valid_flag, "VALID_FLAG"); }
        }

        private bool flagChanged = false;
        /// <summary>
        /// 被更改标记
        /// </summary>
        public bool FlagChanged
        {
            get { return flagChanged; }
            set
            {
                SetPropertyValue(value, ref flagChanged, "FlagChanged");
            }
        }

        private AsyncObservableCollection<CodeTreeNode> children = new AsyncObservableCollection<CodeTreeNode>();
        /// <summary>
        /// 子节点
        /// </summary>
        public AsyncObservableCollection<CodeTreeNode> Children
        {
            get { return children; }
            set { SetPropertyValue(value, ref children, "Children"); }
        }

        private CodeTreeNode parent;

        public CodeTreeNode Parent
        {
            get { return parent; }
            set { SetPropertyValue(value, ref parent, "Parent"); }
        }
        /// <summary>
        /// 添加编码
        /// </summary>
        public void AddCode()
        {
            CodeTreeNode nodeNew = new CodeTreeNode()
            {
                CODE_LEVEL = CODE_LEVEL + 1,
                CODE_CATEGORY = CODE_CATEGORY,
                CODE_NAME = "新编码中文名称",
                CODE_VALUE = "新编码值",
                CODE_TYPE = "",
                CODE_PARENT = CODE_TYPE,
                VALID_FLAG=true,
                Parent = this,
            };
            Children.Add(nodeNew);
        }
        /// <summary>
        /// 非终结点不允许删除
        /// </summary>
        public void DeleteCode()
        {
            List<int> idList = GetIdList(this);
            List<string> conditionList = new List<string>();
            for (int i = 0; i < idList.Count; i++)
            {
                conditionList.Add(string.Format("id={0}", idList[i]));
            }
            if (conditionList.Count > 0)
            {
                string where = string.Join(" or ", conditionList);
                int deleteCount = DALManager.ApplicationDbDal.Delete(EnumAppDbTable.CODE_TREE.ToString(), where);
                LogManager.AddMessage(string.Format("删除数据库中编码{0}及其所有子元素完成,共删除{1}条记录.", where,deleteCount));
            }
            if (Parent != null)
            {
                Parent.Children.Remove(this);
            }
        }
        /// <summary>
        /// 递归获取节点所有的ID
        /// </summary>
        /// <param name="nodeTemp">要获取id的节点</param>
        /// <returns></returns>
        private List<int> GetIdList(CodeTreeNode nodeTemp)
        {
            List<int> listTemp = new List<int>();
            if (nodeTemp.ID != 0)
            {
                listTemp.Add(nodeTemp.ID);
                for (int i = 0; i < nodeTemp.Children.Count; i++)
                {
                    List<int> idChildList = GetIdList(nodeTemp.Children[i]);
                    if (idChildList.Count > 0)
                    {
                        listTemp.AddRange(idChildList);
                    }
                }
            }
            return listTemp;
        }
        /// <summary>
        /// 保存编码
        /// </summary>
        public void SaveCode()
        {
            #region 先将子节点的父元素更新
            EditChildren(this);
            #endregion
            #region 先更新要编辑的点
            List<DynamicModel> modelList = GetEditModels(this);
            if (modelList.Count > 0)
            {
                List<string> fieldNames = new List<string>();
                if (modelList.Count > 0)
                {
                    fieldNames = modelList[0].GetAllProperyName();
                }
                fieldNames.Remove("ID");
                int updateCount = DALManager.ApplicationDbDal.Update(EnumAppDbTable.CODE_TREE.ToString(), "ID", modelList, fieldNames);
                LogManager.AddMessage(string.Format("编码:{0}编辑完成,共执行{1}条数据的更新", CODE_NAME, updateCount), EnumLogSource.数据库存取日志);

                
            }
            #endregion
            #region 再插入新增的点
            InsertCode(this);
            #endregion
        }
        /// <summary>
        /// 插入编码,ID号为0的编码插入比较繁琐,具体流程如下
        /// </summary>
        private void InsertCode(CodeTreeNode nodeToInsert)
        {
            if (nodeToInsert.FlagChanged && nodeToInsert.ID == 0)
            {
                #region 步骤1:插入数据库
                DynamicModel modelNew = nodeToInsert.GetModel();
                int insertCount = DALManager.ApplicationDbDal.Insert(EnumAppDbTable.CODE_TREE.ToString(), modelNew);
                if (insertCount > 0)
                {
                    LogManager.AddMessage(string.Format("编码{0}插入数据库成功", nodeToInsert.CODE_NAME), EnumLogSource.数据库存取日志);
                }
                else
                {
                    LogManager.AddMessage(string.Format("编码{0}插入数据库失败", nodeToInsert.CODE_NAME), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                    MessageBox.Show(string.Format("编码{0}插入失败", nodeToInsert.CODE_NAME));
                    return;
                }
                nodeToInsert.FlagChanged = false;
                #endregion
                #region 步骤2:获取刚插入的数据模型并更新编号
                DynamicModel modelTemp = DALManager.ApplicationDbDal.GetByID(EnumAppDbTable.CODE_TREE.ToString(), string.Format("CODE_PARENT='{0}' order by ID desc", nodeToInsert.CODE_PARENT));
                if (modelTemp != null)
                {
                    int idTemp = (int)(modelTemp.GetProperty("ID"));
                    nodeToInsert.ID = idTemp;
                }
                #endregion
            }
            for (int i = 0; i < nodeToInsert.Children.Count; i++)
            {
                InsertCode(nodeToInsert.Children[i]);
            }
        }
        /// <summary>
        /// 获取要编辑的模型列表
        /// </summary>
        /// <param name="codeNode">要编辑的节点</param>
        /// <returns></returns>
        private List<DynamicModel> GetEditModels(CodeTreeNode codeNode)
        {
            List<DynamicModel> modelList = new List<DynamicModel>();
            if (codeNode.FlagChanged && codeNode.ID != 0)
            {
                modelList.Add(codeNode.GetModel());
                codeNode.FlagChanged = false;
            }
            for (int i = 0; i < codeNode.Children.Count; i++)
            {
                modelList.AddRange(GetEditModels(codeNode.Children[i]));
            }
            return modelList;
        }
        /// <summary>
        /// 迭代重命名
        /// </summary>
        /// <param name="nodeToRename"></param>
        private void EditChildren(CodeTreeNode nodeToRename)
        {
            for (int i = 0; i < nodeToRename.Children.Count; i++)
            {
                nodeToRename.Children[i].CODE_PARENT = nodeToRename.CODE_TYPE;
                EditChildren(nodeToRename.Children[i]);
            }
        }

        /// <summary>
        /// 当前用户权限
        /// </summary>
        public string CurrentPermission
        {
            get { return User.UserViewModel.Instance.CurrentUser.GetProperty("chrQx") as string; }
        }
    }
}