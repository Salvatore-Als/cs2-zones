using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

/// <summary>
/// API to manage zones in Counter-Strike 2
/// </summary>
public interface ICS2ZonesAPI
{
    /// <summary>
    /// Checks if a player is in a specific zone
    /// </summary>
    /// <param name="player">Le joueur à vérifier</param>
    /// <param name="zoneName">Le nom de la zone</param>
    /// <returns>True si le joueur est dans la zone, False sinon</returns>
    bool IsPlayerInZone(CCSPlayerController? player, string zoneName);

    /// <summary>
    /// Event triggered when a player enters a zone
    /// </summary>
    event Action<CCSPlayerController, string>? OnPlayerEnterZone;

    /// <summary>
    /// Event triggered when a player leaves a zone
    /// </summary>
    event Action<CCSPlayerController, string>? OnPlayerLeaveZone;

    /// <summary>
    /// Teleport player in the middle of a zone
    /// </summary>
    /// <param name="player">The player to teleport</param>
    /// <param name="zoneName">The name of the zone</param>
    void TeleportPlayerInZone(CCSPlayerController player, string zoneName);

    /// <summary>
    /// Get the middle position of a zone
    /// </summary>
    /// <param name="zoneName">The name of the zone</param>
    /// <returns>The middle position of the zone</returns>
    Vector GetZoneMiddlePosition(string zoneName);

    /// <summary>
    /// Check if the zone exists
    /// </summary>
    /// <param name="zoneName">The name of the zone</param>
    /// <returns>True if the zone exists, False otherwise</returns>
    bool IsZoneExists(string zoneName);
}