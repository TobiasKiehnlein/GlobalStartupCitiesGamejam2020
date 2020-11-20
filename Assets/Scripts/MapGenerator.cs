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
    [SerializeField] private TileSettings tileSettings;

    public Tile CurrentlyDraggedTile { get; set; }

    private LineRenderer _lineRenderer;
    private List<Tile> _tiles = new List<Tile>();

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        Random.InitState(seed);
        for (int i = -size / 2 + 1; i < size / 2; i++)
        {
            CreateRow(i);
        }
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

            instance.name = $"X:{i} - Y:{offset}";

            var tile = instance.AddComponent<Tile>();
            tile.X = i;
            tile.Y = offset;
            tile.tileSettings = tileSettings;
            tile.lineRenderer = _lineRenderer;
            tile.mapGenerator = this;
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

    public bool IsSwapAllowed(Tile dest)
    {
        Debug.Log(dest);
        Debug.Log(CurrentlyDraggedTile);
        if (CurrentlyDraggedTile == null) return false;
        // if (dest.Flipped == CurrentlyDraggedTile.Flipped) return false; // Not allowed if both are flipped or both aren't
        if (!NextToEachOther(dest, CurrentlyDraggedTile)) return false; // Only Tiles next to each other can be swapped
        return true;
    }

    public bool NextToEachOther(Tile dest, Tile curr)
    {
        if (dest.Y == curr.Y)
        {
            return Math.Abs(dest.X - curr.X) == 1;
        }

        if (Math.Abs(dest.Y - curr.Y) == 1)
        {
            if (Math.Abs(dest.X - curr.X) == 0) return true;
            if (Math.Abs(dest.X - curr.X) == 1)
            {
                return dest.X > curr.X;
            }
        }

        return false;
    }
}

[Serializable]
public class TileConfig
{
    public GameObject go;
    public int probability;
}