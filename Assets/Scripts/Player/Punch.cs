using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour
{
    public GameObject sound;

    int damage = 10;
    float minPower = 5;

    HandGrab handGrab;
    Vector3 currPos, prevPos;
    List<Velocity> stamps = new List<Velocity>();
    float Scaler = 0.75f;

    bool isGripping = false, isTriggering = false, punchTime = false, isPunching = false;
    float punchVal = 0;
    EnemyManager enemyManager;

    public static float punchMag, minMag;

    Enemy enemy;

    private void Start()
    {
        enemyManager = MethodPlus.GetComponentInObjectByTag<EnemyManager>("GameController");
        handGrab = GetComponent<HandGrab>();
        minMag = minPower;
    }

    void Update()
    {
        if (enemyManager.Paused)
        {
            punchTime = false;
            return;
        }

        if (!handGrab.isGrabbing)
        {
            if (handGrab.handSide == HandSide.Right)
            {
                if (Input.GetAxis("12") != 0)
                    isGripping = true;
                else if (Input.GetAxis("12") == 0)
                    isGripping = false;

                if (Input.GetAxis("10") != 0)
                    isTriggering = true;
                else if (Input.GetAxis("10") == 0)
                    isTriggering = false;
            }

            else if (handGrab.handSide == HandSide.Left)
            {
                if (Input.GetAxis("11") != 0)
                    isGripping = true;
                else if (Input.GetAxis("11") == 0)
                    isGripping = false;

                if (Input.GetAxis("9") != 0)
                    isTriggering = true;
                else if (Input.GetAxis("9") == 0)
                    isTriggering = false;
            }

            punchTime = isGripping && isTriggering;
        }
        else
        {
            isPunching = false;
            isGripping = false;
            isTriggering = false;
        }
    }

    void FixedUpdate()
    {
        if (punchTime)
        {
            prevPos = currPos;
            currPos = transform.position;

            for( int x = 0; x < stamps.Count; ++x)
            {
                stamps[x].TimeStamp += Time.deltaTime;
                if (stamps[x].TimeStamp > 1)
                {
                    stamps.RemoveAt(x);
                }
            }

            stamps.Add(new Velocity(0, currPos - prevPos));
        }
    }

    /// <summary>
    /// Determines if the connection is a punch
    /// </summary>
    /// <param name="minPower">The minimum power the punch could be and still be a punch</param>
    /// <returns>Bool if it is a punch</returns>
    public float PunchPower()
    {
        int counterToStopAt = 0;
        for (int counter = stamps.Count - 1; counter > 0; counter--)
        {
            float angleBetween = Vector3.Angle(stamps[counter].PositionStamp, stamps[counter - 1].PositionStamp);
            if (angleBetween > 60 && angleBetween < 300)
            {
                counterToStopAt = counter;
            }
        }
        Vector3 path = stamps[stamps.Count - 1].PositionStamp;
        path = (path + ((path - stamps[counterToStopAt].PositionStamp) / stamps[counterToStopAt].TimeStamp)) * stamps[counterToStopAt].TimeStamp;
        path *= Scaler / Time.fixedDeltaTime;
        return path.magnitude;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy" && punchTime && !isPunching)
        {
            enemy = enemyManager.FindEnemy(other.transform.parent);
            if(enemy != null)
            {
                punchVal = PunchPower();
                StartCoroutine(FollowThrough(other.ClosestPointOnBounds(transform.position)));
            }
        }
    }

    IEnumerator FollowThrough(Vector3 hitPoint)
    {
        yield return new WaitForSeconds(.1f);

        float delayPunch = PunchPower();

        if (delayPunch > punchVal)
            punchVal = delayPunch;

        punchMag = punchVal;

        if(punchVal > minPower)
        {
            enemy.TakeDamage(damage);
            Instantiate(sound, hitPoint, Quaternion.identity);
        }

        yield return "David is a poopy butthole";
    }
}
