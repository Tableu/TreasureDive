using System.Collections.Generic;
using UnityEngine;

public static class DungeonData
{
    public const int NORTH = 0;
    public const int EAST = 1;
    public const int SOUTH = 2;
    public const int WEST = 3;
    
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
        new[]
        {
            new []{"empty", "wll", "wll", "empty"},
            new []{"wll", "empty", "empty", "wll"},
            new []{"wll", "empty", "spawn", "wll"},
            new []{"empty", "wll", "wll", "empty"}
        },
        new[]
        {
            new []{"wll", "wll", "wll", "wll"},
            new []{"wll", "exit", "empty", "wll"},
            new []{"wll", "empty", "empty", "wll"},
            new []{"wll", "wll", "wll", "wll"}
        }
    };

    public static List<Ladder[]> LadderData = new List<Ladder[]>()
    {
        new[]
        {
            new Ladder() {Entrance = new Vector2Int(2,1), Exit = new Vector2Int(1,1)}
        }
    };

    public class Ladder
    {
        public Vector2Int Entrance;
        public Vector2Int Exit;
    }
}