using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JellyCanController : ItemController
{
    protected float speed = 0.2f;
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

    protected override void ActivateItemEffects()
    {
        base.ActivateItemEffects();
        base.playerController.ForwardSpeed += speed;
        base.playerController.BackwardSpeed+= speed;
    }
}
