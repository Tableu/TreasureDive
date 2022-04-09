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
        new Vector2Int(0,1),
        new Vector2Int(1,0),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0)
    };

    private int _viewDirection;

    public int Health;
    public int MaxHealth;

    public int Oxygen;
    public int MaxOxygen;

    public Vector2Int Position;
    public Vector2Int ViewDirection => viewDirections[_viewDirection%4];

    public bool Move(int direction)
    {
        direction = _viewDirection + direction;
        direction = direction % 4;
        if (DungeonManager.Instance.CurrentFloor.IsTileValid(Position + viewDirections[direction]))
        {
            Position += viewDirections[direction];
            DungeonManager.Instance.CurrentFloor.Interact(Position);
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
