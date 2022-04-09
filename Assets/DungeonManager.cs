using System.Collections.Generic;

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
                instance.Initialize();
            }
            return instance;
        }
    }

    private void Initialize()
    {
        _dungeonFloors = new List<DungeonFloor>();
        foreach (string[][] floor in DungeonData.InitialDungeonData)
        {
            _dungeonFloors.Add(new DungeonFloor(floor));
        }
    }

    public void MoveUpFloor()
    {
        _currentFloorIndex--;
        _currentFloor = _dungeonFloors[_currentFloorIndex];
    }

    public void MoveDownFloor()
    {
        _currentFloorIndex++;
        _currentFloor = _dungeonFloors[_currentFloorIndex];
    }
}