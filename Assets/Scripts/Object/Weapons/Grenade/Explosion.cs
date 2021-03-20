using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Explosion : MonoBehaviour
{
    [Tooltip("Damage grenade does")]
    public int damage = 30;
    [Tooltip("Lifetime of explosion effect")]
    public float lifeTime = 1;
    [Header("min and max collider size")]
    public float minSize = .1f;
    public float maxSize = 2.5f;

    SphereCollider col;
    Timer growTimer;
    EnemyManager enemyManager;

    private void Start()
    {
        enemyManager = MethodPlus.GetComponentInObjectByTag<EnemyManager>("GameController");
        col = GetComponent<SphereCollider>();
        growTimer = new Timer(lifeTime);
    }

    private void Update()
    {
        if(Vector3.Distance(enemyManager.player.transform.position, transform.position) < col.radius)
        {
            enemyManager.player.playerHealth.GrenadeDamage();
        }

        if (!growTimer.Check(false))
        {
            col.radius = Mathf.Lerp(minSize, maxSize, growTimer.PercentComplete()*2);
        }
        else
        {
            col.radius = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            enemyManager.FindEnemy(other.transform.parent).TakeDamage(damage);
        }
    }
}
