using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DataHelper
{
    public class HandleRequest
    {
        SearchHelper searchHelper = new SearchHelper();
        //有机会可以把申请、上传和下载合并成一个函数，降低代码重复率
        public HandleRequest()
        {
            string mapPath = System.Web.HttpContext.Current.Server.MapPath("");
            App.AppPath = mapPath.Substring(0, mapPath.LastIndexOf("\\"));
            App.BasicSetting = new SystemConfigure();
            App.BasicSetting.Load();
        }
        /// <summary>
        /// 申请功能
        /// </summary>
        /// <param name="applyModelJson">申请功能的可反序列化字符串</param>
        /// <returns></returns>
        public string apply(string applyModelJson)
        {
            ApplyModel applyModel = new ApplyModel();
            applyModel = applyModel.DeserializeJson(applyModelJson) as ApplyModel;
            string sendFileName = string.Format("Send_{0}_{1}.dat", applyModel.EquipmentNo, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            string receiveFileName = sendFileName.Replace("Send","Receive");
            DataModel dataModel = new DataModel();
            dataModel.applyModel = applyModel;
            dataModel.Save(string.Format(@"{0}\Tmp\{1}", App.AppPath, sendFileName));
            dataModel = searchHelper.SearchFile(string.Format(@"{0}\Tmp\{1}", App.AppPath, receiveFileName));
            return dataModel.applyModel.SerializeJson();
        }
        /// <summary>
        /// 上传功能
        /// </summary>
        /// <param name="uploadModelJson">上传功能的可反序列化字符串</param>
        /// <returns></returns>
        public string upLoad(string uploadModelJson)
        {        
            UploadModel uploadModel = new UploadModel();           
            uploadModel = uploadModel.DeserializeJson(uploadModelJson) as UploadModel;          
            string sendFileName = string.Format("Send_{0}_{1}.dat", uploadModel.EquipmentNo, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            string receiveFileName = sendFileName.Replace("Send", "Receive");
            DataModel dataModel = new DataModel();
            dataModel.uploadModel = uploadModel;
            dataModel.Save(string.Format(@"{0}\Tmp\{1}", App.AppPath, sendFileName));
            dataModel = searchHelper.SearchFile(string.Format(@"{0}\Tmp\{1}", App.AppPath, receiveFileName));
            return dataModel.uploadModel.SerializeJson();
        }
        /// <summary>
        /// 下载功能
        /// </summary>
        /// <param name="downModelJson">下载功能的可反序列化字符串</param>
        /// <returns></returns>
        public string downLoad(string downModelJson)
        {
            DownModel downModel = new DownModel();
            downModel = downModel.DeserializeJson(downModelJson) as DownModel;
            string sendFileName = string.Format("Send_{0}_{1}.dat", downModel.EquipmentNo, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            string receiveFileName = sendFileName.Replace("Send", "Receive");
            DataModel dataModel = new DataModel();
            dataModel.downModel = downModel;
            dataModel.Save(string.Format(@"{0}\Tmp\{1}", App.AppPath, sendFileName));
            dataModel = searchHelper.SearchFile(string.Format(@"{0}\Tmp\{1}", App.AppPath, receiveFileName));
            return dataModel.downModel.SerializeJson();
        }
    }
}
