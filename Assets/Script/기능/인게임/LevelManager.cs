using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]
    private GameObject[] tilePrefabs;

    public Dictionary<Point, Tile> Tiles { get; set; }
    public List<Point> OccupiedPoints { get; private set; }

    [SerializeField]
    private Transform map;

    public Vector3 TileSize
    {
        get
        {
            SpriteRenderer spriteRenderer = tilePrefabs[0].GetComponent<SpriteRenderer>();
            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
            return new Vector3(spriteSize.x, spriteSize.y, 0f);
        }
    }

    private void Awake()
    {
        Tiles = new Dictionary<Point, Tile>();
    }

    public Tile GetTileAt(int x, int y)
    {
        Point point = new Point(x, y);
        if (Tiles.TryGetValue(point, out Tile tile))
        {
            return tile;
        }
        return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        OccupiedPoints = new List<Point>();
    }

    public IEnumerator CreateLevel()
    {
        //Tiles = new Dictionary<Point, Tile>();

        string[] mapData = ReadLevelText();

        int mapXSize = mapData[0].ToCharArray().Length;
        int mapYSize = mapData.Length;


        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));
        for(int y = 0; y < mapYSize; y++)
        {
            char[] newTiles = mapData[y].ToCharArray();

            for (int x = 0; x < mapXSize; x++)
            {
                PlaceTile(newTiles[x].ToString(), x,y, worldStart);
            }
        }
        yield return null;
    }
    private void PlaceTile(string tileType, int x, int y, Vector3 worldStart)
    {
        int tileIndex = int.Parse(tileType);
        Tile newTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<Tile>();

        newTile.Setup(new Point(x, y), new Vector3(worldStart.x + (TileSize.x * x) + 1.35f, worldStart.y - (TileSize.y * y) - 4.74f /*5.16f*/, 0), map); 

        Tiles.Add(new Point(x, y), newTile);
    }
    private string[] ReadLevelText()
    {
        TextAsset bindData = Resources.Load("Level") as TextAsset;

        string data = bindData.text.Replace(Environment.NewLine, string.Empty);

        return data.Split('-');
    }

    public Point PositionToPoint(Vector3 position)
    {
        int x = Mathf.RoundToInt((position.x - 1.35f) / TileSize.x);
        int y = Mathf.RoundToInt((position.y + 4.74f) / TileSize.y);
        return new Point(x, y);
    }

    public void GoMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main");
    }
}
