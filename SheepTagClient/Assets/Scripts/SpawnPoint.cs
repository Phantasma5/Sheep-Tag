using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> sheepSpawns = new List<SpawnPoint>();
    private static List<SpawnPoint> dogSpawns = new List<SpawnPoint>();

    public static GameObject SpawnSheep()
    {
        return sheepSpawns[Random.Range(0, sheepSpawns.Count)].SpawnPrefab(RoundManager.playerSheepPrefab);
    }

    public static GameObject SpawnDog()
    {
        return dogSpawns[Random.Range(0, sheepSpawns.Count)].SpawnPrefab(RoundManager.playerDogPrefab);
    }

    private enum SpawnType
    {
        SHEEP,
        DOG
    }
    [SerializeField] private SpawnType Type;

    public GameObject SpawnPrefab(string prefabName)
    {
        return References.client.clientNet.Instantiate(prefabName, transform.position, transform.rotation);
    }

    private void Awake()
    {
        if (Type == SpawnType.SHEEP)
        {
            sheepSpawns.Add(this);
        }
        else if (Type == SpawnType.DOG)
        {
            dogSpawns.Add(this);
        }
    }

    private void OnDestroy()
    {
        sheepSpawns.Remove(this);
        dogSpawns.Remove(this);
    }

#if UNITY_EDITOR
    [SerializeField] private SpawnPoint[] sheeps;
    [SerializeField] private SpawnPoint[] dogs;

    private void Update()
    {
        sheeps = sheepSpawns.ToArray();
        dogs = dogSpawns.ToArray();
    }
#endif
}
