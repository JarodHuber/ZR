using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public float speed = 20f;
    [HideInInspector]
    public EnemyManager enemyManager;

    /// <summary>
    /// Enemy blood splatter particle effect prefab
    /// </summary>
    public GameObject bloodSplatter;
    /// <summary>
    /// bullet lifetime
    /// </summary>
    public float lifeTime = 10f;
    /// <summary>
    /// amount of damage the bullet deals
    /// </summary>
    public int damage = 10;

    public bool spreadShot = true;
    public float spreadRad = 0;

    Vector3 lastPos;
    Vector3 targetDir;

    Enemy enemy;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        targetDir = transform.position - lastPos;

        // The step size is equal to speed times frame time.
        float step = speed * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
        //transform.LookAt(targetDir);

        // Move our position a step closer to the target.
        transform.rotation = Quaternion.LookRotation(newDir);
        lastPos = transform.position;
    }

    public void SetVelocity(Vector3 velocity, Vector3 lastPos)
    {
        if (!spreadShot)
        {
            this.lastPos = lastPos;
            GetComponent<Rigidbody>().velocity = velocity;
        }
        else
            Spread(transform, velocity, lastPos);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            enemy = enemyManager.FindEnemy(collision.transform);
            enemy.TakeDamage(damage);
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.forward, out hit, GetComponent<MeshRenderer>().bounds.extents.z);
            Instantiate(bloodSplatter, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Spread(Transform bullets, Vector3 velocity, Vector3 lastPos)
    {
        float spreadX = Random.Range(-1, 1);
        float spreadY = Random.Range(-1, 1);
        Vector3 spread = new Vector3(spreadX, spreadY, 0).normalized * spreadRad;
        Rigidbody rb = bullets.GetComponent<Rigidbody>();

        if (bullets.childCount != 0)
        {
            foreach (Transform t in bullets)
            {
                PelletDamage dmg = t.GetComponent<PelletDamage>();
                dmg.lastPos = lastPos;
                dmg.bloodSplatter = bloodSplatter;
                dmg.enemyManager = enemyManager;
                dmg.speed = speed;
                dmg.damage = Mathf.Clamp(damage / bullets.childCount, 1, damage);

                rb = t.GetComponent<Rigidbody>();
                spreadX = Random.Range(-1, 1);
                spreadY = Random.Range(-1, 1);
                spread = new Vector3(spreadX, spreadY, 0).normalized * spreadRad;

                rb.velocity = velocity + spread;
            }
        }
        else
        {
            this.lastPos = lastPos;
            rb.velocity = velocity + spread;
        }
    }
}
