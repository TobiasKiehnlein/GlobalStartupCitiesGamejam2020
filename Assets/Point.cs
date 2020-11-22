using System;
using TMPro;
using UnityEngine;

public class Point : MonoBehaviour
{
    [SerializeField] private float speed = 0;

    public int amount = 1;
    public bool isNature = false;

    private Vector3 _dest;
    private RectTransform _transform;
    private TMP_Text text;

    private void Start()
    {
        _dest = new Vector3(-1 * (isNature ? 2 : -2), 5, 0);
        _transform = GetComponent<RectTransform>();
        _transform.localScale = new Vector3(.2f, .2f, .2f);
        text = GetComponentInChildren<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _dest, Time.deltaTime * speed);
        _transform.localScale = Vector3.Lerp(_transform.localScale, Vector3.one, Time.deltaTime * speed);
        text.text = $"+{amount}";
        text.color = Color.Lerp(text.color, new Color(1, 1, 1, 0), Time.deltaTime * speed);
        if (text.color.a < .1f)
        {
            Destroy(gameObject);
        }
    }
}