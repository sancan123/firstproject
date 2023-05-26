using DataHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Windows.Forms;

namespace PositiveDirectionServer
{
    /// <summary>
    /// PositiveDirection 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://PositiveDirection/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class PositiveDirection : System.Web.Services.WebService
    {        
        [WebMethod]
        public string apply(string applyModelJson)
        {
            HandleRequest handleRequest = new HandleRequest();
            return handleRequest.apply(applyModelJson);
        }
        [WebMethod]
        public string upLoad(string uploadModelJson)
        {
            HandleRequest handleRequest = new HandleRequest();

            return handleRequest.upLoad(uploadModelJson);
        }
        [WebMethod]
        public string downLoad(string downModelJson)
        {
            HandleRequest handleRequest = new HandleRequest();
            return handleRequest.downLoad(downModelJson);
        }
    }
}
