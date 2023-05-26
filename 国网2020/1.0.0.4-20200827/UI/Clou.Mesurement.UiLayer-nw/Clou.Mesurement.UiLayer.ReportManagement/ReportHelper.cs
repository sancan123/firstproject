using Mesurement.UiLayer.DataManager.Mark.ViewModel;
using Mesurement.UiLayer.ViewModel;
using Mesurement.UiLayer.ViewModel.CheckInfo;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Mesurement.UiLayer.DataManager.ViewModel.Meters;
using Mesurement.UiLayer.ViewModel.InputPara;
using Mesurement.UiLayer.DataManager.ViewModel;
using Mesurement.UiLayer.DataManager.ViewModel.Mark;
using System.Runtime.InteropServices;


namespace Mesurement.UiLayer.DataManager
{
    /// <summary>
    /// 报表帮助类:将电能表的数据打印到模板中,并另存到目标文件夹
    /// </summary>
    class ReportHelper
    {
        private static Word.Application wordApp = null;

        private static Excel.Application excelApp = null;
        /// <summary>
        /// office应用程序
        /// </summary>
        public static Word.Application WordApp
        {
            get
            {
                if (wordApp == null)
                {
                    wordApp = new Word.Application();
                }
                return wordApp;
            }
        }
        public static Excel.Application ExcelApp
        {
            get
            {
                if (excelApp == null)
                {
                    excelApp = new Excel.Application();
                }
                return excelApp;
            }
        }
        /// <summary>
        /// 打印报表
        /// </summary>
        /// <param name="meters">表信息</param>
        public static void PrintReport(DynamicViewModel meter)
        {
            string fileName = string.Format(@"{0}\ReportTemplate\{1}", Directory.GetCurrentDirectory(), Properties.Settings.Default.ReportPath);
            if (fileName.EndsWith(".doc") || fileName.EndsWith(".docx"))
            {
            #region word报表           
            #region 复制到临时文件
            string destPath = string.Format(@"{0}\Reports", Directory.GetCurrentDirectory());
            string destFileName = string.Format(@"{0}\temp.doc", destPath);
            try
            {
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                File.Copy(fileName, destFileName, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format(@"复制报表模板到文件:{0}失败,失败原因:{1}", destFileName, e.Message));
            }
            #endregion
            #region 打开word文件
            //防止用户关闭word,重新创建
            try
            {
                WordApp.Documents.Add(destFileName);
                if (Properties.Settings.Default.IsPreview)
                {
                    WordApp.Visible = true;
                }
            }
            catch
            {
                wordApp = new Word.Application();
                WordApp.Documents.Add(destFileName);
                if (Properties.Settings.Default.IsPreview)
                {
                    WordApp.Visible = true;
                }
            }
            #endregion
            #region 加载并解析所有书签
            List<int> indexList = new List<int>();
            Word.Document document = WordApp.ActiveDocument;
            List<BookmarkModel> bookmarks = new List<BookmarkModel>();
            foreach (Word.Bookmark bookmark in document.Bookmarks)
            {
                BookmarkModel markModel = new BookmarkModel(bookmark.Name);
                bookmarks.Add(markModel);
                int indexTemp = 0;
                if (int.TryParse(markModel.DeviceName.Replace("表", ""), out indexTemp))
                {
                    if (!indexList.Contains(indexTemp))
                    {
                        indexList.Add(indexTemp);
                    }
                }
            }
            int maxIndex = indexList.Max();
            #endregion
            #region 解析所有书签并赋值
            string meterPk = meter.GetProperty("PK_LNG_METER_ID") as string;
            string equipNo = meter.GetProperty("AVR_DEVICE_ID") as string;
            DynamicViewModel equipInfo = Equipments.Instance.FindEquipInfo(equipNo);
            OneMeterResult meterResultTemp = new OneMeterResult(meterPk, false);
            foreach (BookmarkModel bookmark in bookmarks)
            {
                if (bookmark.DeviceName == "不识别")
                {
                    continue;
                }
                else if (bookmark.DeviceName == "检定台")
                {
                    #region 检定台信息
                    string valueTemp = equipInfo.GetProperty(bookmark.ResultName) as string;
                    SetBookmarkValue(document, bookmark.Name, valueTemp, bookmark.Format);
                    #endregion
                }
                else if (bookmark.DeviceName == "电能表")
                {
                    #region 电表信息
                    InputParaUnit paraUnit = MetersViewModel.ParasModel.AllUnits.FirstOrDefault(item => item.DisplayName == bookmark.ResultName);
                    object objTemp = null;
                    string valueTemp = "";
                    if (paraUnit != null)
                    {
                        objTemp = meter.GetProperty(paraUnit.FieldName);
                    }
                    if (objTemp != null)
                    {
                        valueTemp = objTemp.ToString();
                    }
                    SetBookmarkValue(document, bookmark.Name, valueTemp, bookmark.Format);
                    #endregion
                }
                else
                {
                    #region 解析检定结论
                    int indexTemp = 0;
                    if (int.TryParse(bookmark.DeviceName.Replace("表", ""), out indexTemp))
                    {
                        indexTemp = indexTemp - 1;
                    }
                    if(bookmark.ResultName== "总结论")
                    {
                        SetBookmarkValue(document, bookmark.Name, GetResultSummaray(meterResultTemp, bookmark.ItemKey), bookmark.Format);
                    }
                    else
                    {
                        DynamicViewModel modelTemp = GetResultModel(meterResultTemp, bookmark.ItemKey);
                        //书签的值
                        string markValueTemp = "";
                        if (modelTemp != null)
                        {
                            markValueTemp = modelTemp.GetProperty(bookmark.ResultName) as string;
                        }
                        //转到书签并给书签赋值
                        SetBookmarkValue(document, bookmark.Name, markValueTemp, bookmark.Format);
                    }
                    #endregion
                }
            }
            #endregion
            if (!Properties.Settings.Default.IsPreview)
            {
                try
                {
                    document.PrintOut();
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("打印报表文件出错:{0}", e.Message));
                }
                document.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
            }
            #endregion
            }
            else if (fileName.EndsWith(".xls") || fileName.EndsWith(".xlsx"))
            { 
            #region excel报表
            #region 复制到临时文件
            string destPath = string.Format(@"{0}\Reports", Directory.GetCurrentDirectory());
            string destFileName = string.Format(@"{0}\temp.xls", destPath);
            try
            {
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                File.Copy(fileName, destFileName, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format(@"复制报表模板到文件:{0}失败,失败原因:{1}", destFileName, e.Message));
            }
            #endregion
            #region 打开excel文件
            //防止用户关闭word,重新创建
            try
            {
                ExcelApp.Workbooks.Add(destFileName);
                if (Properties.Settings.Default.IsPreview)
                {
                    ExcelApp.Visible = true;
                }
            }
            catch
            {
                excelApp = new Excel.Application();
                ExcelApp.Workbooks.Add(destFileName);
                if (Properties.Settings.Default.IsPreview)
                {
                    ExcelApp.Visible = true;
                }
            }
            #endregion
            #region 加载并解析所有书签
            List<int> indexList = new List<int>();
            Excel.Workbook workbook = excelApp.ActiveWorkbook;
            List<BookmarkModel> bookmarks = new List<BookmarkModel>();
            foreach (Excel.Name name in workbook.Names)
            {
                BookmarkModel markModel = new BookmarkModel(name.Name);
                bookmarks.Add(markModel);
                int indexTemp = 0;
                if (int.TryParse(markModel.DeviceName.Replace("表", ""), out indexTemp))
                {
                    if (!indexList.Contains(indexTemp))
                    {
                        indexList.Add(indexTemp);
                    }
                }
            }
            int maxIndex = indexList.Max();
            #endregion
            #region 解析所有书签并赋值
            string meterPk = meter.GetProperty("PK_LNG_METER_ID") as string;
            string equipNo = meter.GetProperty("AVR_DEVICE_ID") as string;
            DynamicViewModel equipInfo = Equipments.Instance.FindEquipInfo(equipNo);
            OneMeterResult meterResultTemp = new OneMeterResult(meterPk, false);
            foreach (BookmarkModel bookmark in bookmarks)
            {
                if (bookmark.DeviceName == "不识别")
                {
                    continue;
                }
                else if (bookmark.DeviceName == "检定台")
                {
                    #region 检定台信息
                    string valueTemp = equipInfo.GetProperty(bookmark.ResultName) as string;
                    SetCellValue(workbook, bookmark.Name, valueTemp, bookmark.Format);
                    #endregion
                }
                else if (bookmark.DeviceName == "电能表")
                {
                    #region 电表信息
                    InputParaUnit paraUnit = MetersViewModel.ParasModel.AllUnits.FirstOrDefault(item => item.DisplayName == bookmark.ResultName);
                    object objTemp = null;
                    string valueTemp = "";
                    if (paraUnit != null)
                    {
                        objTemp = meter.GetProperty(paraUnit.FieldName);
                    }
                    if (objTemp != null)
                    {
                        valueTemp = objTemp.ToString();
                    }
                    SetCellValue(workbook, bookmark.Name, valueTemp, bookmark.Format);
                    #endregion
                }
                else
                {
                    #region 解析检定结论
                    int indexTemp = 0;
                    if (int.TryParse(bookmark.DeviceName.Replace("表", ""), out indexTemp))
                    {
                        indexTemp = indexTemp - 1;
                    }
                    if (bookmark.ResultName == "总结论")
                    {
                        SetCellValue(workbook, bookmark.Name, GetResultSummaray(meterResultTemp, bookmark.ItemKey), bookmark.Format);
                    }
                    else
                    {
                        DynamicViewModel modelTemp = GetResultModel(meterResultTemp, bookmark.ItemKey);
                        //书签的值
                        string markValueTemp = "";
                        if (modelTemp != null)
                        {
                            markValueTemp = modelTemp.GetProperty(bookmark.ResultName) as string;
                        }
                        //转到书签并给书签赋值
                        SetCellValue(workbook, bookmark.Name, markValueTemp, bookmark.Format);
                    }
                    #endregion
                }
            }
            #endregion
            if (!Properties.Settings.Default.IsPreview)
            {
                try
                {
                    workbook.PrintOut();
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("打印报表文件出错:{0}", e.Message));
                }
                workbook.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
            }
            #endregion
            }
        }

        /// <summary>
        /// 打印报表
        /// </summary>
        /// <param name="meters">表信息</param>
        public static void PrintReport(List<DynamicViewModel> meter)
        {
            #region 复制到临时文件
            string fileName = string.Format(@"{0}\ReportTemplate\{1}", Directory.GetCurrentDirectory(), Properties.Settings.Default.ReportPath);
            string destPath = string.Format(@"{0}\Reports", Directory.GetCurrentDirectory());
            string destFileName = string.Format(@"{0}\temp.doc", destPath);
            try
            {
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                File.Copy(fileName, destFileName, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format(@"复制报表模板到文件:{0}失败,失败原因:{1}", destFileName, e.Message));
            }
            #endregion
            #region 打开word文件
            //防止用户关闭word,重新创建
            try
            {
                WordApp.Documents.Add(destFileName);
                if (Properties.Settings.Default.IsPreview)
                {
                    WordApp.Visible = true;
                }
            }
            catch
            {
                wordApp = new Word.Application();
                WordApp.Documents.Add(destFileName);
                if (Properties.Settings.Default.IsPreview)
                {
                    WordApp.Visible = true;
                }
            }
            #endregion
            #region 加载并解析所有书签
            List<int> indexList = new List<int>();
            Word.Document document = WordApp.ActiveDocument;
            List<BookmarkModel> bookmarks = new List<BookmarkModel>();
            int MeterIndexTemp = 0;
            foreach (Word.Bookmark bookmark in document.Bookmarks)
            {
                BookmarkModel markModel = new BookmarkModel(bookmark.Name);
                //bookmarks.Add(markModel);
                //int indexTemp = 0;
                //if (int.TryParse(markModel.DeviceName.Replace("表", ""), out indexTemp))
                //{
                //}
                string strMeterIndex =bookmark.Name.Replace("VX","^").Split('^')[0];
                if (!string.IsNullOrEmpty(strMeterIndex) && strMeterIndex.StartsWith("表"))
                {
                    int.TryParse(strMeterIndex.Replace("表", ""), out MeterIndexTemp);
                    if (MeterIndexTemp <= meter.Count)
                    {
                        MeterIndexTemp = MeterIndexTemp - 1;
                    }
                    else
                    {
                        SetBookmarkValue(document, bookmark.Name, "/", markModel.Format);
                        continue;
                    }
                }

                string meterPk = meter[MeterIndexTemp].GetProperty("PK_LNG_METER_ID") as string;
                string equipNo = meter[MeterIndexTemp].GetProperty("AVR_DEVICE_ID") as string;
                DynamicViewModel equipInfo = Equipments.Instance.FindEquipInfo(equipNo);
                OneMeterResult meterResultTemp = new OneMeterResult(meterPk, false);

                if (markModel.DeviceName == "不识别")
                {
                    continue;
                }
                else if (markModel.DeviceName == "检定台")
                {
                    #region 检定台信息
                    string valueTemp = equipInfo.GetProperty(markModel.ResultName) as string;
                    SetBookmarkValue(document, bookmark.Name, valueTemp, markModel.Format);
                    #endregion
                }
                else if (markModel.DeviceName == "电能表")
                {
                    #region 电表信息
                    InputParaUnit paraUnit = MetersViewModel.ParasModel.AllUnits.FirstOrDefault(item => item.DisplayName == markModel.ResultName);
                    object objTemp = null;
                    string valueTemp = "";
                    if (paraUnit != null)
                    {
                        objTemp = meter[MeterIndexTemp].GetProperty(paraUnit.FieldName);
                    }
                    if (objTemp != null)
                    {
                        valueTemp = objTemp.ToString();
                    }
                    SetBookmarkValue(document, bookmark.Name, valueTemp, markModel.Format);
                    #endregion
                }
                else if (markModel.DeviceName == "已选表")
                {
                    #region 解析综合结论
                    if (markModel.ResultName == "综合结论")
                    {
                        string comprehensiveConclusion = "";
                        for (int i = 0; i < meter.Count; i++)
                        {
                            string meterPkTmp = meter[i].GetProperty("PK_LNG_METER_ID") as string;
                            comprehensiveConclusion = GetResultSummaray(new OneMeterResult(meterPkTmp, false), markModel.ItemKey);
                            if (comprehensiveConclusion != "合格")
                            {
                                break;
                            }                            
                        }
                        SetBookmarkValue(document, bookmark.Name, comprehensiveConclusion, markModel.Format);
                    }
                    #endregion
                }
                else
                {
                    #region 解析检定结论
                    //int indexTemp = 0;
                    //if (int.TryParse(bookmark.DeviceName.Replace("表", ""), out indexTemp))
                    //{
                    //    indexTemp = indexTemp - 1;
                    //}
                    if (markModel.ResultName == "总结论")
                    {
                        SetBookmarkValue(document, bookmark.Name, GetResultSummaray(meterResultTemp, markModel.ItemKey), markModel.Format);
                    }
                    else
                    {
                        DynamicViewModel modelTemp = GetResultModel(meterResultTemp, markModel.ItemKey);
                        //书签的值
                        string markValueTemp = "";
                        if (modelTemp != null)
                        {
                            markValueTemp = modelTemp.GetProperty(markModel.ResultName) as string;
                        }
                        //转到书签并给书签赋值
                        SetBookmarkValue(document, bookmark.Name, markValueTemp, markModel.Format);
                    }
                    #endregion
                }
            }
            #endregion
            if (!Properties.Settings.Default.IsPreview)
            {
                try
                {
                    document.PrintOut();
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("打印报表文件出错:{0}", e.Message));
                }
                document.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
            }
        }


        /// <summary>
        /// 打印Excel结论
        /// </summary>
        /// <param name="meters">表信息集合</param>
        public static void PrintExelConclusion(List<DynamicViewModel> meter)
        {
            #region 获取存储文件位置
            string destPath = string.Format(@"{0}\Reports", Directory.GetCurrentDirectory());
            string destFileName = string.Format(@"{0}\tempConclusion.xls", destPath);
            #endregion
            #region 创建excel文件
            excelApp = new Excel.Application();
            Excel.Workbook wbook = excelApp.Workbooks.Add(true);
            Excel.Worksheet Sheet = wbook.ActiveSheet as Excel.Worksheet;
            excelApp.DisplayAlerts = false;
            Sheet.Name = "结论";
            #endregion
            #region 生成列
            int conlumnIndex = 1;

            Sheet.Cells[1, conlumnIndex++].Value = "表地址";

            Sheet.Cells[1, conlumnIndex++].Value = "电表型号";
 
            Sheet.Cells[1, conlumnIndex++].Value = "生产厂家";

            string meterPkTmp = meter[0].GetProperty("PK_LNG_METER_ID") as string;
            OneMeterResult meterResultTemp = new OneMeterResult(meterPkTmp, false);
            List<string> conlumnNameList = new List<string>();
            for (int i = 0; i < meter.Count; i++)
            {
                meterPkTmp = meter[i].GetProperty("PK_LNG_METER_ID") as string;
                meterResultTemp = new OneMeterResult(meterPkTmp, false);
                for (int j = 0; j < meterResultTemp.Categories.Count; j++)
                {
                    string nameTmp = meterResultTemp.Categories[j].ResultUnits[0].GetProperty("项目名").ToString();
                    if (!conlumnNameList.Contains(nameTmp))
                    {
                        conlumnNameList.Add(nameTmp);
                    }
                    //else
                    //    MessageBox.Show("发现重复项" + nameTmp);
                }
            }
            for (int i = 0; i < conlumnNameList.Count; i++)
            {
                Sheet.Cells[1, conlumnIndex++].Value = conlumnNameList[i];
                Sheet.Cells[1, conlumnIndex++].Value = conlumnNameList[i] + "(不合格原因)";
            }
            #endregion
            #region 为每块表填充数据           
            int RowIndex = 2;
            for (int i = 0; i < meter.Count; i++)
            {
                meterPkTmp = meter[i].GetProperty("PK_LNG_METER_ID") as string;
                meterResultTemp = new OneMeterResult(meterPkTmp, false);
                RowIndex = CreateOneMeterExcelRowForMutichip(meterResultTemp, Sheet, conlumnNameList, RowIndex);
            }
            #endregion
            #region 保存临时文件及结束后台进程
            //-4143保存为Excel97-2003                
            wbook.SaveAs(destFileName, -4143);
            wbook.Close();
            if (wbook != null)
                wbook = null;
            KillExcel(excelApp);                      
            #endregion                       
        }

        /// <summary>
        /// 创建每块表的数据
        /// </summary>
        /// <param name="meterResult">多芯表数据</param>
        /// <param name="sheet">工作表</param>
        /// <param name="colNameList">项目名集合</param>
        /// <param name="rowIndex">行号</param>
        /// <returns></returns>
        private static int CreateOneMeterExcelRowForMutichip(OneMeterResult meterResult, Excel.Worksheet sheet, List<string> colNameList, int rowIndex)
        {
            int intTxm = rowIndex;
            Excel.Range myrange = sheet.Cells.Range["A1", "A1000"];
            //设置单元格格式为文本
            myrange.NumberFormatLocal = "@";
            int conlumnIndex = 1;

            sheet.Cells[rowIndex, conlumnIndex++].Value = meterResult.MeterInfo.GetProperty("AVR_ADDRESS").ToString();
            sheet.Cells[rowIndex, conlumnIndex++].Value = meterResult.MeterInfo.GetProperty("AVR_METER_MODEL").ToString();
            sheet.Cells[rowIndex, conlumnIndex++].Value = meterResult.MeterInfo.GetProperty("AVR_FACTORY").ToString();

            for (int i = 0; i < meterResult.Categories.Count; i++)
            {
                string nameTmp = meterResult.Categories[i].ResultUnits[0].GetDataSource().GetProperty("项目名").ToString();
                int colIndex = 1 + colNameList.IndexOf(nameTmp) * 2 + 3;//集合索引从0开始需要加1，每个项目有项目名和不合格原因占2列，前面还有3列
                string conclusion = meterResult.Categories[i].ResultUnits[0].GetDataSource().GetProperty("结论").ToString();
                sheet.Cells[rowIndex, colIndex].Value = conclusion;
                conlumnIndex++;
                string filedReason = meterResult.Categories[i].ResultUnits[0].GetDataSource().GetProperty("不合格原因").ToString();
                sheet.Cells[rowIndex, colIndex+1].Value = filedReason;
                conlumnIndex++;
            }
            rowIndex++;
            Excel.Range RangeAll = sheet.Range[sheet.Cells[intTxm, 1], sheet.Cells[rowIndex - intTxm, colNameList.Count * 2 + 3]];
            RangeAll.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            return rowIndex;
        }


        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        /// <summary>
        /// 退出Excel
        /// </summary>
        private static void KillExcel(Excel.Application excelApp)
        {
            if (excelApp != null)
                excelApp.Quit();
            GC.Collect();  // 回收资源

            IntPtr t = new IntPtr(excelApp.Hwnd);   //得到这个句柄，具体作用是得到这块内存入口 
            int k = 0;
            GetWindowThreadProcessId(t, out k);   //得到本进程唯一标志k
            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k);   //得到对进程k的引用
            p.Kill();     //关闭进程k
        }


        /// <summary>
        /// 给书签设置值
        /// </summary>
        /// <param name="markName"></param>
        /// <param name="valueTemp"></param>
        private static void SetBookmarkValue(Word.Document document, string markName, string valueTemp, EnumFormat formatTemp = EnumFormat.无)
        {
            //转到书签并给书签赋值
            Word.Bookmark markTemp = null;
            foreach (Word.Bookmark mark in document.Bookmarks)
            {
                if (mark.Name == markName)
                {
                    markTemp = mark;
                    break;
                }
            }
            if (markTemp != null)
            {
                markTemp.Range.Select();
                markTemp.Range.Text = AnalysisText(valueTemp, formatTemp);
            }
        }



        /// <summary>
        /// 给单元格设置值
        /// </summary>
        /// <param name="markName"></param>
        /// <param name="valueTemp"></param>
        private static void SetCellValue(Excel.Workbook workbook, string cellName, string valueTemp, EnumFormat formatTemp = EnumFormat.无)
        {
            //转到书签并给书签赋值
            Excel.Name cellTemp = null;
            foreach (Excel.Name name in workbook.Names)
            {
                if (name.Name == cellName)
                {
                    cellTemp = name;
                    break;
                }
            }
            if (cellTemp != null)
            {
                //cellTemp.Value.Range.Select();
                cellTemp.RefersToRange.Select();
                //设置单元格格式为文本
                cellTemp.RefersToRange.NumberFormatLocal = "@";                
                cellTemp.RefersToRange.Value = AnalysisText(valueTemp, formatTemp);
            }
        }

        /// <summary>
        /// 获取对应编号的检定结论
        /// </summary>
        /// <param name="meterResult"></param>
        /// <param name="itemKey"></param>
        /// <returns></returns>
        private static DynamicViewModel GetResultModel(OneMeterResult meterResult, string itemKey)
        {
            DynamicViewModel modelResult = null;
            for (int i = 0; i < meterResult.Categories.Count; i++)
            {
                modelResult = meterResult.Categories[i].ResultUnits.FirstOrDefault(item => item.GetProperty("项目号") as string == itemKey);
                if (modelResult != null)
                {
                    break;
                }
            }
            return modelResult;
        }
        /// <summary>
        /// 获取类别的总结论
        /// </summary>
        /// <param name="meterResult"></param>
        /// <param name="paraNo"></param>
        /// <returns></returns>
        private static string GetResultSummaray(OneMeterResult meterResult,string paraNo)
        {
            string stringResult = "合格";
            bool havePoint = false;
            for (int i = 0; i < meterResult.Categories.Count; i++)
            {
                var models = meterResult.Categories[i].ResultUnits.Where(item => (item.GetProperty("项目号") as string)!=null && (item.GetProperty("项目号") as string).StartsWith(paraNo));
                if(models != null)
                {
                    foreach(DynamicViewModel modelTemp in models)
                    {
                        havePoint = true;
                        if(modelTemp.GetProperty("结论") as string=="不合格")
                        {
                            stringResult = "不合格";
                            return stringResult;
                        }
                        else if(modelTemp.GetProperty("结论") as string!="合格")
                        {
                            stringResult = "";
                        }
                    }
                }
            }
            if(!havePoint)
            {
                stringResult = "";
            }
            return stringResult;
        }

        #region 解析数据格式
        /// <summary>
        /// 根据数据格式解析文本
        /// </summary>
        /// <param name="valueTemp">要解析的文本</param>
        /// <param name="formatTemp">格式类型</param>
        /// <returns></returns>
        private static string AnalysisText(string valueTemp, EnumFormat formatTemp)
        {
            string valueResult = valueTemp;
            if (valueResult == "")
            {
                valueResult = "/";
                return valueResult;
            }

            switch (formatTemp)
            {
                case EnumFormat.年:
                case EnumFormat.月:
                case EnumFormat.日:
                    valueResult = AnalysisTimeText(valueTemp, formatTemp);
                    break;
                case EnumFormat.额定电流:
                case EnumFormat.最大电流:
                case EnumFormat.有功等级:
                case EnumFormat.无功等级:
                case EnumFormat.有功常数:
                case EnumFormat.无功常数:
                    valueResult = AnalysisCurrentText(valueTemp, formatTemp);
                    break;
                case EnumFormat.格式化电压:
                    valueResult = AnalysisVoltageText(valueTemp, formatTemp);
                    break;
                case EnumFormat.符合要求:
                case EnumFormat.符合技术条件要求:
                    valueResult = AnalysisConclusionText(valueTemp, formatTemp);
                    break;
            }
            return valueResult;
        }
        /// <summary>
        /// 解析日期格式
        /// </summary>
        /// <param name="valueTemp"></param>
        /// <param name="formatTemp"></param>
        /// <returns></returns>
        private static string AnalysisTimeText(string valueTemp, EnumFormat formatTemp)
        {
            DateTime timeTemp = DateTime.Now;
            if (DateTime.TryParse(valueTemp, out timeTemp))
            {
                switch (formatTemp)
                {
                    case EnumFormat.年:
                        return timeTemp.ToString("yyyy");
                    case EnumFormat.月:
                        return timeTemp.ToString("MM");
                    case EnumFormat.日:
                        return timeTemp.ToString("dd");
                }
            }
            return valueTemp;
        }
        /// <summary>
        /// 解析电流格式
        /// </summary>
        /// <param name="valueTemp"></param>
        /// <param name="formatTemp"></param>
        /// <returns></returns>
        private static string AnalysisCurrentText(string valueTemp, EnumFormat formatTemp)
        {
            if (valueTemp == null)
            {
                return "";
            }
            string[] arrayTemp = valueTemp.Split('(');
            if (arrayTemp.Length == 2)
            {
                if (formatTemp == EnumFormat.额定电流 || formatTemp==EnumFormat.有功等级 || formatTemp == EnumFormat.有功常数)
                {
                    return arrayTemp[0];
                }
                else
                {
                    return arrayTemp[1].Replace(")", ""); 
                }
            }
            return valueTemp;
        }

        /// <summary>
        /// 解析电压格式带最大电压
        /// </summary>
        /// <param name="valueTemp"></param>
        /// <param name="formatTemp"></param>
        /// <returns></returns>
        private static string AnalysisVoltageText(string valueTemp, EnumFormat formatTemp)
        {
            if (!string.IsNullOrEmpty(valueTemp))
            {
                if (valueTemp == "57.7")
                {
                    valueTemp = "57.7/100";
                }
                else if (valueTemp == "220")
                {
                    valueTemp = "220/380";
                }
            }
            return valueTemp;
        }

        /// <summary>
        /// 解析结论格式
        /// </summary>
        /// <param name="valueTemp"></param>
        /// <param name="formatTemp"></param>
        /// <returns></returns>
        private static string AnalysisConclusionText(string valueTemp, EnumFormat formatTemp)
        {
            if (!string.IsNullOrEmpty(valueTemp))
            {
                if (valueTemp == "合格")
                {
                    valueTemp = formatTemp.ToString();
                }
                else if (valueTemp == "不合格")
                {
                    valueTemp = "不" + formatTemp.ToString();
                }
            }
            return valueTemp;
        }
        #endregion
    }
}
