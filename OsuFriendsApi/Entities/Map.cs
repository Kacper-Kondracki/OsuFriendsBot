using System;

namespace OsuFriendsApi.Entities
{
    public class Map
    {
        public string Name { get; set; }
        public string Difficulty { get; set; }
        public float Stars { get; set; }
        public int Bpm { get; set; }
        public Uri Url { get; set; }
        public Uri Image { get; set; }
        public string Status { get; set; }
    }
}