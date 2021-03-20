using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FogManager : MonoBehaviour
{
    public GameObject parent;
    public Player player;

    List<ParticleSystem> fog;

    private void Awake()
    {
        fog = MethodPlus.GetComponentsInChildrenWithTag<ParticleSystem>(parent, "Fog").ToList();
    }

    void Update()
    {
        foreach (ParticleSystem p in fog)
            fogUpdate(p);
    }

    void fogUpdate(ParticleSystem fog)
    {
        float distToPlayer = Vector3.Distance(fog.transform.position, player.transform.position);

        if (distToPlayer <= fog.shape.radius * 3 && fog.isPlaying)
            fog.Stop();
        else if (distToPlayer > fog.shape.radius * 3 && fog.isStopped)
            fog.Play();
    }
}
