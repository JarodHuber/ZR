using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WaveControllerEndless : MonoBehaviour
{

    //  WARNING: THE FOLLOWING CODE IS EXTREMELY GROSS  //

    #region Variables
    //AutoPlay Stuff Don't Touch
    public bool AutoPlay = true;

    //Can Now Touch
    public GameObject averageJoe;
    public GameObject trackKid;
    public GameObject footballJock;
    public GameObject nerd; //heckin nerd
    public GameObject healthbar;
    public GameObject healthbarBackground;

    public GameObject playBut;
    public Text waveTxt;

    public List<EnemTime> nextEnemies = new List<EnemTime>();

    public bool paused = false;

    public int wave = 0;
    public int extraGold = 10;

    public int endWaveCandy = 10;
    bool candyGiven = false;

    private List<GameObject> spawns = new List<GameObject>();

    private List<List<SubWave>> currentWave = new List<List<SubWave>>();
    private List<SubWave> baseWave = new List<SubWave>(); //This is for later levels in endless mode

    private bool lastVal = false;


    private int exGoldAvailable = 0;

    private float timestamp;
    private float pauseTime;

    enum Phases { WAIT, START, PLAY, COUNTDOWN };
    Phases stage = Phases.WAIT;

    GameObject UI;
    #endregion

    #region Start and Update
    void Start()
    {
        spawns = GameObject.FindGameObjectsWithTag("Spawner").ToList();
        spawns = spawns.OrderBy(s => s.name).ToList();

        foreach (GameObject s in spawns) print(s.name);

        UI = GameObject.Find("UI");

        //Wave, time, spacing, amount, enemy
       // baseWave.Add(new SubWave(wave, 0f, 1.5f, 10 + (2 * (wave - 1)), averageJoe));
    }

    void Update()
    {
        //string a = spawns[0].name;
        //for(int i = 1; i < spawns.Count; i++) { a += ", " + spawns[i].name; }
        //print(a);

        if (paused && !lastVal)
        {
            pauseTime = Time.time;
            lastVal = true;
        }

        if (!paused && lastVal)
        {
            pauseTime = Time.time - pauseTime;
            lastVal = false;

            if (wave > 0)
            {
                foreach (SubWave sw in currentWave[0])
                {
                    print("screeeeeeeeeeeeeeeee");
                    sw.timestamp += pauseTime;
                    timestamp += pauseTime;
                }
            }
        }

        if (paused) return;

        if (wave > 0) waveTxt.text = "Wave " + wave;
        else waveTxt.text = "";

        if (stage == Phases.WAIT) WaitToStart();
        if (stage == Phases.START) StartWave();
        if (stage == Phases.PLAY) PlayWave();
        if (stage == Phases.COUNTDOWN) StartCoroutine(Countdown());

        if (Input.GetKeyDown(KeyCode.Alpha3)) foreach (GameObject g in NextThree()) if (g != null) print(g.name);
    }
    #endregion

    #region Behaviors
    void WaitToStart() //Just stuff we need to do in between waves
    {
        if (!candyGiven && wave > 0)
        {
            //Candies.Currency += Mathf.Max((int) (-1 * (Mathf.Pow(wave - 2, 2) / 30) + 20), 0);
            candyGiven = true;
        }
    }

    void StartWave()
    {
        wave++;

        //baseWave.Add(new SubWave(wave, 0f, 0.5f, 6 + (2 * (wave - 1)), averageJoe));
        //if(wave > 4) 
        //    for (int i = 0; i < wave / 4; i++) 
        //        baseWave.Add(new SubWave(wave, (i * 3) + 2, 0.75f, 1 + (int)(wave / 3), trackKid));
        //if(wave > 9) 
        //    for (int i = 0; i < (wave / 4) - 1; i++) 
        //        baseWave.Add(new SubWave(wave, (i * 3) + 3, 1f, 1 + (int)(wave / 3), footballJock));
        //if(wave > 14) 
        //    for (int i = 0; i < (wave / 4) - 2; i++) 
        //        baseWave.Add(new SubWave(wave, (i * 3) + 3, 1f, 1 + (int)(wave / 3), nerd));

        currentWave.Add(baseWave);

        candyGiven = false;

        foreach (SubWave sw in currentWave[0])
        {
            sw.ResetTimestamp(); //We need this so that the timestamps are correct

            nextEnemies.AddRange(sw.GetEnemies());
        }

        nextEnemies = nextEnemies.OrderBy(t => t.timestamp).ToList();
        //foreach (EnemTime et in nextEnemies) print(et.ToString());

        timestamp = Time.time;

        playBut.SetActive(false);

        stage = Phases.PLAY;
    }

    void PlayWave() //ABSOLUTELY DISGUSTING
    {
        //foreach (EnemTime et in nextEnemies) print(et.ToString());

        for (int i = 0; i < currentWave[0].Count; i++)
        {
            if (currentWave[0][i].IsTime())
            {
                //print(WAVES[wave - 1][i].timestamp);

                GameObject temp = currentWave[0][i].Spawn();
                if (temp != null)
                {
                    Image h = Instantiate(healthbar, GameObject.Find("HealthbarCanvas").transform).GetComponent<Image>();
                    GameObject e = Instantiate(temp, spawns[0].transform.position, transform.rotation);

                    //EnemyGeneral eg = e.GetComponent<EnemyGeneral>();
                    //eg.healthbar = h;
                    //eg.wave = currentWave[0][i].wave;

                    //Cycle the spawns
                    GameObject spawnTemp = spawns[0];

                    for (int j = 1; j < spawns.Count; j++)
                    {
                        spawns[(j - 1) % spawns.Count] = spawns[j];
                    }

                    spawns[spawns.Count - 1] = spawnTemp;

                    nextEnemies.RemoveAt(nextEnemies.FindIndex(ne => ne.enemy.Equals(temp))); //Lambdas yay
                }
            }
            if (currentWave[0][i].IsDone()) currentWave[0].Remove(currentWave[0][i]);
        }

        if (currentWave[0].Count == 0 && Scan(wave))
        {
            print("wave finished"); //Signal the end of the wave
            stage = Phases.COUNTDOWN;

            nextEnemies.Clear();

            playBut.SetActive(true);
        }
    }

    IEnumerator Countdown()
    {
        stage = Phases.WAIT;

        exGoldAvailable = extraGold;

        playBut.transform.GetChild(0).GetComponent<Text>().text = 10.ToString();

        yield return new WaitForSeconds(1f);

        for (int i = 1; i < 10; i++)
        {
            exGoldAvailable -= extraGold / 10;
            playBut.transform.GetChild(0).GetComponent<Text>().text = (10 - i).ToString();

            yield return new WaitForSeconds(1f);
        }

        exGoldAvailable = 0;
        playBut.transform.GetChild(0).GetComponent<Text>().text = "Bonus missed!";
        if (AutoPlay)
        {
            OnPlayClick();
        }
        else if (!AutoPlay)
        {
            //Wait 1.5 seconds before changing text
            yield return new WaitForSeconds(1.5f);

            playBut.transform.GetChild(0).GetComponent<Text>().text = "Play";
        }
    }

    bool Scan(int wave)
    {
        bool a = true;

        if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0) a = false;

        return a;
    }

    public GameObject[] NextThree()
    {
        GameObject[] g = new GameObject[3];

        for (int i = 0; i < nextEnemies.Count; i++)
        {
            if (i == 3) break;
            if (i >= nextEnemies.Count)
            {
                g[i] = null;
                break;
            }
            g[i] = nextEnemies[i].enemy;
        }

        return g;
    }

    public void OnPlayClick()
    {
        StopAllCoroutines();

        //wave++;
        stage = Phases.START;

        //Candies.Currency += exGoldAvailable;
        print("bonus: " + exGoldAvailable);

        playBut.SetActive(false);
        //print("reeeeeeeeeeeeeeee");
    }
    #endregion
}
