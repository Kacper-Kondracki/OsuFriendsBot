using Newtonsoft.Json;
using OsuFriendsSharp.Entities;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace OsuFriendsSharp
{
    public class OsuFriendsClient
    {
        private readonly string _token;
        private readonly HttpClient _httpClient;
        public const string url = "https://osufriends.ovh/api?";

        public OsuFriendsClient(string token, HttpClient httpClient)
        {
            _token = token;
            _httpClient = httpClient;
        }

        public OsuUser CreateUser(ulong id, Guid? key)
        {
            return new OsuUser(id, key, this);
        }

        public async Task<Status> GetStatusAsync(OsuUser user)
        {
            UriBuilder uriBuilder = new UriBuilder(url);
            uriBuilder.Path += "/get_status?";
            System.Collections.Specialized.NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["discord"] = user.Id.ToString();
            query["key"] = user.Key.ToString();
            query["token"] = _token;
            uriBuilder.Query = query.ToString();

            HttpResponseMessage response = await _httpClient.GetAsync(uriBuilder.Uri).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<Status>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        public async Task<OsuUserDetails> GetDetailsAsync(OsuUser user)
        {
            UriBuilder uriBuilder = new UriBuilder(url);
            uriBuilder.Path += "/get_details?";
            System.Collections.Specialized.NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["discord"] = user.Id.ToString();
            query["key"] = user.Key.ToString();
            query["token"] = _token;
            uriBuilder.Query = query.ToString();

            HttpResponseMessage response = await _httpClient.GetAsync(uriBuilder.Uri).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<OsuUserDetails>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        }
    }
}