using System;
using System.Threading.Tasks;
using System.Web;

namespace OsuFriendsSharp.Entities
{
    public class OsuUser
    {
        public ulong Id { get; }
        public Guid Key { get; }
        public Uri Url { get; }

        private readonly OsuFriendsClient _client;

        internal OsuUser(ulong id, Guid? key, OsuFriendsClient client)
        {
            Id = id;
            Key = key ?? Guid.NewGuid();
            _client = client;

            UriBuilder uriBuilder = new UriBuilder(OsuFriendsClient.url);
            System.Collections.Specialized.NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["discord"] = Id.ToString();
            query["key"] = Key.ToString();
            uriBuilder.Query = query.ToString();
            Url = uriBuilder.Uri;
        }

        public async Task<Status> GetStatus()
        {
            return await _client.GetStatusAsync(this).ConfigureAwait(false);
        }

        public async Task<OsuUserDetails> GetDetails()
        {
            return await _client.GetDetailsAsync(this).ConfigureAwait(false);
        }
    }
}