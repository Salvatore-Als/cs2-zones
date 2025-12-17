using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;

[MinimumApiVersion(244)]
public class CS2ZonesExamplePlugin : BasePlugin
{
    public override string ModuleName => "CS2Zones Example";
    public override string ModuleVersion => "v1.0.0";
    public override string ModuleAuthor => "Rebel's Corp Team";

    private ICS2ZonesAPI? _zonesApi;
    private readonly PluginCapability<ICS2ZonesAPI> _zonesApiCapability = new("cs2zones:api");

    public override void Load(bool hotReload)
    {
    }

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        _zonesApi = _zonesApiCapability.Get();

        if (_zonesApi == null)
        {
            Console.WriteLine($"{this.ModuleName} - Erreur: Impossible de récupérer l'API CS2Zones. Assurez-vous que le plugin CS2Zones est chargé.");
            Console.WriteLine($"{this.ModuleName} - Astuce: Vérifiez que CS2Zones.dll est chargé avant CS2ZonesExample.dll");
            return;
        }

        _zonesApi.OnPlayerEnterZone += OnPlayerEnterZone;
        _zonesApi.OnPlayerLeaveZone += OnPlayerLeaveZone;

        Console.WriteLine($"{this.ModuleName} - Plugin d'exemple chargé avec succès");
    }

    public override void Unload(bool hotReload)
    {
        if (_zonesApi != null)
        {
            _zonesApi.OnPlayerEnterZone -= OnPlayerEnterZone;
            _zonesApi.OnPlayerLeaveZone -= OnPlayerLeaveZone;
        }
    }

    private void OnPlayerEnterZone(CCSPlayerController player, string zoneName)
    {
        if (!IsValidPlayer(player) || !IsAlive(player))
            return;

        player.PrintToChat($" {ChatColors.White}You entered zone  {ChatColors.Red}{zoneName}");
    }

    private void OnPlayerLeaveZone(CCSPlayerController player, string zoneName)
    {
        if (!IsValidPlayer(player) || !IsAlive(player))
            return;

        player.PrintToChat($" {ChatColors.White}You left zone  {ChatColors.Red}{zoneName}");
    }

    private bool IsValidPlayer(CCSPlayerController player)
    {
        return player != null && player.IsValid && player.PlayerPawn.IsValid && player.PlayerPawn.Value?.IsValid == true; 
    }

    private bool IsAlive(CCSPlayerController player)
    {
        return player.PawnIsAlive && player.PlayerPawn.Value?.LifeState == (byte)LifeState_t.LIFE_ALIVE;
    }
}

