using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;

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
                return HookResult.Continue; // Handle when supported by cs#
            }
            catch (NullReferenceException)
            {
                return HookResult.Continue;
            }
        }

        // TODO : on player ping pour reset les corners

        private void FreezeCorner(CCSPlayerController? player)
        {
            if(player == null || !player.IsValid())
                return;

            if(!PlayerZoneManager.PlayerZoneManagers.ContainsKey(player))
            {
                player.PrintToChat("i18n Aucune zone en cours d'édition");
                return;
            }

            PlayerZoneManager playerZoneManager = PlayerZoneManager.PlayerZoneManagers[player];
            if(!playerZoneManager.IsEditingZone()) 
            {
                player.PrintToChat("i18n Vous n'avez pas de zone en cours de création");
                return;
            }

            if(playerZoneManager.EditingZone == null)
                return;

            playerZoneManager.EditingZone.Freeze();
            
            // Afficher un message selon le corner sauvegardé
            if(!playerZoneManager.EditingZone.IsStartCornerFreezed())
                player.PrintToChat("i18n Corner START sauvegardé ! Déplacez votre viseur pour définir le corner END");
            else if(!playerZoneManager.EditingZone.IsEndCornerFreezed())
                player.PrintToChat("i18n Corner END sauvegardé ! La zone est complète");
            else
                player.PrintToChat("i18n Les deux corners sont déjà sauvegardés");
        }
    }
}
