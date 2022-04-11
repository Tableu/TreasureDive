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
        if (!(tile is DungeonData.WALL))
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
            //AkSoundEngine.PostEvent("env_bubbles_event", gameObject);
            return true;
        }

        return false;
    }

    public bool Interact(Vector2Int pos)
    {
        string tile = _layout[pos.y][pos.x];
        if (tile != null && !tile.Equals(""))
        {
            switch (tile)
            {
                case DungeonData.HEALTH_UPGRADE:
                    PlayerManager.Instance.MaxHealth += 10;
                    _layout[pos.y][pos.x] = DungeonData.EMPTY_SPACE;
                    break;
                case DungeonData.HEALTH_REFILL:
                    PlayerManager.Instance.Health += 10;
                    if (PlayerManager.Instance.Health > PlayerManager.Instance.MaxHealth)
                    {
                        PlayerManager.Instance.Health = PlayerManager.Instance.MaxHealth;
                    }
                    _layout[pos.y][pos.x] = DungeonData.EMPTY_SPACE;
                    break;
                case DungeonData.OXYGEN_UPGRADE:
                    PlayerManager.Instance.MaxOxygen += 10;
                    _layout[pos.y][pos.x] = DungeonData.EMPTY_SPACE;
                    break;
                case DungeonData.OXYGEN_REFILL:
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

        return false;
    }
}
