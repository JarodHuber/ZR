using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBounds : MonoBehaviour
{
    [HideInInspector]
    public Bounds bounds;

    private void Awake()
    {
        bounds.center = transform.position;
        bounds.extents = transform.localScale / 2;
    }

    /// <summary>
    /// creates the location for the enmy to spawn at
    /// </summary>
    /// <returns>Vector3 location for enemy to spawn at</returns>
    public Vector3 SpawnLoc()
    {
        Vector3 randomSpot;
        randomSpot.x = Random.Range(bounds.min.x, bounds.max.x);
        randomSpot.y = bounds.center.y;
        randomSpot.z = Random.Range(bounds.min.z, bounds.max.z);

        return randomSpot;
    }
}
