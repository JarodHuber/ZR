using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public Transform player;
    public List<LightGuy> lights = new List<LightGuy>();
    public float distance = 20f;
    public int maxNumOfLights = 10;

    private void Start()
    {
        for (int x = 0; x < transform.childCount; ++x)
        {
            lights.Add(new LightGuy(transform.GetChild(x).GetComponent<Light>()));
        }
    }

    private void Update()
    {
        lights.Sort((light1, light2) => light1.Dist(player).CompareTo(light2.Dist(player)));

        for(int x = 0; x < lights.Count; ++x)
        {
            lights[x].light.enabled = x < maxNumOfLights && lights[x].Dist(player) < distance;
        }
    }
}

public class LightGuy
{
    public Light light;

    public LightGuy(Light light)
    {
        this.light = light;
    }

    public float Dist(Transform target)
    {
        return Vector3.Distance(light.transform.position, target.position);
    }
}