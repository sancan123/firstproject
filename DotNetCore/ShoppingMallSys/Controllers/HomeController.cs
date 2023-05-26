using Microsoft.AspNetCore.Mvc;
using ShoppingMallSys.Models;
using System.Diagnostics;
using System.Data.SQLite;
using System.Xml.Linq;
using System.Data;
using Dapper;

namespace ShoppingMallSys.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            string str = "";
            string r = str == "" ? "" : "";
            _logger = logger;
        }

        public IActionResult Index()
        {
            int a = int.Parse("" == "" ? "0" : "1");
            return View();
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
        public IActionResult Show() {
            
               int num=13;


            Task.Run(async () =>
            {
                int s = num;
                int results = await GetStr(s);
                //num++;
            });

             num = 14;
            //Task.Run(async () =>
            //{
            //    int s = num;
            //    string results = await GetStr9(s);
            //    num++;
            //});
            GetStr9(num);
             num = 15;

            GetStr10(num);
            //Task.Run(async () =>
            //{
            //    int s = num;
            //    string results = await GetStr10(s);
            //    num++;
            //});

            //string DBname = @"C:\Users\00076427\Desktop\DotNetCore\123.db";
            ////string sql = $"Data Source = {DBname}; Version = 3";
            //DataTable dt=new DataTable();
            //SQLiteConnection conn = new SQLiteConnection($"Data Source = {DBname}; Version = 3");
            //conn.Open();
            ////string sqltext = "select *from UserInfo";
            //string sqltext = "insert into UserInfo(UserId,UserName,UserPhone,UserAddress,City,Gender,Classify,Email,Score,Sign,Wealth,Experience) values(5,'123','131231','guangxi','nanning','nan','123','123@163.com','100','100','1999','0000')";
            ////SQLiteDataAdapter dap = new SQLiteDataAdapter(sqltext, conn);
            ////dap.Fill(dt);
            //SQLiteCommand sQLiteCommand=new  SQLiteCommand(sqltext, conn);
            //Task.Run(async  () => {
            //    int results=  await sQLiteCommand.ExecuteNonQueryAsync();
            //});
            ////int result= sQLiteCommand.ExecuteNonQueryAsync();
            //conn.Close();
            return View();
        
        }


        public IActionResult Login(string returnUrl)
        {
            return View();
        }

        public async Task<int> GetStr(int num)
        {
            string DBname = @"C:\Users\00076427\Desktop\DotNetCore\123.db";
            //string sql = $"Data Source = {DBname}; Version = 3";
            DataTable dt = new DataTable();
            SQLiteConnection conn = new SQLiteConnection($"Data Source = {DBname}; Version = 3");
            conn.Open();
            //string sqltext = "select *from UserInfo";
            string sqltext = $"insert into UserInfo(UserId,UserName,UserPhone,UserAddress,City,Gender,Classify,Email,Score,Sign,Wealth,Experience) values({num},'123','131231','guangxi','nanning','nan','123','123@163.com','100','100','1999','0000')";
            //SQLiteDataAdapter dap = new SQLiteDataAdapter(sqltext, conn);
            //dap.Fill(dt);
            SQLiteCommand sQLiteCommand = new SQLiteCommand(sqltext, conn);
            //Task.Run(async () => {
               // int results = await sQLiteCommand.ExecuteNonQueryAsync();
            //});
            //int result= sQLiteCommand.ExecuteNonQueryAsync();

            conn.Close();
            //return "成功";
          return  await sQLiteCommand.ExecuteNonQueryAsync();
        }

        public async void GetStr9(int num)
        {
            string DBname = @"C:\Users\00076427\Desktop\DotNetCore\123.db";
            //string sql = $"Data Source = {DBname}; Version = 3";
            DataTable dt = new DataTable();
            SQLiteConnection conn = new SQLiteConnection($"Data Source = {DBname}; Version = 3");
            conn.Open();
            //string sqltext = "select *from UserInfo";
            string sqltext = $"insert into UserInfo(UserId,UserName,UserPhone,UserAddress,City,Gender,Classify,Email,Score,Sign,Wealth,Experience) values({num},'123','131231','guangxi','nanning','nan','123','123@163.com','100','100','1999','0000')";
            //SQLiteDataAdapter dap = new SQLiteDataAdapter(sqltext, conn);
            //dap.Fill(dt);
            SQLiteCommand sQLiteCommand = new SQLiteCommand(sqltext, conn);
            //Task.Run(async () => {
            int results = await sQLiteCommand.ExecuteNonQueryAsync();
            //});
            //int result= sQLiteCommand.ExecuteNonQueryAsync();

            conn.Close();
            //return "成功";
        }


        public async void GetStr10(int num)
        {
            string DBname = @"C:\Users\00076427\Desktop\DotNetCore\123.db";
            //string sql = $"Data Source = {DBname}; Version = 3";
            DataTable dt = new DataTable();
            SQLiteConnection conn = new SQLiteConnection($"Data Source = {DBname}; Version = 3");
            conn.Open();
            //string sqltext = "select *from UserInfo";
            string sqltext = $"insert into UserInfo(UserId,UserName,UserPhone,UserAddress,City,Gender,Classify,Email,Score,Sign,Wealth,Experience) values({num},'123','131231','guangxi','nanning','nan','123','123@163.com','100','100','1999','0000')";
            //SQLiteDataAdapter dap = new SQLiteDataAdapter(sqltext, conn);
            //dap.Fill(dt);
            SQLiteCommand sQLiteCommand = new SQLiteCommand(sqltext, conn);
            //Task.Run(async () => {
            int results = await sQLiteCommand.ExecuteNonQueryAsync();
            //});
            //int result= sQLiteCommand.ExecuteNonQueryAsync();

            conn.Close();
            //return "成功";
        }
    }
}