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

    public int Health;
    public int MaxHealth;

    public int Oxygen;
    public int MaxOxygen;

    public Vector2Int Position;

    public bool Move(Vector2Int direction)
    {
        if (DungeonManager.Instance.CurrentFloor.IsTileValid(Position + direction))
        {
            Position += direction;
            DungeonManager.Instance.CurrentFloor.PickupItem(Position);
            return true;
        }
        return false;
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
