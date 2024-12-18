namespace OBan
{
    using Exiled.API.Features;
    using HarmonyLib;

    using LiteDB;
    using OBan.EventHandlers;

    public class Plugin : Plugin<Config>
    {
        public override string Prefix => "OBan";
        public override string Name => "OBan";
        public override string Author => "angelseraphim. & Rysik5318";

        public static Plugin plugin;
        public static Harmony Harmony;
        public static LiteDatabase Database;

        internal static GetDirectory getDirectory;
        internal static PlayerEvents playerEvents;

        public override void OnEnabled()
        {
            Harmony = new Harmony("OBan");
            plugin = this;
            getDirectory = new GetDirectory();
            Database = new LiteDatabase(Config.DataBasePath.Replace("%config%", getDirectory.GetParentDirectory()).Replace("%database%", $"OBan{Server.Port}.db"));

            playerEvents = new PlayerEvents(this);
            playerEvents.Register();

            Harmony.PatchAll();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            plugin = null;
            Database.Dispose();
            Database = null;

            playerEvents.Unregister();
            playerEvents = null;

            Harmony.UnpatchAll();
            base.OnDisabled();
        }
    }
}
