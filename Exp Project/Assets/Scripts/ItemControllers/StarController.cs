using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : ItemController
{
    // Start is called before the first frame update
    public float rotationSpeed = 10;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void ActivateItemEffects()
    {
        base.ActivateItemEffects();
        GameManager.Instance.playerController.RotationSpeed += rotationSpeed;
    }
}
