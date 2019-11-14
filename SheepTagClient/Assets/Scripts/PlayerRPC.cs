using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRPC : MonoBehaviour
{
    [HideInInspector] private PlayerInput playerInput;
    public bool sheep = true;
    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        //References.client.gameObject.GetComponent<ClientNetwork>().CallRPC("GetIt", UCNetwork.MessageReceiver.ServerOnly, -1);
    }
    public void YouSheep(bool aSheep)
    {
        if (null == playerInput)
        {
            return;
        }

        if (aSheep)
        {
            playerInput.speed = 2;
            sheep = true;
            //GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("SheepSprite");
        }
        else
        {
            playerInput.speed = 5;
            sheep = false;
            //GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("DogSprite");
        }
    }
    public void Rescue()
    {
        if (sheep && !playerInput.captured)
        {
            References.client.gameObject.GetComponent<ClientNetwork>().CallRPC("FreeTheSheep", UCNetwork.MessageReceiver.ServerOnly, -1);
        }
    }
    public void Capture()
    {
        playerInput.captured = true;
        transform.position = Vector3.zero;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }
    public void Free()
    {
        if (!playerInput.captured)
        {
            return;
        }
        playerInput.captured = false;
        transform.position = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0);
    }

    public void SetCondition(string condition)
    {
        if (sheep == false)
            return;

        NetworkSync ns = GetComponent<NetworkSync>();
        if (ns)
        {
            References.client.clientNet.CallRPC("SetCondition", UCNetwork.MessageReceiver.ServerOnly, -1, new object[] { ns.GetId(), condition });
        }
    }
}
