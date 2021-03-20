using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordGrab : MonoBehaviour
{
    public GameObject swordCollider;
    GrabbableObj go;
    public GameObject instance;
    bool notActive = true;

    private void Start()
    {
        go = GetComponent<GrabbableObj>();
    }

    private void Update()
    {
        if(go != null)
        {
            if(go.state != GrabState.UnGrabbed && notActive)
            {
                instance = Instantiate(swordCollider, transform.position, transform.rotation);
                instance.GetComponent<SwordSwing>().parentSword = gameObject;
                notActive = false;
            }
            else
            {
                notActive = true;
                Destroy(instance);
            }
        }
    }
}
