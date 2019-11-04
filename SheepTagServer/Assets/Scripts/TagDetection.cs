using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagDetection : MonoBehaviour
{
    private TagServer tagServer;
    private ServerNetwork serverNetwork;
    private void Start()
    {
        tagServer = GetComponent<TagServer>();
        serverNetwork = GetComponent<ServerNetwork>();
    }
    void Update()
    {
        foreach (var netObjOuter in serverNetwork.networkObjects)
        {
            if (netObjOuter.Value.it)//find a dog
            {
                foreach (var netObjInner in serverNetwork.networkObjects)//compare each person's position to that dog's position
                {
                    if (2 > Vector3.Distance(netObjOuter.Value.position, netObjInner.Value.position)
                    && netObjOuter.Value.networkId != netObjInner.Value.networkId
                    && !netObjInner.Value.it)
                    {
                        serverNetwork.CallRPC("Capture", UCNetwork.MessageReceiver.AllClients, netObjInner.Value.networkId);
                    }
                }
            }
        }
    }//end update
    public void GetIt()
    {
        RandomIt();//Placeholder
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
