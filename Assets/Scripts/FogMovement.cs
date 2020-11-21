using System;
using UnityEngine;

public class FogMovement : MonoBehaviour
{
    [SerializeField] private int width = 2;
    [SerializeField] private float movementSpeed = 1;
    [SerializeField] private float verticalAmount = 0.5f;
    [SerializeField] private float musicBpm = 110;

    private float _x;
    private float _y;
    private float _spriteWidth;

    void Start()
    {
        _x = transform.position.x;
        _y = transform.position.y;
        _spriteWidth = GetComponent<SpriteRenderer>().size.x;
        if (FindObjectsOfType<FogMovement>().Length < width)
        {
            var go = Instantiate(gameObject);
            go.transform.Translate(new Vector3(go.GetComponent<SpriteRenderer>().size.x, 0, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Time.deltaTime * movementSpeed * Vector3.left);
        transform.position = new Vector3(transform.position.x, _y + Mathf.Sin((float) ((Time.time / 1000) * musicBpm * 2 * Math.PI / 0.6)) * verticalAmount, transform.position.z);
        if (transform.position.x < _x - _spriteWidth)
        {
            transform.position = new Vector3(_x, transform.position.y, transform.position.z);
        }
    }
}