using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using DataHelper;
using static WebApplication1.Controllers.ValuesController.UserModel;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Ajax.Utilities;

namespace WebApplication1.Controllers
{

   // [Route("api/[controller]")]
    public class ValuesController : ApiController
    {
        //private readonly InAppFileSaver _contextAccessor;
        // GET api/values
        [System.Web.Http.HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([System.Web.Http.FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [System.Web.Http.FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }


        [System.Web.Http.HttpPost,System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/values/SaveFile")]
        public String SaveFile()
        {
            var request = System.Web.HttpContext.Current.Request;
            if (request.Files.Count > 0)
            {
                HttpPostedFile file = request.Files.Get(0);
                file.SaveAs(Path.Combine("E:\\广西上传接口\\正反向\\Tmp", file.FileName));
                return "上传成功";
            }
            return "1";
        }


        [System.Web.Http.HttpPost, System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/values/SaveFile")]
        public String IsFileExisit(string filename)
        {
            var request = System.Web.HttpContext.Current.Request.Form["filename"];

            DateTime startTime = DateTime.Now;
            string flag = "";
            while (true)
            {
                if (File.Exists(request))
                {
                    flag = "文件存在";
                    break;
                }
                if (DateTime.Now.Subtract(startTime).TotalMilliseconds >= 180 * 1000)                                         
                    break;

            }
            return flag;
        }

        [System.Web.Http.HttpPost, System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/values/SaveFileData")]
        public String SaveFileData()
        {
            DownModel downModel = new DownModel();
            //downModel = downModel.DeserializeJson(downModelJson) as DownModel;
            //string sendFileName = string.Format("Send_{0}_{1}.dat", downModel.EquipmentNo, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            //string receiveFileName = sendFileName.Replace("Send", "Receive");
            DataModel dataModel = new DataModel();
            dataModel.downModel = downModel;
            //dataModel.Save(string.Format(@"{0}\Tmp\{1}", App.AppPath, sendFileName));
            SearchHelper se = new SearchHelper();
            dataModel = se.SearchFileData(@"E:\Send__20230314144112588.dat");
                
                /*searchHelper.SearchFile(string.Format(@"{0}\Tmp\{1}", App.AppPath, receiveFileName));*/
            return dataModel.SerializeJson();

            
        }

        //[System.Web.Http.HttpPost, System.Web.Http.HttpGet]
        //[System.Web.Http.Route("api/values/Upload")]
        //public string Upload(HttpPostedFileBase file)
        //{
        //    if (file==null)
        //    {
        //        return "0";
        //    }
        //    return "1";
        //}


        [System.Web.Http.HttpPost, System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/values/Upload")]
        public string Upload()
        {
            var pr=new MultipartMemoryStreamProvider();
            Request.Content.ReadAsMultipartAsync(pr);
            foreach (var item in pr.Contents)
            {
                var filename = item.Headers.ContentDisposition.FileName.Trim('\"');
                var filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("."), filename);
                var result = item.ReadAsByteArrayAsync().Result;
                var buffer= item.ReadAsByteArrayAsync();
                //using (var stream = new FileStream(filePath, FileMode.Create))
                //{
                    File.WriteAllBytes(@"C:\Users\00076427\Desktop\DotNetCore\WebApplication1\123.txt", result);
                    //buffer.CopyToAsync(stream);
                //}
                          
            }
            return "1";
        }


        [System.Web.Http.HttpPost, System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/values/UploadTxtFile")]
        public string UploadTxtFile()
        {
            HttpPostedFile hpf = System.Web.HttpContext.Current.Request.Files[0];
            var fileName= hpf.FileName;
            var pr = new MultipartMemoryStreamProvider();
            Request.Content.ReadAsMultipartAsync(pr);
            foreach (var item in pr.Contents)
            {
                var filename = item.Headers.ContentDisposition.FileName.Trim('\"');
                var filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("."), filename);
                var result = item.ReadAsByteArrayAsync().Result;
                var buffer = item.ReadAsByteArrayAsync();
                //using (var stream = new FileStream(filePath, FileMode.Create))
                //{
                File.WriteAllBytes(@"C:\Users\00076427\Desktop\DotNetCore\WebApplication1\123.txt", result);
                //buffer.CopyToAsync(stream);
                //}

            }
            return fileName;
        }


        [System.Web.Http.HttpPost, System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/values/Upload")]
        public string UploadFiles()
        {
            var pr = new MultipartMemoryStreamProvider();
            Request.Content.ReadAsMultipartAsync().ContinueWith(x => {

                var provider = x.Result;

                foreach (var item in provider.Contents)
                {
                    var a = item.ReadAsByteArrayAsync().Result;
                    //var b=Getf
                }
            
            
            });
            //foreach (var item in pr.Contents)
            //{
            //    var filename = item.Headers.ContentDisposition.FileName.Trim('\"');
            //    var filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("."), filename);
            //    var result = item.ReadAsByteArrayAsync().Result;
            //    var buffer = item.ReadAsByteArrayAsync();
            //    //using (var stream = new FileStream(filePath, FileMode.Create))
            //    //{
            //    File.WriteAllBytes(@"C:\Users\00076427\Desktop\DotNetCore\WebApplication1\123.txt", result);
            //    //buffer.CopyToAsync(stream);
            //    //}

            //}
            return "1";
        }


        //[System.Web.Mvc.HttpPost]
        public void UploadHtppFile(string url)
        {
            try
            {
                //可以是任意文件
                url = "https://ss0.bdstatic.com/70cFuHSh_Q1YnxGkpoWK1HF6hhy/it/u=1906469856,4113625838&fm=26&gp=0.jpg";

                //发起请求，读取流，转为byte[]
                var bytes = Url_To_Byte(url);
                //名称尽量不要重复，因为名称重复WriteAllBytes会覆盖之前的
                var fileName = DateTime.Now.ToString("yyyyMMddHHmmssms") + ".jpg";
                var filePath = "upload/" + fileName;
                //文件保存到该路径下
                //Server.MapPath()
                System.IO.File.WriteAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/" + filePath), bytes);

                //return Ok(new { code = 0, hint = "保存成功", file = filePath });
            }
            catch (Exception ex)
            {
                //return Json(new { code = 1, hint = "保存失败：" + ex.Message });
                throw;
            }
        }

        /// <summary>
        /// http路径图片，转为byte[]
        /// </summary>
        /// <param name="imgUrl">图片路径</param>
        /// <returns></returns>
        public static byte[] Url_To_Byte(string imgUrl)
        {
            //创建HttpWebRequest对象，请求图片url
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imgUrl);

            byte[] bytes;
            //获取流
            using (Stream stream = request.GetResponse().GetResponseStream())
            {
                //保存为byte[]
                using (MemoryStream mstream = new MemoryStream())
                {
                    int count = 0;
                    byte[] buffer = new byte[1024];
                    int readNum = 0;
                    while ((readNum = stream.Read(buffer, 0, 1024)) > 0)
                    {
                        count = count + readNum;
                        mstream.Write(buffer, 0, readNum);
                    }
                    mstream.Position = 0;
                    using (BinaryReader br = new BinaryReader(mstream))
                    {
                        bytes = br.ReadBytes(count);
                    }
                }
            }
            return bytes;
        }

        public class UserModel:DataHelper.DataFormat
        {
            public string UserName { get; set; }

            public string Password { get; set; }

            public Info userinfo { get; set; }

            public class Info
            {
                public string Name { get; set; }

                public string Email { get; set; }

                public string Phone { get; set; }

                public string City { get; set; }

                public string Msg { get; set; }
            }
        }
        //public async Task<IActionResult> PostUpload([FromForm] IFormFile formFile) {

        //    await formFile.CopyTo();

        //}
    }
}
