using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int seed = 10;
    [SerializeField] private int size = 21;
    [SerializeField] private List<TileConfig> tiles = new List<TileConfig>();

    private List<Tile> _tiles = new List<Tile>();

    void Start()
    {
        Random.InitState(seed);
        for (int i = 0; i < size; i++)
        {
            CreateRow(i - size / 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void CreateRow(int offset)
    {
        var horizontalOffset = Math.Abs(offset) % 2 == 1 ? 0.5f : 0;
        var currentWidth = Math.Max(3, size - Math.Abs(offset));
        for (int i = -currentWidth / 2; i < Math.Ceiling((float) currentWidth / 2); i++)
        {
            var go = GetRandomTile();
            go.transform.position = new Vector2(i + horizontalOffset - Math.Abs(offset) % 2, offset * 0.89f);
            var instance = Instantiate(go, transform);
            _tiles.Add(new Tile
            {
                GO = instance,
                Visible = false,
                X = i,
                Y = offset
            });
        }
    }

    GameObject GetRandomTile()
    {
        var possibilities = tiles.Aggregate(0, (prev, curr) => prev + curr.probability);
        var i = Random.Range(0, possibilities);
        foreach (var t in tiles)
        {
            i -= t.probability;
            if (i < 0) return t.go;
        }

        return tiles[0].go;
    }

    public int GetSize()
    {
        return size;
    }
}

[Serializable]
public class TileConfig
{
    public GameObject go;
    public int probability;
}