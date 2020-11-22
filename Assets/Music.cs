using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField] private GameSettings _gameSettings;

    public void SetMusic(bool on)
    {
        _gameSettings.muted = !on;
    }
}