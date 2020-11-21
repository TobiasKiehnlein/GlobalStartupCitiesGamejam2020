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
    public Tile CurrentlyHoveredTile { get; set; }

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
        var currentWidth = Math.Max(3, size - Math.Abs(offset));
        for (int i = -currentWidth / 2; i < Math.Ceiling((float) currentWidth / 2); i++)
        {
            var go = GetRandomTile();
            go.transform.position = CoordsToWorldPosition(i, offset);
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

    private Vector2 CoordsToWorldPosition(int x, int y)
    {
        var horizontalOffset = Math.Abs(y) % 2 == 1 ? 0.5f : 0;
        return new Vector3(x + horizontalOffset - Math.Abs(y) % 2, y * 0.89f, 0);
    }

    public int GetSize()
    {
        return size;
    }

    public bool IsSwapAllowed(Tile dest)
    {
        if (CurrentlyDraggedTile == null) return false;
        if (dest.Flipped == CurrentlyDraggedTile.Flipped) return false; // Not allowed if both are flipped or both aren't
        if (!NextToEachOther(dest, CurrentlyDraggedTile)) return false; // Only Tiles next to each other can be swapped
        return true;
    }

    private bool NextToEachOther(Tile dest, Tile curr)
    {
        return Vector3.Magnitude(dest.gameObject.transform.position - curr.gameObject.transform.position) < 1.2;
        // if (dest.Y == curr.Y)
        // {
        //     return Math.Abs(dest.X - curr.X) == 1;
        // }
        //
        // if (Math.Abs(dest.Y - curr.Y) == 1)
        // {
        //     if (Math.Abs(dest.X - curr.X) == 0) return true;
        //     if (Math.Abs(dest.X - curr.X) == 1)
        //     {
        //         return dest.X > curr.X;
        //     }
        // }
        //
        // return false;
    }

    public void SwapTiles()
    {
        if (IsSwapAllowed(CurrentlyHoveredTile))
        {
            // TODO Do stuff
            var x = CurrentlyHoveredTile.X;
            var y = CurrentlyHoveredTile.Y;
            CurrentlyHoveredTile.X = CurrentlyDraggedTile.X;
            CurrentlyHoveredTile.Y = CurrentlyDraggedTile.Y;
            CurrentlyDraggedTile.X = x;
            CurrentlyDraggedTile.Y = y;

            //TODO Change to Method inside Tile and Lerp
            CurrentlyHoveredTile.DestinationPosition = CoordsToWorldPosition(CurrentlyHoveredTile.X, CurrentlyHoveredTile.Y);
            CurrentlyDraggedTile.DestinationPosition = CoordsToWorldPosition(CurrentlyDraggedTile.X, CurrentlyDraggedTile.Y);
            CurrentlyHoveredTile.Flipped = true;
            CurrentlyDraggedTile.Flipped = true;
        }
        else
        {
            // TODO Display Error message to user
            Debug.LogWarning("These Two cannot be swapped");
        }
    }
}

[Serializable]
public class TileConfig
{
    public GameObject go;
    public int probability;
}