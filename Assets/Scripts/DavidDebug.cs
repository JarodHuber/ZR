using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DavidDebug : MonoBehaviour
{
    public GameObject handL, handR;

    // Update is called once per frame
    void Update()
    {
        if(handL.GetComponent<HandGrab>().SelectedObj == null || handR.GetComponent<HandGrab>().SelectedObj == null)
        {
            GetComponent<Text>().text = "Null..";
        }
        else
        {
            GetComponent<Text>().text = "L = " + handL.GetComponent<HandGrab>().SelectedObj.name + "\n" + "R = " + handR.GetComponent<HandGrab>().SelectedObj.name;
        }
    }
}