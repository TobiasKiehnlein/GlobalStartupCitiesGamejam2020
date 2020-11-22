using System;
using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int X;
    public int Y;
    public Vector3 DestinationPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    private bool _flipped;
    private bool _visible;
    private float _animationSpeed = 10;
    private SpriteMask _mask;

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

    public bool Visible
    {
        get => _visible;
        set
        {
            _visible = value;
            if (_mask != null)
            {
                _mask.enabled = !value;
            }
        }
    }

    public TileSettings tileSettings;
    public LineRenderer lineRenderer;
    public MapGenerator mapGenerator;
    public Guid ID;
    public Role role = Role.None;

    private SpriteRenderer[] _spriteRenderer;
    private Color _color;
    private Camera _camera;
    private bool _dragging;
    private Animator _animator;

    private void Start()
    {
        _mask = GetComponent<SpriteMask>();
        _camera = Camera.main;
        _spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        ID = Guid.NewGuid();
        Visible = (mapGenerator.Tiles.First(x => x.X == 0 && x.Y == 0).gameObject.transform.position - transform.position).magnitude < 2.2f;
        Flipped = X == 0 && Y == 0;
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
        if (role == Role.Border) return;
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
        if (role == Role.Border) return;
        _dragging = true;
        mapGenerator.CurrentlyDraggedTile = this;
        if (lineRenderer == null || !_dragging) return;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);
    }

    private void OnMouseUp()
    {
        if (role == Role.Border) return;
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
        if (role == Role.Border) return;
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
            spriteRenderer.color = new Color(1 / tileSettings.darkeningOnHoverAmount, 1 / tileSettings.darkeningOnHoverAmount, 1 / tileSettings.darkeningOnHoverAmount);
        }
    }

    private void OnMouseExit()
    {
        if (_spriteRenderer.Length < 1 || role == Role.Border) return;
        foreach (var spriteRenderer in _spriteRenderer)
        {
            spriteRenderer.color = Color.white;
        }
    }

    public void ActivateByTag(string tag)
    {
        foreach (var componentsInChild in gameObject.GetComponentsInChildren<Transform>())
        {
            if (componentsInChild.gameObject.tag.Contains(tag) || componentsInChild.gameObject.tag.Contains("Dark"))
            {
                componentsInChild.gameObject.SetActive(true);
            }
            else
            {
                componentsInChild.gameObject.SetActive(false);
            }
        }
    }
}

public enum Role
{
    None,
    Village,
    Savanna,
    Mountain,
    ScorchedEarth,
    Border
}