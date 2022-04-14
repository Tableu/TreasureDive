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

    public int Treasure = 0;

    public Vector2Int Position = new Vector2Int(1,1);
    public Vector2Int ViewDirection => viewDirections[_viewDirection%4];
    public int ViewDirectionOffset => _viewDirection;
    public float OxygenPercent => (float)Oxygen / (float)MaxOxygen;

    public void Reset()
    {
        Health = 5;
        MaxHealth = 5;
        Oxygen = 30;
        MaxOxygen = 30;
        Treasure = 0;
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

    public Action OnDeath;
}
