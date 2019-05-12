using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SpawnButton : OnTouch
{
    public GameObject spawnPrefab;

    public Transform spawnRangeBottomLeft;
    public Transform spawnRangeTopRight;

    protected override void Touch()
    {
        var spawnObject = Instantiate(spawnPrefab);
        var randomPosition = new Vector3(UnityEngine.Random.Range(spawnRangeBottomLeft.position.x, spawnRangeTopRight.position.x), UnityEngine.Random.Range(spawnRangeBottomLeft.position.y, spawnRangeTopRight.position.y), UnityEngine.Random.Range(spawnRangeBottomLeft.position.z, spawnRangeTopRight.position.z));

        spawnObject.transform.position = randomPosition;
        spawnObject.transform.SetParent(this.transform);
    }
}
