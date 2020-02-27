using System;
using System.Collections.Generic;
using System.Text;

namespace DataAPI.Models
{
    [Serializable]
    class FriendsListModel
    {
        public uint Id { get; set; }
        List<string> UserIds { get; set; }
    }
}
