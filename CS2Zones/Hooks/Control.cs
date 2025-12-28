using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

namespace CS2Zones
{
    public partial class CS2Zones
    {
        public HookResult OnWeaponFire(EventWeaponFire @event, GameEventInfo info)
        {
            try
            {
                CCSPlayerController? player = @event.Userid;
        
                if(player == null || !player.IsAlive())
                    return HookResult.Continue;

                String weaponName = @event.Weapon;
                if(!weaponName.Contains("knife"))
                    return HookResult.Continue;

                FreezeCorner(player);
                return HookResult.Continue; // TODO HookResult.Handle when supported by cs#
            }
            catch (NullReferenceException)
            {
                return HookResult.Continue;
            }
        }

        public HookResult OnPlayerPing(EventPlayerPing @event, GameEventInfo info)
        {
            try
            {
                CCSPlayerController? player = @event.Userid;
                if(player == null || !player.IsValid())
                    return HookResult.Continue;

                if(!PlayerZoneManager.PlayerZoneManagers.ContainsKey(player))
                    return HookResult.Continue;

                ResetCorners(player);
                return HookResult.Continue;
            }
            catch (NullReferenceException)
            {
                return HookResult.Continue;
            }
        }
    
        private void ResetCorners(CCSPlayerController? player)
        {
            if(player == null)
                return;

            if (!player.IsValidEditor())
                return;

            if (!PlayerZoneManager.PlayerZoneManagers.TryGetValue(player, out var playerZoneManager))
                return;

            if(playerZoneManager.EditingZone == null)
                return;
            
            playerZoneManager.EditingZone.ResetCorners();
            player.PrintToChat($"{PREFIX} Zone position reset !");
        }

        private void FreezeCorner(CCSPlayerController? player)
        {
            if (player == null || !player.IsValidEditor())
                return;

            if (!PlayerZoneManager.PlayerZoneManagers.TryGetValue(player, out var playerZoneManager))
                return;

            if(playerZoneManager.EditingZone == null)
                return;

            playerZoneManager.EditingZone.Freeze();
            
            if(!playerZoneManager.EditingZone.IsStartCornerFreezed())
                player.PrintToChat($"{PREFIX} Corner START saved ! Move your crosshair to define the END corner");
            else if(!playerZoneManager.EditingZone.IsEndCornerFreezed())
                player.PrintToChat($"{PREFIX} Corner END saved !");
        }

        public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
        {
            try
            {
                CCSPlayerController? player = @event.Userid;
                if (player == null || !player.IsValid())
                    return HookResult.Continue;

                if(!PlayerPositions.ContainsKey(player))
                    PlayerPositions[player] = new Vector(0, 0, 0);

                if(!PlayerTargetPositions.ContainsKey(player))
                    PlayerTargetPositions[player] = new Vector(0, 0, 0);

                return HookResult.Continue;
            }
            catch (NullReferenceException)
            {
                return HookResult.Continue;
            }
        }

        public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
        {
            try
            {
                CCSPlayerController? player = @event.Userid;
                if (player == null)
                    return HookResult.Continue;

                CS2Zones.PlayerPositions.Remove(player);
                CS2Zones.PlayerTargetPositions.Remove(player);
            
                return HookResult.Continue;
            }
            catch (NullReferenceException)
            {
                return HookResult.Continue;
            }
        }
    }
}
