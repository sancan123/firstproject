using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
//using System.Data.OracleClinet;
using Oracle.ManagedDataAccess.Client;
using DataHelper;
using PositiveDirectionServer;
using System.Data;
using Npgsql;
using System.Data.Common;

namespace Con3
{
    public class Program
    {
        static void Main(string[] args)
        {


            DataSet ds = ExecuteQuery("SELECT id,name FROM private.info");



            OracleParameter[] oracleParameter =
           {
                new OracleParameter("vid",OracleDbType.Int32,ParameterDirection.Input) ,
                new OracleParameter("vgoodsid",OracleDbType.NVarchar2,ParameterDirection.Input),//指明传入,,
                new OracleParameter("vgoodname",OracleDbType.NVarchar2,ParameterDirection.Input) ,
                new OracleParameter("vgoodprice",OracleDbType.NVarchar2,ParameterDirection.Input) ,
                new OracleParameter("vgoodnum",OracleDbType.NVarchar2,ParameterDirection.Input) ,
                new OracleParameter("vdetail",OracleDbType.NVarchar2,ParameterDirection.Input) ,
            };
            OracleParameter[][] oracleParameter123 ={
                new OracleParameter[] {
                new OracleParameter("vid",OracleDbType.Int32,ParameterDirection.Input) ,
                new OracleParameter("vgoodsid",OracleDbType.NVarchar2,ParameterDirection.Input),//指明传入,,
                new OracleParameter("vgoodname",OracleDbType.NVarchar2,ParameterDirection.Input) ,
                new OracleParameter("vgoodprice",OracleDbType.NVarchar2,ParameterDirection.Input) ,
                new OracleParameter("vgoodnum",OracleDbType.NVarchar2,ParameterDirection.Input) ,
                new OracleParameter("vdetail",OracleDbType.NVarchar2,ParameterDirection.Input) ,
              },
            oracleParameter,
            oracleParameter,

            };
            for (int i = 4; i < 15; i++)
            {
                oracleParameter123[0][0].Value = i;
                oracleParameter123[0][1].Value= ""+i;
            }

            

            OracleConnection conn=new OracleConnection("Data Source=(DESCRIPTION =(ADDRESS =(PROTOCOL = TCP)(HOST =localhost)(PORT =1521))(CONNECT_DATA = (SERVICE_NAME =orcl.szclou.com)));User ID=SCOTT;Password=tiger;Persist Security Info=True;");
            conn.Open();
            
            OracleCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;//指明是执行存储过程

            
            cmd.Parameters.Clear();
            cmd.Parameters.Add("vid", OracleDbType.Int32).Direction = ParameterDirection.Input;//指明传入
            cmd.Parameters.Add("vgoodsid", OracleDbType.NVarchar2).Direction = ParameterDirection.Input;//指明传入的参数是输入给oracle存储过程用的
            cmd.Parameters.Add("vgoodname", OracleDbType.NVarchar2).Direction = ParameterDirection.Input;//指明传入
            cmd.Parameters.Add("vgoodprice", OracleDbType.NVarchar2).Direction = ParameterDirection.Input;//指明传入的参数是输入给oracle存储过程用的
            cmd.Parameters.Add("vgoodnum", OracleDbType.NVarchar2).Direction = ParameterDirection.Input;//指明传入
            cmd.Parameters.Add("vdetail", OracleDbType.NVarchar2).Direction = ParameterDirection.Input;//指明传入的参数是输入给oracle存储过程用的
            cmd.Parameters["vid"].Value = 3;
            cmd.Parameters["vgoodsid"].Value = "3";
            cmd.Parameters["vgoodname"].Value = "na111";
            cmd.Parameters["vgoodprice"].Value = "13000";
            cmd.Parameters["vgoodnum"].Value = "200";
            cmd.Parameters["vdetail"].Value = "very good";
            cmd.CommandText = "PRECEDES_OUTPUT";
            cmd.ExecuteNonQuery();

            conn.Close();

            object obj111 = Common.WebserviceHelper.InvokeWebService2("http://localhost:8001/PositiveDirection.asmx", "getData", new string[] { "Lirunxiang" });

            object obj222 = Common.WebserviceHelper.InvokeWebService2("http://localhost:8001/PositiveDirection.asmx", "getST", new string[] { "Lirunxiang" });



            Hashtable wsInstance=new Hashtable();
            object obj123 = Common.WebserviceHelper.InvokeWebService3("http://localhost:8001/PositiveDirection.asmx", "", "getData", new string[] { "Lirunxiang" }, wsInstance);

            object obj1234 = Common.WebserviceHelper.InvokeWebService3("http://localhost:8001/PositiveDirection.asmx", "", "getST", new string[] { "Lirunxiang" }, wsInstance);
            //Console.WriteLine(DateTime.Now.ToString());
            //object obj = WebServiceHelper.InvokeWebService("http://localhost:8001/PositiveDirection.asmx", "", "getData", new string[] {"Lirunxiang" });
            //Console.WriteLine(DateTime.Now.ToString());
            object obj1 = DataHelper.WebServiceHelper.InvokeWebService5("http://localhost:8001/PositiveDirection.asmx", "", "getData", new string[] { "Lirunxiang" });

            object obj2 = DataHelper.WebServiceHelper.InvokeWebService5("http://localhost:8001/PositiveDirection.asmx", "", "getST", new string[] { "Lirunxiang" });                    
           
            //PositiveDirection pd=new PositiveDirection();
            //pd.upLoad("123");
            //Console.WriteLine(obj2);
            //string path = System.AppDomain.CurrentDomain.BaseDirectory + "test.dll";
            //if (File.Exists(path))
            //{          
            //    Assembly asm = Assembly.LoadFrom("test.dll");
            //    Type t = asm.GetType("com.clou.ljr.PositiveDirection");

            //    object o = Activator.CreateInstance(t);
            //    MethodInfo method = t.GetMethod("getData");
            //    object ob = method.Invoke(o, new string[] { "Lirunxiang" });

            //    Console.WriteLine(ob);

            //}

            Console.WriteLine(obj1);



         

            Console.ReadLine();
        }


        public static DataSet ExecuteQuery(string sqrstr)
        {
            string ConStr = @"PORT=5432;DATABASE=Userinfo;HOST=localhost;PASSWORD=123456;USER ID=postgres";
            NpgsqlConnection SqlConn = new NpgsqlConnection(ConStr);
            DataSet ds = new DataSet();
            try
            {
                using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqrstr, SqlConn))
                {
                    sqldap.Fill(ds);
                }
                return ds;
            }
            catch (System.Exception ex)
            {
                //CloseConnection();
                return ds;
            }
        }


        public int ExecuteNonQuery(string sqrstr)
        {
            try
            {
                string ConStr = @"PORT=5432;DATABASE=Userinfo;HOST=localhost;PASSWORD=123456;USER ID=postgres";
                NpgsqlConnection SqlConn = new NpgsqlConnection(ConStr);
                SqlConn.Open();
                using (NpgsqlCommand SqlCommand = new NpgsqlCommand(sqrstr, SqlConn))
                {
                    int r = SqlCommand.ExecuteNonQuery();  //执行查询并返回受影响的行数
                    SqlConn.Close();
                    return r; //r如果是>0操作成功！ 
                }
            }
            catch (System.Exception ex)
            {
                //CloseConnection();
                return 0;
            }

        }


        public DbDataReader GetReader(string cmdText)
        {
            string ConStr = @"PORT=5432;DATABASE=Userinfo;HOST=localhost;PASSWORD=123456;USER ID=postgres";
            NpgsqlConnection sqlConn = new NpgsqlConnection(ConStr);
            if (sqlConn.State != ConnectionState.Open)
                sqlConn.Open();
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(cmdText, sqlConn))
                {
                    NpgsqlDataReader sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    return sdr;
                }
            }
            catch (System.Exception ex)
            {
                //CloseConnection();
                return null;
            }
        }
        public void Test()
        {
            #region 测试

            //var httpClient = new HttpClient();
            //var url = "http://localhost:9000/Demo/PostMulti";
            //var content = new MultipartFormDataContent();
            //content.Add(new StringContent("小明"), "name");
            //content.Add(new StringContent("18"), "age");
            ////注意：要指定filename，即：test.txt，否则后台不认为是一个文件，而是普通的参数
            //content.Add(new ByteArrayContent(System.IO.File.ReadAllBytes("e:\\test.txt")), "file", "test.txt");
            //var response = await httpClient.PostAsync(url, content);
            //var str = await response.Content.ReadAsStringAsync();

            string url = "http://localhost:8123/api/values/SaveFile";
            string url2 = "http://localhost:8123/api/values/IsFileExisit";
            string fileName = @"E:\广西上传接口\正反向\正反向隔离装置穿透\Tmp\Send__20230313183427344.dat";

            //string fileName = this.textBox1.Text;//文件全路径(e:\abc.txt)
            string safeFileName = Path.GetFileName(fileName);//文件名(abc.txt)

            WebClient client = new WebClient();
            client.Credentials = CredentialCache.DefaultCredentials;
            client.Headers.Add("Content-Type", "application/form-data");//注意头部必须是form-data
            client.QueryString["fname"] = safeFileName;

            byte[] fileb = client.UploadFile(new Uri(url2), "POST", fileName);

            string s = client.UploadString(url, "POST", safeFileName);
            string str = Encoding.Default.GetString(fileb);
            string res = Encoding.UTF8.GetString(fileb).ToString();
            string ss = res.Replace("\"", "");
            string st = Regex.Unescape(res);
            //string s= Regex.Unescape(st);

            DataHelper.DataModel dataModel = new DataHelper.DataModel();

            dataModel = dataModel.DeserializeJson(s) as DataHelper.DataModel;
            Console.WriteLine(res);


            string postString = "";
            byte[] postData = Encoding.UTF8.GetBytes(postString);
            WebClient client2 = new WebClient();
            client2.Credentials = CredentialCache.DefaultCredentials;
            client.Headers.Add("Content-Type", "text/xml");//注意头部必须是form-data
            byte[] responseData = client2.UploadData("", postData);

            string rest = client2.DownloadString(new Uri(url));
            //#region


            //string uriString = "http://localhost:8123/Home/Upload";
            //System.Net.Http.HttpClient httpClient=new System.Net.Http.HttpClient();
            //var re = httpClient.GetAsync("http://localhost:8123/Home/");
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
            //re.Result.EnsureSuccessStatusCode();
            //Console.WriteLine("Hello World!");
            //Console.ReadLine();

            ///// 创建WebClient实例  
            //System.Net.WebClient myWebClient = new WebClient();

            //myWebClient.Credentials = new NetworkCredential("10.1.31.218", "0.");

            //// 要上传的文件  
            //FileStream fs = new FileStream(@"C:\Users\00076427\Desktop\新建文本文档.txt", FileMode.Open, FileAccess.Read);
            //BinaryReader r = new BinaryReader(fs);
            //byte[] postArray = r.ReadBytes((int)fs.Length);

            //Stream postStream = myWebClient.OpenWrite(uriString, "PUT");
            //try
            //{
            //    //使用UploadFile方法可以用下面的格式  
            //    if (postStream.CanWrite)
            //    {
            //        postStream.Write(postArray, 0, postArray.Length);
            //        postStream.Close();
            //        fs.Dispose();
            //        //  log.AddLog("上传日志文件成功！", "Log");
            //        //  basicInfo.writeLogger("上传日志文件成功！" );
            //    }
            //    else
            //    {
            //        postStream.Close();
            //        fs.Dispose();
            //    }
            //}
            //catch (Exception err)
            //{
            //    postStream.Close();
            //    fs.Dispose();
            //    throw err;
            //}
            //finally
            //{
            //    postStream.Close();
            //    fs.Dispose();
            //}
            //#endregion
            #endregion
        }
        public class Product
        {
            public string Name { get; set; }
            public double Price { get; set; }
            public string Category { get; set; }
        }
    }
}
