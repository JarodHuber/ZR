using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HosterHover : MonoBehaviour
{
    public GameObject UI;
    Text objectName, damage, ammo;

    List<Holster> holsters;
    Holster holster;
    bool holsterSet = false;

    (string name, int damage, int bullets, int magSize) holsterContents;
    bool contentsSet = false;

    private void Start()
    {
        objectName = MethodPlus.GetComponentInChildWithName<Text>(transform, "Name");
        damage = MethodPlus.GetComponentInChildWithName<Text>(transform, "damage");
        ammo = MethodPlus.GetComponentInChildWithName<Text>(transform, "ammo");
        holsters = MethodPlus.GetComponentInObjectByTag<HolsterRig>("HolsterRig").holsters;
        UI.SetActive(false);
    }

    private void Update()
    {
        if (!holsterSet)
        {
            foreach (Holster h in holsters)
            {
                if (Vector3.Distance(transform.position, h.transform.position) <= Manager.HolsterTargetDistance)
                {
                    if (h.isHolstering)
                    {
                        holster = h;
                        holsterSet = true;
                    }
                }
            }
        }
        else
        {
            if (!contentsSet)
            {
                holsterContents = holster.holsterContents.Contents();

                UI.SetActive(true);
                objectName.text = holsterContents.name;
                damage.text = holsterContents.damage.ToString();
                ammo.text = holsterContents.bullets + "/" + holsterContents.magSize;

                contentsSet = true;
            }
            else
            {
                if(Vector3.Distance(transform.position, holster.transform.position) > Manager.HolsterTargetDistance)
                {
                    holster = null;
                    holsterSet = false;
                    contentsSet = false;
                    UI.SetActive(false);
                }
            }
        }
    }
}
