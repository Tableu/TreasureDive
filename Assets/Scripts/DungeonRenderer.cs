using System;
using UnityEngine;
using zooperdan.AtlasMaker;
using Vector2Int = UnityEngine.Vector2Int;

public class DungeonRenderer : MonoBehaviour
{
    [SerializeField] public TextAsset jsonFile;
    [SerializeField]public Sprite atlasImage;
    [SerializeField]public Camera camera;
    [SerializeField]public Vector2Int screenDimensions;
    [SerializeField]public Vector3 screenOffset;
    [SerializeField] public GameObject background;

    private JSONResult _atlasData;

    private void Awake()
    {
        instance = this;
        #if UNITY_EDITOR
        screenOffset = new Vector3(-0.75f, 0.56f, 0.5f);
#endif
    }

    void Start()
    {
        _atlasData = JsonUtility.FromJson<JSONResult>(jsonFile.text);
    }
    
    private static DungeonRenderer instance = null;

    public static DungeonRenderer Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject();
                instance = go.AddComponent<DungeonRenderer>();
                Instantiate(go);
            }
            return instance;
        }
    }

    public void InitializeDungeon()
    {
        DrawBackground(_atlasData.layers.Find(layer => layer.name == "ground").id - 1, "ground");
        DrawBackground(_atlasData.layers.Find(layer => layer.name == "ceiling").id - 1, "ceiling");
    }

    public void RenderDungeon()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        for (int z = -_atlasData.depth; z <= 0; z++)
        {
            for (int x = -_atlasData.width; x <= _atlasData.width; x++)
            {
                DrawSquare(x, z);
            }
        }
    }

    private Vector2Int GetPlayerDirectionVectorOffsets(int x, int z)
    {
        Vector2Int position = PlayerManager.Instance.Position;
        switch (PlayerManager.Instance.ViewDirectionOffset)
        {
            case DungeonData.SOUTH:
                return new Vector2Int(position.x - x, position.y - z);
            case DungeonData.EAST:
                return new Vector2Int(position.x - z, position.y + x);
            case DungeonData.NORTH:
                return new Vector2Int(position.x + x, position.y + z);
            case DungeonData.WEST:
                return new Vector2Int(position.x + z, position.y - x);
        }

        return new Vector2Int(0, 0);
    }

    private void DrawSquare(int x, int y)
    {
        Vector2Int directionOffset = GetPlayerDirectionVectorOffsets(x, y);
        
        if (directionOffset.x >= 0 && directionOffset.y >= 0
                                  && directionOffset.y < DungeonManager.Instance.CurrentFloor.Layout.GetLength(0)
                                  && directionOffset.x < DungeonManager.Instance.CurrentFloor.Layout[0].GetLength(0))
        {
            int layerId;
            switch (DungeonManager.Instance.CurrentFloor.Layout[directionOffset.y][directionOffset.x])
            {
                case DungeonData.WALL:
                    layerId = _atlasData.layers.Find(layer => layer.name == "wall").id;
                    DrawSideWalls(layerId - 1, x, y);
                    DrawFrontWalls(layerId - 1, x, y); 
                    break;
                case DungeonData.TREASURE: 
                    layerId = _atlasData.layers.Find(layer => layer.name == "chest").id;
                    DrawObject(layerId - 1, x, y);
                    break;
                case DungeonData.OXYGEN_REFILL:
                    layerId = _atlasData.layers.Find(layer => layer.name == "oxygen_refill").id;
                    DrawObject(layerId-1, x, y);
                    break;
                case DungeonData.ENTRANCE:
                    layerId = _atlasData.layers.Find(layer => layer.name == "entrance").id;
                    DrawObject(layerId-1, x, y);
                    break;
                case DungeonData.EXIT:
                    layerId = _atlasData.layers.Find(layer => layer.name == "exit").id;
                    DrawObject(layerId-1, x, y);
                    break;
                case DungeonData.SQUID:
                    layerId = _atlasData.layers.Find(layer => layer.name == "squid").id;
                    DrawObject(layerId-1, x, y);
                    break;
                case DungeonData.BUBBLES:
                    layerId = _atlasData.layers.Find(layer => layer.name == "bubbles").id;
                    DrawObject(layerId-1, x, y);
                    break;
            }
        }
    }

    private void DrawObject(int layerId, int x, int y)
    {
        bool bothSides = _atlasData.layers[layerId] != null && _atlasData.layers[layerId].mode == 2;
        int xx = bothSides ? x - (x * 2) : 0;
        JSONTile tile = GetTile(layerId, "object", xx, y);

        if (tile != null)
        {
            GameObject go = MakeQuad(tile);

            if (bothSides)
            {
                go.transform.position = camera.ScreenToWorldPoint(new Vector3(tile.screen.x + screenDimensions.x/2, screenDimensions.y-tile.screen.y-tile.coords.h)) + screenOffset + new Vector3(0,0,-y+0.015f);
            }
            else
            {
                int tx = tile.screen.x + (x * tile.coords.w);
                go.transform.position = camera.ScreenToWorldPoint(new Vector3(tx + screenDimensions.x/2, screenDimensions.y-tile.screen.y-tile.coords.h)) + screenOffset + new Vector3(0,0,-y+0.015f);
            }
        }
    }

    private void DrawFrontWalls(int layerId, int x, int y)
    {
        bool bothSides = _atlasData.layers[layerId] != null && _atlasData.layers[layerId].mode == 2;
        int xx = bothSides ? x - (x * 2) : 0;
        JSONTile tile = GetTile(layerId, "front", xx, y);
        if (tile != null)
        {
            GameObject go = MakeQuad(tile);
            if (bothSides)
            {
                go.transform.position = camera.ScreenToWorldPoint(new Vector3(tile.screen.x + screenDimensions.x/2, screenDimensions.y-tile.screen.y-tile.coords.h)) + screenOffset + new Vector3(0,0,-y+0.01f);
            }
            else
            {
                int tx = tile.screen.x + (x * tile.coords.w);
                go.transform.position = camera.ScreenToWorldPoint(new Vector3(tx + screenDimensions.x/2, screenDimensions.y-tile.screen.y-tile.coords.h)) + screenOffset + new Vector3(0,0,-y+0.01f);
            }
        }
    }

    private void DrawSideWalls(int layerId, int x, int y)
    {
        if (x <= 0)
        {
            JSONTile tile = GetTile(layerId, "side", x - (x * 2), y);
            if (tile != null)
            {
                GameObject go = MakeQuad(tile);
                go.transform.position = camera.ScreenToWorldPoint(new Vector3(tile.screen.x + screenDimensions.x/2, screenDimensions.y-tile.screen.y-tile.coords.h)) + screenOffset + new Vector3(0,0,-y+0.02f);
            }
        }

        if (x >= 0)
        {
            JSONTile tile = GetTile(layerId, "side", x, y);
            if (tile != null)
            {
                GameObject go = MakeQuad(tile);
                int tx = screenDimensions.x - tile.screen.x;
                go.transform.position = camera.ScreenToWorldPoint(new Vector3(tx + screenDimensions.x/2, screenDimensions.y-tile.screen.y-tile.coords.h)) + screenOffset + new Vector3(0,0,-y+0.02f);
                go.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    private void DrawBackground(int layerId, string layerName)
    {
        bool bothSides = _atlasData.layers[layerId] != null && _atlasData.layers[layerId].mode == 2;
        for (int z = -_atlasData.depth; z <= 0; z++)
        {
            for (int x = -_atlasData.width; x <= _atlasData.width; x++)
            {
                int xx = bothSides ? x - (x * 2) : 0;
                JSONTile tile = GetTile(layerId, layerName, xx, z);
                if (tile != null)
                {
                    GameObject go = MakeQuad(tile);
                    go.transform.SetParent(background.transform);
                    if (bothSides)
                    {
                        go.transform.position = camera.ScreenToWorldPoint(new Vector3(tile.screen.x + screenDimensions.x/2, screenDimensions.y-tile.screen.y-tile.coords.h)) + screenOffset + new Vector3(0,0,-z+0.03f);
                    }
                    else
                    {
                        int tx = tile.screen.x + (x * tile.coords.w);
                        go.transform.position = camera.ScreenToWorldPoint(new Vector3(tx + screenDimensions.x/2, screenDimensions.y-tile.screen.y-tile.coords.h)) + screenOffset + new Vector3(0,0,-z+0.03f);
                    }
                }
            }
        }
    }

    private GameObject MakeQuad(JSONTile tile)
    {
        GameObject go = new GameObject();
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(atlasImage.texture, new Rect(tile.coords.x, atlasImage.texture.height-tile.coords.y-tile.coords.h, tile.coords.w, tile.coords.h), Vector2.zero, 100);
        go.transform.parent = transform;
        return go;
    }

    private JSONTile GetTile(int layerId, string tileType, int x, int z)
    {
        if (layerId < 0 || layerId >= _atlasData.layers.Count)
        {
            return null;
        }

        JSONLayer layer = _atlasData.layers[layerId];

        if (layer == null)
            return null;
        foreach (JSONTile tile in layer.tiles)
        {
            if (tile.type == tileType && tile.tile.x == x && tile.tile.y == z)
            {
                return tile;
            }
        }

        return null;
    }
}
