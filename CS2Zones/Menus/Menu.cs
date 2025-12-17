using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using MenuManager;

namespace CS2Zones
{
    public partial class CS2Zones
    {
        private bool _drawnZones = false;
        
        private void DelayShowMainMenu(CCSPlayerController player)
        {
            AddTimer(0.1f, () => {
                ShowMainMenu(player);
            });
        }

        private void ShowMainMenu(CCSPlayerController player)
        {
            if (_menuApi == null)
                return;

            if(!PlayerZoneManager.PlayerZoneManagers.ContainsKey(player))
                new PlayerZoneManager(player);

            PlayerZoneManager playerZoneManager = PlayerZoneManager.PlayerZoneManagers[player];
            
            if (playerZoneManager.IsEditingZone())
            {
                player.PrintToChat($"{PREFIX} You have a zone in edit mode. Use !zsave to save or !zabort to cancel, then you can open the menu again.");
                return;
            }

            var menu = _menuApi.GetMenu("Zones management");

            menu.AddMenuOption("Add a zone", (p, option) =>
            {
                _menuApi?.CloseMenu(p);
                OnAddZone(p);
            });

            menu.AddMenuOption("Edit a zone", (p, option) =>
            {
                ShowEditZoneMenu(p);
            });

            menu.AddMenuOption("Delete a zone", (p, option) =>
            {
                ShowDeleteZoneMenu(p);
            });

            string drawnZonesText = _drawnZones ? "Hide zones" : "Show zones";
            menu.AddMenuOption(drawnZonesText, (p, option) =>
            {
                List<Zone> zones = ZoneManager.Zones.ToList();
            
                if (zones.Count == 0)
                {
                    player.PrintToChat($"{PREFIX} No zone available");
                    return;
                }

                _drawnZones = !_drawnZones;
                _menuApi?.CloseMenu(p);
                DelayShowMainMenu(p);
            
                foreach (var zone in zones)
                {
                    zone.Drawn = _drawnZones;
                }
    
                string resultText = _drawnZones ? $"{PREFIX} Zones are displayed" : $"{PREFIX} Zones are hidden";
                player.PrintToChat(resultText);
            });

            menu.Open(player);
        }

        private void OnAddZone(CCSPlayerController player)
        {
            if (!PlayerZoneManager.PlayerZoneManagers.ContainsKey(player))
                new PlayerZoneManager(player);

            PlayerZoneManager playerZoneManager = PlayerZoneManager.PlayerZoneManagers[player];
            
            playerZoneManager.StartNewZone();
            this.PrintHelper(player);
        }

        private void ShowEditZoneMenu(CCSPlayerController player)
        {
            ShowZoneMenu(player, "Select a zone to edit", (p, option) =>
            {
                _menuApi?.CloseMenu(p);
                OnEditZone(p, option.Text);
            });
        }

        private void OnEditZone(CCSPlayerController player, string zoneName)
        {
            Zone? zone = ZoneManager.GetZoneByName(zoneName);
            if (zone == null)
            {
                player.PrintToChat($"{PREFIX} The zone '{zoneName}' does not exist");
                return;
            }

            if (!PlayerZoneManager.PlayerZoneManagers.ContainsKey(player))
                new PlayerZoneManager(player);

            PlayerZoneManager playerZoneManager = PlayerZoneManager.PlayerZoneManagers[player];
            
            if (playerZoneManager.IsEditingZone())
            {
                player.PrintToChat($"{PREFIX} You are already editing a zone");
                return;
            }

            if (!playerZoneManager.EditZone(zone))
            {
                player.PrintToChat($"{PREFIX} Unable to put the zone in edit mode");
                return;
            }

            player.PrintToChat($"{PREFIX} You are now editing the zone '{zoneName}'");
            this.PrintHelper(player);
        }

        private void ShowDeleteZoneMenu(CCSPlayerController player)
        {
            ShowZoneMenu(player, "Select a zone to delete", (p, option) =>
            {
                _menuApi?.CloseMenu(p);
                DelayShowMainMenu(p);
                OnDeleteZone(p, option.Text);
            });
        }

        private void OnDeleteZone(CCSPlayerController player, string zoneName)
        {
            Zone? zone = ZoneManager.GetZoneByName(zoneName);
            if (zone == null)
            {
                player.PrintToChat($"{PREFIX} The zone '{zoneName}' does not exist");
                return;
            }

            zone.Delete();
            player.PrintToChat($"{PREFIX} Zone '{zoneName}' deleted successfully");
        }

        private void ShowZoneMenu(CCSPlayerController player, string title, Action<CCSPlayerController, ChatMenuOption> callback)
        {
            if (_menuApi == null)
                return;

            HashSet<Zone> zonesBeingEdited = new HashSet<Zone>();
            foreach (var playerZoneManager in PlayerZoneManager.PlayerZoneManagers.Values)
            {
                if (playerZoneManager.EditingZone != null)
                    zonesBeingEdited.Add(playerZoneManager.EditingZone);
            }

            List<Zone> savedZones = ZoneManager.Zones
                .Where(zone => !zonesBeingEdited.Contains(zone))
                .ToList();
            
            if (savedZones.Count == 0)
            {
                player.PrintToChat($"{PREFIX} No zone available");
                return;
            }

            IMenu menu = _menuApi.GetMenu(title);

            foreach (Zone zone in savedZones)
            {
                menu.AddMenuOption(zone.Name, (p, option) =>
                {
                    callback(p, option);
                });
            }

            menu.Open(player);
        }

        private void PrintHelper(CCSPlayerController player)
        {
            if(player == null || !player.IsValid())
                return;

            player.PrintToChat($"{PREFIX} New zone in edit mode ! Use your crosshair to define the corners.");
            player.PrintToChat($"{PREFIX} Commands:");
            player.PrintToChat($"{PREFIX} - Left click with knife to define a corner.");
            player.PrintToChat($"{PREFIX} - Ping to reset the corners.");
            player.PrintToChat($"{PREFIX} - Use !zname <name> to name the zone.");
            player.PrintToChat($"{PREFIX} - Use !zsave to save the zone.");
            player.PrintToChat($"{PREFIX} - Use !zabort to cancel the edit.");
        }
    }
}

