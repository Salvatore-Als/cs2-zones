using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Zones;
using RayTrace;

public static class Player
{
    static public bool IsValid(this CCSPlayerController? player)
    {
        if(player == null)
            return false;

        return player != null && player.IsValid && player.PlayerPawn.IsValid && player.PlayerPawn.Value?.IsValid == true; 
    }

    static public bool IsConnected(this CCSPlayerController? player)
    {
        if(player == null)
            return false;

        return player.IsValid() && player.Connected == PlayerConnectedState.PlayerConnected;
    }

    static public bool IsT(this CCSPlayerController? player)
    {
        if(player == null)
            return false;

        return player.IsValid() && player.TeamNum == (int)CsTeam.Terrorist;
    }

    static public bool IsCt(this CCSPlayerController? player)
    {
        if(player == null)
            return false;

        return player.IsValid() && player.TeamNum == (int)CsTeam.CounterTerrorist;
    }

    static public bool IsAlive(this CCSPlayerController? player)
    {
        if(player == null)
            return false;

        return player.IsConnected() && player.PawnIsAlive && player.PlayerPawn.Value?.LifeState == (byte)LifeState_t.LIFE_ALIVE;
    }

    static public CCSPlayerPawn? Pawn(this CCSPlayerController? player)
    {
        if(player == null || !player.IsAlive())
            return null;

        CCSPlayerPawn? pawn = player.PlayerPawn.Value;

        return pawn;
    }

    private static Vector GetEyePosition(CCSPlayerController player)
    {
        CCSPlayerPawn? playerPawn = player.PlayerPawn.Value;
        if (playerPawn is null) 
            return Vector.Zero;

        Vector origin = playerPawn.AbsOrigin!;
        float viewOffset = playerPawn.ViewOffset.Z;
        return new Vector(origin.X, origin.Y, origin.Z + viewOffset);
    }

    public static Vector? TraceEyesPosition(this CCSPlayerController player)
    {
        if (player == null || !player.IsValid() || player.Pawn() == null)
            return new Vector(0, 0, 0);

        CCSPlayerPawn pawn = player.PlayerPawn.Value;

        QAngle eyeAngles = new QAngle(pawn.EyeAngles.X, pawn.EyeAngles.Y, pawn.EyeAngles.Z);
        Vector eyePosition =  Player.GetEyePosition(player);

        Vector forward = new Vector();
        NativeAPI.AngleVectors(eyeAngles.Handle, forward.Handle, 0, 0);

        TraceOptions traceOptions = new TraceOptions
        {
            InteractsExclude = 0
        };

        if (RayTrace.CRayTrace.TraceShape( eyePosition, eyeAngles, pawn, traceOptions, out TraceResult result))
        {
            return new Vector(
                result.EndPosX,
                result.EndPosY,
                result.EndPosZ
            );
        }

        return null;
    }

    public static bool IsValidEditor(this CCSPlayerController? player)
    {
        if(player == null || !player.IsValid() || !player.IsAlive()) 
            return false;

        if(!CS2Zones.PlayerZoneManager.PlayerZoneManagers.ContainsKey(player))
            return false;

        CS2Zones.PlayerZoneManager playerZoneManager = CS2Zones.PlayerZoneManager.PlayerZoneManagers[player];
        return playerZoneManager.IsEditingZone();
    }
}