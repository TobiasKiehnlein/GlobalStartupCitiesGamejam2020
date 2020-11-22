using System;
using TMPro;
using UnityEngine;

public class ShowOnGameover : MonoBehaviour
{
    [SerializeField] private Score _score;
    private TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_score.isGameFinished)
        {
            text.color = Color.Lerp(text.color, Color.white, Time.deltaTime);
        }
    }
}