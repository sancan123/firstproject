using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataHelper
{
    public class SearchHelper
    {
        List<string> searchedFileName = new List<string>();

        OracleHelper oracleHelper;
        /// <summary>
        /// 搜索文件
        /// </summary>
        /// <param name="file">要搜索的文件名</param>
        /// <returns></returns>
        public DataModel SearchFile(string file)
        {
            DataModel dataModel = null;
            DateTime startTime = DateTime.Now;
            while (true)
            {
                try
                {
                    string fileName = file.Substring(file.LastIndexOf('\\') + 1);
                    if (!fileName.StartsWith("Receive")) continue;
                    dataModel = DataModel.Load(file) as DataModel;
                    if (dataModel != null)
                    {
                        DeleteFile(file);
                        break;
                    }
                    if (DateTime.Now.Subtract(startTime).TotalMilliseconds >= App.BasicSetting.WaitingForSearchTime * 1000) break;
                    Thread.Sleep(App.BasicSetting.SearchInterval);
                    //if (DateTime.Now.Subtract(startTime).TotalMilliseconds >= 180* 1000) break;
                    //Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteErrorLog(ex.Message + "\r\n" + ex.StackTrace);
                    dataModel = new DataModel(ex.Message + "\r\n" + ex.StackTrace);
                    break;
                }
            }
            if (dataModel == null)
            {
                if (App.BasicSetting.AgencyModel == Agency.正向代理)
                    dataModel = new DataModel("摆渡超时，正向隔离装置接收文件异常，请与隔离装置管理人员联系！");
            }
            return dataModel;
        }
        /// <summary>
        /// 正向发送线程函数
        /// </summary>
        public void ForwardSendWork()
        {
            string[] files = null;
            while (true)
            {
                try
                {
                    files = Directory.GetFileSystemEntries(string.Format(@"{0}\Tmp", App.AppPath), "*.dat");
                    foreach (string file in files)
                    {
                        string fileName = file.Substring(file.LastIndexOf('\\') + 1);
                        if (!fileName.StartsWith("Send")) continue;
                        CopyFile(file, string.Format(@"{0}\{1}", App.BasicSetting.SendDirectory, fileName), true);
                        RefreshMsg(true, fileName);
                        searchedFileName.Add(fileName);
                        DeleteFile(file);
                    }
                    Thread.Sleep(App.BasicSetting.SearchInterval);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteErrorLog(ex.Message + "\r\n" + ex.StackTrace);
                }
            }
        }
        /// <summary>
        /// 正向接收线程函数
        /// </summary>
        public void ForwardReceiveWork()
        {
            string[] files = null;
            while (true)
            {
                try
                {
                    files = Directory.GetFileSystemEntries(App.BasicSetting.ReceiveDirectory, "*.dat");
                    foreach (string file in files)
                    {
                        string fileName = file.Substring(file.LastIndexOf('\\') + 1);
                        if (!fileName.StartsWith("Receive")) continue;
                        CopyFile(file, string.Format(@"{0}\Tmp\{1}", App.AppPath, fileName), true);
                        RefreshMsg(false, fileName);
                        searchedFileName.Add(fileName);
                        DeleteFile(file);
                    }
                    Thread.Sleep(App.BasicSetting.SearchInterval);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteErrorLog(ex.Message + "\r\n" + ex.StackTrace);
                }
            }
        }



        /// <summary>
        /// 反向接收线程函数
        /// </summary>
        public void ReverseReceiveWork()
        {
            string[] files = null;
            while (true)
            {
                try
                {
                    files = Directory.GetFileSystemEntries(App.BasicSetting.ReceiveDirectory, "*.dat");
                    foreach (string file in files)
                    {
                        string fileName = file.Substring(file.LastIndexOf('\\') + 1);
                        if (!fileName.StartsWith("Send")) continue;
                        CopyFile(file, string.Format(@"{0}\Tmp\{1}", App.AppPath, fileName), true);
                        RefreshMsg(false, fileName);
                        searchedFileName.Add(fileName);
                        DeleteFile(file);
                    }
                    Thread.Sleep(App.BasicSetting.SearchInterval);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteErrorLog(ex.Message + "\r\n" + ex.StackTrace);
                }
            }
        }
        /// <summary>
        /// 反向发送线程函数
        /// </summary>
        public void ReverseSendWork()
        {
            DataModel dataModel = null;
            string receivedFileName = string.Empty;
            string ReadyToSendFileName = string.Empty;
            while (true)
            {
                try
                {
                    if (searchedFileName.Count <= 0) continue;
                    receivedFileName = searchedFileName.Last();
                    dataModel = DataModel.Load(string.Format(@"{0}\Tmp\{1}", App.AppPath, receivedFileName)) as DataModel;
                    if (dataModel == null) continue;
                    searchedFileName.Remove(searchedFileName.Last());
                    DeleteFile(string.Format(@"{0}\Tmp\{1}", App.AppPath, receivedFileName));
                    ReadyToSendFileName = receivedFileName.Replace("Send", "Receive");





                    if (App.BasicSetting.IsOpenProxy)
                    {
                        if (dataModel.downModel != null)
                        {
                            if (dataModel.downModel.misPara == null) continue;
                            LogHelper.WriteRunLog("开始下载电能表信息：\r\n");
                            LogHelper.WriteRunLog("开始执行中间库操作：" + dataModel.downModel.Sql + "\r\n");
                            oracleHelper = new OracleHelper(dataModel.downModel.misPara.Ip, dataModel.downModel.misPara.Port, dataModel.downModel.misPara.DataSource, dataModel.downModel.misPara.UserId, dataModel.downModel.misPara.Password, dataModel.downModel.misPara.WebServiceURL);
                            //Action action = () =>
                            //{
                                DataTable dt = oracleHelper.ExecuteReader(dataModel.downModel.Sql);
                                LogHelper.WriteRunLog("结束中间库查询操作：" + "\r\n");
                                dataModel.downModel.dateTableToList = FormatHelper.DataTableToList(dt);
                            //};
                            //Task task1 = TaskEx.Run(action);
                            //await task1;

                        }
                        else if (dataModel.uploadModel != null)
                        {
                            if (dataModel.uploadModel.misPara == null) continue;
                            if (dataModel.uploadModel.RlstDic == null)
                                dataModel.uploadModel.RlstDic = new Dictionary<string, string>();
                            oracleHelper = new OracleHelper(dataModel.uploadModel.misPara.Ip, dataModel.uploadModel.misPara.Port, dataModel.uploadModel.misPara.DataSource, dataModel.uploadModel.misPara.UserId, dataModel.uploadModel.misPara.Password, dataModel.uploadModel.misPara.WebServiceURL);
                            LogHelper.WriteRunLog("开始上传电能表信息：\r\n");

                            foreach (string key in dataModel.uploadModel.DataDic.Keys)
                            {
                                LogHelper.WriteRunLog("开始执行中间库写入操作：" + "\r\n");
                                string result = oracleHelper.Execute(dataModel.uploadModel.DataDic[key]);
                                dataModel.uploadModel.RlstDic[key] = result;
                                LogHelper.WriteRunLog("结束中间库写入操作：" + "\r\n");
                            }
                            if (dataModel.uploadModel.WebDic == null)
                                dataModel.uploadModel.WebDic = new Dictionary<string, List<WebPara>>();
                            LogHelper.WriteRunLog("开始调用平台接口：\r\n");

                            //Action action2 = ()=>{
                                foreach (string key in dataModel.uploadModel.WebDic.Keys)
                                {
                                    foreach (WebPara webPara in dataModel.uploadModel.WebDic[key])
                                    {

                                        LogHelper.WriteRunLog("开始调用平台接口：" + webPara.webserviceURL + "方法名：" + webPara.interfaceName + "\r\n");
                                        object result = WebServiceHelper.InvokeWebService2(webPara.webserviceURL,"", webPara.interfaceName, new string[] { webPara.inputPara });
                                        webPara.returnPara = result.ToString();
                                        LogHelper.WriteRunLog("结束调用平台接口：" + webPara.webserviceURL + "方法名：" + webPara.interfaceName + "\r\n");
                                    }
                                }
                                LogHelper.WriteRunLog("结束调用平台接口：\r\n");
                            //};
                            //Task task2 = TaskEx.Run(action2);
                            //await task2;
                        }
                        else if (dataModel.applyModel != null)
                        {
                            if (dataModel.applyModel.webPara != null)
                            {
                                object result = WebServiceHelper.InvokeWebService(dataModel.applyModel.webPara.webserviceURL, "", dataModel.applyModel.webPara.interfaceName, new string[] { dataModel.applyModel.webPara.inputPara });
                                dataModel.applyModel.webPara.returnPara = result.ToString();
                            }
                        }
                    }
                    LogHelper.WriteRunLog("开始将中间库和平台接口返回的数据重新写入数据文件并返回给正向服务器：\r\n");
                    //Action action3 = () => {
                        dataModel.Save(string.Format(@"{0}\{1}", App.BasicSetting.SendDirectory, ReadyToSendFileName));
                        RefreshMsg(true, ReadyToSendFileName);
                    //};
                    //Task task3= TaskEx.Run(action3);
                    //await task3;
                    Thread.Sleep(App.BasicSetting.SearchInterval);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteErrorLog(ex.Message + "\r\n" + ex.StackTrace);
                }
            }
        }
        /// <summary>
        /// 刷新消息
        /// </summary>
        /// <param name="isSend">true-发送，false-接收</param>
        /// <param name="fileName">发送或接收的文件名称</param>
        public void RefreshMsg(bool isSend, string fileName)
        {
            App.Message.ShowMsg(isSend, fileName);
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="file"></param>
        public void DeleteFile(string file)
        {
            if (File.GetAttributes(file) != FileAttributes.Normal)
                File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }
        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="sourceFileName">被复制的文件</param>
        /// <param name="destFileName">目标文件</param>
        public void CopyFile(string sourceFileName, string destFileName, bool overwrite)
        {
            if (File.GetAttributes(sourceFileName) != FileAttributes.Normal)
                File.SetAttributes(sourceFileName, FileAttributes.Normal);
            File.Copy(sourceFileName, destFileName, overwrite);
            if (App.BasicSetting.BackupFile)
            {
                string fileName = sourceFileName.Substring(sourceFileName.LastIndexOf('\\') + 1);
                if (!Directory.Exists(string.Format(@"{0}\Backup", App.AppPath)))
                    Directory.CreateDirectory(string.Format(@"{0}\Backup", App.AppPath));
                File.Copy(sourceFileName, string.Format(@"{0}\Backup\{1}", App.AppPath, fileName), overwrite);
            }
        }
    }
}
