using UnityEngine;

[CreateAssetMenu(fileName = "TileSettings", menuName = "ScriptableObjects/TileSettings", order = 1)]
public class TileSettings : ScriptableObject
{
    public float darkeningOnHoverAmount = 2;

    public int pDeadForrestToLivingForest = 1;
    public int pRuinedVillageToVillage = 1;
    public int pRuinedVillageToGrasslands = 1;
    public int pCrackedSavannaToSheepMeadows = 1;
    public int pCrackedSavannaToGrassland = 1;
    public int pEvilMountainToMine = 1;
    public int pEvilMountainToBeautifulMountain = 1;
    public int pScorchedEarthToFarmlands = 1;
    public int pScorchedEarthToFlowers = 1;
}