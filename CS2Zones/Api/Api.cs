using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

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
            
            Zone? zone = ZoneManager.GetZoneByName(zoneName);
            if (zone == null)
                return false;

            return zone.PlayersInZone.Contains(player);
        }

        public void TeleportPlayerInZone(CCSPlayerController player, string zoneName)
        {
            if (player == null || !player.IsValid() || !player.IsAlive()) 
                return;

            var pawn = player.Pawn();
            if(pawn == null)
                return;

            Zone? zone = ZoneManager.GetZoneByName(zoneName);
            if (zone == null)
                return;
    
            QAngle eyeAngle = pawn.EyeAngles;
            Vector middle = zone.GetMiddle();
            pawn.Teleport(middle, eyeAngle, new Vector(0, 0, 0));
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