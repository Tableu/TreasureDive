using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonFloor
{
    private string[][] _layout;

    public string[][] Layout => _layout;

    private List<DungeonData.Ladder> _ladders;

    public List<DungeonData.Ladder> Ladders => _ladders;

    public DungeonFloor(string[][] layout, DungeonData.Ladder[] ladders)
    {
        _layout = layout;
        _ladders = ladders.ToList();
    }

    //Can we move onto tile
    public bool IsTileValid(Vector2Int pos)
    {
        if (pos.y >= 0 && pos.y < _layout.GetLength(0)
                       && pos.x >= 0 && pos.x < _layout[0].GetLength(0))
        {
            string tile = _layout[pos.y][pos.x];
            if (!(tile is DungeonData.WALL))
            {
                return true;
            }
        }

        return false;
    }

    public bool UseEntrance(Vector2Int pos)
    {
        DungeonData.Ladder ladder = Ladders.Find(ladder => ladder.Entrance == pos);
        if (ladder != null)
        {
            Debug.Log("Used a ladder entrance");
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
            Debug.Log("Used a ladder exit");
            PlayerManager.Instance.Position = ladder.Entrance;
            DungeonManager.Instance.MoveUpFloor();
            return true;
        }

        return false;
    }

    public bool Interact(Vector2Int pos)
    {
        if (pos.y >= 0 && pos.y < _layout.GetLength(0)
                       && pos.x >= 0 && pos.x < _layout[0].GetLength(0))
        {
            string tile = _layout[pos.y][pos.x];
            if (tile != null && !tile.Equals(""))
            {
                switch (tile)
                {
                    case DungeonData.HEALTH_UPGRADE:
                        Debug.Log("Upgraded health!");
                        PlayerManager.Instance.MaxHealth += 10;
                        _layout[pos.y][pos.x] = DungeonData.EMPTY_SPACE;
                        break;
                    case DungeonData.HEALTH_REFILL:
                        PlayerManager.Instance.Health += 10;
                        Debug.Log("Refilled health!");
                        if (PlayerManager.Instance.Health > PlayerManager.Instance.MaxHealth)
                        {
                            PlayerManager.Instance.Health = PlayerManager.Instance.MaxHealth;
                        }

                        _layout[pos.y][pos.x] = DungeonData.EMPTY_SPACE;
                        break;
                    case DungeonData.OXYGEN_UPGRADE:
                        Debug.Log("Upgraded oxygen!");
                        PlayerManager.Instance.MaxOxygen += 10;
                        _layout[pos.y][pos.x] = DungeonData.EMPTY_SPACE;
                        break;
                    case DungeonData.OXYGEN_REFILL:
                        Debug.Log("Refilled oxygen!");
                        PlayerManager.Instance.Oxygen += 10;
                        if (PlayerManager.Instance.Oxygen > PlayerManager.Instance.MaxOxygen)
                        {
                            PlayerManager.Instance.Oxygen = PlayerManager.Instance.MaxOxygen;
                        }

                        _layout[pos.y][pos.x] = DungeonData.EMPTY_SPACE;
                        break;
                    case DungeonData.EXIT:
                        return UseExit(pos);
                    case DungeonData.ENTRANCE:
                        return UseEntrance(pos);
                    default:
                        return false;
                    //Collectibles cases
                }


                return true;
            }
        }

        return false;
    }
}
