using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.DAL.DataBaseView;
using Mesurement.UiLayer.ViewModel.CodeTree;
using System.Linq;

namespace Mesurement.UiLayer.ViewModel.Schema
{
    public class FullTree : SchemaNodeViewModel
    {
        private static FullTree instance = null;
        public static FullTree Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FullTree();
                }
                return instance;
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            CodeTreeNode nodeSchemaRoot = CodeTreeViewModel.Instance.CodeNodes.FirstOrDefault(item => item.CODE_TYPE == "SchemaCategory");
            for (int i = 0; i < nodeSchemaRoot.Children.Count; i++)
            {
                SchemaNodeViewModel nodeTemp = GetNode(nodeSchemaRoot.Children[i]);
                if (nodeTemp != null)
                {
                    Children.Add(nodeTemp);
                    SchemaFramework.AddNewPair(nodeTemp.ParaNo, nodeTemp.Name);
                    SetParaNo(nodeTemp);
                }
            }
        }
        /// <summary>
        /// 获取方案节点
        /// </summary>
        /// <param name="codeNode"></param>
        /// <returns></returns>
        private SchemaNodeViewModel GetNode(CodeTreeNode codeNode)
        {
            if (!codeNode.VALID_FLAG)
            {
                return null;
            }
            SchemaNodeViewModel schemaNode = new SchemaNodeViewModel();
            schemaNode.Name = codeNode.CODE_NAME;
            #region 设置当前节点的信息
            if (codeNode.CODE_LEVEL == 2)
            {
                schemaNode.ParaNo = codeNode.CODE_VALUE.PadLeft(2, '0');
            }
            else
            {
                schemaNode.ParaNo = codeNode.CODE_VALUE.PadLeft(3, '0');
            }
            schemaNode.Level = codeNode.CODE_LEVEL - 1;
            #endregion
            for (int i = 0; i < codeNode.Children.Count; i++)
            {
                SchemaNodeViewModel schemaNodeChild = GetNode(codeNode.Children[i]);
                if (schemaNodeChild != null)
                {
                    schemaNode.Children.Add(schemaNodeChild);
                    schemaNodeChild.ParentNode = schemaNode;
                }
            }
            if (schemaNode.Children.Count == 0)
            {
                schemaNode.IsTerminal = true;
            }
            else
            {
                schemaNode.IsTerminal = false;
            }
            return schemaNode;
        }
        /// <summary>
        /// 设置节点编号
        /// </summary>
        /// <param name="nodeTemp"></param>
        private void SetParaNo(SchemaNodeViewModel nodeTemp)
        {
            if (nodeTemp.ParentNode != null)
            {
                nodeTemp.ParaNo = nodeTemp.ParentNode.ParaNo + nodeTemp.ParaNo;
                SchemaFramework.AddNewPair(nodeTemp.ParaNo, nodeTemp.Name);
                if (nodeTemp.IsTerminal)
                {
                    nodeTemp.ViewNo = ResultViewHelper.GetParaNoView(nodeTemp.ParaNo);
                    DynamicModel modelTemp = SchemaFramework.GetParaFormat(nodeTemp.ParaNo);
                    if (modelTemp != null)
                    {
                        nodeTemp.SortNo = modelTemp.GetProperty("DEFAULT_SORT_NO") as string;
                    }
                    if (string.IsNullOrEmpty(nodeTemp.SortNo))
                    {
                        nodeTemp.SortNo = "999";
                    }
                }
            }

            for (int i = 0; i < nodeTemp.Children.Count; i++)
            {
                SetParaNo(nodeTemp.Children[i]);
            }
        }
    }
}
