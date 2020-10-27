using Serilog.Events;
using System;

namespace OsuFriendsBot
{
    public class Config
    {
        public string Token { get; set; }
        public string OsuFriendsApiToken { get; set; }
        public Uri OsuFriendsApiUrl { get; set; }
        public string Prefix { get; set; }
        public string ConnectionString { get; set; }
        public string LogPath { get; set; }
        public LogEventLevel MinimumLevel { get; set; }
    }
}