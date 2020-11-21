using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int X;
    public int Y;
    public bool Visible;
    public Vector3 DestinationPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    public bool test = true;
    private bool _flipped;
    private float _animationSpeed = 10;

    public bool Flipped
    {
        get => _flipped;
        set
        {
            _flipped = value;
            if (_animator != null && Flipped)
            {
                _animator.SetBool("IsFlipped", value);
            }
        }
    }

    public TileSettings tileSettings;
    public LineRenderer lineRenderer;
    public MapGenerator mapGenerator;
    public Guid ID;

    private SpriteRenderer[] _spriteRenderer;
    private Color _color;
    private Camera _camera;
    private bool _dragging;
    private Animator _animator;

    private void Start()
    {
        ID = Guid.NewGuid();
        _animator = GetComponent<Animator>();
        Visible = false;
        Flipped = X == 0 && Y == 0;
        _spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        _camera = Camera.main;
    }

    private void Update()
    {
        if (DestinationPosition.x != float.MaxValue)
        {
            transform.position = Vector3.Lerp(transform.position, DestinationPosition, Time.deltaTime * _animationSpeed);
            // transform.position = DestinationPosition;
            // DestinationPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        }
    }

    public void OnMouseDrag()
    {
        // if (_camera == null) return;
        // var mousePosition = Input.mousePosition;
        // mousePosition.z = 10;
        // Vector3 worldPosition = _camera.ScreenToWorldPoint(mousePosition);
        // transform.position = worldPosition;
        if (!_dragging) return;
        var mousePosition = Input.mousePosition;
        mousePosition.z = 10;
        Vector3 worldPosition = _camera.ScreenToWorldPoint(mousePosition);
        if (lineRenderer == null) return;
        lineRenderer.SetPosition(1, worldPosition);
    }

    private void OnMouseDown()
    {
        _dragging = true;
        mapGenerator.CurrentlyDraggedTile = this;
        if (lineRenderer == null || !_dragging) return;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);
    }

    private void OnMouseUp()
    {
        _dragging = false;
        mapGenerator.SwapTiles();
        mapGenerator.CurrentlyDraggedTile = null;
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }

    private void OnMouseEnter()
    {
        mapGenerator.CurrentlyHoveredTile = this;
        if (_spriteRenderer.Length < 1) return;

        if (mapGenerator.IsSwapAllowed(this))
        {
            foreach (var spriteRenderer in _spriteRenderer)
            {
                spriteRenderer.color = Color.yellow;
            }

            return;
        }

        foreach (var spriteRenderer in _spriteRenderer)
        {
            spriteRenderer.color = new Color(_color.r / tileSettings.darkeningOnHoverAmount, _color.g / tileSettings.darkeningOnHoverAmount, _color.b / tileSettings.darkeningOnHoverAmount);
        }
    }

    private void OnMouseExit()
    {
        if (_spriteRenderer.Length < 1) return;
        foreach (var spriteRenderer in _spriteRenderer)
        {
            spriteRenderer.color = Color.white;
        }
    }
}