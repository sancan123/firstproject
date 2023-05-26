using Microsoft.AspNetCore.Mvc;
using NetCore6.Clients;

namespace NetCore6.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : Controller
    {
        private readonly MyHttpClient _myHttpClient;
        public IActionResult Index()
        {
            return View();
        }
        public TestController(MyHttpClient myHttpClient)
        {
            _myHttpClient = myHttpClient;
        }

        [HttpGet]
        public async Task<object> GetAsync()
        {
            return await _myHttpClient.ShowAsync();
        }
            public async void Index2()
          {
              await Show();
          }
        [HttpPost]
        [Route("api/test/Show")]
        public async Task<string> Show()
        {
            int a,b=0,c=0;
            Action action = () => {

                a = b - c;
            };
            //Task.Run(
            //     action
            //    ); 
            await Task.Delay(1000);
            //await a = b - c;
            return "123";
        }
        [HttpGet]
        public string GetName()
        {
            return $"123";

        }
        [HttpPost]
        public string  GetName(string a,string b) {
        
                 return $"{a}+{b}";
        
        }

    }
}
