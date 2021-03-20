using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableObj : MonoBehaviour
{
    [HideInInspector]
    public GrabState state = GrabState.UnGrabbed;
    [HideInInspector]
    public GameObject grabParent, hand = null, currentParent;
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public Vector3 currPos, prevPos;

    public ObjectType objectType = ObjectType.None;

    float Scaler = 0.75f;
    int counterToStopAt = 0;
    List<Velocity> stamps = new List<Velocity>();

    List<Holster> holsters;

    (bool check, Holster holster) holsterCheck;

    void Start()
    {
        grabParent = GameObject.Find("GrabParent");
        currentParent = grabParent;
        gameObject.transform.SetParent(currentParent.transform, true);
        rb = gameObject.GetComponent<Rigidbody>();

        currPos = transform.position;

        holsters = MethodPlus.GetComponentInObjectByTag<HolsterRig>("HolsterRig").holsters;
    }

    void Update()
    {
        if (state == GrabState.UnGrabbed && hand != null)
        {
            holsterCheck = HolsterCheck();
            if (holsterCheck.check)
            {
                if(currentParent != holsterCheck.holster.gameObject)
                {
                    currentParent = holsterCheck.holster.gameObject;
                    gameObject.transform.SetParent(currentParent.transform, true);
                    holsterCheck.holster.HolsterObject(gameObject, objectType);
                    holsterCheck.holster.isHolstering = true;
                    hand = null;
                    stamps.Clear();
                }
            }
            else
            {
                if (currentParent != grabParent)
                {
                    currentParent = grabParent;
                    gameObject.transform.SetParent(currentParent.transform, true);
                    rb.isKinematic = false;
                    rb.velocity = releaseVelocity();
                    hand = null;
                    stamps.Clear();
                }
            }
        }
        else if (state != GrabState.UnGrabbed  && hand.GetComponent<HandGrab>() != null)
        {
            if(currentParent != hand)
            {
                Holster temp = currentParent.GetComponent<Holster>();
                if (temp != null)
                {
                    temp.holsterContents = null;
                    temp.isHolstering = false;
                }

                currentParent = hand;
                gameObject.transform.SetParent(currentParent.transform, true);
                rb.isKinematic = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (state != GrabState.UnGrabbed && hand.GetComponent<HandGrab>() != null)
        {
            prevPos = currPos;
            currPos = transform.position;

            for (int x = 0; x < stamps.Count; ++x)
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
    /// velocity the object is thrown at
    /// </summary>
    /// <returns>Vector3 velocity to throw object</returns>
    public Vector3 releaseVelocity()
    {
        //counterToStopAt = 0;
        //for (int counter = stamps.Count - 1; counter > 0; counter--)
        //{
        //    float angleBetween = Vector3.Angle(stamps[counter].PositionStamp, stamps[counter - 1].PositionStamp);
        //    if(angleBetween > 60 && angleBetween < 300)
        //    {
        //        counterToStopAt = counter;
        //    }
        //}
        //Vector3 path = stamps[stamps.Count - 1].PositionStamp;
        //path = (path + ((path - stamps[counterToStopAt].PositionStamp) / stamps[counterToStopAt].TimeStamp)) * stamps[counterToStopAt].TimeStamp;
        //path *= Scaler / Time.fixedDeltaTime;
        //return path;

        Vector3 toReturn = stamps[0].PositionStamp;

        for( int x = 1; x < stamps.Count; ++x)
        {
            toReturn += stamps[x].PositionStamp;
        }

        toReturn /= stamps.Count;

        return toReturn * (Scaler / Time.fixedDeltaTime);
    }

    (bool check, Holster holster) HolsterCheck()
    {
        foreach(Holster h in holsters)
        {
            if(!h.isHolstering && (Vector3.Distance(h.transform.position, transform.position) <= Manager.HolsterTargetDistance) && h.HolsterContains(objectType))
            {
                return (true, h);
            }
        }
        return (false, null);
    }
}