using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;

namespace CS2Zones
{
    public static class ZoneManager
    {
        public static List<Zone> Zones { get; private set; } = new List<Zone>();

        public static void AddZone(Zone zone)
        {
            if(zone == null)
                return;
                
            if(!Zones.Contains(zone))
                Zones.Add(zone);
        }

        public static bool RemoveZone(Zone zone)
        {
            if(zone == null)
                return false;
                
            return Zones.Remove(zone);
        }

        public static Zone? GetZoneByName(string name)
        {
            return Zones.FirstOrDefault(z => z.Name == name);
        }

        public static void ClearZones()
        {
            Zones.Clear();
        }
    }
}