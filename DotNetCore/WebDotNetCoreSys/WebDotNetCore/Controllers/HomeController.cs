using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebDotNetCore.Models;

namespace WebDotNetCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }



        public IActionResult ResData()
        {
            List<UserInfo> list = new List<UserInfo>() { 
               new UserInfo (){
                 Id="1",
                 UserName="user-1",
                Grade="255",
                Gender="男",
                 Remark="55",
                 City="广州",
                 Sgin="sgin-1",
                 Work="1345",
                 Money="100000000",
               },
                new UserInfo (){
                 Id="2",
                 UserName="User-2",
                 Grade="367",
                 Gender="女",
                 Remark="66",
                 City="深圳",
                 Sgin="sgin-2",
                 Work="23451",
                 Money="10000000",
               }

            };

            var dataj =new  {
              code=0,
              msg="",
              count=list.Count,
              data= list,
            };

            UserInfo info = new UserInfo()
            {
                Id= "1",

            };
            return Json(dataj);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public class UserInfo{

            public string Id { get; set; }
            public string ?UserName { get; set; }

            public string Grade { get; set; }

            public string Gender { get; set; }

            public string Remark { get; set; }

            public string City { get; set; }
            public string Sgin { get; set; }

            public string Work { get; set; }

            public string Money { get; set; }

        }
    }
}