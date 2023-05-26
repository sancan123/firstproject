using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:8123/api/values/SaveFileData";

            //if (string.IsNullOrEmpty(this.textBox1.Text))
            //{
            //    MessageBox.Show("请先选择要上传的文件");
            //    return;
            //}

            string fileName = @"E:\广西上传接口\正反向\正反向隔离装置穿透\Tmp\Send__20230313183427344.dat";

            //string fileName = this.textBox1.Text;//文件全路径(e:\abc.txt)
            string safeFileName = Path.GetFileName(fileName);//文件名(abc.txt)

            WebClient client = new WebClient();
            client.Credentials = CredentialCache.DefaultCredentials;
            client.Headers.Add("Content-Type", "application/form-data");//注意头部必须是form-data
            client.QueryString["fname"] = safeFileName;
            byte[] fileb = client.UploadFile(new Uri(url), "POST", fileName);

            object obj = BytesToObject(fileb);
            DataHelper.DataModel d = obj as DataHelper.DataModel;
            string str = Encoding.Default.GetString(fileb);
            string res = Encoding.UTF8.GetString(fileb);

            string st = Regex.Unescape(res);
            //string s = Regex.Unescape(st);

            DataHelper.DataModel dataModel = new DataHelper.DataModel();

            dataModel = dataModel.DeserializeJson(st) as DataHelper.DataModel;
            Console.WriteLine(res);
        }

        /// <summary>
        /// 将一个序列化后的byte[]数组还原
        /// </summary>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        public static object BytesToObject(byte[] Bytes)
        {
            using (MemoryStream ms = new MemoryStream(Bytes))
            {
                IFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }
    }
}
