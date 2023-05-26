using Clou.Mesurement.UiLayer.ViewModel.CodeTree;
using Clou.Mesurement.UiLayer.ViewModel.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clou.Mesurement.UiLayer.ViewModel.SchemaNew
{
    public class FullTree :SchemaNodeViewModel
    {
        /// <summary>
        /// 初始化方案架构
        /// </summary>
        private void Initialize()
        {
            CodeTreeNode nodeSchemaRoot = CodeTreeViewModel.Instance.CodeNodes.FirstOrDefault(item => item.CODE_TYPE == "SchemaCategory");
            for (int i = 0; i < nodeSchemaRoot.Children.Count; i++)
            {
                SchemaNodeViewModel nodeTemp = GetNode(nodeSchemaRoot.Children[i]);
                if (nodeTemp != null)
                {
                    Children.Add(nodeTemp);
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
            if (codeNode.Parent == null)
            {
                schemaNode.ParaNo = codeNode.CODE_VALUE.PadLeft(2, '0');
            }
            else
            {
                schemaNode.ParaNo = schemaNode.ParentNode.ParaNo + codeNode.CODE_VALUE.PadLeft(3, '0');
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
    }
}
