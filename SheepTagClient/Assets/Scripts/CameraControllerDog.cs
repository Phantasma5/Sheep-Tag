using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerDog : MonoBehaviour
{
    void Update()
    {
        if(!GetComponent<NetworkSync>().owned)
        {
            return;
        }
        Vector3 pos = Camera.main.transform.position;
        pos.x = References.localPlayer.transform.position.x;
        pos.y = References.localPlayer.transform.position.y;
        Camera.main.transform.position = pos;
    }
}
