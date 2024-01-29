using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPlateController : ItemController
{
    // Start is called before the first frame update
    private void Awake()
    {
        itemType = EItemType.Decoraction;
    }

    public override void ActivateItemEffects()
    {
        base.ActivateItemEffects();
        GameManager.Instance.playerController.MaxHealth += 1;
        GameManager.Instance.playerController.Health += 2;
    }
}
