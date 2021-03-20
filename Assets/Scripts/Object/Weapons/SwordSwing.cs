using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSwing : MonoBehaviour
{
    public GameObject parentSword;
    public Color activeMat, inactiveMat;
    public float swingSpeed = 0;
    public float SwingReq = .1f;
    public Vector3 prevSpace = Vector3.up;
    public bool isActive = false;
    string active = "Sword";
    string inactive = "Untagged";

    void Update()
    {
        transform.position = parentSword.transform.position;
        transform.rotation = parentSword.transform.rotation;

    //    if (true)
    //    {
    //        isActive = true;
    //        gameObject.GetComponent<MeshRenderer>().material.color = activeMat;
    //    }
    //    else
    //        isActive = false;

    //    if (isActive && gameObject.tag != active)
    //    {
            
    //        gameObject.tag = active;
    //    }
    //    else if (!isActive && gameObject.tag != inactive)
    //    {
    //        gameObject.GetComponent<MeshRenderer>().material.color = inactiveMat;
    //        gameObject.tag = inactive;
    //    }

        
    //}
    //private void FixedUpdate()
    //{
        
    //    prevSpace = transform.position;
    //}

    //public float SwingSpeed()
    //{
    //    return Mathf.Abs(Vector3.Distance(transform.position, prevSpace)) * 50; 
    }
}