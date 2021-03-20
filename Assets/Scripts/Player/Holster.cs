using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holster : MonoBehaviour
{
    public ObjectsToHolster holster;
    [HideInInspector]
    public Transform gunAnchor, pistolAnchor, grenadeAnchor;
    public HolsterContents holsterContents;
    public bool isHolstering = false;

    private void Start()
    {
        if (HolsterContains(ObjectType.Gun))
        {
            gunAnchor = MethodPlus.GetChildWithName(transform, "GunAnchor").transform;
        }
        if (HolsterContains(ObjectType.Pistol))
        {
            pistolAnchor = MethodPlus.GetChildWithName(transform, "PistolAnchor").transform;
        }
        if (HolsterContains(ObjectType.Grenade))
        {
            grenadeAnchor = MethodPlus.GetChildWithName(transform, "GrenadeAnchor").transform;
        }
    }

    public void HolsterObject(GameObject objToHolster, ObjectType holsterType)
    {
        if (HolsterContains(holsterType))
        {
            if(holsterType == ObjectType.Gun)
            {
                objToHolster.transform.position = gunAnchor.position;
                objToHolster.transform.rotation = gunAnchor.rotation;
            }
            if (holsterType == ObjectType.Pistol)
            {
                objToHolster.transform.position = pistolAnchor.position;
                objToHolster.transform.rotation = pistolAnchor.rotation;
            }
            if (holsterType == ObjectType.Grenade)
            {
                objToHolster.transform.position = grenadeAnchor.position;
                objToHolster.transform.rotation = grenadeAnchor.rotation;
            }

            holsterContents = new HolsterContents(objToHolster);
        }
        else
            Debug.LogError("Trying to snap to a holster that won't take it");
    }

    public bool HolsterContains(ObjectType objectType)
    {
        return (holster & (ObjectsToHolster)objectType) != 0;
    }
}

public class HolsterContents
{
    public GameObject ObjHeld;
    Gun Gun;

    public HolsterContents(GameObject objHeld)
    {
        ObjHeld = objHeld;

        if (objHeld.tag == "Gun")
            Gun = objHeld.GetComponent<Gun>();
    }

    public (string name, int damage, int bullets, int magSize) Contents()
    {
        return (ObjHeld.name, Gun.damage, Gun.curMag, Gun.magSize);
    }
}
