using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float minZoom = 3;
    [SerializeField] private float maxZoom = 10;
    [SerializeField] private float zoomSpeed = 1;
    [SerializeField] private float movementSpeed = 1;
    [SerializeField] private int edgeMovementOffset = 100;

    private Camera _camera;
    private int _size = 10;

    private void Start()
    {
        _camera = GetComponent<Camera>();

        var mapGenerator = FindObjectOfType<MapGenerator>();
        if (mapGenerator != null)
            _size = mapGenerator.GetSize();
    }

    // Update is called once per frame
    void Update()
    {
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, minZoom, maxZoom);
        transform.Translate(Input.GetAxis("Horizontal") * movementSpeed / 10, Input.GetAxis("Vertical") * movementSpeed / 10, 0);
        var mousePosition = Input.mousePosition;
        if (mousePosition.y > Screen.height - edgeMovementOffset)
        {
            transform.Translate(0, movementSpeed / 10, 0);
        }
        else if (mousePosition.y < edgeMovementOffset)
        {
            transform.Translate(0, -movementSpeed / 10, 0);
        }

        if (mousePosition.x > Screen.width - edgeMovementOffset)
        {
            transform.Translate(movementSpeed / 10, 0, 0);
        }
        else if (mousePosition.x < edgeMovementOffset)
        {
            transform.Translate(-movementSpeed / 10, 0, 0);
        }

        Vector2 cameraPosition = Vector2.ClampMagnitude(new Vector2(transform.position.x, transform.position.y), _size / 2.5f);
        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, transform.position.z);
    }
}