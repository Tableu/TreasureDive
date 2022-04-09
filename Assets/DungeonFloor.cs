using System.Collections.Generic;
using UnityEngine;

public class DungeonFloor
{
    private string[][] _layout;

    public string[][] Layout => _layout;

    private List<DungeonData.Ladder> _ladders;

    public List<DungeonData.Ladder> Ladders => _ladders;

    public DungeonFloor(string[][] layout)
    {
        _layout = layout;
    }

    //Can we move onto tile
    public bool IsTileValid(Vector2Int pos)
    {
        string tile = _layout[pos.y][pos.x];
        if (tile is DungeonData.HEALTH_UPGRADE || tile is DungeonData.HEALTH_REFILL ||
            tile is DungeonData.OXYGEN_UPGRADE || tile is DungeonData.OXYGEN_REFILL ||
            tile is DungeonData.ENTRANCE || tile is DungeonData.EXIT || 
            tile is DungeonData.EMPTY_SPACE)
        {
            return true;
        }
        return false;
    }

    public bool UseEntrance(Vector2Int pos)
    {
        DungeonData.Ladder ladder = Ladders.Find(ladder => ladder.Entrance == pos);
        if (ladder != null)
        {
            PlayerManager.Instance.Position = ladder.Exit;
            DungeonManager.Instance.MoveDownFloor();
            return true;
        }

        return false;
    }

    public bool UseExit(Vector2Int pos)
    {
        DungeonData.Ladder ladder = Ladders.Find(ladder => ladder.Exit == pos);
        if (ladder != null)
        {
            PlayerManager.Instance.Position = ladder.Entrance;
            DungeonManager.Instance.MoveUpFloor();
            return true;
        }

        return false;
    }

    public bool PickupItem(Vector2Int pos)
    {
        string tile = _layout[pos.y][pos.x];
        if (tile != null && !tile.Equals(""))
        {
            switch (tile)
            {
                case DungeonData.HEALTH_UPGRADE:
                    PlayerManager.Instance.MaxHealth += 10;
                    break;
                case DungeonData.HEALTH_REFILL:
                    PlayerManager.Instance.Health += 10;
                    if (PlayerManager.Instance.Health > PlayerManager.Instance.MaxHealth)
                    {
                        PlayerManager.Instance.Health = PlayerManager.Instance.MaxHealth;
                    }
                    break;
                case DungeonData.OXYGEN_UPGRADE:
                    PlayerManager.Instance.MaxOxygen += 10;
                    break;
                case DungeonData.OXYGEN_REFILL:
                    if (PlayerManager.Instance.Oxygen > PlayerManager.Instance.MaxOxygen)
                    {
                        PlayerManager.Instance.Oxygen = PlayerManager.Instance.MaxOxygen;
                    }
                    break;
                default:
                    return false;
                //Collectibles cases
            }

            _layout[pos.y][pos.x] = DungeonData.EMPTY_SPACE;
            return true;
        }

        return false;
    }
}
