using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
//using System.Net.Http.Formatting;

namespace Common
{
    public  class HttpClientHelper
    {
        public HttpClientHelper()
        {

        }

        public void GetData()
        {

            string url = "http://localhost:8123/api/values/SaveFile";
            string url2 = "http://localhost:8123/api/values/IsFileExisit";
            HttpClient httpClient = new HttpClient();
            httpClient.MaxResponseContentBufferSize = 256000;

            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");

            var responeseMessage = httpClient.GetAsync(url).Result;
            var formatters = new List<MediaTypeFormatter>()
            {
                //new MyCustomFormatter(),
                new JsonMediaTypeFormatter(),
                new XmlMediaTypeFormatter(),
            };
            if (responeseMessage.IsSuccessStatusCode)
            {
                var products = responeseMessage.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            }
        }


        public class Product
        {
            public string Name { get; set; }
            public double Price { get; set; }
            public string Category { get; set; }
        }
    }
}
