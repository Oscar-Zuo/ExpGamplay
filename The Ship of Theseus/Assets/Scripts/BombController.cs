using System.Collections;
using UnityEngine;

public class BombController : ItemController
{
    [SerializeField] float animation_lasting_speed_ = 5.0f;
    [SerializeField] float damage = 20.0f;

    Animator boom_animator_;
    float animation_speed_;
    bool is_booming = false;

    [SerializeField] protected AudioClip boom_audio_;

    private void Start()
    {
        audio_source_ = GetComponent<AudioSource>();
        boom_animator_ = GetComponent<Animator>();
        AnimationClip[] clips = boom_animator_.runtimeAnimatorController.animationClips;
        animation_speed_ = clips[0].length / animation_lasting_speed_;
        boom_animator_.SetFloat("Speed", animation_speed_);

        //Debug.Log(boom_animation_.GetNextAnimatorClipInfo(0)[0].)
    }

    IEnumerator StartBooming()
    {
        boom_animator_.SetTrigger("CountingDown");
        yield return new WaitForSeconds(animation_lasting_speed_);
        boom_animator_.SetTrigger("Boom");
        is_booming = true;
        if (GameManager.instance_.ship_controller_.IsOnShip(GetComponent<Collider2D>()))
        {
            GameManager.instance_.ship_controller_.Health -= damage;
        }
        else
        {
            GameObject pirate = GameManager.GetPirateShipByPosition(transform.position);
            if (pirate!=null)
            {
                pirate.GetComponent<PiratesController>().Health--;
            }
        }
        audio_source_.clip = boom_audio_;
        audio_source_.Play();
        yield return new WaitForSeconds(boom_audio_.length);
        if (transform.parent != null && transform.parent.tag.Equals("player"))
        {
            var player_controller = transform.parent.gameObject.GetComponent<CharacterController>();
            if (player_controller != null && player_controller.ItemList.Contains(gameObject))
            {
                player_controller.ItemList.Remove(gameObject);
                player_controller.ReorderItemList();
            }
        }
        Destroy(gameObject);
    }

    public override void StartTossing(Vector2 position)
    {
        if (boom_animator_ && !is_booming)
        {
            boom_animator_.Play("CountingDown", 0, 0);
            StopAllCoroutines();
        }
        base.StartTossing(position);
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
