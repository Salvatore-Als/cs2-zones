using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using System.Drawing;
using CSTimer = CounterStrikeSharp.API.Modules.Timers;

namespace CS2Zones
{
    public class Zone
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Untitled Zone";
        public Color Color { get; set; } = Color.Red;

        public Vector StartCorner { get; set; } = new Vector(0, 0, 0);
        public Vector EndCorner { get; set; } = new Vector(0, 0, 0);

        private bool _startCornerFreezed = false;
        private bool _endCornerFreezed = false;
    
        public bool Freezed { get; set; } = false;
        public bool Drawn { get; set; } = false;

        public List<CCSPlayerController> PlayersInZone { get; set; } = new List<CCSPlayerController>();

        // TODO : _triggerTeam 

        // Make crash because of m_vacMaxs and m_vacMins, using another way to detect the zone
        // i'll back to this later as optimization
        /*public CTriggerMultiple? CreateTrigger(Vector corner1, Vector corner2, string name)
        {
            CTriggerMultiple trigger = Utilities.CreateEntityByName<CTriggerMultiple>("trigger_multiple")!;

            if (trigger == null)
                throw new Exception($"Unable to create trigger ");

            nint handle = trigger.Handle;
            if(handle == 0)
                throw new Exception($"Unable to get handle of trigger ");

            trigger.Entity!.Name = "cs2_zone_" + name;
            trigger.Spawnflags = 1;
            trigger.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags &= ~(uint)(1 << 2);
            trigger.Collision.SolidType = SolidType_t.SOLID_VPHYSICS;
            trigger.Collision.SolidFlags = 0;
            trigger.Collision.CollisionGroup = 14;

            Schema.SetSchemaValue(handle, "CCollisionProperty", "m_vecMaxs", corner2);
            Schema.SetSchemaValue(handle, "CCollisionProperty", "m_vecMins", corner1);

            trigger.DispatchSpawn();

            Vector middle = GetMiddle(corner1, corner2);

            trigger.Teleport(middle, new QAngle(0, 0, 0), new Vector(0, 0, 0));
            trigger.AcceptInput("Enable");

            return trigger;
        }*/

        public void OnTick()
        {
            if (Drawn)
                Draw();

            var players = Utilities.GetPlayers()
                .Where(player => player.IsValid() && player.IsAlive())
                .ToList();

            // Boucle unique pour détecter les entrées/sorties de la zone
            foreach (var player in players)
            {
                Vector absOrigin = player.PlayerPawn.Value?.AbsOrigin;
                if (absOrigin == null)
                    continue;

                bool inZone = VectorIsInZone(absOrigin);
                bool wasInZone = PlayersInZone.Contains(player);

                player.PrintToCenter($"in zone : {inZone} was in zone : {wasInZone}");

                if (inZone && !wasInZone)
                {
                    PlayersInZone.Add(player);
                    CS2Zones.apiInstance?.TriggerPlayerEnterZone(player, Name);
                }
                else if (!inZone && wasInZone)
                {
                    PlayersInZone.Remove(player);
                    CS2Zones.apiInstance?.TriggerPlayerLeaveZone(player, Name);
                }
            }
        }

        public Vector GetMiddle()
        {
            return new Vector(
                (StartCorner.X + EndCorner.X) / 2.0f,
                (StartCorner.Y + EndCorner.Y) / 2.0f,
                (StartCorner.Z + EndCorner.Z) / 2.0f
            );
        }
        
        public void Draw()
        {
            if(!Drawn)
                return;
    
            if(!_startCornerFreezed)
                Drawer.DrawPoint(StartCorner, Color);
            else
                Drawer.DrawWireframe3D(StartCorner, EndCorner, Color);
        }

        public void SetupCorners(Vector origin)
        {
            if(_startCornerFreezed && _endCornerFreezed)
                return;

            if(!_startCornerFreezed)
                StartCorner = origin;
            else if (!_endCornerFreezed)
                EndCorner = origin;
        }

        public void Freeze()
        {
            if(!_startCornerFreezed) {
                _startCornerFreezed = true;
                return;
            }

            _endCornerFreezed = true;
        }

        public void ResetCorners()
        {
            _startCornerFreezed = false;
            _endCornerFreezed = false;
            EndCorner = new Vector(0, 0, 0);
        }

        public void Save()
        {
            // La sauvegarde est gérée par ZoneConfigManager dans CS2Zones.cs
        }

        public void Delete()
        {
            // Supprimer de la config
            string mapName = Server.MapName;
            ZoneConfigManager.DeleteZoneFromMap(Id, mapName);
            
            // Supprimer du ZoneManager
            ZoneManager.RemoveZone(this);
        }

        public ZoneSaveStatus GetSaveStatus()
        {
            if(!_startCornerFreezed || !_endCornerFreezed)
                return ZoneSaveStatus.CornersNotFreezed;

            if(string.IsNullOrWhiteSpace(Name) || Name == "Untitled Zone")
                return ZoneSaveStatus.NameNotValid;

            Zone? existingZone = ZoneManager.GetZoneByName(Name);
            if(existingZone != null && existingZone != this)
                return ZoneSaveStatus.NameAlreadyExists;

            return ZoneSaveStatus.PossibleToSave;
        }

        public bool IsStartCornerFreezed()
        {
            return _startCornerFreezed;
        }

        public bool IsEndCornerFreezed()
        {
            return _endCornerFreezed;
        }

        public string GetInformation(ZoneSnapshot? originalSnapshot = null)
        {
            ZoneSaveStatus saveStatus = GetSaveStatus();
            string saveStatusText = "";
            switch(saveStatus) {
                case ZoneSaveStatus.PossibleToSave:
                    saveStatusText = "i18n Possible de sauvegarder !";
                    break;
                case ZoneSaveStatus.NameNotValid:
                    saveStatusText = "i18n Nom non valide !";
                    break;
                case ZoneSaveStatus.NameAlreadyExists:
                    saveStatusText = "i18n Nom déjà existant !";
                    break;
                case ZoneSaveStatus.CornersNotFreezed:
                    saveStatusText = "i18n Coins non fixés !";
                    break;
                default:
                    saveStatusText = "i18n Inconnu !";
                    break;
            }

            string modifiedText = "";
            if(originalSnapshot != null && IsModifiedFromSnapshot(originalSnapshot))
            {
                modifiedText = "\n - ⚠️ Zone modifiée ! N'oubliez pas de sauvegarder !";
            }

            if(originalSnapshot == null)
            {
                modifiedText = "\n - ⚠️ Nouvelle zone ! N'oubliez pas de sauvegarder !";
            }

            string information = $"i18n Zone: {Name} \n - Statut: {saveStatusText}{modifiedText}";
            return information;
        }

        public ZoneSnapshot CreateSnapshot()
        {
            return new ZoneSnapshot
            {
                Name = Name,
                Color = Color,
                StartCorner = StartCorner,
                EndCorner = EndCorner,
                StartCornerFreezed = _startCornerFreezed,
                EndCornerFreezed = _endCornerFreezed
            };
        }

        public void RestoreFromSnapshot(ZoneSnapshot snapshot)
        {
            Name = snapshot.Name;
            Color = snapshot.Color;
            StartCorner = snapshot.StartCorner;
            EndCorner = snapshot.EndCorner;
            _startCornerFreezed = snapshot.StartCornerFreezed;
            _endCornerFreezed = snapshot.EndCornerFreezed;
        }

        public bool IsModifiedFromSnapshot(ZoneSnapshot snapshot)
        {
            if (snapshot == null)
                return false;

            if (Name != snapshot.Name)
                return true;

            if (Color != snapshot.Color)
                return true;

            float tolerance = 0.1f;
            if (Math.Abs(StartCorner.X - snapshot.StartCorner.X) > tolerance ||
                Math.Abs(StartCorner.Y - snapshot.StartCorner.Y) > tolerance ||
                Math.Abs(StartCorner.Z - snapshot.StartCorner.Z) > tolerance)
                return true;

            if (Math.Abs(EndCorner.X - snapshot.EndCorner.X) > tolerance ||
                Math.Abs(EndCorner.Y - snapshot.EndCorner.Y) > tolerance ||
                Math.Abs(EndCorner.Z - snapshot.EndCorner.Z) > tolerance)
                return true;

            if (_startCornerFreezed != snapshot.StartCornerFreezed ||
                _endCornerFreezed != snapshot.EndCornerFreezed)
                return true;

            return false;
        }

        public bool VectorIsInZone(Vector pos)
        {
            if (!_startCornerFreezed || !_endCornerFreezed)
                return false;

            float minX = Math.Min(StartCorner.X, EndCorner.X);
            float maxX = Math.Max(StartCorner.X, EndCorner.X);
            float minY = Math.Min(StartCorner.Y, EndCorner.Y);
            float maxY = Math.Max(StartCorner.Y, EndCorner.Y);
            float minZ = Math.Min(StartCorner.Z, EndCorner.Z);
            float maxZ = Math.Max(StartCorner.Z, EndCorner.Z);

            float z = pos.Z + 36; // for player center

            return pos.X >= minX && pos.X <= maxX &&
                   pos.Y >= minY && pos.Y <= maxY &&
                   z >= minZ && z <= maxZ;
        }


        public void SetStartCorner(Vector corner)
        {
            StartCorner = corner;
            _startCornerFreezed = true;
        }

        public void SetEndCorner(Vector corner)
        {
            EndCorner = corner;
            _endCornerFreezed = true;
        }
    }

    public class ZoneSnapshot
    {
        public string Name { get; set; } = "";
        public Color Color { get; set; }
        public Vector StartCorner { get; set; }
        public Vector EndCorner { get; set; }
        public bool StartCornerFreezed { get; set; }
        public bool EndCornerFreezed { get; set; }
    }

    public enum ZoneSaveStatus
    {
        PossibleToSave,
        NameNotValid,
        NameAlreadyExists,
        CornersNotFreezed,
    }
}