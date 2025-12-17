using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace CS2Zones
{
    public partial class CS2Zones
    {
        [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_ONLY)]
        [RequiresPermissions("@css/zones")]
        public void OnZonesMenuCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (_menuApi == null)
                return;
            
            if (player == null || !player.IsValid() || !player.IsAlive())
                return;

            ShowMainMenu(player);
        }
    
        [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_ONLY)]
        [RequiresPermissions("@css/zones")]
        public void OnSaveCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || !player.IsValidEditor())
                return;

            if (!PlayerZoneManager.PlayerZoneManagers.TryGetValue(player, out var playerZoneManager))
                return;

            try
            {
                playerZoneManager.SaveZone();
                player.PrintToChat($"{PREFIX} Zone saved successfully !");
            }
            catch(Exception ex)
            {
                player.PrintToChat($"{PREFIX} {ex.Message}");
                return;
            }
        }

        [CommandHelper(minArgs: 1, usage: "<name>", whoCanExecute: CommandUsage.CLIENT_ONLY)]
        [RequiresPermissions("@css/zones")]
        public void OnSetNameCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || !player.IsValidEditor())
                return;

            if (!PlayerZoneManager.PlayerZoneManagers.TryGetValue(player, out var playerZoneManager))
                return;

            if (playerZoneManager.EditingZone == null)
                return;

            string zoneName = command.GetArg(1);
            if(string.IsNullOrWhiteSpace(zoneName)) {
                player.PrintToChat($"{PREFIX} Name is required !");
                return;
            }

            if(ZoneManager.GetZoneByName(zoneName) != null) {
                player.PrintToChat($"{PREFIX} Name already used !");
                return;
            }

            playerZoneManager.EditingZone.Name = zoneName;
        }


        [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_ONLY)]
        [RequiresPermissions("@css/zones")]
        public void OnZoneAbortCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || !player.IsValidEditor())
                return;

            if (!PlayerZoneManager.PlayerZoneManagers.TryGetValue(player, out var playerZoneManager))
                return;

            bool wasSavedZone = playerZoneManager.IsEditingSavedZone();
            playerZoneManager.AbortEditing();

            if(wasSavedZone) 
                player.PrintToChat($"{PREFIX} Edition cancelled. The zone has been restored to its initial state.");
            else
                player.PrintToChat($"{PREFIX} Edition cancelled. The unsaved zone has been deleted.");
        }
    }
}
