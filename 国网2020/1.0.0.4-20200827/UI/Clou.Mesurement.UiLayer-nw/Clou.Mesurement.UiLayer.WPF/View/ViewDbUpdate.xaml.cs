using DevComponents.WpfDock;
using System.Linq;
using Mesurement.UiLayer.ViewModel;
using System.Windows;
using Microsoft.Win32;
using Mesurement.UiLayer.DAL;
using System.Collections.Generic;
using Mesurement.UiLayer.Utility.Log;
using System;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// 用来同步数据库数据
    /// </summary>
    public partial class ViewDbUpdate
    {
        public ViewDbUpdate()
        {
            InitializeComponent();
            Name = "数据库更新";
        }

        string filePath = string.Empty;

        void fileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenFileDialog fileDialog = sender as OpenFileDialog;
            filePath = fileDialog.FileName;
            textBlockFilePath.Text = filePath;
        }

        private void Click_Browse(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "(access数据库文件)|*.mdb";
            fileDialog.FileOk += fileDialog_FileOk;
            fileDialog.ShowDialog();
        }
        private void Click_UpdateDb(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdataApplicationDb();
            }
            catch (Exception ex)
            {
                LogManager.AddMessage("更新数据库数据异常:{0}"+ex.Message, EnumLogSource.数据库存取日志, EnumLevel.Error);
            }
        }
        private void UpdataApplicationDb()
        {
            #region 要更新的数据库
            string connString = string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};jet oledb:database password=cl3000s-hv4", filePath);
            GeneralDal dalSource = new GeneralDal(connString);
            #endregion
            #region 添加Code_Tree
            List<DynamicModel> itemsCode = dalSource.GetModels("code_tree", "select top 100 * from code_tree order by id desc");
            List<DynamicModel> itemsCodeToInsert = new List<DynamicModel>(); 
            foreach (DynamicModel itemCode in itemsCode)
            {
                string whereTemp = string.Format("CODE_CN_NAME='{0}' and CODE_LEVEL='{1}' and CODE_PARENT='{2}'", itemCode.GetProperty("CODE_CN_NAME"), itemCode.GetProperty("CODE_LEVEL"), itemCode.GetProperty("CODE_PARENT"));
                if (DALManager.ApplicationDbDal.GetCount("code_tree", whereTemp) == 0)
                {
                    itemsCodeToInsert.Add(itemCode);
                }
                else
                {
                    break;
                }
            }
            if (itemsCodeToInsert.Count > 0)
            {
                int insertCount = DALManager.ApplicationDbDal.Insert("code_tree", itemsCodeToInsert);
                LogManager.AddMessage(string.Format("编码树表中共插入{0}条数据", insertCount), EnumLogSource.数据库存取日志, EnumLevel.Tip);
            }
            #endregion
            #region 添加结论视图
            List<string> idsSource = dalSource.GetDistinct("dsptch_dic_view", "PK_VIEW_NO");
            List<string> idsCurrent = DALManager.ApplicationDbDal.GetDistinct("dsptch_dic_view", "PK_VIEW_NO");
            var idsToInsert = idsSource.Where(item => !idsCurrent.Contains(item));
            List<string> whereList = new List<string>();
            foreach(string viewNo in idsToInsert)
            {
                whereList.Add(string.Format("pk_view_no = '{0}'", viewNo));
            }
            if (whereList.Count > 0)
            {
                List<DynamicModel> viewsToInsert = dalSource.GetList("dsptch_dic_view", string.Join(" or ", whereList));
                int insertCount = DALManager.ApplicationDbDal.Insert("dsptch_dic_view", viewsToInsert);
                LogManager.AddMessage(string.Format("结论视图表中共插入{0}条数据", insertCount), EnumLogSource.数据库存取日志, EnumLevel.Tip);
            }
            #endregion
        }
    }
}
