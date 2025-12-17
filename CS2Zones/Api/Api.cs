using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;

namespace CS2Zones
{
    public class CS2ZonesAPI : ICS2ZonesAPI
    {
        public event Action<CCSPlayerController, string>? OnPlayerEnterZone;
        public event Action<CCSPlayerController, string>? OnPlayerLeaveZone;

        public bool IsPlayerInZone(CCSPlayerController? player, string zoneName)
        {
            if (player == null || !player.IsValid())
                return false;

            if (CS2Zones.globalCtx == null)
                return false;

            Zone zone = ZoneManager.GetZoneByName(zoneName);
            if (zone == null)
                return false;

            return zone.PlayersInZone.Contains(player);
        }

        internal void TriggerPlayerEnterZone(CCSPlayerController player, string zoneName)
        {
            OnPlayerEnterZone?.Invoke(player, zoneName);
        }

        internal void TriggerPlayerLeaveZone(CCSPlayerController player, string zoneName)
        {
            OnPlayerLeaveZone?.Invoke(player, zoneName);
        }
    }
}