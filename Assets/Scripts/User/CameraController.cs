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

    public float moveSpeed = 7.5f;
    public float speedMult = 1.5f;
    public float dragSpeed = 2f;
    public float zoomSpeed = 2f;
    public float minZoom = 4f;
    public float maxZoom = 10f;

    private Vector3 dragOrigin;
    private Vector2Int currentChunkPosition;


    void Update()
    {
        Movement();
    }

    void Movement()
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
}
