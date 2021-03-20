using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JarodDebug : MonoBehaviour
{
    void Update()
    {
        GetComponent<Text>().text = WaveUI.debug.ToString();
    }
}
