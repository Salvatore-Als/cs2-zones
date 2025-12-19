# CS2Zones

A Counter-Strike 2 plugin for creating and managing zones on maps. This plugin allows you to define zones, detect when players enter/leave them, and provides an API for other plugins to interact with zones.

## 📹 Video Presentation

[![CS2Zones Presentation](https://img.youtube.com/vi/dHfA977DKQc/0.jpg)](https://youtu.be/dHfA977DKQc)

## Sponsor

[![VeryGames](https://www.verygames.com/fr/assets/images/verygames.webp)](https://verygames.com/en?af=cs2zones)

Thanks for the free Server 💋

## Features

- Create and manage zones on maps
- Visual zone editing with in-game commands
- Save zones per map in JSON configuration files
- API for other plugins to interact with zones
- Event system for player enter/leave zone detection
- Teleport players to zone centers

## Structure

The project is organized into three main components:

- **CS2Zones**: Main plugin that handles zone creation, management, and detection
- **CS2ZonesAPI**: Public API interface that other plugins can use to interact with zones
- **CS2ZonesExample**: Example plugin demonstrating how to use the CS2Zones API

## Installation

1. Build the solution using Visual Studio or `dotnet build`
2. Copy the compiled DLLs to your CounterStrikeSharp plugins directory
3. Install the required dependencies (see below)

## Dependencies

This plugin requires the following dependencies:

- **[MenuManagerCS2](https://github.com/NickFox007/MenuManagerCS2)** - For in-game menu system
- **[CS2TraceRay](https://github.com/schwarper/CS2TraceRay/tree/main/CS2TraceRay)** - For ray tracing functionality

**Note**: This plugin does not require a database. All zone data is stored in JSON configuration files.

## Usage

### How it works

#### Creating a Zone

1. **Open the menu**: Type `!zone` in chat and manage what you need 
2. **Set corners**: 
   - Equip your knife
   - Use left click (weapon fire) to place the first corner
   - Use left click again to place the second corner (you need 2 corners to create a zone)
   - **Reset corners**: Use ping to reset the zone corners
3. **Name the zone**: Type `!zname <name>` 
4. **Save the zone**: `!zsave` 
5. **Cancel editing**: Type `!zabort`

### Commands
Requires `@css/zones` permission

- `css_zone` - Open the zones management menu (requires `@css/zones` permission)
- `css_zname <name>` - Set the name of the zone you're editing
- `css_zsave` - Save the current zone
- `css_zabort` - Cancel the current edit and abandon zone creation

### API Usage

More informations and usages on the CSZonesAPI.cs : https://github.com/Salvatore-Als/cs2-zones/blob/main/CS2ZonesAPI/CS2ZonesAPI.cs

Here's a complete example of how to use the CS2Zones API in your own plugin:

```csharp
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

[MinimumApiVersion(244)]
public class MyPlugin : BasePlugin
{
    private ICS2ZonesAPI? _zonesApi;
    private readonly PluginCapability<ICS2ZonesAPI> _zonesApiCapability = new("cs2zones:api");

    public override void Load(bool hotReload)
    {
        // Register commands that use the zones API
        AddCommand("css_zonetp", "Teleport to zone", OnTeleportCommand);
        AddCommand("css_zonecheck", "Check if in zone", OnCheckCommand);
    }

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        // Get the API instance
        _zonesApi = _zonesApiCapability.Get();

        if (_zonesApi == null)
        {
            Console.WriteLine("CS2Zones API not available!");
            return;
        }

        // Subscribe to zone events
        _zonesApi.OnPlayerEnterZone += OnPlayerEnterZone;
        _zonesApi.OnPlayerLeaveZone += OnPlayerLeaveZone;
    }

    public override void Unload(bool hotReload)
    {
        // Unsubscribe from events
        if (_zonesApi != null)
        {
            _zonesApi.OnPlayerEnterZone -= OnPlayerEnterZone;
            _zonesApi.OnPlayerLeaveZone -= OnPlayerLeaveZone;
        }
    }

    // Example: Teleport player to a zone
    private void OnTeleportCommand(CCSPlayerController player, CommandInfo command)
    {
        if (!player.IsValid || !player.IsAlive())
            return;

        _zonesApi?.TeleportPlayerInZone(player, "tp");
        player.PrintToChat($" {ChatColors.Green}Teleported to zone!");
    }

    // Example: Check if player is in a zone
    private void OnCheckCommand(CCSPlayerController player, CommandInfo command)
    {
        if (!player.IsValid)
            return;

        bool isInZone = _zonesApi?.IsPlayerInZone(player, "tp") ?? false;
        if (isInZone)
        {
            player.PrintToChat($" {ChatColors.Green}You are in zone 'tp'");
        }
        else
        {
            player.PrintToChat($" {ChatColors.Red}You are not in zone 'tp'");
        }
    }

    // Example: React when player enters a zone
    private void OnPlayerEnterZone(CCSPlayerController player, string zoneName)
    {
        if (!player.IsValid || !player.IsAlive())
            return;

        // Do something when player enters a specific zone
        switch (zoneName)
        {
            case "spawn":
                player.PrintToChat($" {ChatColors.Green}Welcome to spawn zone!");
                break;
            case "safe":
                player.PrintToChat($" {ChatColors.Blue}You entered a safe zone!");
                break;
        }
    }

    // Example: React when player leaves a zone
    private void OnPlayerLeaveZone(CCSPlayerController player, string zoneName)
    {
        if (!player.IsValid)
            return;

        player.PrintToChat($" {ChatColors.Yellow}You left zone: {zoneName}");
    }
}
```

For a complete working example, see the `CS2ZonesExample` project in this repository.

## Configuration

Zones are saved per map in JSON files located at:
```
csgo/addons/counterstrikesharp/configs/plugins/CS2Zones/zones_{mapname}.json
```

**No database required** - All data is stored in simple JSON files.

## TODO

- [ ] Add menu options to increase/decrease zone length/width
- [ ] Move detection to trigger_multiple spawn with OnTouch events (existing code is commented out) to avoid position detection in ListenerTick
- [ ] Implement i18n (internationalization) support

## Contributing

Contributions are welcome! If you'd like to contribute, feel free to open a Pull Request with your improvements. :)
