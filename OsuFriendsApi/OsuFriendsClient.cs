using Newtonsoft.Json;
using OsuFriendsApi.Entities;
using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace OsuFriendsApi
{
    public class OsuFriendsClient
    {
        private string _token;
        private readonly HttpClient _httpClient;
        public const string url = "https://osufriends.ovh/";

        public OsuFriendsClient(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
            if (string.IsNullOrEmpty(_httpClient.DefaultRequestHeaders.UserAgent.ToString()))
            {
                _httpClient.DefaultRequestHeaders.Add("user-agent", "hello"); // For some reason api doesn't accept null user agent
            }
        }

        public void SetToken(string token)
        {
            _token = token;
        }

        public OsuUser CreateUser(Guid? key = null)
        {
            return new OsuUser(key, this);
        }

        public async Task<Status?> GetStatusAsync(OsuUser user)
        {
            UriBuilder uriBuilder = new UriBuilder(url);
            uriBuilder.Path += "status/";

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["key"] = user.Key.ToString();
            query["secret"] = _token;
            uriBuilder.Query = query.ToString();

            HttpResponseMessage response = await _httpClient.GetAsync(uriBuilder.Uri).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Status?>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        public async Task<OsuUserDetails> GetDetailsAsync(OsuUser user)
        {
            UriBuilder uriBuilder = new UriBuilder(url);
            uriBuilder.Path += "details/";

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["key"] = user.Key.ToString();
            query["secret"] = _token;
            uriBuilder.Query = query.ToString();

            HttpResponseMessage response = await _httpClient.GetAsync(uriBuilder.Uri).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<OsuUserDetails>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        }
    }
}