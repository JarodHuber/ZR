using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastBullet : MonoBehaviour
{
    [HideInInspector]
    public float speed = 20f;
    [HideInInspector]
    public EnemyManager enemyManager;

    [Tooltip("Enemy blood splatter particle effect prefab")]
    public GameObject bloodSplatter;
    [Tooltip("range of the bullet")]
    public float range = 50f;
    [Tooltip("amount of damage the bullet deals")]
    public int damage = 10;
    [Tooltip("number of bullets per shot")]
    public int bulletCount = 1;
    [Tooltip("bolt vs spread")]
    public bool spreadShot = true;
    [Tooltip("the radius of the spread (how far off the bullet can be)")]
    public float spreadRad = 0;

    /// <summary>
    /// the list of directions the bullets travel
    /// </summary>
    List<Vector3> directions = new List<Vector3>();

    public static string outWhatHit;

    public void SetVelocity(Vector3 velocity)
    {
        directions.Clear();

        if (!spreadShot)
            directions.Add(velocity);
        else
            Spread(velocity);

        CastEvent();
    }

    void Spread(Vector3 velocity)
    {
        float spreadX = Random.Range(-1, 1);
        float spreadY = Random.Range(-1, 1);
        Vector3 spread = new Vector3(spreadX, spreadY, 0).normalized * spreadRad;

        if (bulletCount != 1)
        {
            for (int x = 0; x<bulletCount; x++)
            {
                spreadX = Random.Range(-1, 1);
                spreadY = Random.Range(-1, 1);
                spread = new Vector3(spreadX, spreadY, 0).normalized * spreadRad;

                directions.Add(velocity + spread);
            }
        }
        else
            directions.Add(velocity + spread);
    }

    void CastEvent()
    {
        RaycastHit hit;
        if (directions.Count == 0)
            Debug.LogError("directions' not being added to");
        else if (directions.Count == 1)
        {
            if (Physics.Raycast(transform.position, directions[0], out hit, range, ~LayerMask.NameToLayer("Player")))
                HandleCollision(hit);
        }
        else
        {
            foreach(Vector3 v in directions)
                if (Physics.Raycast(transform.position, v, out hit, range, ~LayerMask.NameToLayer("Player")))
                    HandleCollision(hit);
        }

        Destroy(gameObject);
    }

    void HandleCollision(RaycastHit collision)
    {
        outWhatHit = collision.collider.gameObject.name;
        if (collision.collider.tag == "Enemy")
        {
            Enemy enemy = enemyManager.FindEnemy(collision.transform);
            enemy.TakeDamage(damage/bulletCount);

            GameObject decal = Instantiate(bloodSplatter, collision.point, Quaternion.LookRotation(collision.normal));
            decal.transform.SetParent(collision.transform);
        }
    }
}
