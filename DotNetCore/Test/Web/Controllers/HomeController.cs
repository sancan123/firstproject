using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;
using System.Diagnostics;
using System.Data.SQLite;

//using System.Web.Mvc;
using Web.Common;
using Web.Models;
using System.Data;

namespace Web.Controllers
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
           
            ViewData["d"] = GetModelList(); 
            return View();
        }

        [HttpPost]
        public IActionResult Index(string id)
        {
            var s = GetModelList();
            return View(s.ToList());
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
        public   IActionResult ShowDetail()
        {
            return View();
        }

        
        public JsonResult Send()
        {

            string dataConnPath = @"Data Source =C:\Users\00076427\Desktop\DotNetCore\123.db";
            SQLiteConnection dataConn = new SQLiteConnection(dataConnPath);
            dataConn.Open();
            DataTable dt = new DataTable();
            SQLiteCommand command = new SQLiteCommand("select * from UserInfo", dataConn);
            command.ExecuteNonQuery();
            SQLiteDataAdapter mAdapt = new SQLiteDataAdapter(command);
            mAdapt.Fill(dt);

            List<UserModel> lists = new List<UserModel>();
            if (dt!=null&&dt.Rows.Count>0)
            {
               
                foreach (DataRow dr in dt.Rows)
                {
                    UserModel u = new UserModel();
                    u.Id = dr["UserId"].ToString();
                    u.Address = dr["UserAddress"].ToString();
                    u.UserName = dr["UserName"].ToString();
                    u.Phone = dr["UserPhone"].ToString();
                    u.City = dr["City"].ToString();
                    u.Email = dr["Email"].ToString();
                    u.sign = dr["Sign"].ToString();
                    u.classify = dr["Classify"].ToString();
                    u.Gender = dr["Gender"].ToString();
                    u.wealth = dr["Wealth"].ToString();
                    u.score = dr["Score"].ToString();
                    u.Experience = dr["Experience"].ToString();
                    lists.Add(u);
                }
            }

            OracleHelper oracleHelper1 = new OracleHelper("127.0.0.1", 1521, "orcl.szclou.com", "scott", "tiger", "");
            string sql123 = "select * from  MT_INTUIT_MET_CONC";
            var data = oracleHelper1.ExecuteReader(sql123);


            List<UserModel> list = new List<UserModel>()
            {
                new UserModel(){ 
                UserName="你好demo1",
                Phone="13170473797",
                Address="湖南长沙",
                City="长沙",
                Gender="男",
                classify="工程师",
                Email="123@163.com",
                score="98",
                sign="sign1",
                wealth="134141",
                },
                 new UserModel(){
                UserName="你好demo2",
                Phone="13170473797",
                Address="广东广州",
                City="广州",
                Gender="男",
                classify="工程师",
                Email="123@163.com",
                score="99",
                sign="sign1",
                wealth="1341444",
                },
                  new UserModel(){
                UserName="你好demo3",
                Phone="13170473797",
                Address="广西南宁",
                City="南宁",
                Gender="男",
                classify="工程师",
                Email="123@163.com",
                score="96",
                sign="sign1",
                wealth="134145555",
                }
            };

            var datalist = new 
            {
                data=lists,
                code=0
            };
            return Json(datalist);
        }


        public IActionResult Default()
        {
            List<UserModel> list = new List<UserModel>()
            {
                new UserModel(){
                UserName="刘洋",
                Phone="1233",
                Address="41411"
                },
                new UserModel(){
                UserName="李莉",
                Phone="1332",
                Address="41242"
                },
                new UserModel(){
                    UserName = "李湘", Phone = "1424", Address = "41414"
                },
                new UserModel(){
                    UserName="李奇",
                Phone="42411",
                Address="52341"
                },
                new UserModel(){
                UserName="142424",
                Phone="432423",
                Address="14141"
                },
                new UserModel(){UserName = "李爽", Phone = "14231", Address = "141414"},
            };

            ViewData["list"] = list;
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }
        [HttpPost]
        public  IActionResult Detail(string sid)
        {
            List<UserModel> list = new List<UserModel>()
            {
                new UserModel(){
                UserName="刘洋",
                Phone="1233",
                Address="41411"
                },
                new UserModel(){
                UserName="李莉",
                Phone="1332",
                Address="41242"
                },
                new UserModel(){
                    UserName = "李湘", Phone = "1424", Address = "41414"
                },
                new UserModel(){
                    UserName="李奇",
                Phone="42411",
                Address="52341"
                },
                new UserModel(){
                UserName="142424",
                Phone="432423",
                Address="14141"
                },
                new UserModel(){UserName = "李爽", Phone = "14231", Address = "141414"},
            };
            var data = list.Select(s=>s.Id==sid);
            return View();
        }
        public IActionResult showindex()
        {
            return View();  
        }



        public List<UserModel> GetModelList()
        {
            string dataConnPath = @"Data Source =C:\Users\00076427\Desktop\DotNetCore\123.db";
            SQLiteConnection dataConn = new SQLiteConnection(dataConnPath);
            dataConn.Open();
            DataTable dt = new DataTable();
            SQLiteCommand command = new SQLiteCommand("select * from UserInfo", dataConn);
            command.ExecuteNonQuery();
            SQLiteDataAdapter mAdapt = new SQLiteDataAdapter(command);
            mAdapt.Fill(dt);

            List<UserModel> lists = new List<UserModel>();
            if (dt != null && dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    UserModel u = new UserModel();
                    u.Id = dr["UserId"].ToString();
                    u.Address = dr["UserAddress"].ToString();
                    u.UserName = dr["UserName"].ToString();
                    u.Phone = dr["UserPhone"].ToString();
                    u.City = dr["City"].ToString();
                    u.Email = dr["Email"].ToString();
                    u.sign = dr["Sign"].ToString();
                    u.classify = dr["Classify"].ToString();
                    u.Gender = dr["Gender"].ToString();
                    u.wealth = dr["Wealth"].ToString();
                    u.score = dr["Score"].ToString();
                    u.Experience = dr["Experience"].ToString();
                    lists.Add(u);
                }
                return lists;
            }
            else
            {
                return null;
            }
            //return new List<UserModel>();
        }
    }
}