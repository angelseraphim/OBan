namespace OBan.EventHandlers
{
    using Exiled.Events.EventArgs.Player;

    using static OBan.DataBase.Extensions;
    using static OBan.DataBase.Structure;

    internal class PlayerEvents
    {
        private readonly Plugin plugin;

        public PlayerEvents(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void Register()
        {
            Exiled.Events.Handlers.Player.Verified += OnVerified;
        }

        public void Unregister()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
        }

        private void OnVerified(VerifiedEventArgs ev)
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
