using SteamApi;
using DataAPI.Models;
using CToken = System.Threading.CancellationToken;

namespace DataAPI
{
    /// <summary>
    /// Handles data related to Dota 2 game.
    /// 
    /// Internally handles data access from dota2 web api
    /// and local SQLite database.
    /// 
    /// Publicly exposes only methods that return data
    /// models. Decides internally which data source
    /// to use.
    /// </summary>
    public class Dota2ApiDataAccess
    {
        protected DotaApiClient _client;

        public Dota2ApiDataAccess()
        {
            _client = new DotaApiClient();
        }
    }
}
