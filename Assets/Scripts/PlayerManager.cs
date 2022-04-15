using System;
using System.Collections;
using UnityEngine;

public class PlayerManager
{
    private static PlayerManager instance;

    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerManager();
            }
            return instance;
        }
    }

    private Vector2Int[] viewDirections = new[]
    {
        new Vector2Int(0,-1),
        new Vector2Int(1,0),
        new Vector2Int(0,1),
        new Vector2Int(-1,0)
    };

    private int _viewDirection = 0;

    public int Health = 5;
    public int MaxHealth = 5;

    public int Oxygen = 30;
    public int MaxOxygen = 30;

    public int WeaponCooldown = 0;
    public int WeaponMaxCooldown = 3;

    public int Treasure = 0;

    public Vector2Int Position = new Vector2Int(1,1);
    public Vector2Int ViewDirection => viewDirections[_viewDirection%4];
    public int ViewDirectionOffset => _viewDirection;
    public float OxygenPercent => (float)Oxygen / (float)MaxOxygen;
    public float HealthPercent => (float) Health / (float) MaxHealth;
    public float WeaponCooldownPercent => (float) WeaponCooldown / (float) WeaponMaxCooldown;

    public void Reset()
    {
        Health = 5;
        MaxHealth = 5;
        Oxygen = 30;
        MaxOxygen = 30;
        Treasure = 0;
        WeaponCooldown = 0;
        WeaponMaxCooldown = 3;
        Position = new Vector2Int(1, 1);
        _viewDirection = 0;
    }
    public bool Move(int direction)
    {
        direction = _viewDirection + direction;
        direction = direction % 4;
        if (DungeonManager.Instance.CurrentFloor.IsTileValid(Position + viewDirections[direction]))
        {
            Position += viewDirections[direction];
            Debug.Log("Moving " + viewDirections[direction]);
            DungeonManager.Instance.CurrentFloor.Interact(Position);
            DungeonRenderer.Instance.RenderDungeon();
            return true;
        }
        return false;
    }

    public void CheckForEnemies()
    {
        foreach (Vector2Int direction in viewDirections)
        {
            var edge = Position + direction;
            if (edge.y >= 0 && edge.y < DungeonManager.Instance.CurrentFloor.Layout.GetLength(0)
                            && edge.x >= 0 && edge.x < DungeonManager.Instance.CurrentFloor.Layout[0].GetLength(0)
                            && DungeonManager.Instance.CurrentFloor.Layout[edge.y][edge.x] == DungeonData.SQUID)
            {
                Oxygen -= 10;
                if (Oxygen == 0)
                {
                    OnDeath?.Invoke();
                }
            }
        }
        if (Position.y >= 0 && Position.y < DungeonManager.Instance.CurrentFloor.Layout.GetLength(0)
                        && Position.x >= 0 && Position.x < DungeonManager.Instance.CurrentFloor.Layout[0].GetLength(0)
                        && DungeonManager.Instance.CurrentFloor.Layout[Position.y][Position.x] == DungeonData.SQUID)
        {
            Oxygen -= 10;
            if (Oxygen == 0)
            {
                OnDeath?.Invoke();
            }
        }
    }

    public IEnumerator Attack()
    {
        WeaponCooldown = WeaponMaxCooldown;
        var pos1 = Position + viewDirections[_viewDirection];
        var pos2 = Position + viewDirections[_viewDirection] * 2;
        if (pos1.x >= 0 && pos1.y >= 0
                                   && pos1.y < DungeonManager.Instance.CurrentFloor.Layout.GetLength(0)
                                   && pos1.x < DungeonManager.Instance.CurrentFloor.Layout[0].GetLength(0)
                                   && (DungeonManager.Instance.CurrentFloor.Layout[pos1.y][pos1.x] is DungeonData.SQUID 
                                       || DungeonManager.Instance.CurrentFloor.Layout[pos1.y][pos1.x] is DungeonData.EMPTY_SPACE))
        {
            if (DungeonManager.Instance.CurrentFloor.Layout[pos1.y][pos1.x] is DungeonData.SQUID)
            {
                //AkSoundEngine.PostEvent("enemy_damage_event", GameObject.Find("WwiseGlobal"));
                DungeonManager.Instance.KillEnemy(pos1);
            }
            DungeonManager.Instance.CurrentFloor.Layout[pos1.y][pos1.x] = DungeonData.BUBBLES;
        }
        if (pos2.x >= 0 && pos2.y >= 0
                              && pos2.y < DungeonManager.Instance.CurrentFloor.Layout.GetLength(0)
                              && pos2.x < DungeonManager.Instance.CurrentFloor.Layout[0].GetLength(0) 
                              && (DungeonManager.Instance.CurrentFloor.Layout[pos2.y][pos2.x] is DungeonData.SQUID
                              || DungeonManager.Instance.CurrentFloor.Layout[pos2.y][pos2.x] is DungeonData.EMPTY_SPACE))
        {
            if (DungeonManager.Instance.CurrentFloor.Layout[pos2.y][pos2.x] is DungeonData.SQUID)
            {
                //AkSoundEngine.PostEvent("enemy_damage_event", GameObject.Find("WwiseGlobal"));
                DungeonManager.Instance.KillEnemy(pos2);
            }
            DungeonManager.Instance.CurrentFloor.Layout[pos2.y][pos2.x] = DungeonData.BUBBLES;
        }
        DungeonRenderer.Instance.RenderDungeon();
        yield return new WaitForSeconds(1);
        if (pos1.x >= 0 && pos1.y >= 0
                        && pos1.y < DungeonManager.Instance.CurrentFloor.Layout.GetLength(0)
                        && pos1.x < DungeonManager.Instance.CurrentFloor.Layout[0].GetLength(0)
                        && DungeonManager.Instance.CurrentFloor.Layout[pos1.y][pos1.x] == DungeonData.BUBBLES)
        {
            DungeonManager.Instance.CurrentFloor.Layout[pos1.y][pos1.x] = DungeonData.EMPTY_SPACE;
        }
        if (pos2.x >= 0 && pos2.y >= 0
                        && pos2.y < DungeonManager.Instance.CurrentFloor.Layout.GetLength(0)
                        && pos2.x < DungeonManager.Instance.CurrentFloor.Layout[0].GetLength(0)
                        && DungeonManager.Instance.CurrentFloor.Layout[pos2.y][pos2.x] == DungeonData.BUBBLES)
        {
            DungeonManager.Instance.CurrentFloor.Layout[pos2.y][pos2.x] = DungeonData.EMPTY_SPACE;
        }
        DungeonRenderer.Instance.RenderDungeon();
    }

    public void Rotate(int direction)
    {
        if (direction > 0)
        {
            _viewDirection++;
            if (_viewDirection > 3)
            {
                _viewDirection = 0;
            }
        }else if (direction < 0)
        {
            _viewDirection--;
            if (_viewDirection < 0)
            {
                _viewDirection = 3;
            }
        }
        Debug.Log("Rotating " + _viewDirection);
        DungeonRenderer.Instance.RenderDungeon();
    }

    public IEnumerator OxygenTimer()
    {
        while (Oxygen > 0)
        {
            Oxygen--;
            yield return new WaitForSeconds(1);
        }
        OnDeath?.Invoke();
    }

    public IEnumerator WeaponTimer()
    {
        yield return new WaitForSeconds(1);
        while (WeaponCooldown > 0)
        {
            WeaponCooldown--;
            yield return new WaitForSeconds(1);
        }
    }

    public Action OnDeath;
}
