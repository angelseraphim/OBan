using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using HarmonyLib;
using LiteDB;
using static OBan.DataBase.Extensions;
using static OBan.DataBase.Structure;

namespace OBan
{
    public class Plugin : Plugin<Config>
    {
        public override string Prefix => "OBan";
        public override string Name => "OBan";
        public override string Author => "angelseraphim.";

        public static Plugin plugin;
        public static Harmony Harmony;
        public static LiteDatabase Database;
        public static GetDirectory getDirectory;

        public override void OnEnabled()
        {
            Harmony = new Harmony("OBan");
            plugin = this;
            getDirectory = new GetDirectory();
            Database = new LiteDatabase(Config.DataBasePath.Replace("%config%", getDirectory.GetParentDirectory()).Replace("%database%", $"OBan{Server.Port}.db"));
            
            Exiled.Events.Handlers.Player.Verified += OnVerified;

            Harmony.PatchAll();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            plugin = null;
            Database.Dispose();
            Database = null;

            Exiled.Events.Handlers.Player.Verified -= OnVerified;

            Harmony.UnpatchAll();
            base.OnDisabled();
        }

        public void OnVerified(VerifiedEventArgs ev)
        {
            if (ev.Player == null || string.IsNullOrEmpty(ev.Player.UserId))
                return;

            if (!TryGetValue(ev.Player.UserId, out PlayerInfo info))
            {
                InsertPlayer(ev.Player.UserId, ev.Player.Nickname, ev.Player.IPAddress);
                return;
            }

            info.LastIp = ev.Player.IPAddress;

            if (!info.Nicknames.Contains(ev.Player.Nickname))
                info.Nicknames.Add(ev.Player.Nickname);

            if (!info.IPs.Contains(ev.Player.IPAddress))
                info.IPs.Add(ev.Player.IPAddress);
            
            PlayerInfoCollection.Update(info);
        }
    }
}
