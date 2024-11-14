using Exiled.API.Interfaces;
using System.ComponentModel;

namespace OBan
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("Ban SteamId if ip was banned")]
        public string DataBasePath { get; set; } = "%config%/%database%";
        [Description("Ban SteamId if ip was banned")]
        public bool BanId { get; set; } = true;
        [Description("Ban last IPs if SteamId was banned (0 to disable)")]
        public int BanLastIPs { get; set; } = 1;
        [Description("Date constuction")]
        public string TimeConstruction { get; set; } = "dd/MM/yyyy HH:mm";
        [Description("Disconnect reason (If player connected)")]
        public string DisconnectReason { get; set; } = "You have been BANNED from this server for %overdate%\nReason: %reason% \nBanned by %issuer%";
    }
}
