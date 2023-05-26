using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public System.Web.Mvc.ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [System.Web.Http.HttpGet]
        public JsonResult  Upload()
        {
            var s=  System.Web.HttpContext.Current.Request.Files;
            System.Net.Http.HttpClient httpClient=new System.Net.Http.HttpClient();
             //httpClient.Send()
            // httpClient.DefaultRequestHeaders.Accept.Clear();
            var aaa =new {
            data="134",
            code="1"
            };
            return Json(aaa,JsonRequestBehavior.AllowGet);  
        }
        [System.Web.Http.HttpGet]
        public void UploadImg()
        {
            var savePath = $"{Directory.GetCurrentDirectory()}/test/";
            using (FileStream output = new FileStream(savePath + Request.Files[0].FileName, FileMode.Create))
            {
                var file = Request.Files[0];
                //file.SaveAs(savePath);
                file.SaveAs(savePath + Request.Files[0].FileName);
            }
        }


        //[Microsoft.AspNetCore.Mvc.HttpPost]
        //public string PostMulti([FromForm] string name, [FromForm] int age)
        //{
            
        //    //var str = "";
        //    //foreach (var header in Request.Headers)
        //    //{
        //    //    str += $"{header.Key}: {header.Value.ToString()}\r\n";
        //    //}
        //    //if (Request.Form.Files.Count > 0)
        //    //{
        //    //    var file = Request.Form.Files[0];
        //    //    var fileName = file.FileName;
        //    //    var fileLength = file.Length;
        //    //    using var stream = file.OpenReadStream();
        //    //    var bytearr = new byte[fileLength];
        //    //    stream.ReadAsync(bytearr);
        //    //    var fileContent = Encoding.UTF8.GetString(bytearr);
        //    //    str += "\r\n" + fileContent;
        //    //}
        //    return $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {name} {age} \r\n{str}";
        //}

    }
}
