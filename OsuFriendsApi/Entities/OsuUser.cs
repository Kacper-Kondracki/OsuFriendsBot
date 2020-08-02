using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;

namespace OsuFriendsApi.Entities
{
    public class OsuUser
    {
        public Guid Key { get; }
        public Uri Url { get; }

        private readonly OsuFriendsClient _client;

        internal OsuUser(Guid? key, OsuFriendsClient client)
        {
            Key = key ?? Guid.NewGuid();
            _client = client;
            UriBuilder uriBuilder = new UriBuilder(OsuFriendsClient.url);
            uriBuilder.Path += $"auth/{Key}";
            Url = uriBuilder.Uri;
        }

        public async Task<Status?> GetStatusAsync()
        {
            return await _client.GetStatusAsync(this).ConfigureAwait(false);
        }

        public async Task<OsuUserDetails> GetDetailsAsync()
        {
            return await _client.GetDetailsAsync(this).ConfigureAwait(false);
        }
    }
}