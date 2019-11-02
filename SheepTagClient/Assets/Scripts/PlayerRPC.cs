using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRPC : MonoBehaviour
{
    [HideInInspector] private PlayerInput playerInput;
    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        References.client.gameObject.GetComponent<ClientNetwork>().CallRPC("GetIt", UCNetwork.MessageReceiver.ServerOnly, -1);
    }
    public void YouSheep(bool aSheep)
    {
        if(null == playerInput)
        {
            return;
        }

        //TODO: Change Sprite
        if(aSheep)
        {
            playerInput.speed = 5;
        }
        else
        {
            playerInput.speed = 10;
        }
    }
    public void Rescue()
    {
        References.client.gameObject.GetComponent<ClientNetwork>().CallRPC("FreeTheSheep", UCNetwork.MessageReceiver.ServerOnly, -1);
    }
    public void Capture()
    {
        playerInput.captured = true;
        References.player.transform.position = Vector3.zero;
    }
    public void Free()
    {
        if(!playerInput.captured)
        {
            return;
        }
        playerInput.captured = false;
        References.player.transform.position = new Vector3(Random.Range(-10,10), Random.Range(-10, 10), 0);
    }
}
