using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Murderer : MonoBehaviour
{
    public EnemyManager enemyManager;

    public void Murder()
    {
        enemyManager.enemies[0].TakeDamage(enemyManager.enemies[0].health);
    }
}
