using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorData : MonoBehaviour
{
    [SerializeField] TagServer server;

    [SerializeField] ServerNetwork.NetworkObject[] mirror;

    // Update is called once per frame
    void Update()
    {
        if (server)
        {
            mirror = new ServerNetwork.NetworkObject[server.serverNet.networkObjects.Values.Count];
            int i = 0;
            foreach (var netObj in server.serverNet.networkObjects)
            {
                mirror[i] = netObj.Value;

                i++;
            }
        }
    }
}
