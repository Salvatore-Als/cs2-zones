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

                // xxx

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

                // xxx

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

            // xxx valide trigger name with prefix cs2zones_

            return true;
        }*/
    }
}
