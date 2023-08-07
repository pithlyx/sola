using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    [System.Serializable]
    public class HotkeysSection
    {
        public KeyCode upKey = KeyCode.W;
        public KeyCode downKey = KeyCode.S;
        public KeyCode leftKey = KeyCode.A;
        public KeyCode rightKey = KeyCode.D;
        public KeyCode zoomInKey = KeyCode.Equals;
        public KeyCode zoomOutKey = KeyCode.Minus;
    }

    [SerializeField]
    private HotkeysSection hotkeys;

    [Header("Camera Settings")]
    public float moveSpeed = 7.5f;
    public float speedMult = 1.5f;
    public float minZoom = 4f;
    public float maxZoom = 10f;
    public float zoomSpeed = 2f;
    public bool canDrag = true;
    public float dragSpeed = 2f;

    private Vector3 dragOrigin;
    private Vector2Int currentChunkPosition;

    void Update()
    {
        Move();
        Drag();
    }

    void Move()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(hotkeys.upKey))
        {
            moveDirection += Vector3.up;
        }
        if (Input.GetKey(hotkeys.downKey))
        {
            moveDirection += Vector3.down;
        }
        if (Input.GetKey(hotkeys.leftKey))
        {
            moveDirection += Vector3.left;
        }
        if (Input.GetKey(hotkeys.rightKey))
        {
            moveDirection += Vector3.right;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDirection *= speedMult;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            Camera.main.orthographicSize = Mathf.Clamp(
                Camera.main.orthographicSize - scroll * zoomSpeed,
                minZoom,
                maxZoom
            );
        }
        if (Input.GetKey(hotkeys.zoomInKey))
        {
            Camera.main.orthographicSize = Mathf.Clamp(
                Camera.main.orthographicSize - zoomSpeed * Time.deltaTime,
                minZoom,
                maxZoom
            );
        }
        if (Input.GetKey(hotkeys.zoomOutKey))
        {
            Camera.main.orthographicSize = Mathf.Clamp(
                Camera.main.orthographicSize + zoomSpeed * Time.deltaTime,
                minZoom,
                maxZoom
            );
        }

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void Drag()
    {
        // Drag functionality
        if (Input.GetMouseButtonDown(0) && canDrag)
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0) || !canDrag)
            return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);
        transform.Translate(move, Space.World);
        dragOrigin = Input.mousePosition;
    }
}
