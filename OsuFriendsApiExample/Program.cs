using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OsuFriendsApi;
using Serilog;

namespace OsuFriendsApiExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            Log.Information("Hi");

            HttpClient httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Add("user-agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("user-agent", "peppy is gay");

            var x = api.CreateUser(new Guid("083903b4-8ef7-476a-a930-2cb91ebc24ca"));
            Log.Information("User: {@user}", x);
            Log.Information("Status: {@status}", await x.GetStatus());
            Log.Information("Status: {@status}", await x.GetDetails());
            await Task.CompletedTask;
        }
    }
}
