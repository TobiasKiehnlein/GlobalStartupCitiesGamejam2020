using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class ScoreMover : MonoBehaviour
{
    [SerializeField] private Score _score;
    [SerializeField] private float speed = 1;

    // Update is called once per frame
    void Update()
    {
        if (!_score.isGameFinished) return;
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, Screen.height / 2, 0), Time.deltaTime * speed);
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(2f, 2f, 2f), Time.deltaTime * speed);
    }
}