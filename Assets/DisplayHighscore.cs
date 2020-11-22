using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayHighscore : MonoBehaviour
{
    [SerializeField] private Score _score;

    private TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        text.text = $"Highscore: {_score.highscore}";
    }

    // Update is called once per frame
    void Update()
    {
        if (_score.isGameFinished)
        {
            text.text = $"Highscore: {_score.highscore}";
        }
    }
}