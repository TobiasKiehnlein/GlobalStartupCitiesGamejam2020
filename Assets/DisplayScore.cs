using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayScore : MonoBehaviour
{
    [SerializeField] private Score Score;
    [SerializeField] private bool IsNatureScore;
    [SerializeField] private float shrinkingSpeed = 1;

    private TMP_Text _text;
    private RectTransform _transform;

    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<TMP_Text>();
        _transform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        var text = (IsNatureScore ? Score.naturePoints : Score.civilizedPoints).ToString("000");

        if (!text.Equals(_text.text))
        {
            _text.text = text;
            _transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }

        _transform.localScale = Vector3.Lerp(_transform.localScale, Vector3.one, Time.deltaTime * shrinkingSpeed);

        if (Score.isGameFinished)
        {
            if (IsNatureScore && Score.naturePoints > Score.civilizedPoints)
            {
                _text.fontStyle |= FontStyles.Strikethrough;
            }
            else if (!IsNatureScore && Score.naturePoints < Score.civilizedPoints)
            {
                _text.fontStyle |= FontStyles.Strikethrough;
            }

            if (IsNatureScore && Score.naturePoints < Score.civilizedPoints)
            {
                _text.fontStyle |= FontStyles.Bold;
            }
            else if (!IsNatureScore && Score.naturePoints > Score.civilizedPoints)
            {
                _text.fontStyle |= FontStyles.Bold;
            }
        }
    }
}