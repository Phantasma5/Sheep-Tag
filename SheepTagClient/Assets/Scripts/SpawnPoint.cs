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
    private void OnValidate()
    {
        if (UnityEditor.EditorApplication.isPlaying)
        {
            if (Type == SpawnType.SHEEP && !sheepSpawns.Contains(this))
            {
                dogSpawns.Remove(this);
                sheepSpawns.Add(this);
            }
            if (Type == SpawnType.DOG && !dogSpawns.Contains(this))
            {
                sheepSpawns.Remove(this);
                dogSpawns.Add(this);
            }
        }
    }
#endif
}
