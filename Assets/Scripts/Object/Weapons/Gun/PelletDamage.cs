using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelletDamage : MonoBehaviour
{
    [HideInInspector]
    public float speed = 20f;
    [HideInInspector]
    public EnemyManager enemyManager;
    [HideInInspector]
    public Vector3 lastPos;
    [HideInInspector]
    public GameObject bloodSplatter;
    [HideInInspector]
    public int damage = 1;

    Enemy enemy;

    Vector3 targetDir;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            enemy = enemyManager.FindEnemy(collision.transform);
            enemy.TakeDamage(damage);
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.forward, out hit, GetComponent<MeshRenderer>().bounds.extents.z);
            GameObject spawnedDecal = Instantiate(bloodSplatter, hit.point, Quaternion.LookRotation(hit.normal));
            spawnedDecal.transform.SetParent(hit.collider.transform);
            Destroy(gameObject);
        }
        else
            Destroy(gameObject);
    }
}
