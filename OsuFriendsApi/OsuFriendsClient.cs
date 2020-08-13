using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;
        public const string url = "https://osufriends.ovh/";

        public OsuFriendsClient(HttpClient httpClient, ILogger<OsuFriendsClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            if (string.IsNullOrEmpty(_httpClient.DefaultRequestHeaders.UserAgent.ToString()))
            {
                _httpClient.DefaultRequestHeaders.Add("user-agent", "hello"); // For some reason api doesn't accept null user agent
            }
        }

        /// <summary>
        /// Sets secret key for API
        /// </summary>
        /// <param name="token">The API token</param>
        public void SetToken(string token)
        {
            _token = token;
        }

        /// <summary>
        /// Creates osu! user.
        /// </summary>
        /// <param name="key">Random if null.</param>
        /// <returns>New osu! user.</returns>
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

            _logger.LogTrace("Request status for {key}", user.Key);
            HttpResponseMessage response = await _httpClient.GetAsync(uriBuilder.Uri).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            _logger.LogTrace("Status of {key}: {status}", user.Key, content);
            return JsonConvert.DeserializeObject<Status?>(content);
        }

        public async Task<OsuUserDetails> GetDetailsAsync(OsuUser user)
        {
            UriBuilder uriBuilder = new UriBuilder(url);
            uriBuilder.Path += "details/";

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["key"] = user.Key.ToString();
            query["secret"] = _token;
            uriBuilder.Query = query.ToString();

            _logger.LogTrace("Request details for {key}", user.Key);
            HttpResponseMessage response = await _httpClient.GetAsync(uriBuilder.Uri).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            _logger.LogTrace("Details of {key}: {details}", user.Key, content);
            return JsonConvert.DeserializeObject<OsuUserDetails>(content);
        }
    }
}