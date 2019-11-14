using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnLocalSpawn : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] components;

    public void LocalSpawn()
    {
        foreach(MonoBehaviour component in components)
        {
            component.enabled = true;
        }
    }
}
