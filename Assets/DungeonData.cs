using System.Collections.Generic;
using UnityEngine;

public static class DungeonData
{
    public const string EXIT = "exit";
    public const string ENTRANCE = "entrance";
    public const string WALL = "wll";
    public const string EMPTY_SPACE = "empty";
    public const string OXYGEN_REFILL = "oxy_refill";
    public const string SPAWN_POINT = "spawn";
    public const string HEALTH_UPGRADE = "health_up";
    public const string HEALTH_REFILL = "health_refill";
    public const string OXYGEN_UPGRADE = "oxy_up";
    
    public static List<string[][]> InitialDungeonData = new List<string[][]>()
    {
        Floor_1
    };
    public static string[][] Floor_1 = new[]
    {
        new []{"empty", "empty", "empty"},
        new []{"empty", "empty", "empty"}
    };

    public static List<Ladder[]> LadderData = new List<Ladder[]>()
    {
        Floor_1_Ladders
    };
    public static Ladder[] Floor_1_Ladders = new[]
    {
        new Ladder() {Entrance = new Vector2Int(0,0), Exit = new Vector2Int(1,1)}
    };

    public class Ladder
    {
        public Vector2Int Entrance;
        public Vector2Int Exit;
    }
}