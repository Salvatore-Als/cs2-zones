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
                if(!PlayerZoneManager.PlayerZoneManagers.ContainsKey(player))
                    continue;
                
                PlayerZoneManager.PlayerZoneManagers[player].OnPlayerTick();
            }
        }
    }
}