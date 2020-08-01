using System.Threading.Tasks;

namespace OsuFriendBot
{
    internal class Program
    {
        private static Task Main(string[] args)
        {
            return new Startup(args).StartAsync();
        }
    }
}