using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GrabbableObj))]
public class GrabbedSnap : MonoBehaviour
{
    [HideInInspector]
    public GrabbableObj go;

    bool isSnapped = false;

    [Tooltip("set to the offset GameObject in player's left hand")]
    public Transform snapOffsetL;

    [Tooltip("set to the offset GameObject in player's right hand")]
    public Transform snapOffsetR;

    private void Start()
    {
        go = GetComponent<GrabbableObj>();
    }

    void Update()
    {
        if(!isSnapped && go.state == GrabState.GrabR)
        {
            this.gameObject.transform.position = snapOffsetR.position;
            this.gameObject.transform.rotation = snapOffsetR.rotation;
            isSnapped = true;
        } else if(!isSnapped && go.state == GrabState.GrabL)
        {
            this.gameObject.transform.position = snapOffsetL.position;
            this.gameObject.transform.rotation = snapOffsetL.rotation;
            isSnapped = true;
        } else if (isSnapped && go.state == GrabState.UnGrabbed)
        {
            isSnapped = false;
        }
    }
}
