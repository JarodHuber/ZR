using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [HideInInspector]
    public float speed = 20f;
    [HideInInspector]
    public Player player;

    Vector3 lastPos;

    private void Start()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        Vector3 targetDir = transform.position - lastPos;

        // The step size is equal to speed times frame time.
        float step = speed * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);

        // Move our position a step closer to the target.
        transform.rotation = Quaternion.LookRotation(newDir);
        lastPos = transform.position;

        if (Vector3.Distance(player.transform.position, transform.position) < 1)
            Hit();
    }

    public void SetVelocity(Vector3 velocity)
    {
        GetComponent<Rigidbody>().velocity = velocity;
    }

    private void Hit()
    {
        player.playerHealth.TakeDamage();
        Destroy(gameObject);
    }
}
