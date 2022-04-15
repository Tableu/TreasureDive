using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private List<DungeonFloor> _dungeonFloors;
    private DungeonFloor _currentFloor;
    private int _currentFloorIndex;
    private List<Vector2Int> enemyPos;
    private Vector2Int[] viewDirections = new[]
    {
        new Vector2Int(0,-1),
        new Vector2Int(1,0),
        new Vector2Int(0,1),
        new Vector2Int(-1,0)
    };

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

    private void Awake()
    {
        instance = this;
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
        StopCoroutine(Enemies());
        StartCoroutine(Enemies());
        DungeonRenderer.Instance.InitializeDungeon();
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
        StopCoroutine(Enemies());
        StartCoroutine(Enemies());
        //AkSoundEngine.PostEvent("env_bubbles_event", GameObject.Find("WwiseGlobal"));
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
        StopCoroutine(Enemies());
        StartCoroutine(Enemies());
        //AkSoundEngine.PostEvent("env_bubbles_event", GameObject.Find("WwiseGlobal"));
        DungeonRenderer.Instance.RenderDungeon();
    }

    public void KillEnemy(Vector2Int pos)
    {
        foreach (Vector2Int enemy in enemyPos.ToList())
        {
            if (enemy.Equals(pos))
            {
                enemyPos.Remove(enemy);
            }
        }
    }

    public IEnumerator Enemies()
    {
        enemyPos = new List<Vector2Int>();
        for (int y = 0; y < _currentFloor.Layout.Length; y++)
        {
            for (int x = 0; x < _currentFloor.Layout[0].Length; x++)
            {
                if (_currentFloor.Layout[y][x] == DungeonData.SQUID)
                {
                    enemyPos.Add(new Vector2Int(x,y));
                }
            }
        }
        while (true)
        {
            for (var index = 0; index < enemyPos.Count; index++)
            {
                Vector2Int enemy = enemyPos[index];
                List<Vector2Int> edges = new List<Vector2Int>();
                foreach (Vector2Int direction in viewDirections)
                {
                    edges.Add(enemy + direction);
                }

                edges = edges.OrderBy(x => Vector2.Distance(PlayerManager.Instance.Position, x)).ToList();
                foreach (Vector2Int edge in edges)
                {
                    if (edge.y >= 0 && edge.y < CurrentFloor.Layout.GetLength(0)
                                    && edge.x >= 0 && edge.x < CurrentFloor.Layout[0].GetLength(0)
                                    && (CurrentFloor.Layout[edge.y][edge.x] == DungeonData.EMPTY_SPACE
                                    || CurrentFloor.Layout[edge.y][edge.x] == DungeonData.BUBBLES))
                    {
                        CurrentFloor.Layout[edge.y][edge.x] = DungeonData.SQUID;
                        CurrentFloor.Layout[enemy.y][enemy.x] = DungeonData.EMPTY_SPACE;
                        enemyPos[index] = edge;
                        break;
                    }
                }
            }

            PlayerManager.Instance.CheckForEnemies();
            DungeonRenderer.Instance.RenderDungeon();
            yield return new WaitForSeconds(2);
        }
    }

    public Action OnRestart;
}