using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Capabilities;
using MenuManager;

namespace CS2Zones
{
    [MinimumApiVersion(244)]
    public partial class CS2Zones : BasePlugin
    {
        public override string ModuleName => "CS2Zones";
        public override string ModuleVersion => "v1.0.0";
        public override string ModuleAuthor => "Kriax";

        public static PluginCapability<ICS2ZonesAPI> zonesApiCapability { get; } = new("cs2zones:api");
        public static CS2Zones? globalCtx;
        public static string MapName { get; set; } = "";

        private IMenuApi? _menuApi;
        private readonly PluginCapability<IMenuApi?> _menuApiCapability = new("menu:nfcore");

        public static CS2ZonesAPI? apiInstance = new CS2ZonesAPI();
        private List<nint> _triggerHandles = new List<nint>();

        public override void Load(bool hotReload)
        {
            globalCtx = this;
            
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
            
            globalCtx = null;
            _menuApi = null;
        }

        public override void OnAllPluginsLoaded(bool hotReload)
        {
            _menuApi = _menuApiCapability.Get();
            if(_menuApi == null)
            {
                Console.WriteLine($"{this.ModuleName} - Error: Unable to retrieve MenuManager API");
                return;
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
        }

        private void RegisterListeners()
        {
            RegisterListener<Listeners.OnTick>(OnTick);
            RegisterListener<Listeners.OnMapStart>(OnMapStart);
        }

        private void OnMapStart(string mapName)
        {
            MapName = mapName;
            LoadZonesForCurrentMap();
        }

        private void LoadZonesForCurrentMap()
        {
            ZoneManager.ClearZones();

            List<Zone> zones = ZoneConfigManager.LoadZonesForMap(MapName);
            
            foreach (var zone in zones)
                ZoneManager.AddZone(zone);

            Console.WriteLine($"[CS2Zones] {zones.Count} zones loaded for map: {MapName}");
        }

        public void RegisterCommands()
        {
            AddCommand("css_zones_add", "Créer une nouvelle zone", OnNewZonesCommand);
            AddCommand("css_zones_save", "Sauvegarder la zone en cours d'édition", OnSaveCommand);
            AddCommand("css_zones_name", "Renommer la zone", OnSetNameCommand);
            AddCommand("css_zones_edit", "Mettre une zone en mode édition", OnEditCommand);
            AddCommand("css_zones_abort", "Annuler l'édition en cours", OnZoneAbortCommand);

            AddCommand("css_zadd", "Créer une nouvelle zone", OnNewZonesCommand);
            AddCommand("css_zsave", "Sauvegarder la zone en cours d'édition", OnSaveCommand);
            AddCommand("css_zname", "Renommer la zone", OnSetNameCommand);
            AddCommand("css_zedit", "Mettre une zone en mode édition", OnEditCommand);
            AddCommand("css_zabort", "Annuler l'édition en cours", OnZoneAbortCommand);
        }
    }
}
