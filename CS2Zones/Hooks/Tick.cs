using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CS2Zones
{
    public partial class CS2Zones
    {
        public void OnTick()
        {
            List<CCSPlayerController> players = Utilities.GetPlayers()
                .Where(player => player.IsValid() && player.IsAlive())
                .ToList();
            
            foreach(Zone zone in ZoneManager.Zones)
            {
                zone.OnTick();
            }

            foreach(var player in players)
            {
                // Update static player positions because of Vector Ram Leak
                UpdatePlayerPositions(player);
            
                if(!PlayerZoneManager.PlayerZoneManagers.ContainsKey(player))
                    continue;
                
                PlayerZoneManager.PlayerZoneManagers[player].OnPlayerTick();
            }
        }

        private void UpdatePlayerPositions(CCSPlayerController player)
        {
            if (player == null || !player.IsValid())
                return;

            if(!PlayerPositions.ContainsKey(player))
                PlayerPositions[player] = new Vector(0, 0, 0);

            PlayerPositions[player].X = player.PlayerPawn.Value?.AbsOrigin.X ?? 0;
            PlayerPositions[player].Y = player.PlayerPawn.Value?.AbsOrigin.Y ?? 0;
            PlayerPositions[player].Z = player.PlayerPawn.Value?.AbsOrigin.Z ?? 0;
        }
    }
}