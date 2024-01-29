using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JellyCanController : ItemController
{
    protected float speed = 0.2f;
    // Start is called before the first frame update

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void ActivateItemEffects()
    {
        base.ActivateItemEffects();
        GameManager.Instance.playerController.ForwardSpeed += speed;
        GameManager.Instance.playerController.BackwardSpeed += speed;
    }
}
