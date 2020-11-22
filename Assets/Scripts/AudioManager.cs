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
    [SerializeField] private Score _score;
    private AudioMixerSnapshot _editor;

    private static AudioManager _instance;
    private AudioSource[] _audioSources;
    private AudioMixerSnapshot main;
    private AudioMixerSnapshot nature;
    private AudioMixerSnapshot bad;
    private AudioMixerSnapshot civ;

    private AudioMixerSnapshot active;

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

        main = mixer.FindSnapshot("Main");
        nature = mixer.FindSnapshot("Nature");
        bad = mixer.FindSnapshot("Bad");
        civ = mixer.FindSnapshot("Civ");

        mixer.updateMode = AudioMixerUpdateMode.Normal;
        bad.TransitionTo(timeToReach);
    }

    private void Update()
    {
        foreach (var audioSource in _audioSources)
        {
            audioSource.mute = _gameSettings.muted;
        }

        if (_score.naturePoints > _score.civilizedPoints && active != nature)
        {
            active = nature;
            nature.TransitionTo(timeToReach);
        }
        else if (_score.naturePoints < _score.civilizedPoints && active != civ)
        {
            active = civ;
            civ.TransitionTo(timeToReach);
        }
    }
}