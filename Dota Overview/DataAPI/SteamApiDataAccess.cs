using SteamApi;
using DataAPI.Models;
using System.Collections.Generic;
using System.Linq;
using DataAPI.Database;
using CToken = System.Threading.CancellationToken;
using System.Threading.Tasks;
using System;

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
        /// Cache for profile ids already in database.
        /// Used for checking if web api request is needed.
        /// </summary>
        protected HashSet<ulong> _profileIdCache;


        /// <summary>
        /// Handles http request to steam web api.
        /// </summary>
        protected readonly SteamApiClient _client;


        /// <summary>
        /// Instantiates SteamApiDataAccess object.
        /// </summary>
        public SteamApiDataAccess()
        {
            _client = new SteamApiClient();
        }


        /// <summary>
        /// Loads data from database to caches in memory.
        /// </summary>
        public void Init()
        {
            Task[] loadDataFromDbTasks = new Task[]
            {
                Task.Factory.StartNew(() => LoadProfileIdCache())
            };
            Task.WaitAll(loadDataFromDbTasks);
        }


        /// <summary>
        /// Loads all profile ids to id cache.
        /// </summary>
        private void LoadProfileIdCache()
        {
            var profileIds = DatabaseAccess.ExecuteQuery<ulong>(SQL.SelectAllSteamProfileIds);
            _profileIdCache = new HashSet<ulong>(profileIds);
        }


        /// <summary>
        /// Gets steam profile collection by sending
        /// http request to steam web api.
        /// </summary>
        /// <param name="id64s">collection of 64-bit steam ids</param>
        public virtual IEnumerable<SteamProfileModel> GetSteamProfileModels(CToken token, params ulong[] id64s)
        {
            var resultProfiles = new List<SteamProfileModel>();
            List<string> getProfilesFromWebApi = new List<string>();

            foreach (ulong id in id64s)
            {
                if (_profileIdCache.Contains(id))
                {
                    resultProfiles.AddRange(DatabaseAccess.ExecuteQuery<SteamProfileModel>(SQL.SelectSteamProfileById,
                        new { Id64 = id }));
                }
                else
                {
                    getProfilesFromWebApi.Add(id.ToString());
                }
            }

            if (getProfilesFromWebApi.Count > 0)
            {
                var profiles = GetSteamProfilesFromWebApi(token, getProfilesFromWebApi.ToArray());
                DatabaseAccess.ExecuteCommand(SQL.InsertSteamProfile, profiles);
                resultProfiles.AddRange(profiles);
            }
            
            return resultProfiles;
        }


        /// <summary>
        /// Gets single steam profile by id.
        /// </summary>
        /// <param name="id64">64-bit steam id</param>
        public virtual SteamProfileModel GetSteamProfileModel(ulong id64, CToken token)
        {
            if (_profileIdCache.Contains(id64)) // Check cache if profile exists in db
            {
                return DatabaseAccess.ExecuteQuery<SteamProfileModel>(SQL.SelectSteamProfileById,
                    new { Id64 = id64 })[0];
            }
            else 
            {
                // Get profile from web api
                var profile = GetSteamProfileFromWebAPI(id64.ToString(), token);

                // store profile to db
                DatabaseAccess.ExecuteCommand(SQL.InsertSteamProfile, profile); 

                return profile;
            }
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
                AvatarMediumBytes = GetProfilePic(apiResponse.AvatarMediumURL),
                AvatarSmallBytes = GetProfilePic(apiResponse.AvatarURL)
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
                AvatarMediumBytes = GetProfilePic(x.AvatarMediumURL),
                AvatarSmallBytes = GetProfilePic(x.AvatarURL)
            });
        }


        /// <summary>
        /// Gets image bytes by sending http request to url
        /// </summary>
        protected virtual byte[] GetProfilePic(string url) => _client.GetProfilePicBytesAsync(url)
            .Result;
    }
}
