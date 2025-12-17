using System.Text.Json;
using CounterStrikeSharp.API;

namespace CS2Zones
{
    public class ConfigManager
    {
        private static string ConfigDirectory => Path.Combine(Server.GameDirectory, "csgo", "addons", "counterstrikesharp", "configs", "plugins", "CS2Zones");
        private static MapConfig? _currentConfig;
        private static string? _currentMapName;

        private static string GetConfigFilePath(string mapName)
        {
            string fileName = $"zones_{mapName}.json";
            return Path.Combine(ConfigDirectory, fileName);
        }

        public static void LoadConfig(string mapName)
        {
            _currentMapName = mapName;
            string configPath = GetConfigFilePath(mapName);
            
            if (!File.Exists(configPath))
            {
                Console.WriteLine($"[CS2Zones] No configuration file found for map: {mapName}");
                _currentConfig = new MapConfig
                {
                    MapName = mapName,
                    Zones = new List<Config>()
                };
                return;
            }

            try
            {
                string jsonContent = File.ReadAllText(configPath);
                _currentConfig = JsonSerializer.Deserialize<MapConfig>(jsonContent);

                if (_currentConfig == null)
                {
                    Console.WriteLine($"[CS2Zones] Error: Invalid configuration for map: {mapName}");
                    _currentConfig = new MapConfig
                    {
                        MapName = mapName,
                        Zones = new List<Config>()
                    };
                    return;
                }

                if (_currentConfig.Zones == null)
                {
                    _currentConfig.Zones = new List<Config>();
                }

                Console.WriteLine($"[CS2Zones] Configuration loaded for map: {mapName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CS2Zones] Error loading configuration: {ex.Message}");
                _currentConfig = new MapConfig
                {
                    MapName = mapName,
                    Zones = new List<Config>()
                };
            }
        }

        public static List<Zone> LoadZones()
        {
            if (_currentConfig == null || _currentConfig.Zones == null)
            {
                return new List<Zone>();
            }

            List<Zone> zones = new List<Zone>();
            foreach (var config in _currentConfig.Zones)
            {
                try
                {
                    Zone zone = config.ToZone();
                    zones.Add(zone);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CS2Zones] Error loading zone: {ex.Message}");
                }
            }

            Console.WriteLine($"[CS2Zones] {zones.Count} zones loaded");
            return zones;
        }

        public static void Save()
        {
            if (_currentConfig == null || _currentMapName == null)
            {
                Console.WriteLine($"[CS2Zones] Error: No configuration loaded");
                return;
            }

            if (!Directory.Exists(ConfigDirectory))
                Directory.CreateDirectory(ConfigDirectory);

            _currentConfig.MapName = _currentMapName;
            _currentConfig.Zones = ZoneManager.Zones.Select(zone => Config.FromZone(zone)).ToList();

            try
            {
                string configPath = GetConfigFilePath(_currentMapName);
                JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
                string jsonContent = JsonSerializer.Serialize(_currentConfig, options);
                File.WriteAllText(configPath, jsonContent);
                Console.WriteLine($"[CS2Zones] {_currentConfig.Zones.Count} zones saved for map: {_currentMapName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CS2Zones] Error saving configuration: {ex.Message}");
            }
        }

        public static void DeleteZone(Guid zoneId)
        {
            if (_currentConfig == null || _currentConfig.Zones == null)
            {
                Console.WriteLine($"[CS2Zones] Error: No configuration loaded");
                return;
            }

            _currentConfig.Zones.RemoveAll(z => z.Id == zoneId.ToString());
            Save();
            Console.WriteLine($"[CS2Zones] Zone {zoneId} deleted from configuration");
        }
    }
}

