using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using CS2TraceRay.Class;
using CS2TraceRay.Enum;
using CS2TraceRay.Struct;

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

    public static Vector? TraceEyesPosition(this CCSPlayerController player)
    {
        Vector? absOrigin = player.PlayerPawn!.Value!.AbsOrigin;
        if (absOrigin == null)
            return null;

        Vector eyePosition = new Vector(absOrigin.X, absOrigin.Y, absOrigin.Z + player.PlayerPawn.Value.ViewOffset.Z);

        CGameTrace? trace = player.GetGameTraceByEyePosition(TraceMask.MaskAll, Contents.NoDraw, player);
        if(trace == null)
            return null;

        Vector endPos = new Vector(trace.Value.EndPos.X, trace.Value.EndPos.Y, trace.Value.EndPos.Z);
        
        // Check if the vector is valid (not at the origin)
        if (endPos.X == 0 && endPos.Y == 0 && endPos.Z == 0)
            return null;

        return endPos;
    }
}