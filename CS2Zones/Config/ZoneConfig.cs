using System.Drawing;
using CounterStrikeSharp.API.Modules.Utils;

namespace CS2Zones
{
    public class ZoneConfig
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public int ColorR { get; set; }
        public int ColorG { get; set; }
        public int ColorB { get; set; }
        public float StartCornerX { get; set; }
        public float StartCornerY { get; set; }
        public float StartCornerZ { get; set; }
        public float EndCornerX { get; set; }
        public float EndCornerY { get; set; }
        public float EndCornerZ { get; set; }

        public static ZoneConfig FromZone(Zone zone)
        {
            return new ZoneConfig
            {
                Id = zone.Id.ToString(),
                Name = zone.Name,
                ColorR = zone.Color.R,
                ColorG = zone.Color.G,
                ColorB = zone.Color.B,
                StartCornerX = zone.StartCorner.X,
                StartCornerY = zone.StartCorner.Y,
                StartCornerZ = zone.StartCorner.Z,
                EndCornerX = zone.EndCorner.X,
                EndCornerY = zone.EndCorner.Y,
                EndCornerZ = zone.EndCorner.Z
            };
        }

        public Zone ToZone()
        {
            Zone zone = new Zone();
            zone.Id = Guid.Parse(Id);
            zone.Name = Name;
            zone.Color = Color.FromArgb(ColorR, ColorG, ColorB);
            zone.SetStartCorner(new Vector(StartCornerX, StartCornerY, StartCornerZ));
            zone.SetEndCorner(new Vector(EndCornerX, EndCornerY, EndCornerZ));
            return zone;
        }
    }

    public class MapZoneConfig
    {
        public string MapName { get; set; } = "";
        public List<ZoneConfig> Zones { get; set; } = new List<ZoneConfig>();
    }
}

