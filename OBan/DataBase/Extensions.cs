using Exiled.API.Features;
using LiteDB;
using System.Collections.Generic;
using static OBan.DataBase.Structure;

namespace OBan.DataBase
{
    public static class Extensions
    {
        public static ILiteCollection<PlayerInfo> PlayerInfoCollection => Plugin.Database.GetCollection<PlayerInfo>($"OBan{Server.Port}");

        public static void InsertPlayer(string userId, string nickname, string ip)
        {
            PlayerInfo insert = new PlayerInfo()
            {
                UserId = userId,
                LastIp = ip,
                Nicknames = new List<string> { nickname },
                IPs = new List<string> { ip }
            };

            PlayerInfoCollection.Insert(insert);
        }

        public static bool TryGetValue(string id, out PlayerInfo info)
        {
            info = PlayerInfoCollection.FindById(id);
            return info != null;
        }

        public static bool TryGetByValue(string id, out PlayerInfo info)
        {
            info = PlayerInfoCollection.FindOne(x => x.UserId == id);
            return info != null;
        }

        public static void DeletePlayer(string playerId)
        {
            if (!TryGetValue(playerId, out PlayerInfo info))
                return;

            PlayerInfoCollection.Delete(playerId);
        }
    }
}
