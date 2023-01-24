using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    //GameObject player;
    [SerializeField] protected float speed = 4;
    [SerializeField] protected float health = 10;
    [SerializeField] protected int touchDamage = 1;
    [SerializeField] protected List<GameObject> itemList;
    protected int numOfEnemies = 1;
    protected float statusModifier = 1;
    private GameObject combinedObject = null;
    private Vector3 originScale;
    //private GameManager gameManager;
    private Rigidbody2D rb2D;

    public int TouchDamage { get => touchDamage; set => touchDamage = value; }

    void Start()
    {
        //animator=GetComponent<Animator>();
        //player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        //gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        // this enemy drops item
        if (Random.value <= GameManager.instance.enemyDropItemChance)
        {
            itemList.Add(GameManager.instance.GetRandomItem());
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        originScale = transform.localScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 playerPostion = GameManager.instance.player.GetComponent<Rigidbody2D>().position;
        float lookAtAngle = Vector2.SignedAngle(rb2D.transform.up, playerPostion - rb2D.position);
        rb2D.MoveRotation(rb2D.rotation+ lookAtAngle * Time.fixedDeltaTime * 5);
        rb2D.AddForce((playerPostion - rb2D.position).normalized * numOfEnemies * numOfEnemies, ForceMode2D.Impulse);
        rb2D.velocity = rb2D.velocity.normalized * Mathf.Clamp(speed * statusModifier, 0, GameManager.instance.playerController.ForwardSpeed);
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
        if (itemList!= null&&itemList.Count>0)
        {
            foreach (var item in itemList)
            {
                Instantiate(item, transform.position-new Vector3(0,0,1), Quaternion.identity);
            }
        }
        Destroy(gameObject);
    }

    protected void AskForDestroy(GameObject fromObject)
    {
        if (fromObject == combinedObject)
            Destroy(gameObject);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        // combine enemies
        GameObject collisionObject = collision.gameObject;
        if (collision.gameObject.tag == "Enemy")
        {
            EnemyController otherEnemyController = collisionObject.GetComponent<EnemyController>();
            numOfEnemies += otherEnemyController.numOfEnemies;
            statusModifier = Mathf.Pow(numOfEnemies, 1f / 3f);
            health += otherEnemyController.health + 1;
            rb2D.mass += otherEnemyController.rb2D.mass;
            transform.localScale = originScale * Mathf.Clamp(statusModifier, 1, 3);
            if (otherEnemyController.itemList.Count>0)
            {
                itemList.AddRange(otherEnemyController.itemList);
                GetComponent<SpriteRenderer>().color = Color.yellow;
            }


            // have some chance to generate item when combines
            if (Random.value <= GameManager.instance.enemyDropItemChance /1.5*statusModifier)
            {
                itemList.Add(GameManager.instance.GetRandomItem());
                GetComponent<SpriteRenderer>().color = Color.yellow;
            }

            combinedObject = collisionObject;
            otherEnemyController.AskForDestroy(gameObject);
        }
    }
}
