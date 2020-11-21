using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int seed = 10;
    [SerializeField] private GameObject borderTile;
    [SerializeField] private List<TileConfig> tiles = new List<TileConfig>();
    [SerializeField] private TileSettings tileSettings;
    [SerializeField] private GameSettings gameSettings;

    public Tile CurrentlyDraggedTile { get; set; }
    public Tile CurrentlyHoveredTile { get; set; }

    private LineRenderer _lineRenderer;
    public List<Tile> Tiles = new List<Tile>();
    private int _size = 21;

    void Start()
    {
        _size = gameSettings.mapRadius * 2 + 3;
        _lineRenderer = GetComponent<LineRenderer>();
        Random.InitState(seed);
        for (int i = -_size / 2; i < _size / 2 + 1; i++)
        {
            CreateRow(i, i == -_size / 2 || i == _size / 2);
        }
    }

    void CreateRow(int offset, bool onlyBorders = false)
    {
        var currentWidth = Math.Max(3, _size - Math.Abs(offset));
        var lower = -currentWidth / 2;
        var upper = Math.Ceiling((float) currentWidth / 2);
        for (int i = lower; i < upper; i++)
        {
            var isBorder = onlyBorders || i == lower || i == (int) (upper - 1);
            var go = isBorder ? borderTile : GetRandomTile();
            go.transform.position = CoordsToWorldPosition(i, offset);
            var instance = Instantiate(go, transform);

            instance.name = $"X:{i} - Y:{offset}";

            var tile = instance.AddComponent<Tile>();
            Tiles.Add(tile);
            tile.X = i;
            tile.Y = offset;
            tile.tileSettings = tileSettings;
            tile.lineRenderer = _lineRenderer;
            tile.mapGenerator = this;
            if (isBorder)
            {
                tile.role = Role.Border;
                tile.Flipped = true;
            }
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
        var horizontalOffset = Math.Abs(y) % 2 == 1 ? 1.5f : 0;
        return new Vector3(x + horizontalOffset - Math.Abs(y) % 2, y * 0.89f, 0);
    }

    public int GetSize()
    {
        return _size;
    }

    public bool IsSwapAllowed(Tile dest)
    {
        if (CurrentlyDraggedTile == null) return false;
        if (CurrentlyDraggedTile.role == Role.Border || dest.role == Role.Border) return false; //borders cannot be moved
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
            MakeVisibleAroundTile(CurrentlyDraggedTile);
            MakeVisibleAroundTile(CurrentlyHoveredTile);
        }
        else
        {
            // TODO Display Error message to user
            Debug.LogWarning("These Two cannot be swapped");
        }
    }

    private void MakeVisibleAroundTile(Tile tile, float amount = 2.2f)
    {
        foreach (var t in Tiles.Where(x => (x.gameObject.transform.position - tile.transform.position).magnitude < amount))
        {
            t.Visible = true;
        }
    }

    private bool IsGameFinished()
    {
        if (Tiles.All(x => x.Flipped)) return true;
        return false;
    }
}

[Serializable]
public class TileConfig
{
    public GameObject go;
    public int probability;
}