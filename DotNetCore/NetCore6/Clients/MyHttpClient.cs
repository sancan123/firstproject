namespace NetCore6.Clients
{
    public class MyHttpClient
    {
        private readonly HttpClient _client;

        public MyHttpClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> ShowAsync()
        {
            return await _client.GetStringAsync("https://www.baidu.com");
        }
    }
}
