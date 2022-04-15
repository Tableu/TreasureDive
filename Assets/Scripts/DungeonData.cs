using System.Collections.Generic;
using UnityEngine;

public static class DungeonData
{
    public const int NORTH = 0;
    public const int EAST = 1;
    public const int SOUTH = 2;
    public const int WEST = 3;
    
    public const string EXIT = "exi";
    public const string ENTRANCE = "ent";
    public const string WALL = "wll";
    public const string EMPTY_SPACE = "emp";
    public const string OXYGEN_REFILL = "o2r";
    public const string SPAWN_POINT = "spn";
    public const string HEALTH_UPGRADE = "hup";
    public const string HEALTH_REFILL = "hre";
    public const string OXYGEN_UPGRADE = "o2u";
    public const string TREASURE = "tre";
    public const string SQUID = "sqi";
    public const string BUBBLES = "bub";
    
    public static List<string[][]> InitialDungeonData = new List<string[][]>()
    {
        new[]
        {
            new []{"emp", "wll", "wll", "wll", "wll","wll", "wll","wll", "wll","emp"},
            new []{"wll", "emp", "emp", "wll", "emp","emp", "o2r","emp", "emp","wll"},
            new []{"wll", "emp", "tre", "wll", "emp","emp", "emp","emp", "emp","wll"},
            new []{"wll", "emp", "wll", "wll", "emp","emp", "emp","ent", "emp","wll"},
            new []{"wll", "emp", "emp", "o2r", "emp","wll", "emp","wll", "emp","wll"},
            new []{"wll", "emp", "emp", "emp", "emp","wll", "emp","wll", "emp","wll"},
            new []{"wll", "emp", "emp", "emp", "sqi","wll", "tre","wll", "tre","wll"},
            new []{"emp", "wll", "wll", "wll", "wll","wll", "wll","wll", "wll","emp"}
        },
        new[]
        {
            new []{"emp", "wll", "wll", "wll", "wll", "wll", "wll", "wll", "wll", "emp"},
            new []{"wll", "exi", "emp", "emp", "emp", "emp", "emp", "sqi", "tre", "wll"},
            new []{"wll", "o2r", "emp", "emp", "emp", "emp", "emp", "emp", "emp", "wll"},
            new []{"wll", "wll", "wll", "wll", "emp", "emp", "wll", "wll", "wll", "wll"},
            new []{"wll", "emp", "emp", "emp", "emp", "emp", "emp", "emp", "emp", "wll"},
            new []{"wll", "emp", "wll", "emp", "wll", "wll", "wll", "emp", "wll", "wll"},
            new []{"wll", "wll", "wll", "emp", "wll", "tre", "wll", "emp", "emp", "wll"},
            new []{"wll", "ent", "wll", "emp", "wll", "o2r", "wll", "wll", "emp", "wll"},
            new []{"wll", "emp", "wll", "emp", "wll", "emp", "wll", "emp", "emp", "wll"},
            new []{"wll", "emp", "emp", "emp", "wll", "emp", "emp", "emp", "wll", "wll"},
            new []{"emp", "wll", "wll", "wll", "wll", "wll", "wll", "wll", "wll", "emp"}
        },
        new[]
        {
            new []{"emp", "wll", "wll", "wll", "wll", "wll", "wll", "wll", "wll", "emp"},
            new []{"wll", "emp", "o2r", "wll", "sqi", "emp", "wll", "emp", "ent", "wll"},
            new []{"wll", "exi", "emp", "wll", "emp", "emp", "wll", "emp", "emp", "wll"},
            new []{"wll", "emp", "wll", "wll", "wll", "emp", "wll", "emp", "wll", "wll"},
            new []{"wll", "emp", "emp", "emp", "emp", "emp", "emp", "emp", "emp", "wll"},
            new []{"wll", "emp", "emp", "emp", "emp", "emp", "o2r", "emp", "emp", "wll"},
            new []{"wll", "wll", "wll", "wll", "wll", "wll", "wll", "wll", "wll", "wll"}
        }
    };

    public static List<Ladder[]> LadderData = new List<Ladder[]>()
    {
        new[]
        {
            new Ladder() {Entrance = new Vector2Int(7,3), Exit = new Vector2Int(1,1)}
        },
        new []
        {
            new Ladder(){Entrance = new Vector2Int(1, 7), Exit = new Vector2Int(1,2)}
        }
    };

    public class Ladder
    {
        public Vector2Int Entrance;
        public Vector2Int Exit;
    }
}