using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    GameObject player;
    [SerializeField] protected float speed = 4;
    [SerializeField] protected float health = 10;
    [SerializeField] protected int touchDamage = 1;
    private Rigidbody2D rb2D;

    public int TouchDamage { get => touchDamage; set => touchDamage = value; }

    void Start()
    {
        //animator=GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 playerPostion = player.GetComponent<Rigidbody2D>().position;
        float lookAtAngle = Vector2.SignedAngle(rb2D.transform.up, playerPostion - rb2D.position);
        rb2D.MoveRotation(rb2D.rotation+ lookAtAngle * Time.fixedDeltaTime * 5);
        rb2D.AddForce((playerPostion - rb2D.position).normalized, ForceMode2D.Impulse);
        rb2D.velocity = rb2D.velocity.normalized * speed;
        //transform.Translate(Vector3.up * Time.deltaTime * speed);
    }

    public void GetHit(float damage,float knockBackForce, GameObject damageSource)
    {
        health -= damage;
        //Vector2 forceDirection = damageSource.transform.position - transform.position;
        rb2D.AddForce(-rb2D.transform.up * knockBackForce, ForceMode2D.Impulse);

        if (health < 0)
        {
            Dead();
        }
    }

    public void Dead()
    {
        animator.SetTrigger("Dead");
        Destroy(gameObject);
    }
}
