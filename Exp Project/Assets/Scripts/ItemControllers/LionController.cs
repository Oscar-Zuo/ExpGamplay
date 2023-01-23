using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LionController : ItemController
{
    public float fireRateModifier = 0.9f;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void ActivateItemEffects()
    {
        base.ActivateItemEffects();
        playerController.FireRateModifier *= fireRateModifier;
    }
}
