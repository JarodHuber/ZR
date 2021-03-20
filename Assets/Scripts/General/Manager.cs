using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using System;
using System.Linq;
using Object = UnityEngine.Object;

[RequireComponent(typeof(SpawnManager), typeof(EnemyManager), typeof(ObjectSpawnManager))]
[RequireComponent(typeof(FogManager))]
public class Manager : MonoBehaviour
{
    [HideInInspector]
    public ObjectSpawnManager objectSpawnManager;
    [HideInInspector]
    public SpawnManager spawnManager;
    [HideInInspector]
    public EnemyManager enemyManager;
    [HideInInspector]
    public FogManager fogManager;

    public static readonly float HolsterTargetDistance = .2f;

    public bool Paused = true;

    [Tooltip("objects in the scene that can push the pause button likely just the player's hands")]
    public List<GameObject> stuffThatCanPushPauseButton = new List<GameObject>();
    [Tooltip("Button events from the ButtonEventHandler prefab")]
    public ButtonEvents buttonEvents;
    [Tooltip("the player prefab instance in the scene")]
    public GameObject Player;
    [Tooltip("the pause menu prefab")]
    public GameObject pauseMenu;
    [Tooltip("the player's center eye anchor in the player prefab instance in the scene")]
    public Transform playerPos;


    public Player player;

    OVRPlayerController playerController;
    PushableButton pushableButton;
    GameObject instance;

    bool lockPause = false, pastPause = false;

    private void Awake()
    {
        player = new Player(playerPos, Player.GetComponent<PlayerHealth>(), Player.transform);
        playerController = Player.GetComponent<OVRPlayerController>();
        objectSpawnManager = GetComponent<ObjectSpawnManager>();
        enemyManager = GetComponent<EnemyManager>();
        spawnManager = GetComponent<SpawnManager>();
        fogManager = GetComponent<FogManager>();

        fogManager.player = player;
        enemyManager.player = player;
        enemyManager.spawnManager = spawnManager;
        spawnManager.enemyManager = enemyManager;
        objectSpawnManager.enemyManager = enemyManager;
    }

    private void Start()
    {
        if (Paused && !lockPause)
        {
            StartPause(true);
        }
    }

    private void Update()
    {
        if(Input.GetButtonDown("b6"))
            Paused = true;

        if (Paused && !lockPause)
        {
            StartPause();
        }
        else if (!Paused)
        {
            EndPause();
        }
        if (Paused)
        {
            pauseMenu.transform.position = SetPausePos();
        }

        if(Paused != pastPause)
        {
            player.playerHealth.Paused = Paused;
            enemyManager.Paused = Paused;
            spawnManager.Paused = Paused;
            objectSpawnManager.Paused = Paused;
            pastPause = Paused;
        }
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    public void StartGame()
    {
        Paused = false;
        objectSpawnManager.SpawnObjects();
    }

    /// <summary>
    /// Sets the position for the pause menu to spawn
    /// </summary>
    /// <returns>Returns the Vector3 position to spawn the pause menu at</returns>
    Vector3 SetPausePos()
    {
        Vector3 returnBoy = playerController.CameraRig.trackingSpace.position;
        returnBoy.y = Player.transform.position.y;
        return returnBoy;
    }

    /// <summary>
    /// Opens the pause menu
    /// </summary>
    /// <param name="start">whether pause menu is opening on Start()</param>
    void StartPause(bool start = false)
    {
        Vector3 playerCenter = SetPausePos();

        playerController.EnableLinearMovement = false;
        instance = Instantiate(pauseMenu, playerCenter, Player.transform.rotation);
        instance.transform.LookAt(playerCenter);
        pushableButton = instance.GetComponentInChildren<PushableButton>();

        if(start)
            pushableButton.ButtonPress.AddListener(delegate { buttonEvents.StartGame(this); });
        else
            pushableButton.ButtonPress.AddListener(delegate { buttonEvents.PauseAlter(this); });

        pushableButton.stuffThatCanPushThisButton = stuffThatCanPushPauseButton;
        lockPause = true;
    }
    /// <summary>
    /// Closes the pause menu
    /// </summary>
    void EndPause()
    {
        playerController.EnableLinearMovement = true;
        lockPause = false;
        pushableButton.ButtonPress.RemoveAllListeners();
        Destroy(instance);
    }
}

public class Player
{
    public Transform transform;
    public PlayerHealth playerHealth;

    Transform transformForCenter;

    public Player(Transform transform, PlayerHealth playerHealth, Transform transformForCenter)
    {
        this.transform = transform;
        this.playerHealth = playerHealth;
        this.transformForCenter = transformForCenter;
    }
    
    /// <summary>
    /// Determines proper player center deprecated
    /// </summary>
    /// <returns>Returns the Vector3 player center</returns>
    public Vector3 playerCenter()
    {
        Vector3 centerPos = transform.position;
        centerPos.y = transformForCenter.position.y;

        return centerPos;
    }
}

public class Enemy
{
    public Transform transform;
    public NavMeshAgent agent;
    public AudioSource audioSource;
    public EnemyManager enemyManager;
    public Timer attackTimer;
    public int health, damage;
    public float speed, reach;
    public GameObject bullet;
    public float bulletSpeed;
    public bool isRanged;

    public Timer soundTimer;

    public float[] soundLock, attackSoundLock;

    public Enemy(Transform transform, NavMeshAgent agent, AudioSource audioSource, int soundCount, int attackSoundCount, EnemyManager enemyManager, int health, int damage, float speed, float attackDelay, float reach, bool isRanged, GameObject bullet, float bulletSpeed, float soundDelay)
    {
        this.transform = transform;
        this.agent = agent;
        this.audioSource = audioSource;
        this.enemyManager = enemyManager;
        this.health = health;
        this.damage = damage;
        this.speed = speed;
        attackTimer = new Timer(attackDelay);
        this.reach = reach;
        this.bullet = bullet;
        this.bulletSpeed = bulletSpeed;
        this.isRanged = isRanged;
        soundTimer = new Timer(soundDelay);

        soundLock = new float[soundCount];
        for (int x = 0; x < soundCount; x++)
            soundLock[x] = 1;
        attackSoundLock = new float[attackSoundCount];
        for (int x = 0; x < attackSoundCount; x++)
            attackSoundLock[x] = 1;
    }

    /// <summary>
    /// Attack the player
    /// </summary>
    /// <param name="player">Player to attack</param>
    public void Attack(Player player)
    {
        Vector3 targetPostition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(targetPostition);

        if (attackTimer.Check())
        {
            player.playerHealth.TakeDamage(attackSoundToPlay(), damage);
        }
    }

    public int attackSoundToPlay()
    {
        int val = 0;
        if (attackSoundLock.Contains(1))
        {
            val = MethodPlus.SkewedNum(attackSoundLock.Length, attackSoundLock);
            attackSoundLock[val] = 0;
        }
        else
        {
            for (int x = 0; x < attackSoundLock.Length; x++)
                attackSoundLock[x] = 1;

            val = MethodPlus.SkewedNum(attackSoundLock.Length, attackSoundLock);
            attackSoundLock[val] = 0;
        }

        return val;
    }


    /// <summary>
    /// Attack the player
    /// </summary>
    /// <param name="player">Player to attack</param>
    public void RangedAttack(Player player)
    {
        Vector3 targetPostition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(targetPostition);

        if (attackTimer.Check())
            Fire(player);
    }

    /// <summary>
    /// Have the enemy take damage
    /// </summary>
    /// <param name="damage">The amount of damage to take</param>
    public void TakeDamage(int damage)
    {
        health -= damage;
        
        if (health <= 0)
            enemyManager.KillEnemy(transform);
    }

    /// <summary>
    /// Shoot the bullet at the player
    /// </summary>
    /// <param name="player">Player to shoot</param>
    void Fire(Player player)
    {
        EnemyBullet enemyBullet = Object.Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<EnemyBullet>();

        enemyBullet.SetVelocity((player.playerCenter() - transform.position).normalized * bulletSpeed);
        enemyBullet.player = player;
        enemyBullet.speed = bulletSpeed;
    }
}

public class Velocity
{
    public float TimeStamp;
    public Vector3 PositionStamp;

    public Velocity(float timeStamp, Vector3 positionStamp)
    {
        this.TimeStamp = timeStamp;
        this.PositionStamp = positionStamp;
    }
}
