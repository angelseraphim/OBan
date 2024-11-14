using LiteDB;
using System;
using System.Collections.Generic;

namespace OBan.DataBase
{
    public class Structure
    {
        [Serializable]
        public class PlayerInfo
        {
            [BsonId]
            public string UserId { get; set; }
            public string LastIp { get; set; }
            public List<string> Nicknames { get; set; }
            public List<string> IPs { get; set; }
        }
    }
}
