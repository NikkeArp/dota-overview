using DataAPI.Database;
using SteamApi;

namespace DataAPI
{
    /// <summary>
    /// Container for data access to Steam and Dota 2 application data.
    /// 
    /// Can not be inherited from. If you wish to create new data access
    /// logic, inherit from SteamApiDataAccess or Dota2ApiDataAccess.
    /// </summary>
    public sealed class DataAccess
    {
        public SteamApiDataAccess SteamData { get; }
        public Dota2ApiDataAccess Dota2Data { get; }

        /// <summary>
        /// Instantiates DataAccess object
        /// </summary>
        /// <param name="devKey">Steam web api developer key: https://steamcommunity.com/dev </param>
        /// <param name="steamDataAccess">object that handles general Steam data</param>
        /// <param name="dota2DataAccess">object that handles Dota 2 game data</param>
        public DataAccess(string devKey, string connectionString, SteamApiDataAccess steamDataAccess, Dota2ApiDataAccess dota2DataAccess)
        {
            DatabaseAccess.Init(connectionString);
            ApiClient.SetDeveloperKey(devKey);

            SteamData = steamDataAccess;
            SteamData.Init();

            Dota2Data = dota2DataAccess;
        }
    }
}
