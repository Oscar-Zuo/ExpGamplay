using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPlateController : ItemController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        itemType = EItemType.Decoraction;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void ActivateItemEffects()
    {
        base.ActivateItemEffects();
        GameManager.instance.playerController.MaxHealth += 1;
        GameManager.instance.playerController.Health += 2;
    }
}
