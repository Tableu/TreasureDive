using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager
{
    private List<DungeonFloor> _dungeonFloors;
    private DungeonFloor _currentFloor;
    private int _currentFloorIndex;

    public List<DungeonFloor> DungeonFloors => _dungeonFloors;
    public DungeonFloor CurrentFloor => _currentFloor;
    
    private static DungeonManager instance = null;

    public static DungeonManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DungeonManager();
            }
            return instance;
        }
    }

    public void Initialize()
    {
        _dungeonFloors = new List<DungeonFloor>();
        int i = 0;
        foreach (string[][] floor in DungeonData.InitialDungeonData)
        {
            string[][] copy = new string[floor.Length][];
            for (int x = 0; x < floor.Length; x++)
            {
                copy[x] = floor[x].Clone() as string[];
            }
            
            _dungeonFloors.Add(new DungeonFloor(copy, DungeonData.LadderData[i]));
            if (i < DungeonData.LadderData.Count - 1)
            {
                i++;
            }
        }
        _currentFloor = _dungeonFloors[0];
        _currentFloorIndex = 0;
        DungeonRenderer.Instance.RenderDungeon();
    }

    public void Restart()
    {
        PlayerManager.Instance.Reset();
        Initialize();
        OnRestart?.Invoke();
    }
    public void MoveUpFloor()
    {
        _currentFloorIndex--;
        if (_currentFloorIndex >= 0)
        {
            _currentFloor = _dungeonFloors[_currentFloorIndex];
            Debug.Log("Moving up a floor");
        }
        else
        {
            _currentFloorIndex++;
        }
        DungeonRenderer.Instance.RenderDungeon();
    }

    public void MoveDownFloor()
    {
        _currentFloorIndex++;
        if (_currentFloorIndex < _dungeonFloors.Count)
        {
            _currentFloor = _dungeonFloors[_currentFloorIndex];
            Debug.Log("Moving down a floor");
        }
        else
        {
            _currentFloorIndex--;
        }
        DungeonRenderer.Instance.RenderDungeon();
    }

    public Action OnRestart;
}