using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;

namespace CS2Zones
{
    public class PlayerZoneManager
    {
        static public Dictionary<CCSPlayerController, PlayerZoneManager> PlayerZoneManagers { get; set; } = new Dictionary<CCSPlayerController, PlayerZoneManager>();
    
        public Zone? EditingZone { get; private set; } = null;
        private CCSPlayerController? _player = null;
        private ZoneSnapshot? _zoneSnapshot = null; 

        public PlayerZoneManager(CCSPlayerController player)
        {
            _player = player;
            PlayerZoneManagers[player] = this;
        }

        public void OnPlayerTick()
        {
            if(EditingZone == null || _player == null)
                return;

            Vector? eyeVector = _player.TraceEyesPosition();
            if(eyeVector == null) 
                return;

            if (eyeVector.X == 0 && eyeVector.Y == 0 && eyeVector.Z == 0) 
                return;
        
            EditingZone.SetupCorners(eyeVector);
            _player.PrintToCenter(EditingZone.GetInformation(_zoneSnapshot));
        }

        public void StartNewZone()
        {
            AbortEditing();

            EditingZone = new Zone();
            EditingZone.Drawn = true;
            EditingZone.Name = EditingZone.Id.ToString();
            EditingZone.Color = Color.Red;
            ZoneManager.AddZone(EditingZone);
            _zoneSnapshot = null; 
        }

        public bool EditZone(Zone zone)
        {
            if(zone == null)
                return false;

            if(EditingZone != null)
                return false;

            EditingZone = zone;
            _zoneSnapshot = zone.CreateSnapshot();
            EditingZone.Drawn = true;
            return true;
        }

        public void SaveZone()
        {
            if(EditingZone == null)
                return;

            ZoneSaveStatus saveStatus = EditingZone.GetSaveStatus();
            if(saveStatus != ZoneSaveStatus.PossibleToSave) {
                switch(saveStatus) {
                    case ZoneSaveStatus.NameNotValid:
                        throw new Exception("The zone name is not valid");
                    case ZoneSaveStatus.NameAlreadyExists:
                        throw new Exception("The zone name already exists");
                    case ZoneSaveStatus.CornersNotFreezed:
                        throw new Exception("The corners of the zone are not frozen");
                    default:
                        throw new Exception("The zone is not savable");
                }
            }
        
            EditingZone.Save();
            EditingZone.Drawn = false;
            
            EditingZone = null;
            _zoneSnapshot = null; 
        }

        public void AbortEditing()
        {
            if(EditingZone == null)
                return;

            if(IsEditingSavedZone() && _zoneSnapshot != null)
            {
                EditingZone.RestoreFromSnapshot(_zoneSnapshot);
                EditingZone.Drawn = false; 
                _zoneSnapshot = null;
            }
            else
                ZoneManager.RemoveZone(EditingZone);

            EditingZone = null;
            _zoneSnapshot = null;
        }

        public bool IsEditingZone()
        {
            return EditingZone != null;
        }

        public bool IsEditingSavedZone()
        {
            if(EditingZone == null)
                return false;

            return ZoneManager.Zones.Contains(EditingZone);
        }
    }
}