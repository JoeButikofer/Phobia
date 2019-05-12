using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class GameObjectEvent : UnityEvent<GameObject> { }

class SpawnManager : MonoBehaviour
{
    public string CountParameterName;
    public int defaultValue;

    public List<Transform> SpawnTransform;
    public GameObject ObjectPrefab;

    public Transform player;
    public float MinimumSpawnDistance;

    public GameObjectEvent NewObjectSpawned;

    private int count = 0;
    private List<GameObject> spawnedObjects;

    void Start()
    {
        spawnedObjects = new List<GameObject>();

        SessionParameters parameters = FindObjectOfType<SessionParameters>();
        count = defaultValue;
        if (parameters != null)
        {
            count = parameters.GetInt(CountParameterName, defaultValue, true);
        }

        StartCoroutine(CheckLoop());
    }

    void Spawn(Transform transform)
    {
        if (ObjectPrefab != null)
        {
            var obj = (GameObject)Instantiate(ObjectPrefab, transform.position, transform.rotation);
            obj.transform.SetParent(this.transform);
            spawnedObjects.Add(obj);

            NewObjectSpawned.Invoke(obj);
        }
        else
        {
            Debug.LogError("Spawn Manager : Object Prefab is null, can't spawn the object");
        }
    }

    Transform FindNextSpawnTransform()
    {
        var possibleTransform = new List<Transform>(SpawnTransform);

        if (player != null)
        {
            // Remove every transform too close of the player
            possibleTransform.RemoveAll(transform => Vector3.Distance(transform.position, player.position) < MinimumSpawnDistance);

            // Or too close of another phobic object
            for (int i = possibleTransform.Count - 1; i >= 0; --i)
            {
                var hits = Physics.OverlapSphere(possibleTransform[i].position, MinimumSpawnDistance);

                foreach (var hit in hits)
                {
                    if (hit.CompareTag("PhobicObject"))
                    {
                        possibleTransform.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        if (possibleTransform.Count > 0)
            return possibleTransform[UnityEngine.Random.Range(0, possibleTransform.Count)];
        else
            return null;
    }

    int CountMissing()
    {
        spawnedObjects.RemoveAll(item => item == null);
        return count - spawnedObjects.Count;
    }

    IEnumerator CheckLoop()
    {
        while(true)
        {
            int missing = CountMissing();

            for(int i = 0; i < missing; i++)
            {
                var transform = FindNextSpawnTransform();
                if (transform != null)
                {
                    Spawn(transform);
                }
            }

            yield return new WaitForSeconds(1);
        }
    }
}
