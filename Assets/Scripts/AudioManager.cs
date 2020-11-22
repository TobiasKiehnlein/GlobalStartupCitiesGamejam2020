using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private float timeToReach;
    [SerializeField] private GameSettings _gameSettings;
    private AudioMixerSnapshot _editor;

    private static AudioManager _instance;
    private AudioSource[] _audioSources;

    public static AudioManager Instance => _instance;


    private void Awake()
    {
        transform.parent = null;
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        _audioSources = GetComponents<AudioSource>();

        var main = mixer.FindSnapshot("Main");
        var nature = mixer.FindSnapshot("Nature");
        var bad = mixer.FindSnapshot("Bad");
    }

    private void Update()
    {
        foreach (var audioSource in _audioSources)
        {
            audioSource.mute = _gameSettings.muted;
        }
    }
}