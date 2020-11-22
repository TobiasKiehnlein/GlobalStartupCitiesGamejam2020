using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    public bool mouseMovementEnabled = true;
    public int mapRadius = 5;
    public int seed = 11;
    public bool muted = false;
    public Lang lang = Lang.en;
}