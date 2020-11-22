using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int seed;
    [SerializeField] private GameObject borderTile;
    [SerializeField] private List<TileConfig> tiles = new List<TileConfig>();
    [SerializeField] private TileSettings tileSettings;
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private Score score;
    [SerializeField] private GameObject pointPopup;

    public Tile CurrentlyDraggedTile { get; set; }
    public Tile CurrentlyHoveredTile { get; set; }

    private LineRenderer _lineRenderer;
    public List<Tile> Tiles = new List<Tile>();
    private int _size = 21;

    void Start()
    {
        score.civilizedPoints = 0;
        score.naturePoints = 0;
        score.isGameFinished = false;
        seed = gameSettings.seed;
        //TODO remove later and add seed system
        seed = Random.Range(0, int.MaxValue);
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
            GameObject go = null;
            Role role = Role.None;
            if (isBorder)
            {
                go = borderTile;
                role = Role.Border;
            }
            else
            {
                (go, role) = GetRandomTile(i == 0 && offset == 0);
            }

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
            tile.role = role;
            if (isBorder)
            {
                tile.Flipped = true;
            }
        }
    }

    (GameObject, Role) GetRandomTile(bool forceVillage = false)
    {
        var possibilities = tiles.Aggregate(0, (prev, curr) => prev + curr.probability);
        var i = Random.Range(0, possibilities);
        GameObject tile = null;
        Role role = Role.None;
        foreach (var t in tiles)
        {
            i -= t.probability;
            if (i < 0)
            {
                tile = t.Prefabs[Random.Range(0, t.Prefabs.Count)];
                role = t.Role;
                break;
            }
        }

        if (tile == null || forceVillage)
        {
            tile = tiles.Find(x => x.Role == Role.RuinedVillage).Prefabs[Random.Range(0, tiles[0].Prefabs.Count)];
            role = Role.Village;
        }

        return (tile, role);
    }

    private Vector3 CoordsToWorldPosition(int x, int y)
    {
        var horizontalOffset = Math.Abs(y) % 2 == 1 ? 1.5f : 0;
        return new Vector3(x + horizontalOffset - Math.Abs(y) % 2, y * 0.89f, .1f * y);
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
            CalculatePointsAndMorphTile();

            var x = CurrentlyHoveredTile.X;
            var y = CurrentlyHoveredTile.Y;
            CurrentlyHoveredTile.X = CurrentlyDraggedTile.X;
            CurrentlyHoveredTile.Y = CurrentlyDraggedTile.Y;
            CurrentlyDraggedTile.X = x;
            CurrentlyDraggedTile.Y = y;

            CurrentlyHoveredTile.DestinationPosition = CoordsToWorldPosition(CurrentlyHoveredTile.X, CurrentlyHoveredTile.Y);
            CurrentlyDraggedTile.DestinationPosition = CoordsToWorldPosition(CurrentlyDraggedTile.X, CurrentlyDraggedTile.Y);
            CurrentlyHoveredTile.Flipped = true;
            CurrentlyDraggedTile.Flipped = true;
            MakeVisibleAroundTile(CurrentlyDraggedTile);
            MakeVisibleAroundTile(CurrentlyHoveredTile);

            // CreateCities();

            HandleGameFinished();
        }
        else
        {
            // TODO Display Error message to user
            Debug.LogWarning("These Two cannot be swapped");
        }
    }

    private void CreateCities()
    {
        foreach (var tile in Tiles)
        {
            if (tile.role != Role.Village) continue;
            var possibleOtherVillageTiles = Tiles.Where(x => x.role == Role.Village && (x.transform.position - tile.transform.position).magnitude < 2).ToList();
            if (possibleOtherVillageTiles.Count < 3) continue;

            // there are at least two villages next to the current one

            var possibleCities = GetPowerSet(possibleOtherVillageTiles).Where(x => x.Count() == 3).Select(x => x.OrderBy(city => city.Y));

            // Check if shape is like this:
            //  x   or x x
            // x x      x
            foreach (var possibleCity in possibleCities)
            {
                var city = possibleCity.OrderByDescending(x => x.Y).Reverse().ToList();
                if (city[0].Y == city[1].Y && city[0].Y - 1 == city[2].Y)
                {
                    // Possibly x x
                    //           x
                    foreach (var tile1 in city)
                    {
                        Tiles.Find(x => x.ID == tile1.ID).role = Role.City;
                    }

                    return;
                }

                if (city[0].Y - 1 == city[1].Y && city[1].Y == city[2].Y)
                {
                    foreach (var tile1 in city)
                    {
                        Tiles.Find(x => x.ID == tile1.ID).role = Role.City;
                    }

                    return;
                }
            }
        }
    }

    public IEnumerable<IEnumerable<T>> GetPowerSet<T>(List<T> list)
    {
        return from m in Enumerable.Range(0, 1 << list.Count)
            select
                from i in Enumerable.Range(0, list.Count)
                where (m & (1 << i)) != 0
                select list[i];
    }

    void CalculatePointsAndMorphTile()
    {
        var unflipped = CurrentlyDraggedTile.Flipped ? CurrentlyHoveredTile : CurrentlyDraggedTile;
        var flipped = !CurrentlyDraggedTile.Flipped ? CurrentlyHoveredTile : CurrentlyDraggedTile;
        var civilizedTilesInRadius3 = Tiles.Count(x => x.Flipped && (
                                                                     x.role == Role.City ||
                                                                     x.role == Role.Mine ||
                                                                     x.role == Role.Village ||
                                                                     x.role == Role.SheepMeadows ||
                                                                     x.role == Role.Farmland ||
                                                                     x.role == Role.Mine)
                                                                 && (x.gameObject.transform.position - flipped.transform.position).magnitude < 4);
        // var natureTilesInRadius3 = Tiles.Count(x => x.Flipped && !IsCivilized(x) && (x.gameObject.transform.position - flipped.transform.position).magnitude < 4);

        switch (unflipped.role)
        {
            case Role.DeadForrest:
                var amount = Tiles.Count(x => x.Flipped && x.role == Role.LivingForrest);
                GivePoints(tileSettings.pDeadForrestToLivingForest * amount, PointType.Natural);
                unflipped.role = Role.LivingForrest;
                break;
            case Role.RuinedVillage:
                if (civilizedTilesInRadius3 > 0)
                {
                    // Ruined Village gets village if there is at least one civilized Tile up to three tiles away
                    unflipped.ActivateByTag("Village");
                    var flippedAround = Tiles.Count(x => x.Flipped && (x.transform.position - flipped.gameObject.transform.position).magnitude < 2);
                    GivePoints(tileSettings.pRuinedVillageToVillage * flippedAround, PointType.Civilized);
                    unflipped.role = Role.Village;
                }
                else
                {
                    // Otherwise it will be grassland
                    unflipped.ActivateByTag("Grassland");
                    GivePoints(tileSettings.pRuinedVillageToGrasslands, PointType.Natural);
                    unflipped.role = Role.Grasslands;
                }

                break;
            case Role.CrackedSavanna:
                var villageAmountNextElt = Tiles.Count(x => x.Flipped && x.role == Role.Village && (x.gameObject.transform.position - flipped.transform.position).magnitude < 2);
                if (villageAmountNextElt > 0)
                {
                    // If there is at least one village next to the current tile make it into a sheep meadow
                    unflipped.ActivateByTag("Meadow");
                    GivePoints(tileSettings.pCrackedSavannaToSheepMeadows, PointType.Civilized);
                    unflipped.role = Role.SheepMeadows;
                }
                else
                {
                    // Otherwise turn it into Grassland
                    unflipped.ActivateByTag("Grassland");
                    GivePoints(tileSettings.pCrackedSavannaToGrassland, PointType.Natural);
                    unflipped.role = Role.Grasslands;
                }

                break;
            case Role.EvilMountain:
                var villagesInRange = Tiles.Count(x => x.Flipped && x.role == Role.Village && (x.gameObject.transform.position - flipped.transform.position).magnitude < 2);
                var citiesInRange = Tiles.Count(x => x.Flipped && x.role == Role.City && (x.gameObject.transform.position - flipped.transform.position).magnitude < 4);

                if (villagesInRange + citiesInRange > 0)
                {
                    // If there is a village next to it or a city in range 3 turn it into a mine
                    unflipped.ActivateByTag("Mine");
                    GivePoints(tileSettings.pEvilMountainToMine * villagesInRange + tileSettings.pEvilMountainToMine * 3 * citiesInRange, PointType.Civilized);
                    unflipped.role = Role.Mine;
                }
                else
                {
                    var neighboringForest = Tiles.Count(x => (x.role == Role.DeadForrest || x.role == Role.LivingForrest) && (x.gameObject.transform.position - flipped.gameObject.transform.position).magnitude < 2);
                    // otherwise change it to a mountain
                    unflipped.ActivateByTag("Mountain");
                    GivePoints(tileSettings.pEvilMountainToBeautifulMountain * neighboringForest, PointType.Natural);
                    unflipped.role = Role.BeautifulMountain;
                }

                break;
            case Role.ScorchedEarth:
                var villagesAndCitiesInRange = Tiles.Count(x => x.Flipped && (x.role == Role.Village || x.role == Role.City) && (x.gameObject.transform.position - flipped.transform.position).magnitude < 2);

                if (villagesAndCitiesInRange > 0)
                {
                    unflipped.ActivateByTag("Farmland");
                    GivePoints(tileSettings.pScorchedEarthToFarmlands, PointType.Civilized);
                    unflipped.role = Role.Farmland;
                }
                else
                {
                    unflipped.ActivateByTag("Flower");
                    GivePoints(tileSettings.pScorchedEarthToFlowers, PointType.Natural);
                    unflipped.role = Role.Flower;
                }

                break;
        }
    }

    bool IsCivilized(Tile tile)
    {
        return
            tile.role == Role.City ||
            tile.role == Role.Mine ||
            tile.role == Role.Village ||
            tile.role == Role.SheepMeadows ||
            tile.role == Role.Farmland ||
            tile.role == Role.Mine;
    }

    void GivePoints(int amount, PointType type)
    {
        var point = pointPopup.GetComponent<Point>();
        point.isNature = type == PointType.Natural;
        point.amount = amount;
        point.transform.position = CurrentlyHoveredTile.transform.position;
        Instantiate(point);

        if (type == PointType.Civilized)
        {
            score.civilizedPoints += amount;
        }
        else
        {
            score.naturePoints += amount;
        }
    }

    private void MakeVisibleAroundTile(Tile tile, float amount = 2.2f)
    {
        foreach (var t in Tiles.Where(x => (x.gameObject.transform.position - tile.transform.position).magnitude < amount))
        {
            t.Visible = true;
        }
    }

    private void HandleGameFinished()
    {
        if (!Tiles.All(x => x.Flipped || x.role == Role.Border)) return;
        score.isGameFinished = true;
        var s = Math.Min(score.civilizedPoints, score.naturePoints);
        score.highscore = Math.Max(score.highscore, s);
    }
}

[Serializable]
public class TileConfig
{
    public List<GameObject> Prefabs;
    public Role Role;
    public int probability;
}

enum PointType
{
    Natural,
    Civilized
}