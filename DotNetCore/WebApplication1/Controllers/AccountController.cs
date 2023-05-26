using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class AccountController : ApiController
    {
        // GET: api/Account
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Account/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Account
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Account/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Account/5
        public void Delete(int id)
        {
        }
        // Basic基础认证
        public async Task Basic(string user, string password, string url)
        {
            // 如果认证页面是 https 的，请参考一下 jwt 认证的 HttpClientHandler
            // 创建  client 
            HttpClient client = new HttpClient();

            // 创建身份认证
            // using System.Net.Http.Headers;
            AuthenticationHeaderValue authentication = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}")
                ));


            client.DefaultRequestHeaders.Authorization = authentication;

            byte[] response = await client.GetByteArrayAsync(url);
            client.Dispose();
        }
        // Jwt认证
        public async Task Bearer(string token, string url)
        {
            // HttpClientHandler及其派生类使开发人员能够配置各种选项, 包括从代理到身份验证。
            // helpLink https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler?view=netframework-4.8
            var httpclientHandler = new HttpClientHandler();

            // 如果服务器有 https 证书，但是证书不安全，则需要使用下面语句
            // => 也就是说，不校验证书，直接允许
            httpclientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;


            //var httpclientHandler = new HttpClientHandler()
            //{
            //    ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true,
            //};

            using (var httpClient = new HttpClient(httpclientHandler))
            {
                // 创建身份认证
                // System.Net.Http.Headers.AuthenticationHeaderValue;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                await httpClient.GetAsync(url);
                httpClient.Dispose();
            }
        }
    }
}
