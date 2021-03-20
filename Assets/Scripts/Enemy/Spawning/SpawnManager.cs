using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [HideInInspector]
    public bool Paused = true;
    [HideInInspector]
    public EnemyManager enemyManager;
    [HideInInspector]
    public List<EnemTime> nextEnemies = new List<EnemTime>();

    [Tooltip("locations the enemies can spawn in... is auto-set on start")]
    public List<SpawnBounds> spawnBounds = new List<SpawnBounds>();
    [Tooltip("types of enemies that can be spawned, enemy prefabs are added here")]
    public List<GameObject> enemyTypes = new List<GameObject>();
    [Tooltip("the minimum time in-between waves")]
    public float Delay = 10f;
    [Tooltip("Explosion marker for new wave")]
    public GameObject explosionFab;
    [Tooltip("where explosion for new wave marker is spawned")]
    public Transform explosionPoint;

    [Header("Do not touch below")]
    public SpawnStage stage = SpawnStage.Wait;

    List<SubWave> Wave = new List<SubWave>();
    List<SpawnBounds> spawnBoundsForCycle = new List<SpawnBounds>();
    public int wave, totalEnemiesForWave, currentNumberOfEnemies;

    AudioSource audioSource;
    float pauseTime = 0;
    bool lastVal = false;
    Timer waitTimer;

    float playerTime = 0;

    private void Start()
    {
        waitTimer = new Timer(Delay);
        audioSource = GetComponent<AudioSource>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("SpawnZone"))
            spawnBounds.Add(obj.GetComponent<SpawnBounds>());
        spawnBoundsForCycle = spawnBounds;
    }

    private void Update()
    {
        if (Paused && !lastVal)
        {
            pauseTime = Time.time;
            lastVal = true;
        }

        if (!Paused && lastVal)
        {
            pauseTime = Time.time - pauseTime;
            lastVal = false;

            if (wave > 0)
            {
                foreach (SubWave sw in Wave)
                    sw.timestamp += pauseTime;
            }
        }

        if (Paused) return;

        playerTime += Time.deltaTime;

        if (stage == SpawnStage.Wait)
            Wait();
        else if (stage == SpawnStage.Start)
            StartSpawn();
        else if (stage == SpawnStage.Play)
            Play();
    }

    /// <summary>
    /// Wait for the next wave
    /// </summary>
    public void Wait()
    {
        if (waitTimer.Check() && currentNumberOfEnemies <= totalEnemiesForWave / 2)
            stage = SpawnStage.Start;
    }

    /// <summary>
    /// Prepare the next wave for spawning
    /// </summary>
    public void StartSpawn()
    {
        wave++;
        Instantiate(explosionFab, explosionPoint.position, explosionFab.transform.rotation);
        totalEnemiesForWave = currentNumberOfEnemies;
        audioSource.Play();

        Wave.Add(new SubWave(wave, 0f, 0.5f, 6 + (int)(2.0f * (wave - 1)), enemyTypes[0], 0));
        if (wave > 6)
            for (int i = 0; i < wave / 6; i++)
                Wave.Add(new SubWave(wave, (i * 3) + 2, 0.75f, 1 + (int)(wave / 3), enemyTypes[1], 1));
        if (wave > 12)
            for (int i = 0; i < (wave / 6) - 1; i++)
                Wave.Add(new SubWave(wave, (i * 3) + 3, 1f, 1 + (int)(wave / 3), enemyTypes[2], 2));
        if (wave > 24)
            for (int i = 0; i < (wave / 6) - 2; i++)
                Wave.Add(new SubWave(wave, (i * 3) + 3, 1f, 1 + (int)(wave / 3), enemyTypes[0], 0));

        foreach (SubWave sw in Wave)
        {
            sw.ResetTimestamp(); //We need this so that the timestamps are correct
            totalEnemiesForWave += sw.amount;
            nextEnemies.AddRange(sw.GetEnemies());
        }

        currentNumberOfEnemies = totalEnemiesForWave;
        stage = SpawnStage.Play;
    }

    /// <summary>
    /// Spawn the enemies
    /// </summary>
    public void Play()
    {
        for (int i = 0; i < Wave.Count; i++)
        {
            if (Wave[i].IsTime())
            {

                GameObject temp = Wave[i].Spawn();
                if (temp != null)
                {
                    GameObject e = Instantiate(temp, spawnBoundsForCycle[0].SpawnLoc(), transform.rotation);

                    if(Wave[i].enemyType < enemyTypes.Count)
                    {
                        if (Wave[i].enemyType == 0)
                            enemyManager.SetEnemy(e);
                        else if (Wave[i].enemyType == 1)
                            enemyManager.SetEnemy(e, 20, 6f);
                        else if (Wave[i].enemyType == 2)
                            enemyManager.SetEnemy(e, 15, 1.5f, 20, true, 3f);
                    }
                    else
                        enemyManager.SetEnemy(e);

                    //Cycle the spawnBounds
                    SpawnBounds spawnTemp = spawnBoundsForCycle[0];

                    for (int j = 1; j < spawnBoundsForCycle.Count; j++)
                        spawnBoundsForCycle[(j - 1) % spawnBoundsForCycle.Count] = spawnBoundsForCycle[j];

                    spawnBoundsForCycle[spawnBoundsForCycle.Count - 1] = spawnTemp;

                    nextEnemies.RemoveAt(nextEnemies.FindIndex(ne => ne.enemy.Equals(temp))); //Lambdas yay
                }
            }
            if (Wave[i].IsDone()) Wave.Remove(Wave[i]);
        }

        if (Wave.Count == 0)
        {
            print("wave finished"); //Signal the end of the wave
            stage = SpawnStage.Wait;

            nextEnemies.Clear();
            spawnBoundsForCycle = spawnBounds;
        }
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("wave", wave);
        PlayerPrefs.SetInt("timeSurvived", (int)playerTime);
    }
}

public class SubWave
{
    public int wave;
    public int enemyType;

    private GameObject enemy;

    private bool isStarted = false;

    public int amount;

    private float time;
    private float spacing;
    public float timestamp;

    public SubWave(int wave, float time, float spacing, int amount, GameObject enemy, int enemyType)
    {
        this.wave = wave;
        this.time = time;
        this.spacing = spacing;
        this.amount = amount;
        this.enemy = enemy;
        this.enemyType = enemyType;

        this.timestamp = Time.time;
    }

    /// <summary>
    /// Spawn the next enemy
    /// </summary>
    /// <returns>Returns the enemy to spawn</returns>
    public GameObject Spawn()
    {
        if (amount == 0) return null;
        if (timestamp + (spacing) < Time.time)
        {
            timestamp = Time.time;
            amount--;
            return enemy;
        }
        else return null;
    }

    /// <summary>
    /// Tell when it's time to spawn the next Subwave
    /// </summary>
    /// <returns>Returns true when next Subwave is ready to start spawning</returns>
    public bool IsTime()
    {
        if (isStarted) return true;
        if (timestamp + (time) < Time.time)
        {
            isStarted = true;
            timestamp = Time.time - spacing; //Minus spacing so they spawn right away instead of with a delay
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Tell when Subwave is done spawning
    /// </summary>
    /// <returns>Returns true when there are no more enemies to spawn in the Subwave</returns>
    public bool IsDone()
    {
        if (amount == 0) return true;
        else return false;
    }

    /// <summary>
    /// sets the times to spawn the enemies in the Subwave
    /// </summary>
    /// <returns>Returns a list of EnemTime to determine spawn time</returns>
    public List<EnemTime> GetEnemies()
    {
        List<EnemTime> g = new List<EnemTime>();
        for (int i = 0; i < amount; i++) g.Add(new EnemTime(time + (i * spacing), enemy));

        return g;
    }

    /// <summary>
    /// resets the enemy timestamp
    /// </summary>
    public void ResetTimestamp() { timestamp = Time.time; }
}

public class EnemTime
{
    public float timestamp;
    public GameObject enemy;

    public EnemTime(float timestamp, GameObject enemy)
    {
        this.timestamp = timestamp;
        this.enemy = enemy;
    }

    public override string ToString()
    {
        return "Time: " + timestamp + " Enemy: " + enemy.name;
    }
}

