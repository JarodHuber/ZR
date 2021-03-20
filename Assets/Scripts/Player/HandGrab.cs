using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class HandGrab : MonoBehaviour
{

    [HideInInspector]
    public bool isGrabbing = false;
    [HideInInspector]
    public GameObject SelectedObj, GrabbedObj;
    [HideInInspector]
    public OculusHaptics haptics;

    GrabType typeOfObjSelected;
    Player player;
    OVRPlayerController ovr;
    GameObject handOnLedge;
    bool hasStarted = false;
    Transform playerPos;
    Manager manager;
    Vector3 prev, curr;

    [Space(-30)]
    [Header("Place on the OVRControllerPrefab")]
    public HandSide handSide;

    SkinnedMeshRenderer controllerMesh = null;

    void Start()
    {
        controllerMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        haptics = GetComponent<OculusHaptics>();
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainScene")
        {
            manager = GameObject.Find("Manager").GetComponent<Manager>();
            player = manager.player;
            playerPos = player.transform;
            ovr = player.transform.GetComponent<OVRPlayerController>();
        }
        SphereCollider sc = GetComponent<SphereCollider>();
        sc.center = new Vector3(0, -0.01f, -0.04f);
        sc.radius = 0.09f;
    }

    void Update()
    {
        if(manager != null && ovr != null)
        {
            if ((!isGrabbing || GrabbedObj.tag != "Ledge") && (!ovr.EnableRotation || !ovr.EnableLinearMovement || ovr.GravityModifier == 0) && !manager.Paused)
            {
                ovr.EnableLinearMovement = true;
                ovr.EnableRotation = true;
                ovr.GravityModifier = 0.379f;
            }
        }

        if(handSide == HandSide.Right)
        {
            if (!isGrabbing && SelectedObj != null && typeOfObjSelected == GrabType.obj && Input.GetAxis("12") != 0)
                Grab();
            else if (isGrabbing && Input.GetAxis("12") == 0 && typeOfObjSelected == GrabType.obj)
                UnGrab();
            if (!isGrabbing && SelectedObj != null && typeOfObjSelected == GrabType.ledge && Input.GetAxis("12") != 0)
                GrabLedge();
            else if (isGrabbing && Input.GetAxis("12") == 0 && typeOfObjSelected == GrabType.ledge)
                UnGrabLedge();
        }

        else if (handSide == HandSide.Left)
        {
            if (!isGrabbing && SelectedObj != null && typeOfObjSelected == GrabType.obj && Input.GetAxis("11") != 0)
                Grab();
            else if (isGrabbing && Input.GetAxis("11") == 0 && typeOfObjSelected == GrabType.obj)
                UnGrab();
            if (!isGrabbing && SelectedObj != null && typeOfObjSelected == GrabType.ledge && Input.GetAxis("11") != 0)
                GrabLedge();
            else if (isGrabbing && Input.GetAxis("11") == 0 && typeOfObjSelected == GrabType.ledge)
                UnGrabLedge();
        }
    }
    private void FixedUpdate()
    {
        prev = curr;
        curr = transform.position;
        if(handSide == HandSide.Left)
            if (isGrabbing && Input.GetAxis("11") != 0 && typeOfObjSelected == GrabType.ledge)
                KeepGrabbingLedge();
        else if (handSide == HandSide.Right)
            if (isGrabbing && Input.GetAxis("12") != 0 && typeOfObjSelected == GrabType.ledge)
                KeepGrabbingLedge();
    }
    void GrabLedge()
    {
        controllerMesh.enabled = false;
        player.transform.parent = transform;
        GrabbedObj = SelectedObj;
        isGrabbing = true;
        ovr.EnableLinearMovement = false;
        ovr.EnableRotation = false;
        ovr.GravityModifier = 0;
    }
    void UnGrabLedge()
    {
        controllerMesh.enabled = true;
        GrabbedObj = null;
        SelectedObj = null;
        typeOfObjSelected = GrabType.blank;
        isGrabbing = false;
        ovr.EnableLinearMovement = true;
        ovr.EnableRotation = true;
        ovr.GravityModifier = 0.379f;
        ovr.CameraRig.trackingSpace.position = playerPos.position;
        Destroy(handOnLedge);
        hasStarted = false;
    }
    void KeepGrabbingLedge()
    {
        Vector3 mov = curr - prev;
        playerPos.position += mov;
    }

    void Grab()
    {
        controllerMesh.enabled = false;
        GrabbedObj = SelectedObj;
        isGrabbing = true;
        GrabbedObj.GetComponent<GrabbableObj>().hand = this.gameObject;
        EnumSet(GrabbedObj);
    }
    public void UnGrab()
    {
        controllerMesh.enabled = true;
        GrabbedObj.GetComponent<GrabbableObj>().state = GrabState.UnGrabbed;
        GrabbedObj = null;
        SelectedObj = null;
        typeOfObjSelected = GrabType.blank;
        isGrabbing = false;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponent<GrabbableObj>() != null && !isGrabbing 
            && other.gameObject.GetComponent<GrabbableObj>().state == GrabState.UnGrabbed)
        {
            SelectedObj = other.gameObject;
            typeOfObjSelected = GrabType.obj;
        } else if (other.gameObject.tag == "Ledge" && !isGrabbing)
        {
            SelectedObj = other.gameObject;
            typeOfObjSelected = GrabType.ledge;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (SelectedObj == other.gameObject && !isGrabbing)
        {
            SelectedObj = null;
        }
    }

    void EnumSet(GameObject ObjectToSet)
    {
        if (handSide == HandSide.Left)
        {
            ObjectToSet.GetComponent<GrabbableObj>().state = GrabState.GrabL;
        } else if (handSide == HandSide.Right)
        {
            ObjectToSet.GetComponent<GrabbableObj>().state = GrabState.GrabR;
        }
    }
}