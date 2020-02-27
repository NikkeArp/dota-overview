using SteamApi;
using DataAPI.Models;
using System.Collections.Generic;
using System.Linq;
using CToken = System.Threading.CancellationToken;

namespace DataAPI
{
    // TODO: Database data access

    /// <summary>
    /// Handles data related to Steam.
    /// 
    /// Internally handles data access from Valve's steam web api
    /// and local SQLite database.
    /// 
    /// Publicly exposes only methods that return data
    /// models. Decides internally which data source
    /// to use.
    /// </summary>
    public class SteamApiDataAccess
    {
        /// <summary>
        /// Handles http request to steam web api.
        /// </summary>
        protected SteamApiClient _client;

        /// <summary>
        /// Instantiates SteamApiDataAccess object
        /// </summary>
        public SteamApiDataAccess()
        {
            _client = new SteamApiClient();
        }

        /// <summary>
        /// Gets steam profile collection by sending
        /// http request to steam web api.
        /// </summary>
        /// <param name="id64s">collection of 64-bit steam ids</param>
        public virtual IEnumerable<SteamProfileModel> GetSteamProfileModels(CToken token, params string[] id64s)
        {
            // database access here in the future

            return GetSteamProfilesFromWebApi(token, id64s);
        }

        /// <summary>
        /// Gets single steam profile by id
        /// </summary>
        /// <param name="id64">64-bit steam id</param>
        public virtual SteamProfileModel GetSteamProfileModel(string id64, CToken token)
        {
            // database access here in the future

            return GetSteamProfileFromWebAPI(id64, token);
        }

        /// <summary>
        /// Gets single steam profile by sending
        /// http request to steam web api.
        /// </summary>
        /// <param name="id64">64-bit steam id</param>
        protected virtual SteamProfileModel GetSteamProfileFromWebAPI(string id64, CToken token)
        {
            var apiResponse = _client.GetSteamAccountAsync(id64, token).Result;
            return new SteamProfileModel(apiResponse)
            {
                AvatarFullBytes = GetProfilePic(apiResponse.AvatarFullURL),
                AvatarMediumBytes = GetProfilePic(apiResponse.AvatarMediumURL)
            };
        }

        /// <summary>
        /// Gets steam profile collection by sending
        /// http request to steam web api.
        /// </summary>
        /// <param name="id64s">collection of 64-bit steam ids</param>
        protected virtual IEnumerable<SteamProfileModel> GetSteamProfilesFromWebApi(CToken token,
            params string[] id64s)
        {
            var apiResponse = _client.GetSteamAccountsAsync(token, id64s)
                .Result;

            return apiResponse.Select(x => new SteamProfileModel(x)
            {
                AvatarFullBytes = GetProfilePic(x.AvatarFullURL),
                AvatarMediumBytes = GetProfilePic(x.AvatarMediumURL)
            });
        }

        /// <summary>
        /// Gets image bytes by sending http request to url
        /// </summary>
        protected virtual byte[] GetProfilePic(string url) => _client.GetProfilePicBytesAsync(url)
            .Result;
    }
}
