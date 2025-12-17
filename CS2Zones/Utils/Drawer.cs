
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using System.Drawing;
using CSTimer = CounterStrikeSharp.API.Modules.Timers;

public static class Drawer
{
    public static void DrawPoint(Vector point, Color color)
    {
        float radius = 1.0f; // Circle radius in units
        int segments = 1;
        float step = (float)(2.0 * Math.PI) / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * step;
            Vector start = new Vector(
                point.X + (float)(radius * Math.Cos(angle)),
                point.Y + (float)(radius * Math.Sin(angle)),
                point.Z
            );
            Vector end = new Vector(
                point.X + (float)(radius * Math.Cos(angle + step)),
                point.Y + (float)(radius * Math.Sin(angle + step)),
                point.Z
            );

            DrawLaserBetween(start, end, color);
        }

        // Draw a vertical line to better see the point
        Vector top = new Vector(point.X, point.Y, point.Z + radius);
        Vector bottom = new Vector(point.X, point.Y, point.Z - radius);
        DrawLaserBetween(top, bottom, color);
    }

    public static void DrawLaserBetween(Vector start, Vector end, Color color)
    {
        CBeam beam = Utilities.CreateEntityByName<CBeam>("beam")!;
        if (beam == null)
            return;

        beam.Render = color;
        beam.Width = 0.5f;
        beam.Teleport(start, new QAngle(0, 0, 0), new Vector(0, 0, 0));

        beam.EndPos.X = end.X;
        beam.EndPos.Y = end.Y;
        beam.EndPos.Z = end.Z;

        beam.DispatchSpawn();

        // Kill after 0.1 second
        CS2Zones.CS2Zones.globalCtx?.AddTimer(0.1f,() => {
            try 
            {
                Guard.IsValidEntity(beam); // Safe, otherwise we will lag the server :(
            } catch
            {
                return;
            }
        
            beam.Remove();
        }, CSTimer.TimerFlags.STOP_ON_MAPCHANGE);
    }

    public static void DrawWireframe3D(Vector corner1, Vector corner8, Color color)
    {
        Vector corner2 = new(corner1.X, corner8.Y, corner1.Z);
        Vector corner3 = new(corner8.X, corner8.Y, corner1.Z);
        Vector corner4 = new(corner8.X, corner1.Y, corner1.Z);

        Vector corner5 = new(corner8.X, corner1.Y, corner8.Z);
        Vector corner6 = new(corner1.X, corner1.Y, corner8.Z);
        Vector corner7 = new(corner1.X, corner8.Y, corner8.Z);

        //top square
        DrawLaserBetween(corner1, corner2, color);
        DrawLaserBetween(corner2, corner3, color);
        DrawLaserBetween(corner3, corner4, color);
        DrawLaserBetween(corner4, corner1, color);

        //bottom square
        DrawLaserBetween(corner5, corner6, color);
        DrawLaserBetween(corner6, corner7, color);
        DrawLaserBetween(corner7, corner8, color);
        DrawLaserBetween(corner8, corner5, color);

        //connect them both to build a cube
        DrawLaserBetween(corner1, corner6, color);
        DrawLaserBetween(corner2, corner7, color);
        DrawLaserBetween(corner3, corner8, color);
        DrawLaserBetween(corner4, corner5, color);
    }   
}