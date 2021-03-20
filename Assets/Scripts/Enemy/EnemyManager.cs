using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    [HideInInspector]
    public bool Paused = true;
    [HideInInspector]
    public SpawnManager spawnManager;
    [HideInInspector]
    public Player player;
    [HideInInspector]
    public List<Enemy> enemies = new List<Enemy>();

    [Tooltip("Things you want the enemies to recognise in-between the player")]
    public LayerMask playerMask;
    [Tooltip("bullet prefab for the enemy")]
    public GameObject enemyBulletFab;
    [Tooltip("delay between shots")]
    public float enemyFireSpeed = 10;
    [Tooltip("delay between enemy general sounds")]
    public float soundDelay = 1;
    [Tooltip("List of general sounds the enemies make")]
    public List<AudioClip> enemySounds;

    void Update()
    {
        if (Paused)
            foreach (Enemy e in enemies)
                e.agent.isStopped = true;
        else
            foreach (Enemy e in enemies)
                e.agent.isStopped = false;

        if (Paused) return;

        foreach (Enemy e in enemies)
            EnemyUpdate(e);
    }

    /// <summary>
    /// handles each enemy and updates what it's doing
    /// </summary>
    /// <param name="enemy">enemy to update</param>
    void EnemyUpdate(Enemy enemy)
    {
        PlayGeneralSound(enemy);
        if (player.transform.position != enemy.agent.destination)
            enemy.agent.SetDestination(player.transform.position);

        float distToPlayer = Vector3.Distance(enemy.transform.position, player.playerCenter());
        bool rayCast = Physics.Raycast(enemy.transform.position, (player.transform.position - enemy.transform.position).normalized, out RaycastHit hitInfo, distToPlayer, playerMask);
        Debug.DrawRay(enemy.transform.position, (player.transform.position - enemy.transform.position));

        if (!rayCast || hitInfo.transform.tag != "Obstacle")
        {
            enemy.agent.speed = enemy.speed;

            if (distToPlayer < enemy.reach)
            {
                float distanceToBeAt = Mathf.Max(1.25f, enemy.reach / 2);
                if (distToPlayer < distanceToBeAt)
                {
                    Vector3 pointToGoTo = player.transform.position + ((enemy.transform.position - player.transform.position).normalized * distanceToBeAt);
                    NavMeshHit hit;

                    if (NavMesh.Raycast(enemy.transform.position, pointToGoTo, out hit, playerMask))
                        pointToGoTo = hit.position;
                    enemy.agent.SetDestination(pointToGoTo);
                }

                if (distToPlayer == distanceToBeAt)
                    enemy.agent.isStopped = true;
                else if (distToPlayer != distanceToBeAt)
                    enemy.agent.isStopped = false;

                if (!enemy.isRanged)
                    enemy.Attack(player);
                else
                    enemy.RangedAttack(player);
            }
            else
                enemy.attackTimer.Reset();
        }
        else
            enemy.agent.speed = enemy.speed / 2;
    }

    /// <summary>
    /// adds the enemy to the enemies list allowing it to function
    /// </summary>
    /// <param name="e">Enemy prefab instance</param>
    /// <param name="health">how much health the enemy has</param>
    /// <param name="speed">how fast the enemy can move</param>
    /// <param name="reach">how far the enemy can attack from</param>
    /// <param name="isRanged">whether the enemy attacks are melee (false) or ranged (true)</param>
    /// <param name="attackDelay">time between attacks</param>
    /// <param name="damage">amount of damage the enemy does</param>
    public void SetEnemy(GameObject e, int health = 30, float speed = 2f, float reach = 2, bool isRanged = false, float attackDelay = 1, int damage = 1)
    {
        enemies.Add(new Enemy(e.transform, e.GetComponent<NavMeshAgent>(), e.GetComponent<AudioSource>(), enemySounds.Count, player.playerHealth.damageSounds.Count, this, health, damage, speed, attackDelay, reach, isRanged, enemyBulletFab, enemyFireSpeed, soundDelay));
    }

    /// <summary>
    /// kills the enemy
    /// </summary>
    /// <param name="enemy">transform to find the enemy</param>
    public void KillEnemy(Transform enemy)
    {
        enemies.Remove(FindEnemy(enemy));
        spawnManager.currentNumberOfEnemies--;
        Destroy(enemy.gameObject);
    }

    /// <summary>
    /// finds the enemy out of the list of enemies
    /// </summary>
    /// <param name="marker">transform marker to find the enemy</param>
    /// <returns>returns the enemy with the matching transform</returns>
    public Enemy FindEnemy(Transform marker)
    {
        Enemy enemy = null;

        foreach(Enemy e in enemies)
            if (e.transform == marker)
                enemy = e;

        return enemy;
    }

    /// <summary>
    /// plays one of the general enemy sounds
    /// </summary>
    /// <param name="enemy">enemy that emits the sound</param>
    void PlayGeneralSound(Enemy enemy)
    {
        if (!enemy.audioSource.isPlaying)
        {
            if (enemy.soundTimer.Check())
            {
                int val = 0;
                if (enemy.soundLock.Contains(1))
                {
                    val = MethodPlus.SkewedNum(enemySounds.Count, enemy.soundLock);
                    enemy.soundLock[val] = 0;
                }
                else
                {
                    for (int x = 0; x < enemy.soundLock.Length; x++)
                        enemy.soundLock[x] = 1;

                    val = MethodPlus.SkewedNum(enemySounds.Count, enemy.soundLock);
                    enemy.soundLock[val] = 0;
                }

                enemy.audioSource.clip = enemySounds[val];
                enemy.audioSource.Play();
            }
        }
        else
            enemy.soundTimer.Reset();
    }
}
