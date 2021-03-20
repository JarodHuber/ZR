using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GrabbedSnap))]
public class Gun : MonoBehaviour
{
    #region GunVariables
    [HideInInspector]
    public GrabbableObj go;
    [HideInInspector]
    public EnemyManager enemyManager;
    [HideInInspector]
    public int curMag = 0;

    [Header("Gun Variables")]
    [Tooltip("Set to an empty GameObject at the tip of the gun")]
    public GameObject frontBarrel;
    [Tooltip("Set to an empty GameObject at the other end of the gun")]
    public GameObject backBarrel;

    [Space(10)]
    [Tooltip("prefab of the muzzle-flash particle system")]
    public GameObject flashEffectFab;

    [Space(10)]
    [Tooltip("the force of the vibration in the controller")]
    public VibrationForce vibrationForce;
    [Tooltip("whether you had to pull the trigger for every fire (true) or just hold down the trigger (false)")]
    public bool SemiAutomatic = true;
    [Tooltip("the time in seconds before the next fire")]
    public float fireDelay = 0;
    [Tooltip("time required to reload")]
    public float reloadTime = 1f;
    [Tooltip("number of bullets that can be fired before reload required")]
    public int magSize = 10;
    [Tooltip("time after drop before you can fire it again..?")]
    public float lockTime = 2;

    Animator anim;
    bool firing = false, reloading = false, grabLock = false;
    Timer lockTimer, delayTimer, reloadTimer;
    float reloadRatio = 0;
    float rightTrigger, leftTrigger;
    #endregion

    #region BulletVariables
    [Header("Bullet Variables")]
    [Tooltip("Enemy blood splatter particle effect prefab")]
    public GameObject bloodSplatter;
    [Tooltip("Environment Shot particle effect prefab")]
    public GameObject environmentHit;
    [Tooltip("range of the bullet")]
    public float range = 50f;
    [Tooltip("amount of damage the bullet deals")]
    public int damage = 10;
    [Tooltip("number of bullets per shot")]
    public int bulletCount = 1;
    [Tooltip("bolt vs spread")]
    public bool spreadShot = false;
    [Tooltip("the radius of the spread (how far off the bullet can be)")]
    public float spreadRad = 0;
    [Tooltip("Layers the bullet can hit")]
    public LayerMask bulletMask;

    /// <summary>
    /// the list of directions the bullets travel
    /// </summary>
    List<Vector3> directions = new List<Vector3>();

    int decalHalf = 0;

    public static string outWhatHit;
    #endregion

    #region Line Rendering
    [Header("Bullet Effect")]
    public List<LineRenderer> laserLineRenderer = new List<LineRenderer>();
    public Material laserMaterial;
    public float laserWidth = 0.01f;
    public float laserMaxLength = 5f;
    bool laserLock = false;
    Timer fireHold = new Timer(.05f);
    #endregion

    private void Start()
    {
        lockTimer = new Timer(reloadTime);
        reloadTimer = new Timer(reloadTime);
        delayTimer = new Timer(fireDelay);
        delayTimer.Reset(delayTimer.delay);

        go = GetComponent<GrabbableObj>();
        curMag = magSize;


        for (int x = 0; x < bulletCount; x++)
        {
            if(GetComponent<LineRenderer>() == null)
                laserLineRenderer.Add(gameObject.AddComponent<LineRenderer>());
            else
            {
                GameObject renderChild = new GameObject("lineRenderer");
                renderChild.transform.position = frontBarrel.transform.position;
                renderChild.transform.SetParent(transform);

                laserLineRenderer.Add(renderChild.AddComponent<LineRenderer>());
            }
        }

        foreach (LineRenderer l in laserLineRenderer)
        {
            Vector3[] initLaserPositions = new Vector3[2] { frontBarrel.transform.position, frontBarrel.transform.position };
            l.SetPositions(initLaserPositions);
            l.material = laserMaterial;
            l.startWidth = laserWidth;
            l.endWidth = laserWidth;
            l.enabled = false;
        }
        laserLock = true;
    }

    void Update()
    {
        if (go != null)
        {
            if (enemyManager.Paused) return;

            rightTrigger = Input.GetAxis("10");
            leftTrigger = Input.GetAxis("9");

            if (!reloading && delayTimer.Check(false) && !firing && !grabLock && curMag > 0)
            {
                if (go.state == GrabState.GrabR && rightTrigger != 0)
                    Fire();
                else if (go.state == GrabState.GrabL && leftTrigger != 0)
                    Fire();
            }

            if (curMag < magSize)
            {
                if (go.state == GrabState.GrabR && Input.GetButton("b0"))
                    Reload();
                else if (go.state == GrabState.GrabL && Input.GetButton("b2"))
                    Reload();
                if (go.state == GrabState.GrabR && Input.GetButtonUp("b0"))
                    StopReload();
                else if (go.state == GrabState.GrabL && Input.GetButtonUp("b2"))
                    StopReload();
            }
            else
            {
                if (go.state == GrabState.GrabR && Input.GetButtonUp("b0"))
                    StopReload();
                else if (go.state == GrabState.GrabL && Input.GetButtonUp("b2"))
                    StopReload();
            }

            if (firing && !grabLock)
            {
                if (go.state == GrabState.GrabL && leftTrigger == 0)
                    UnFire();
                else if (go.state == GrabState.GrabR && rightTrigger == 0)
                    UnFire();

                if (go.state == GrabState.UnGrabbed)
                    grabLock = true;
            }


            if (grabLock)
                GrabLock();

            if (!laserLock && fireHold.Check())
            {
                foreach (LineRenderer l in laserLineRenderer)
                    l.enabled = false;
                laserLock = true;
            }
            else if(!laserLock)
                foreach (LineRenderer l in laserLineRenderer)
                    l.SetPosition(0, frontBarrel.transform.position);
        }
    }

    /// <summary>
    /// fire the gun
    /// </summary>
    public void Fire()
    {
        firing = SemiAutomatic;

        curMag = Mathf.Clamp(curMag - 1, 0, magSize);

        SetVelocity(Path());

        delayTimer.Reset();
        reloadTimer.Reset();
        reloadRatio = 0;

        GameObject instance = Instantiate(flashEffectFab, frontBarrel.transform.position,
            Quaternion.LookRotation(frontBarrel.transform.position - backBarrel.transform.position));
        instance.transform.SetParent(frontBarrel.transform);

        //go.hand.GetComponent<HandGrab>().haptics.Vibrate(vibrationForce);
    }

    /// <summary>
    /// Unlocks fire for semi-autos
    /// </summary>
    public void UnFire()
    {
        firing = false;
        decalHalf = 0;
        delayTimer.Reset(delayTimer.delay);
    }

    /// <summary>
    /// reload the gun
    /// </summary>
    void Reload()
    {
        reloading = true;
        reloadTimer.CountByTime();
        reloadRatio = Mathf.Clamp(reloadTimer.PercentComplete(), 0, 1);
        curMag = (int)Mathf.Lerp(0, magSize, reloadRatio);
    }

    /// <summary>
    /// end reload
    /// </summary>
    void StopReload()
    {
        reloading = false;
        reloadTimer.Reset();
        reloadRatio = 0;
    }

    /// <summary>
    /// used to keep player from spam dropping, grabbing and firing to shoot stupid fast
    /// </summary>
    void GrabLock()
    {
        firing = true;
        if (lockTimer.Check())
        {
            firing = false;
            grabLock = false;
        }
    }

    /// <summary>
    /// Determines the path forward from the barrel
    /// </summary>
    /// <returns>Returns the Vector3 direction forward from the barrel</returns>
    Vector3 Path()
    {
        return (frontBarrel.transform.position - backBarrel.transform.position).normalized;
    }

    #region Bullet
    /// <summary>
    /// Sets the direction for the bullet to travel
    /// </summary>
    /// <param name="velocity">Base direction for the bullet to travel</param>
    public void SetVelocity(Vector3 velocity)
    {
        directions.Clear();

        if (!spreadShot)
            directions.Add(velocity);
        else
            Spread(velocity);

        CastEvent();
    }

    /// <summary>
    /// adds a degree of error for the direction fo the bullet to travel
    /// </summary>
    /// <param name="velocity">Base direction for the bullet to travel</param>
    void Spread(Vector3 velocity)
    {
        Vector3 spread = Random.insideUnitCircle * spreadRad;

        if (bulletCount != 1)
        {
            for (int x = 0; x < bulletCount; x++)
            {
                spread = Random.insideUnitCircle * spreadRad;

                directions.Add(velocity + spread);
            }
        }
        else
            directions.Add(velocity + spread);
    }

    /// <summary>
    /// shoot the raycast
    /// </summary>
    void CastEvent()
    {
        laserLock = false;
        Vector3[] initLaserPositions = new Vector3[2] { frontBarrel.transform.position, frontBarrel.transform.position };

        RaycastHit hit;
        if (directions.Count == 0)
            Debug.LogError("directions' not being added to");
        else if (directions.Count == 1)
        {
            if (Physics.Raycast(frontBarrel.transform.position, directions[0], out hit, range, bulletMask))
            {
                initLaserPositions[1] = hit.point;
                HandleCollision(hit);
            }
            else
                initLaserPositions[1] += range * directions[0];

            laserLineRenderer[0].enabled = true;
            laserLineRenderer[0].SetPositions(initLaserPositions);
        }
        else
        {
            for(int x = 0; x<directions.Count; x++)
            {
                if (Physics.Raycast(frontBarrel.transform.position, directions[x], out hit, range, bulletMask))
                {
                    initLaserPositions[1] = hit.point;
                    HandleCollision(hit);
                }
                else
                {
                    initLaserPositions[1] += range*directions[x];
                }
                laserLineRenderer[x].enabled = true;
                laserLineRenderer[x].SetPositions(initLaserPositions);
            }
        }
    }

    /// <summary>
    /// what to do if the bullet hits
    /// </summary>
    /// <param name="collision">Bullet hit data</param>
    void HandleCollision(RaycastHit collision)
    {
        if (bulletCount > 1)
            decalHalf++;
        outWhatHit = collision.collider.gameObject.name;
        if (collision.collider.tag == "Enemy")
        {
            Enemy enemy = enemyManager.FindEnemy(collision.transform.parent);
            enemy.TakeDamage(damage / bulletCount);

            GameObject decal = Instantiate(bloodSplatter, collision.point, Quaternion.LookRotation(collision.normal));
            decal.transform.SetParent(collision.transform.parent);
        }
        else if(collision.collider.tag == "Obstacle" && decalHalf % 2 == 0)
        {
            GameObject decal = Instantiate(environmentHit, collision.point, Quaternion.LookRotation(collision.normal));
        }
        else if(collision.collider.tag == "Grenade")
        {
            collision.collider.GetComponent<Grenade>().BlowUp();
        }
    }
    #endregion
}
