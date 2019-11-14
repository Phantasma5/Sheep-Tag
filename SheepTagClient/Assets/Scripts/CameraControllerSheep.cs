using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerSheep : MonoBehaviour
{
    [HideInInspector] private Vector3 camStartPos;
    [HideInInspector] private Vector3 camDragDif;
    [HideInInspector] private Vector3 camOrigin;
    private void Update()
    {
        if (!GetComponent<NetworkSync>().owned)
        {
            return;
        }
        CameraInput();
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
}
