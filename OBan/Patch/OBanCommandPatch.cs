namespace OBan.Patch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandSystem;
    using CommandSystem.Commands.RemoteAdmin;

    using Exiled.API.Features;

    using GameCore;

    using HarmonyLib;

    using OBan.DataBase;

    using static OBan.DataBase.Structure;

    [HarmonyPatch(typeof(OfflineBanCommand), nameof(OfflineBanCommand.Execute))]
    internal class OBanCommandPatch
    {
        private static void Postfix(ArraySegment<string> arguments, ICommandSender sender, ref bool __result, ref string response)
        {
            if (!__result)
                return;

            Player player = Player.Get(sender);

            string admin = player != null ? player.Nickname : "Dedicated server";
            string target = arguments.At(0);
            long time = Misc.RelativeTimeToSeconds(arguments.At(1), 60);
            string reason = arguments.At(2);

            DateTime overDate = DateTime.Now.AddSeconds(time);

            List<Player> targetsList = Player.List.Where(p => p.UserId == target || p.IPAddress == target).ToList();

            foreach (Player targetPlayer in targetsList)
            {
                targetPlayer.Disconnect(Plugin.plugin.Config.DisconnectReason.Replace("%reason%", reason).Replace("%issuer%", admin).Replace("%overdate%", overDate.ToString(Plugin.plugin.Config.TimeConstruction)));

                if (ConfigFile.ServerConfig.GetBool("broadcast_kicks"))
                {
                    string broadcastText = ConfigFile.ServerConfig.GetString("broadcast_kick_text", "%nick% has been kicked from this server.").Replace("%nick%", targetPlayer.Nickname);
                    ushort broadcastDuration = ConfigFile.ServerConfig.GetUShort("broadcast_kick_duration", 5);
                    Map.Broadcast(broadcastDuration, broadcastText);
                }
            }

            if (Extensions.TryGetValue(target, out PlayerInfo info))
            {
                List<string> Ips = new List<string>();
                if (info.IPs.Count > Plugin.plugin.Config.BanLastIPs)
                    Ips = info.IPs.Skip(info.IPs.Count - Plugin.plugin.Config.BanLastIPs).ToList();
                else
                    Ips = info.IPs.ToList();

                foreach (string ip in Ips)
                {
                    BanPlayer(info.Nicknames.LastOrDefault(), ip, time, reason, admin);
                }
            }
            else if (Extensions.TryGetByValue(target, out info) && Plugin.plugin.Config.BanId)
            {
                BanPlayer(info.Nicknames.LastOrDefault(), info.UserId, time, reason, admin);
            }
        }

        private static void BanPlayer(string nick, string id, long time, string reason, string issuer)
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
