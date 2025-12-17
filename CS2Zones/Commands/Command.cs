using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace CS2Zones
{
    public partial class CS2Zones
    {
        [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_ONLY)]
        //[RequiresPermissions("@css/zones")]
        public void OnNewZonesCommand(CCSPlayerController? player, CommandInfo command)
        {
            if(player == null || !player.IsValid())
                return;

            if(!player.IsAlive()) 
            {
                player.PrintToChat("i18n Vous devez être vivant pour utiliser cette commande");
                return;
            }

            if(!PlayerZoneManager.PlayerZoneManagers.ContainsKey(player)) 
                new PlayerZoneManager(player);

            PlayerZoneManager playerZoneManager = PlayerZoneManager.PlayerZoneManagers[player];
            
            playerZoneManager.StartNewZone();
            player.PrintToChat("i18n Nouvelle zone en édition ! Utilisez votre viseur pour définir les coins.");
            player.PrintToChat("i18n Utilisez !setname <nom> pour nommer la zone.");
        }

        [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_ONLY)]
        //[RequiresPermissions("@css/zones")]
        public void OnSaveCommand(CCSPlayerController? player, CommandInfo command)
        {
            if(player == null || !player.IsValid())
                return;

            if(!player.IsAlive()) 
            {
                player.PrintToChat("i18n Vous devez être vivant pour utiliser cette commande");
                return;
            }

            if(!PlayerZoneManager.PlayerZoneManagers.ContainsKey(player)) 
            {
                player.PrintToChat("i18n Aucune zone en cours d'édition");
                return;
            }

            PlayerZoneManager playerZoneManager = PlayerZoneManager.PlayerZoneManagers[player];
            if(!playerZoneManager.IsEditingZone()) 
            {
                player.PrintToChat("i18n Vous n'avez pas de zone en cours d'edition");
                return;
            }

            playerZoneManager.SaveZone();
            player.PrintToChat("i18n Zone sauvegardée avec succès !");
        }

        [CommandHelper(minArgs: 1, usage: "<nom>", whoCanExecute: CommandUsage.CLIENT_ONLY)]
        //[RequiresPermissions("@css/zones")]
        public void OnSetNameCommand(CCSPlayerController? player, CommandInfo command)
        {
            if(player == null || !player.IsValid())
                return;

            if(!player.IsAlive()) 
            {
                player.PrintToChat("i18n Vous devez être vivant pour utiliser cette commande");
                return;
            }

            if(!PlayerZoneManager.PlayerZoneManagers.ContainsKey(player)) 
            {
                player.PrintToChat("i18n Aucune zone en cours d'édition");
                return;
            }

            PlayerZoneManager playerZoneManager = PlayerZoneManager.PlayerZoneManagers[player];
            if(!playerZoneManager.IsEditingZone()) 
            {
                player.PrintToChat("i18n Vous n'avez pas de zone en cours d'edition");
                return;
            }

            string zoneName = command.GetArg(1);
            if(string.IsNullOrWhiteSpace(zoneName)) 
            {
                player.PrintToChat("i18n Le nom de la zone ne peut pas être vide");
                return;
            }

            if(playerZoneManager.EditingZone == null) 
            {
                player.PrintToChat("i18n Aucune zone en cours d'édition");
                return;
            }

            playerZoneManager.EditingZone.Name = zoneName;
            player.PrintToChat($"i18n Le nom de la zone a été changé pour : {zoneName}");
        }

        [CommandHelper(minArgs: 1, usage: "<zonename>", whoCanExecute: CommandUsage.CLIENT_ONLY)]
        //[RequiresPermissions("@css/zones")]
        public void OnEditCommand(CCSPlayerController? player, CommandInfo command)
        {
            if(player == null || !player.IsValid())
                return;

            if(!player.IsAlive()) 
            {
                player.PrintToChat("i18n Vous devez être vivant pour utiliser cette commande");
                return;
            }

            string zoneName = command.GetArg(1);
            if(string.IsNullOrWhiteSpace(zoneName)) 
            {
                player.PrintToChat("i18n Vous devez spécifier le nom de la zone");
                return;
            }

            Zone? zone = ZoneManager.GetZoneByName(zoneName);
            if(zone == null) 
            {
                player.PrintToChat($"i18n La zone '{zoneName}' n'existe pas");
                return;
            }

            if(!PlayerZoneManager.PlayerZoneManagers.ContainsKey(player)) 
                new PlayerZoneManager(player);

            PlayerZoneManager playerZoneManager = PlayerZoneManager.PlayerZoneManagers[player];
            
            if(playerZoneManager.IsEditingZone()) 
            {
                player.PrintToChat("i18n Vous avez déjà une zone en cours d'édition. Utilisez !save pour la sauvegarder d'abord.");
                return;
            }

            if(!playerZoneManager.EditZone(zone)) 
            {
                player.PrintToChat("i18n Impossible de mettre la zone en mode édition");
                return;
            }

            player.PrintToChat($"i18n Zone '{zoneName}' mise en mode édition");
            player.PrintToChat("i18n Utilisez votre viseur pour modifier les coins de la zone.");
        }

        [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_ONLY)]
        //[RequiresPermissions("@css/zones")]
        public void OnZoneAbortCommand(CCSPlayerController? player, CommandInfo command)
        {
            if(player == null || !player.IsValid())
                return;

            if(!player.IsAlive()) 
            {
                player.PrintToChat("i18n Vous devez être vivant pour utiliser cette commande");
                return;
            }

            if(!PlayerZoneManager.PlayerZoneManagers.ContainsKey(player)) 
            {
                player.PrintToChat("i18n Aucune zone en cours d'édition");
                return;
            }

            PlayerZoneManager playerZoneManager = PlayerZoneManager.PlayerZoneManagers[player];
            if(!playerZoneManager.IsEditingZone()) 
            {
                player.PrintToChat("i18n Vous n'avez pas de zone en cours d'edition");
                return;
            }

            bool wasSavedZone = playerZoneManager.IsEditingSavedZone();
            playerZoneManager.AbortEditing();

            if(wasSavedZone) 
                player.PrintToChat("i18n Édition annulée. La zone a été restaurée à son état initial.");
            else
                player.PrintToChat("i18n Édition annulée. La zone non sauvegardée a été supprimée.");
        }
    }
}
