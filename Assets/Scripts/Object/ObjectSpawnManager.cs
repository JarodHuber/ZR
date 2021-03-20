using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectSpawnManager : MonoBehaviour
{
    [HideInInspector]
    public bool Paused = true;
    [HideInInspector]
    public EnemyManager enemyManager;

    [Tooltip("parent for GrabbableObj")]
    public Transform grabParent;
    [Tooltip("total number of objects to spawn")]
    public int numOfObjects = 5;
    [Tooltip("radius that objects can spawn in")]
    public float spawnRadius = 50;
    [Tooltip("types of objects to spawn")]
    public List<GameObject> objsToSpawn = new List<GameObject>();

    [Header("Gun Offsets In Player Prefab Instance")]
    public Transform gunSnapOffsetL;
    public Transform gunSnapOffsetR;
    [Header("Pistol Offsets In Player Prefab Instance")]
    public Transform pistolSnapOffsetL;
    public Transform pistolSnapOffsetR;

    GrabbedSnap grabbedSnap;
    MeshRenderer mesh;
    GameObject instance;

    private void Start()
    {
        if (!Paused)
            SpawnObjects();
    }

    /// <summary>
    /// spawns the objects
    /// </summary>
    public void SpawnObjects()
    {
        for (int x = 0; x < numOfObjects; x++)
        {
            instance = Instantiate(objsToSpawn[x % objsToSpawn.Count], Vector3.zero, Quaternion.identity, grabParent);

            mesh = instance.GetComponentInChildren<MeshRenderer>();
            instance.transform.position = RandomNavMeshLocation(spawnRadius);

            instance.name = objsToSpawn[x % objsToSpawn.Count].name;
            ObjectType objectType = instance.GetComponent<GrabbableObj>().objectType;

            if (objectType == ObjectType.Gun)
            {
                grabbedSnap = instance.GetComponent<GrabbedSnap>();
                grabbedSnap.snapOffsetL = gunSnapOffsetL;
                grabbedSnap.snapOffsetR = gunSnapOffsetR;
                instance.GetComponent<Gun>().enemyManager = enemyManager;
            }
            else if(objectType == ObjectType.Pistol)
            {
                grabbedSnap = instance.GetComponent<GrabbedSnap>();
                grabbedSnap.snapOffsetL = pistolSnapOffsetL;
                grabbedSnap.snapOffsetR = pistolSnapOffsetR;
                instance.GetComponent<Gun>().enemyManager = enemyManager;
            }
        }
    }

    /// <summary>
    /// creates a random position on the navmesh
    /// </summary>
    /// <param name="radius">radius that objects can spawn in</param>
    /// <returns>Vector3 position on the navmesh + the half the height of the object</returns>
    Vector3 RandomNavMeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        Vector3 finalPosition = Vector3.zero;
        NavMeshHit hit;

        randomDirection += transform.position;

        if(NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
            finalPosition.y += mesh.bounds.extents.y;
        }

        return finalPosition;
    }
}
