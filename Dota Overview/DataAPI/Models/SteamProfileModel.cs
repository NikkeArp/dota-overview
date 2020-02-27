using SteamApi;
using SteamApi.Models;

namespace DataAPI.Models
{
    /// <summary>
    /// Model to represent single steam profile.
    /// </summary>
    public class SteamProfileModel
    {
        /// <summary>
        /// 64-bit Steam id.
        /// Notes: Steam api uses 64-bit id for profiles
        /// </summary>
        public ulong Id64 { get; set; }

        /// <summary>
        /// 32-bit Steam id.
        /// Notes: Valve's game APIs mainly use 32-bit id.
        /// </summary>
        public uint Id32 => SteamIdConverter.SteamIdTo32(Id64);

        /// <summary>
        /// Steam username.
        /// Notes: can change often
        /// </summary>
        public string PersonaName { get; set; }

        /// <summary>
        /// ISO 3166 country code.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Unix epoch timestamp of creation datetime
        /// </summary>
        public ulong TimeCreated { get; set; }

        /// <summary>
        /// Unix epoch timestamp for last logoff.
        /// Notes: usually private information
        /// </summary>
        public ulong LastLogOff { get; set; }

        /// <summary>
        ///     1 = Private,
        ///     2 = Friends Only,
        ///     3 = Friends of firends,
        ///     4 = Users Only,
        ///     5 = Public
        /// </summary>
        public uint VisibilityState { get; set; }

        /// <summary>
        /// Users current profile state:
        ///     0 = Offline OR private profile,
        ///     1 = Online,
        ///     2 = Busy,
        ///     3 = Away,
        ///     4 = Snooze,
        ///     5 = Looking to trade,
        ///     6 = Looking to play
        /// </summary>
        public uint PersonaState { get; set; }

        /// <summary>
        /// If set to 1 the user has configured the profile.
        /// </summary>
        public uint ProfileState { get; set; }

        /// <summary>
        /// Full (184x184) sized avatar image url
        /// </summary>
        public string AvatarFullUrl { get; set; }

        /// <summary>
        /// Medium (64x64) sized avatar image url
        /// </summary>
        public string AvatarMediumUrl { get; set; }

        /// <summary>
        /// Small (32x32) sized avatar image url
        /// </summary>
        public string AvatarSmallUrl { get; set; }

        /// <summary>
        /// Full (184x184) avatar image as bytes
        /// </summary>
        public byte[] AvatarFullBytes { get; set; }

        /// <summary>
        /// Medium (64x64) avatar image as bytes
        /// </summary>
        public byte[] AvatarMediumBytes { get; set; }

        /// <summary>
        /// Instantiates SteamProfileModel object from
        /// steam api response model.
        /// </summary>
        /// <param name="account">steam api response model</param>
        public SteamProfileModel(SteamAccount account)
        {
            Id64 = ulong.Parse(account.Id);
            PersonaName = account.PersonaName;
            AvatarFullUrl = account.AvatarFullURL;
            AvatarMediumUrl = account.AvatarMediumURL;
            AvatarSmallUrl = account.AvatarURL;
            CountryCode = account.LocCountryCode;
            VisibilityState = account.CommunityVisibilityState;
            TimeCreated = account.TimeCreated;
            LastLogOff = account.LastLogOff;
            PersonaState = account.PersonaState;
            ProfileState = account.ProfileState;
        }
    }
}