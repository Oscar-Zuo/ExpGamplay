using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APAmmoController : ItemController
{
    // Start is called before the first frame update
    public float damage = 0.5f;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void ActivateItemEffects()
    {
        base.ActivateItemEffects();
        GameManager.Instance.playerController.PlayerBaseDamage += damage;
    }

}
