using CounterStrikeSharp.API.Core;

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
}