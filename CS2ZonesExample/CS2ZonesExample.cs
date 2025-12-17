using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Commands;
using System.Drawing;

[MinimumApiVersion(244)]
public class CS2ZonesExamplePlugin : BasePlugin
{
    public override string ModuleName => "CS2Zones Example";
    public override string ModuleVersion => "v1.0.0";
    public override string ModuleAuthor => "Kriax";

    private ICS2ZonesAPI? _zonesApi;
    private readonly PluginCapability<ICS2ZonesAPI> _zonesApiCapability = new("cs2zones:api");

    public override void Load(bool hotReload)
    {
        AddCommand("css_zonetp", "Teleport to a zone", OnZoneCommand);
        AddCommand("css_zonecheck", "Check if a player is in a zone", OnZoneCheckCommand);
    }

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        _zonesApi = _zonesApiCapability.Get();

        if (_zonesApi == null)
            return;

        _zonesApi.OnPlayerEnterZone += OnPlayerEnterZone;
        _zonesApi.OnPlayerLeaveZone += OnPlayerLeaveZone;

        Console.WriteLine($"{this.ModuleName} - Loaded");
    }

    public override void Unload(bool hotReload)
    {
        if (_zonesApi != null)
        {
            _zonesApi.OnPlayerEnterZone -= OnPlayerEnterZone;
            _zonesApi.OnPlayerLeaveZone -= OnPlayerLeaveZone;
        }
    }

    [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_ONLY)]
    private void OnZoneCommand(CCSPlayerController player, CommandInfo command)
    {
        if (!IsValidPlayer(player) || !IsAlive(player))
            return;

        _zonesApi?.TeleportPlayerInZone(player, "tp");
    }

    [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_ONLY)]
    private void OnZoneCheckCommand(CCSPlayerController player, CommandInfo command)
    {
        if (!IsValidPlayer(player) || !IsAlive(player))
            return;

        bool isInZone = _zonesApi?.IsPlayerInZone(player, "tp") ?? false;
        if(isInZone) {
            player.PrintToChat($" {ChatColors.White}You are in zone {ChatColors.Red}tp");
        } else {
            player.PrintToChat($" {ChatColors.White}You are not in zone {ChatColors.Red}tp");
        }
    }

    private void OnPlayerEnterZone(CCSPlayerController player, string zoneName)
    {
        if (!IsValidPlayer(player) || !IsAlive(player))
            return;

        switch(zoneName) {
            case "colorblue":
                SetPlayerColor(player, Color.Blue);
                break;
            case "colorred":
                SetPlayerColor(player, Color.Red);
                break;
        }

        player.PrintToChat($" {ChatColors.White}You entered zone  {ChatColors.Red}{zoneName}");
    }

    private void OnPlayerLeaveZone(CCSPlayerController player, string zoneName)
    {
        if (!IsValidPlayer(player) || !IsAlive(player))
            return;

        if(zoneName.Contains("color")) {
            SetPlayerColor(player, Color.White);
        }

        player.PrintToChat($" {ChatColors.White}You left zone  {ChatColors.Red}{zoneName}");
    }

    private void SetPlayerColor(CCSPlayerController? player, Color colour)
    {
        if(player == null || !IsAlive(player))
            return;

        CCSPlayerPawn? pawn = player.PlayerPawn?.Value;

        if(pawn == null)
            return;

        pawn.RenderMode = RenderMode_t.kRenderTransColor;
        pawn.Render = colour;
        Utilities.SetStateChanged(pawn,"CBaseModelEntity","m_clrRender");
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

