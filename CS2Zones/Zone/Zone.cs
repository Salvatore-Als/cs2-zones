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
                if(player == null)
                    continue;

                Vector? absOrigin = player.PlayerPawn.Value?.AbsOrigin;
                if (absOrigin == null)
                    continue;

                bool inZone = VectorIsInZone(absOrigin);
                bool wasInZone = PlayersInZone.Contains(player);

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
            float minX = Math.Min(StartCorner.X, EndCorner.X);
            float maxX = Math.Max(StartCorner.X, EndCorner.X);
            float minY = Math.Min(StartCorner.Y, EndCorner.Y);
            float maxY = Math.Max(StartCorner.Y, EndCorner.Y);
            float minZ = Math.Min(StartCorner.Z, EndCorner.Z);
            float maxZ = Math.Max(StartCorner.Z, EndCorner.Z);

            return new Vector(
                minX + (maxX - minX) / 2.0f,
                minY + (maxY - minY) / 2.0f,
                minZ + (maxZ - minZ) / 2.0f
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
            ConfigManager.Save();
        }

        public void Delete()
        {
            Drawn = false; 
            ZoneManager.RemoveZone(this);
            ConfigManager.Save();
        }

        public ZoneSaveStatus GetSaveStatus()
        {
            if(!_startCornerFreezed || !_endCornerFreezed)
                return ZoneSaveStatus.CornersNotFreezed;

            if(string.IsNullOrWhiteSpace(Name))
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
            string header = $"Editing zone {Name}";
            string statusText = "";

            ZoneSaveStatus saveStatus = GetSaveStatus();

            if (originalSnapshot != null) // Editing existing zone
            {
                bool isModified = IsModifiedFromSnapshot(originalSnapshot);
                
                if (isModified)
                    statusText = GetSaveStatusMessage(saveStatus);
                else
                    statusText = "No modification has been made";
            }
            // New zones
            else
                statusText = GetSaveStatusMessage(saveStatus);

            return $"{header}\n- {statusText}";
        }

        private string GetSaveStatusMessage(ZoneSaveStatus status)
        {
            switch (status)
            {
                case ZoneSaveStatus.PossibleToSave:
                    return "⚠️ Don't forget to save !";
                case ZoneSaveStatus.NameNotValid:
                    return "⚠️ Invalid name !";
                case ZoneSaveStatus.NameAlreadyExists:
                    return "⚠️ Name already exists !";
                case ZoneSaveStatus.CornersNotFreezed:
                    return "⚠️ Corners not frozen !";
                default:
                    return "⚠️ Unknown !";
            }
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
        public Vector StartCorner { get; set; } = new Vector(0, 0, 0);
        public Vector EndCorner { get; set; } = new Vector(0, 0, 0);
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