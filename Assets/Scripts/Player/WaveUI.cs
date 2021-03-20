using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveUI : MonoBehaviour
{
    public HandSide side;
    SpawnManager spawnManager;
    Text text;

    Player player;

    public static float debug;
    private void Start()
    {
        spawnManager = MethodPlus.GetComponentInObjectByTag<SpawnManager>("GameController");
        player = spawnManager.enemyManager.player;
        text = GetComponent<Text>();
    }

    void Update()
    {
        if (side == HandSide.Left)
        {
            text.text = "Wave: " + spawnManager.wave;
            debug = (Vector3.Angle(transform.forward, player.transform.forward));
        }
        else
            text.text = spawnManager.currentNumberOfEnemies + " Zombies nearby";

        if ((Vector3.Angle(transform.forward, player.transform.forward) > 80))
            text.enabled = false;
        else
            text.enabled = true;
    }
}
