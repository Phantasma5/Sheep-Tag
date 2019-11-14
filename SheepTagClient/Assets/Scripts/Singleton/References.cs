using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class References : MonoBehaviour
{
    public static References instance;
    public static GameObject localPlayer;
    public static TagClient client;

#if UNITY_EDITOR
    [SerializeField] private References _instance;
    [SerializeField] private GameObject _localPlayer;
    [SerializeField] private TagClient _client;
    
    private void Update()
    {
        _instance = instance;
        _localPlayer = localPlayer;
        _client = client;
    }
#endif

    private void Awake()
    {
        if(null != instance)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        //DontDestroyOnLoad(this.gameObject);
        FindReferences();
    }
    private void FindReferences()
    {
        client = GetComponent<TagClient>();
    }

    private void OnDestroy()
    {
        if(instance == this)
        {
            instance = null;
        }
    }
}
