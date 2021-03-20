using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject explosionFab;
    public float lifeTime = 3;
    public bool startThrow = false;

    GrabbableObj go;
    Timer grenadeTimer;

    float rightTrigger, leftTrigger;

    private void Start()
    {
        go = GetComponent<GrabbableObj>();
        grenadeTimer = new Timer(lifeTime);
    }

    private void Update()
    {
        if (!startThrow && go.state != GrabState.UnGrabbed)
        {
            rightTrigger = Input.GetAxis("10");
            leftTrigger = Input.GetAxis("9");

            if (go.state == GrabState.GrabL && leftTrigger != 0)
                startThrow = true;
            else if (go.state == GrabState.GrabR && rightTrigger != 0)
                startThrow = true;
        }

        if(startThrow)
            if (grenadeTimer.Check())
                BlowUp();
    }

    public void BlowUp()
    {
        if (go.state != GrabState.UnGrabbed)
            go.hand.GetComponent<HandGrab>().UnGrab();

        Holster temp = go.currentParent.GetComponent<Holster>();
        if (temp != null)
        {
            temp.holsterContents = null;
            temp.isHolstering = false;
        }

        Instantiate(explosionFab, transform.position, explosionFab.transform.rotation);
        Destroy(gameObject);
    }
}
