using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHelper
{
    [Serializable()]
    public class DataModel:DataFormat
    {
        public DataModel(string runTimeErr)
        {
            downModel = new DownModel();
            uploadModel = new UploadModel();
            applyModel = new ApplyModel();
            downModel.RunTimeErr = runTimeErr;
            uploadModel.RunTimeErr = runTimeErr;
            applyModel.RunTimeErr = runTimeErr;
        }
        public DataModel()
        {
        }
        /// <summary>
        /// 下载信息模型
        /// </summary>
        public DownModel downModel { get; set; }
        /// <summary>
        /// 上传数据模型
        /// </summary>
        public UploadModel uploadModel { get; set; }
        /// <summary>
        /// 申请设备模型
        /// </summary>
        public ApplyModel applyModel { get; set; }
        /// <summary>
        /// 运行错误
        /// </summary>
        public string RunTimeErr { get; set; }

        /// <summary>
        /// 返回的正常消息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 营销接口参数
        /// </summary>
        public MisPara misPara { get; set; }
        /// <summary>
        /// 台体编号
        /// </summary>
        public string EquipmentNo { get; set; }
    }
}
