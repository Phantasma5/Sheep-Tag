using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NetworkedRoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        NetworkSync sync = GetComponent<NetworkSync>();
        if(sync)
        {
            sync.getDataFunction = GetSyncData;
            sync.setDataFunction = SetSyncData;
        }
    }

    public void GetSyncData(ref BinaryWriter aBinWriter)
    {

    }

    public void SetSyncData(ref BinaryReader aBinReader)
    {

    }
}
