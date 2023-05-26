using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;

namespace WebApplication1
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://WebApplication1/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod(Description ="获取系统当前时间")]
        public string GetDateTime(string shij)
        {
            string script = "<script>console.log('nihao')</script>";

            System.Web.HttpContext.Current.Response.Write(script);

            string path = @"E:\广西上传接口\正反向代理软件\正反向隔离装置穿透\Resource\Client\Log\\ErrorLog";
            string file = string.Format(@"{0}\{1}.txt", path, DateTime.Now.ToString("yyyy-MM-dd"));
            if (!File.Exists(file))
            {
                File.Create(file);
            }
            string text = string.Format(@"{0}:{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "13424242424") + "\r\n\r\n";

            File.AppendAllText(file, text);

            return DateTime.Now.ToString();
        }
    }
}
