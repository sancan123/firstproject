using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Common
{
    internal class import
    {
        #region 使用NPOI导出
        /// <summary>     
        /// DataTable导出到Excel文件     
        /// </summary>     
        /// <param name="dtSource">源DataTable</param>     
        /// <param name="strHeaderText">表头文本</param>     
        /// <param name="strFileName">保存位置</param>  
        /// <param name="strSheetName">工作表名称</param>  
        /// <Author>CallmeYhz 2015-11-26 10:13:09</Author>     
        //public static void Export(DataTable dtSource, string strHeaderText, string strSheetName)
        //{
        //    if (strSheetName == "")
        //    {
        //        strSheetName = "Sheet";

        //    }
        //    #region 选择文件保存路径和文件名
        //    SaveFileDialog SaveFile = new SaveFileDialog();
        //    string localFilePath = "";
        //    string[] oldColumnNames;
        //    string[] newColumnNames;

        //    if (dtSource == null)
        //    {
        //        MessageBox.Show("为找到要导出的数据表!", "系统信息");
        //        return;
        //    }
        //    oldColumnNames = new string[dtSource.Columns.Count];
        //    for (int i = 0; i < dtSource.Columns.Count; i++)
        //    {
        //        oldColumnNames[i] = dtSource.Columns[i].ColumnName.ToString();
        //    }
        //    newColumnNames = oldColumnNames;
        //    //设置文件类型
        //    SaveFile.Filter = "Miscrosoft Office Excel 97-2003 工作表|*.xls|Miscrosoft Office Excel 2007 工作表|*.xlsx|所有文件(*.*)|*.*";
        //    //设置默认文件类型显示顺序
        //    SaveFile.FilterIndex = 1;
        //    //保存对话框是否记忆上次打开的目录
        //    SaveFile.RestoreDirectory = true;
        //    //设置默认文件名
        //    //SaveFile.FileName = FileName;

        //    if (SaveFile.ShowDialog() == DialogResult.OK)
        //    {
        //        //获取文件路径，带文件名
        //        localFilePath = SaveFile.FileName.ToString(); //获得文件路径

        //        //using (MemoryStream ms = Export(dtSource, strHeaderText, strSheetName, oldColumnNames, newColumnNames))
        //        //{
        //        //    using (FileStream fs = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
        //        //    {
        //        //        byte[] data = ms.ToArray();
        //        //        fs.Write(data, 0, data.Length);
        //        //        fs.Flush();
        //        //    }
        //        //}
        //        MessageBox.Show("导出数据成功!", "系统信息");
        //    }
        //    #endregion
        //}
        #endregion
    }
}
