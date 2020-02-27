using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace DataAPI.Database
{
    public partial class DatabaseAccess
    {
        private static string _connStr;
        private static Dictionary<SQL, string> _storedQueries;

        public static void Init(string connectionString)
        {
            if (_storedQueries == null)
            {
                _storedQueries = new Dictionary<SQL, string>();
                LoadStoredQueries();
            }
            if (string.IsNullOrEmpty(_connStr))
            {
                _connStr = connectionString;
            }
        }

        private static void LoadStoredQueries()
        {
            #region SteamProfile Queries
            _storedQueries.Add(SQL.SelectAllSteamProfileIds,
                "SELECT Id64 FROM SteamProfiles");

            _storedQueries.Add(SQL.SelectSteamProfileById,
                "SELECT * FROM SteamProfiles WHERE Id64 = @Id64");

            _storedQueries.Add(SQL.SelectSteamProfilesByCountry, 
                "SELECT * FROM SteamProfiles WHERE CountryCode = @CountryCode");

            _storedQueries.Add(SQL.InsertSteamProfile,
                @"INSERT INTO SteamProfiles
                    (Id64, PersonaName, CountryCode, TimeCreated, LastLogOff, VisibilityState,
                    PersonaState, ProfileState, AvatarFullUrl, AvatarMediumUrl, AvatarSmallUrl,
                    AvatarFullBytes, AvatarMediumBytes, AvatarSmallBytes)
                 VALUES
                    (@Id64, @PersonaName, @CountryCode, @TimeCreated, @LastLogOff, @VisibilityState,
                    @PersonaState, @ProfileState, @AvatarFullUrl, @AvatarMediumUrl, @AvatarSmallUrl,
                    @AvatarFullBytes, @AvatarMediumBytes, @AvatarSmallBytes)");
            #endregion
        }

        private static string FetchQuery(SQL key)
        {
            if (_storedQueries.TryGetValue(key, out string query))
            {
                return query;
            }
            else
            {
                throw new Exception("Query not found");
            }
        }

        public static void ExecuteCommand(SQL queryKey, object obj = null)
        {
            string query = FetchQuery(queryKey);
            using (IDbConnection connection = new SQLiteConnection(_connStr))
            {
                connection.Execute(query, obj);
            }
        }

        public static IReadOnlyList<T> ExecuteQuery<T>(SQL queryKey, object obj = null)
        {
            string query = FetchQuery(queryKey);
            using (IDbConnection connection = new SQLiteConnection(_connStr))
            {
                return connection.Query<T>(query, obj).AsList();
            }
        }
    }
}
