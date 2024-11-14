using HarmonyLib;
using Exiled.API.Features;
using System;
using CommandSystem.Commands.RemoteAdmin;
using CommandSystem;
using static OBan.DataBase.Structure;
using System.Linq;
using OBan.DataBase;

namespace OBan.Patch
{
    [HarmonyPatch(typeof(OfflineBanCommand), nameof(OfflineBanCommand.Execute))]
    internal class OBanCommandPatch
    {
        static void Postfix(ArraySegment<string> arguments, ICommandSender sender, ref bool __result, ref string response)
        {
            if (!__result)
                return;

            Player player = Player.Get(sender);

            string admin = player != null ? player.Nickname : "Dedicated server";
            string target = arguments.At(0);
            long time = Misc.RelativeTimeToSeconds(arguments.At(1), 60);
            string reason = arguments.At(2);

            DateTime overDate = DateTime.Now.AddSeconds(time);

            Player[] targetsList = Player.List.Where(p => p.UserId == target || p.IPAddress == target).ToArray();

            if (targetsList.Any())
            {
                targetsList.ForEach(t => t.Disconnect(Plugin.plugin.Config.DisconnectReason.Replace("%reason%", reason).Replace("%issuer%", admin).Replace("%overdate%", overDate.ToString(Plugin.plugin.Config.TimeConstruction))));
            }

            if (Extensions.TryGetValue(target, out PlayerInfo info))
            {
                string[] Ips = info.IPs.Skip(info.IPs.Count - Plugin.plugin.Config.BanLastIPs).ToArray();

                foreach (string ip in Ips)
                {
                    BanPlayer(info.Nicknames.LastOrDefault(), ip, time, reason, admin);
                }
            }
            else if (Extensions.TryGetByValue(target, out info))
            {
                BanPlayer(info.Nicknames.LastOrDefault(), info.UserId, time, reason, admin);
            }
        }

        static void BanPlayer(string nick, string id, long time, string reason, string issuer)
        {
            bool IsIp = Misc.ValidateIpOrHostname(id, out Misc.IPAddressType type, false, false);

            BanHandler.IssueBan(new BanDetails
            {
                OriginalName = nick,
                Id = id,
                IssuanceTime = TimeBehaviour.CurrentTimestamp(),
                Expires = TimeBehaviour.GetBanExpirationTime((uint)time),
                Reason = reason,
                Issuer = issuer
            }, IsIp ? BanHandler.BanType.IP : BanHandler.BanType.UserId);
        }
    }
}
