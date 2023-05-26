using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web.Script.Serialization;

namespace DataHelper
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Serializable()]
    public class DataFormat
    {
        /// <summary>
        /// 加载可反序列化文件
        /// </summary>
        /// <param name="fileName">要反序列化的文件</param>
        /// <returns></returns>
        public static DataFormat Load(string fileName)
        {
            if (!File.Exists(fileName)) return null;

            string jsonFormatData = File.ReadAllText(fileName);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            DataFormat obj = null;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    obj = (DataFormat)jss.Deserialize(jsonFormatData, typeof(DataModel));
                    break;
                }
                catch (ArgumentException e)
                {
                    jss.MaxJsonLength *= 2;
                }
                catch (Exception ex)
                {
                    return obj;
                }

            }
            return obj;
        }
        /// <summary>
        /// 保存序列化文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        public void Save(string fileName)
        {
            try
            {
                string fold = fileName.Substring(0, fileName.LastIndexOf("\\"));
                if (!Directory.Exists(fold)) Directory.CreateDirectory(fold);
                string jsonFormatData = SerializeJson();
                File.AppendAllText(fileName, jsonFormatData);
                if (File.GetAttributes(fileName) != FileAttributes.Normal)
                    File.SetAttributes(fileName, FileAttributes.Normal);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public string SerializeJson()
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            string jsonFormatData = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    jsonFormatData = jss.Serialize(this);
                    break;
                }
                catch (InvalidOperationException e)
                {
                    jss.MaxJsonLength *= 2;
                }
                catch (Exception ex)
                {
                    return jsonFormatData;
                }
            }
            return jsonFormatData;
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="jsonFormatData">要反序列化的json字符串</param>
        /// <returns></returns>
        public DataFormat DeserializeJson(string jsonFormatData)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            DataFormat obj = null;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    obj = (DataFormat)jss.Deserialize(jsonFormatData, this.GetType());
                    break;
                }
                catch (ArgumentException e)
                {
                    jss.MaxJsonLength *= 2;
                }
                catch (Exception ex)
                {
                    return obj;
                }
            }
            return obj;
        }
    }
}
