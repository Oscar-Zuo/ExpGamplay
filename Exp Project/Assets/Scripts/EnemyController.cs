using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    //GameObject player;
    [SerializeField] protected float speed = 4;
    [SerializeField] protected float rotateSpeed = 5;
    [SerializeField] protected float health = 10;
    [SerializeField] protected int touchDamage = 1;
    [SerializeField] protected List<GameObject> itemList;
    protected int numOfEnemies = 1;
    protected float statusModifier = 1;
    private EnemyController combinedObject = null;
    private Vector3 originScale;
    //private GameManager gameManager;
    private Rigidbody2D rb2D;

    public int TouchDamage { get => touchDamage; set => touchDamage = value; }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
;
        // this enemy drops item
        if (Random.value <= GameManager.Instance.enemyDropItemChance)
        {
            itemList.Add(GameManager.Instance.GetRandomItem());
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        originScale = transform.localScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 playerPostion = GameManager.Instance.playerController.transform.position;
        float lookAtAngle = Vector2.SignedAngle(rb2D.transform.up, playerPostion - rb2D.position);
        rb2D.MoveRotation(rb2D.rotation + lookAtAngle * Time.fixedDeltaTime * rotateSpeed);
        rb2D.AddForce((playerPostion - rb2D.position).normalized * numOfEnemies * numOfEnemies, ForceMode2D.Impulse);
        rb2D.velocity = rb2D.velocity.normalized * speed;
    }

    public void GetHit(float damage, float knockBackForce, GameObject damageSource)
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
        foreach (var item in itemList)
        {
            if (item != null)
                Instantiate(item, transform.position - new Vector3(0, 0, 1), Quaternion.identity);
        }
        Destroy(gameObject);
    }

    protected void AskForCombine(EnemyController otherEnemy)
    {
        combinedObject = otherEnemy;
        Combine(otherEnemy);
    }

    protected void Combine(EnemyController otherEnemy)
    {
        numOfEnemies += otherEnemy.numOfEnemies;
        statusModifier = Mathf.Pow(numOfEnemies, 1f / 3f);
        health += otherEnemy.health + 1;
        rb2D.mass += otherEnemy.rb2D.mass;
        transform.localScale = originScale * Mathf.Clamp(statusModifier, 1, 3);
        if (otherEnemy.itemList.Count > 0)
        {
            itemList.AddRange(otherEnemy.itemList);
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }

        // have some chance to generate item when combines
        if (Random.value <= GameManager.Instance.enemyDropItemChance * statusModifier)
        {
            itemList.Add(GameManager.Instance.GetRandomItem());
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        // combine enemies
        if (collision.gameObject.tag == "Enemy")
        {
            EnemyController otherEnemyController = collision.gameObject.GetComponent<EnemyController>();

            if (combinedObject is null)
            {
                otherEnemyController.AskForCombine(this);
                Destroy(gameObject);
            }
        }
    }
}
