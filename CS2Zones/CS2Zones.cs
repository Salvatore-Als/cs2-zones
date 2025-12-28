using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Utils;
using MenuManager;

namespace CS2Zones
{
    [MinimumApiVersion(244)]
    public partial class CS2Zones : BasePlugin
    {
        public static string PREFIX = $" {ChatColors.Green}[CS2Zones]: {ChatColors.White}";
    
        public override string ModuleName => "CS2Zones";
        public override string ModuleVersion => "v1.1.2";
        public override string ModuleAuthor => "Kriax";

        public static PluginCapability<ICS2ZonesAPI> zonesApiCapability { get; } = new("cs2zones:api");
        public static CS2Zones? globalCtx;

        private IMenuApi? _menuApi;
        private readonly PluginCapability<IMenuApi?> _menuApiCapability = new("menu:nfcore");

        public static CS2ZonesAPI? apiInstance = new CS2ZonesAPI();
        private List<nint> _triggerHandles = new List<nint>();
        
        public static Dictionary<CCSPlayerController, Vector> PlayerPositions { get; private set; } = new Dictionary<CCSPlayerController, Vector>();
        public static Dictionary<CCSPlayerController, Vector> PlayerTargetPositions { get; private set; } = new Dictionary<CCSPlayerController, Vector>();

        public override void Load(bool hotReload)
        {
            globalCtx = this;
        
            if(apiInstance == null)
                apiInstance = new CS2ZonesAPI();

            Capabilities.RegisterPluginCapability(zonesApiCapability, () => apiInstance);
        
            Console.WriteLine(@"
		     ____ ____ ____    _____                     
		    / ___/ ___|___ \  |__  /___  _ __   ___  ___ 
		    | |   \___ \ __) |   / // _ \| '_ \ / _ \/ __|
		    | |___ ___) / __/   / /| (_) | | | |  __/\__ \
		     \____|____/_____| /____\___/|_| |_|\___||___/ 

		  	    								  by Kriax
		    ");

            RegisterHooks();
            RegisterListeners();
            RegisterCommands();
        }

        public override void Unload(bool hotReload)
        {
            foreach (var playerZoneManager in PlayerZoneManager.PlayerZoneManagers.Values.ToList())
            {
                playerZoneManager.AbortEditing();
            }

            foreach (var zone in ZoneManager.Zones.ToList())
            {
                if (zone.Drawn)
                    zone.Drawn = false;
            }

            _triggerHandles.Clear();
            ZoneManager.ClearZones();
            PlayerZoneManager.PlayerZoneManagers.Clear();
            PlayerPositions.Clear();
            PlayerTargetPositions.Clear();  
            globalCtx = null;
            _menuApi = null;
        }

        public override void OnAllPluginsLoaded(bool hotReload)
        {
            _menuApi = _menuApiCapability.Get();
            if(_menuApi == null)
            {
                Console.WriteLine($"{this.ModuleName} - Error: Unable to retrieve MenuManager API");
            }

            if(hotReload)
            {
                AddTimer(0.1f, () => {
                    LoadZones();
                });

                // Add all players to the PlayerTargetPositions dictionary
                foreach(var player in Utilities.GetPlayers())
                {
                    if(player.IsValid() && player.IsAlive())
                        PlayerTargetPositions[player] = new Vector(0, 0, 0);

                    if(!PlayerTargetPositions.ContainsKey(player))
                        PlayerTargetPositions[player] = new Vector(0, 0, 0);
                }
            }
        }

        private void RegisterHooks()
        {
            /*HookEntityOutput("trigger_multiple", "OnStartTouch", (CEntityIOOutput output, string name, CEntityInstance activator, CEntityInstance caller, CVariant value, float delay) =>
            {
                TriggerMultipleOnStartTouch(activator, caller);
                return HookResult.Continue;
            });

            HookEntityOutput("trigger_multiple", "OnEndTouch", (CEntityIOOutput output, string name, CEntityInstance activator, CEntityInstance caller, CVariant value, float delay) =>
            {
                TriggerMultipleOnEndTouch(activator, caller);
                return HookResult.Continue;
            });*/
            
            RegisterEventHandler<EventWeaponFire>(OnWeaponFire, HookMode.Pre);
            RegisterEventHandler<EventPlayerPing>(OnPlayerPing, HookMode.Pre);
            RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull, HookMode.Pre);
            RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect, HookMode.Pre);
        }

        private void RegisterListeners()
        {
            RegisterListener<Listeners.OnTick>(OnTick);
            RegisterListener<Listeners.OnMapStart>(OnMapStart);
        }

        private void OnMapStart(string mapName)
        {
            LoadZones();
        }

        private void LoadZones()
        {
            string mapName = Server.MapName;
            if (string.IsNullOrEmpty(mapName))
            {
                Console.WriteLine("[CS2Zones] Warning: Server.MapName is not available yet, zones will be loaded on map start");
                return;
            }

            ConfigManager.LoadConfig(mapName);
        
            foreach (var zone in ZoneManager.Zones.ToList())
                zone.Drawn = false; // security

            ZoneManager.ClearZones(); // security

            List<Zone> zones = ConfigManager.LoadZones();
            
            foreach (var zone in zones)
            {
                zone.Drawn = false; // security
                ZoneManager.AddZone(zone);
            }

            Console.WriteLine($"[CS2Zones] Loaded {zones.Count} zones for map: {mapName}");
        }

        public void RegisterCommands()
        {
            AddCommand("css_zones", "Open the zones management menu", OnZonesMenuCommand);
            AddCommand("css_zsave", "Save the current zone", OnSaveCommand);
            AddCommand("css_zname", "Rename the zone", OnSetNameCommand);
            AddCommand("css_zabort", "Cancel the current edit", OnZoneAbortCommand);
        }
    }
}
