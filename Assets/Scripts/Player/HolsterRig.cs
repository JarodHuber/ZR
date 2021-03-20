using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HolsterRig : MonoBehaviour
{
    public List<Holster> holsters { get; private set; }

    Manager manager;
    Player player;
    Vector3 rot = Vector3.zero;

    Transform forwardAnchor;

    float angleBetween;

    private void Start()
    {
        manager = MethodPlus.GetComponentInObjectByTag<Manager>("GameController");
        player = manager.player;

        holsters = transform.GetComponentsInChildren<Holster>().ToList();
        forwardAnchor = MethodPlus.GetChildWithName(manager.Player.transform, "ForwardDirection").transform;
    }

    private void Update()
    {
        transform.position = player.transform.position;

        rot = forwardAnchor.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, rot.y, 0);
    }
}
