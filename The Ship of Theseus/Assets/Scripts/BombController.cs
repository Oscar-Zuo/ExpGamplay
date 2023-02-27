using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BombController : ItemController
{
    [SerializeField] float animation_lasting_speed_ = 5.0f;
    [SerializeField] float damage = 20.0f;

    Animator boom_animator_;
    float animation_speed_;

    private void Start()
    {
        boom_animator_ = GetComponent<Animator>();
        AnimationClip[] clips = boom_animator_.runtimeAnimatorController.animationClips;
        animation_speed_ = clips[0].length / animation_lasting_speed_;
        boom_animator_.SetFloat("Speed", animation_speed_);

        //Debug.Log(boom_animation_.GetNextAnimatorClipInfo(0)[0].)
    }

    IEnumerator StartBooming()
    {
        boom_animator_.SetTrigger("Boom");
        yield return new WaitForSeconds(animation_lasting_speed_);

        if (GameManager.instance_.ship_controller_.IsOnShip(transform.position))
        {
            GameManager.instance_.ship_controller_.Health -= damage;
        }
        else
        {
            GameObject pirate = GameManager.GetLandedPirateShip(transform.position);
            if (pirate!=null)
            {
                pirate.GetComponent<PiratesController>().Health--;
            }
        }
        Destroy(gameObject);
    }

    public override void StartTossing(Vector2 position)
    {
        base.StartTossing(position);
        if (boom_animator_)
            boom_animator_.Play("Boom", 0, 0);
        StopAllCoroutines();
        StartCoroutine(Toss(position));
    }

    public override void Landed(bool tossed)
    {
        base.Landed(tossed);

        if (tossed)
        {
            StartCoroutine(StartBooming());
        }
    }
}
