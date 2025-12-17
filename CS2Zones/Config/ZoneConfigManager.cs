using System.Text.Json;
using CounterStrikeSharp.API;

namespace CS2Zones
{
    public class ZoneConfigManager
    {
        private static string ConfigDirectory => Path.Combine(Server.GameDirectory, "csgo", "addons", "counterstrikesharp", "configs", "plugins", "CS2Zones");
        private static string GetConfigFilePath(string mapName)
        {
            string fileName = $"zones_{mapName}.json";
            return Path.Combine(ConfigDirectory, fileName);
        }

        public static List<Zone> LoadZonesForMap(string mapName)
        {
            string configPath = GetConfigFilePath(mapName);
            
            if (!File.Exists(configPath))
            {
                Console.WriteLine($"[CS2Zones] No configuration file found for map: {mapName}");
                return new List<Zone>();
            }

            try
            {
                string jsonContent = File.ReadAllText(configPath);
                MapZoneConfig? mapConfig = JsonSerializer.Deserialize<MapZoneConfig>(jsonContent);

                if (mapConfig == null || mapConfig.Zones == null)
                {
                    Console.WriteLine($"[CS2Zones] Error: Invalid configuration for map: {mapName}");
                    return new List<Zone>();
                }

                List<Zone> zones = new List<Zone>();
                foreach (var zoneConfig in mapConfig.Zones)
                {
                    try
                    {
                        Zone zone = zoneConfig.ToZone();
                        zones.Add(zone);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[CS2Zones] Error loading zone: {ex.Message}");
                    }
                }

                Console.WriteLine($"[CS2Zones] {zones.Count} zones loaded for map: {mapName}");
                return zones;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CS2Zones] Error loading configuration: {ex.Message}");
                return new List<Zone>();
            }
        }

        public static void SaveZonesForMap(string mapName)
        {
            if (!Directory.Exists(ConfigDirectory))
                Directory.CreateDirectory(ConfigDirectory);

            string configPath = GetConfigFilePath(mapName);
            MapZoneConfig mapConfig = new MapZoneConfig
            {
                MapName = mapName,
                Zones = ZoneManager.Zones.Select(zone => ZoneConfig.FromZone(zone)).ToList()
            };

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonContent = JsonSerializer.Serialize(mapConfig, options);
                File.WriteAllText(configPath, jsonContent);
                Console.WriteLine($"[CS2Zones] {mapConfig.Zones.Count} zones saved for map: {mapName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CS2Zones] Error saving configuration: {ex.Message}");
            }
        }

        public static void DeleteZoneFromMap(Guid zoneId, string mapName)
        {
            string configPath = GetConfigFilePath(mapName);
            
            if (!File.Exists(configPath))
            {
                Console.WriteLine($"[CS2Zones] No configuration file found for map: {mapName}");
                return;
            }

            try
            {
                string jsonContent = File.ReadAllText(configPath);
                MapZoneConfig? mapConfig = JsonSerializer.Deserialize<MapZoneConfig>(jsonContent);

                if (mapConfig == null || mapConfig.Zones == null)
                {
                    Console.WriteLine($"[CS2Zones] Error: Invalid configuration for map: {mapName}");
                    return;
                }

                mapConfig.Zones.RemoveAll(z => z.Id == zoneId.ToString());

                var options = new JsonSerializerOptions { WriteIndented = true };
                string updatedJson = JsonSerializer.Serialize(mapConfig, options);
                File.WriteAllText(configPath, updatedJson);
                Console.WriteLine($"[CS2Zones] Zone {zoneId} deleted from configuration for map: {mapName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CS2Zones] Error deleting zone: {ex.Message}");
            }
        }
    }
}

