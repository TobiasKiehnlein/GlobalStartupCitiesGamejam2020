using UnityEngine;

[CreateAssetMenu(fileName = "Score", menuName = "ScriptableObjects/Score", order = 1)]
public class Score : ScriptableObject
{
    public int naturePoints = 0;
    public int civilizedPoints = 0;
    public int highscore = -1;
    public bool isGameFinished = false;
}