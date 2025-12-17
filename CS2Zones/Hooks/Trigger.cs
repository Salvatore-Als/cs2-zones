using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;

namespace CS2Zones
{
    public partial class CS2Zones
    {
        // TODO : enable this later when create trigger multiple will not crash the server
    
        /*public HookResult TriggerMultipleOnStartTouch(CEntityInstance activator, CEntityInstance caller)
        {
            try
            {
                if(!ValidateTriggerMultiple(activator, caller))
                    return HookResult.Continue;

                var player = new CCSPlayerController(new CCSPlayerPawn(activator.Handle).Controller.Value!.Handle);
                string triggerName = caller.Entity.Name;

                return HookResult.Continue;
            }
            catch (NullReferenceException)
            {
                return HookResult.Continue;
            }
        }

        public HookResult TriggerMultipleOnEndTouch(CEntityInstance activator, CEntityInstance caller)
        {
            try
            {
                if(!ValidateTriggerMultiple(activator, caller))
                    return HookResult.Continue;

                var player = new CCSPlayerController(new CCSPlayerPawn(activator.Handle).Controller.Value!.Handle);
                string triggerName = caller.Entity.Name;
                // TODO : call la fonction api on leave zone
                Server.PrintToChatAll($"Player {player.PlayerName} left zone {triggerName}");

                return HookResult.Continue;
            }
            catch (NullReferenceException)
            {
                return HookResult.Continue;
            }
        }

        private bool ValidateTriggerMultiple(CEntityInstance activator, CEntityInstance caller)
        {
            if (activator == null || caller == null)
                    return false;

            if (activator.DesignerName != "player")
                return false;

            var player = new CCSPlayerController(new CCSPlayerPawn(activator.Handle).Controller.Value!.Handle);

            if (player == null)
                return false;

            if (!player.IsValid() || caller.Entity!.Name == null) 
                return false;

            var callerHandle = caller.Handle;

            if(!_triggerHandles.Contains(callerHandle))
                return false;

            // TODO : BOUGER LE VALID TRIGGER NAME DANS UNE FONCTION, la on check si ça commence par cs2zones_
            string triggerName = caller.Entity.Name;
            if(!triggerName.StartsWith("cs2zones_"))
                return false;

            return true;
        }*/
    }
}
