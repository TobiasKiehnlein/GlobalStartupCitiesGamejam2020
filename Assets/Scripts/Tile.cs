using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public int X;
    public int Y;
    public bool Visible;
    public bool Flipped;
    public TileSettings tileSettings;
    public LineRenderer lineRenderer;
    public MapGenerator mapGenerator;

    private SpriteRenderer _spriteRenderer;
    private Color _color;
    private Camera _camera;
    private bool _dragging;

    private void Start()
    {
        Visible = false;
        Flipped = false;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _camera = Camera.main;
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
    }

    private void OnMouseUp()
    {
        _dragging = false;
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }

    private void OnMouseEnter()
    {
        // TODO detect if I'm allowed
        if (mapGenerator.IsSwapAllowed(this) && _spriteRenderer != null)
        {
            Debug.Log("ALLOWED");
            _color = _spriteRenderer.color;
            _spriteRenderer.color = Color.yellow;
            return;
        }

        Debug.Log("not allowed");

        if (_spriteRenderer == null) return;
        _color = _spriteRenderer.color;
        _spriteRenderer.color = new Color(_color.r / tileSettings.darkeningOnHoverAmount, _color.g / tileSettings.darkeningOnHoverAmount, _color.b / tileSettings.darkeningOnHoverAmount);
    }

    private void OnMouseExit()
    {
        if (_spriteRenderer == null) return;
        _spriteRenderer.color = _color;
    }
}