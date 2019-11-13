using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    #region References

    #endregion
    #region Variables
    [HideInInspector] public float speed = 5;
    [HideInInspector] private Vector3 camStartPos;
    [HideInInspector] private Vector3 camDragDif;
    [HideInInspector] private Vector3 camOrigin;
    /*[HideInInspector]*/ public bool captured = false;
    #endregion
    private void Awake()
    {
        if (GetComponent<NetworkSync>().owned)
        {
            References.player = this.gameObject;
        }
    }
    private void CameraInput()
    {
        if (Input.GetMouseButtonDown(2))
        {
            camStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(2))
        {
            camDragDif = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            Camera.main.transform.position = camStartPos - camDragDif;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Camera.main.transform.position = camOrigin;
            Camera.main.orthographicSize = 5;
        }
        float zoom = Input.GetAxis("Mouse ScrollWheel");
        if (zoom < 0f)
        {
            Camera.main.orthographicSize *= 1.1f;
        }
        if (zoom > 0f)
        {
            Camera.main.orthographicSize *= 0.9f;
        }
    }
    private void PlayerMovement()
    {
        if(captured)
        {
            return;
        }
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, Input.GetAxis("Vertical") * speed * Time.deltaTime, 0);
        transform.position += movement;

        //Check if the player is within 10 from origin box
        if(transform.position.x < 2 &&
            transform.position.x > -2 &&
            transform.position.y < 2 &&
            transform.position.y > -2)
        {
            GetComponent<PlayerRPC>().Rescue();
        }
    }
    void Update()
    {
        if (GetComponent<NetworkSync>().owned)
        {
            PlayerMovement();
            CameraInput();
        }
    }
}
