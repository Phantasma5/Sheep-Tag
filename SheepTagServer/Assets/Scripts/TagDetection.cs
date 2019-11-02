using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagDetection : MonoBehaviour
{
    private TagServer tagServer;
    private ServerNetwork serverNetwork;
    private float cooldown;
    private void Start()
    {
        cooldown = Time.time;
        tagServer = GetComponent<TagServer>();
        serverNetwork = GetComponent<ServerNetwork>();
    }
    void Update()
    {
        foreach (var netObjOutter in serverNetwork.networkObjects)
        {
            Debug.Log("enter foreach");
            if (netObjOutter.Value.it)//find a dog
            {
                Debug.Log("Dog?");
                foreach (var netObjInner in serverNetwork.networkObjects)//compare each person's position to that dog's position
                {
                    Debug.Log("Inner");
                    if (2 > Vector3.Distance(netObjOutter.Value.position, netObjInner.Value.position)
                    && netObjOutter.Value.networkId != netObjInner.Value.networkId
                    && false != netObjInner.Value.it)
                    {
                        Debug.Log("If State");
                        if (cooldown < Time.time)//cooldown so it doesn't change who is it back and forth every frame.
                        {
                            Debug.Log("CallRPC");
                            serverNetwork.CallRPC("Capture", UCNetwork.MessageReceiver.AllClients, netObjInner.Value.networkId);
                            cooldown = Time.time + 2;
                        }
                    }
                }
            }
        }
    }//end update
    public void GetIt()
    {
        RandomIt();
        foreach (var netObj in serverNetwork.networkObjects)
        {
            if (netObj.Value.it)
            {
                serverNetwork.CallRPC("YouSheep", UCNetwork.MessageReceiver.AllClients, netObj.Value.networkId, false);
            }
            else
            {
                serverNetwork.CallRPC("YouSheep", UCNetwork.MessageReceiver.AllClients, netObj.Value.networkId, true);
            }
        }
    }//end GetIt()
    private void RandomIt()
    {
        bool makeIt = true;
        foreach (var netObj in serverNetwork.networkObjects)
        {
            makeIt = !makeIt;
            netObj.Value.it = makeIt;
        }
    }
    public void CheckIt()
    {
        bool dog = false;
        bool sheep = false;
        foreach (var netObj in serverNetwork.networkObjects)
        {
            if (netObj.Value.it)
            {
                dog = true;
            }
            if (!netObj.Value.it)
            {
                sheep = true;
            }
        }
        if (false == sheep)
        {
            serverNetwork.CallRPC("SideEmpty", UCNetwork.MessageReceiver.AllClients, -1, false);
        }
        if (false == dog)
        {
            serverNetwork.CallRPC("SideEmpty", UCNetwork.MessageReceiver.AllClients, -1, true);
        }
    }//end CheckIt()

    public void FreeTheSheep()
    {
        foreach (var netObj in serverNetwork.networkObjects)
        {
            if (!netObj.Value.it)
            {
                serverNetwork.CallRPC("Free", UCNetwork.MessageReceiver.AllClients, netObj.Value.networkId);
            }
        }
    }

}//end class
