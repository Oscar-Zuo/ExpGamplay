using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    [SerializeField] protected float backwardSpeed = 5, maxSpeed = 15, minSpeed = 3;
    [SerializeField] protected int maxHealth = 5;
    [SerializeField] protected float playerDamage = 1;
    [SerializeField] protected float fireRateModifier = 1;
    protected int health;
    public float invincibleTime = 1;
    protected Vector2 speed;
    Vector3 oldPosition;
    bool invincible = false;
    BoxCollider2D boxCollider;
    private List<GameObject> decoractionsList = new List<GameObject>();
    Transform decoractionsContainer; 
    [SerializeField] private WeaponsSlotsController weaponsSlotsController;
    [SerializeField] protected float forwardSpeed = 7, rotationSpeed = 180;

    GameManager gameManager;
    private Rigidbody2D rb2D;

    public Vector2 Speed { get => speed; set => speed = value; }
    public float ForwardSpeed { get => forwardSpeed; set => forwardSpeed = Mathf.Clamp(value, minSpeed, maxSpeed); }
    public float BackwardSpeed { get => backwardSpeed; set => backwardSpeed = Mathf.Clamp(value, minSpeed, maxSpeed); }
    public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }
    public int Health 
    { 
        get => health;
        set 
        {
            health = Mathf.Clamp(value, 0, maxHealth);
            if (health <= 0)
            {
                gameManager.GameOver();
            }
        }
    }

    public float PlayerDamage { get => playerDamage; set => playerDamage = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float FireRateModifier { get => fireRateModifier; set => fireRateModifier = value; }

    // Start is called before the first frame update
    void Start()
    {
        Health = MaxHealth;
        oldPosition=transform.position;
        rb2D = GetComponent<Rigidbody2D>();
        boxCollider= GetComponent<BoxCollider2D>();
        decoractionsContainer = transform.Find("Decoractions");
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();  
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 translation;
        if (Input.GetAxis("Vertical") > 0)
        {
            translation = Input.GetAxis("Vertical") * Time.fixedDeltaTime * transform.up * ForwardSpeed;
            rb2D.MovePosition(rb2D.position + translation);
        }
            
        else if (Input.GetAxis("Vertical") < 0)
        {
            translation = Input.GetAxis("Vertical") * Time.fixedDeltaTime * transform.up * backwardSpeed;
            rb2D.MovePosition(rb2D.position + translation);
        }

        float rotation = -Input.GetAxis("Horizontal") * Time.fixedDeltaTime;
        // rotate tank
        rb2D.MoveRotation(rb2D.rotation + rotation * RotationSpeed);

        // Get speed for bullets
        speed = transform.up * Vector3.Distance(oldPosition, transform.position);
        oldPosition = transform.position;
    }

    public void OnHit(int Damage)
    {
        // not in invincible
        if (!invincible)
        {
            Health -= Damage;
            invincible = true;
            // player become invincible
            GetComponent<FlashingController>().StartFlashing();
            StartCoroutine(ESetInvincible());
        }
    }

    public IEnumerator ESetInvincible()
    {
        invincible = true;
        yield return new WaitForSeconds(invincibleTime);
        invincible = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Item"))
        {
            GameObject item = collision.gameObject;
            ItemController itemController = item.GetComponent<ItemController>();
            switch (itemController.ItemType)
            {
                case EItemType.Weapon:
                    weaponsSlotsController.AddWeapon(itemController.ItemObject);
                    itemController.ActivateItemEffects();
                    break;
                case EItemType.Decoraction:
                    itemController.ActivateItemEffects();
                    AddDecoraction(itemController.ItemObject);
                    break;
            }
            Destroy(item);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            GameObject enemy = collision.gameObject;
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            OnHit(enemyController.TouchDamage);
        }
    }

    public void AddDecoraction(GameObject decoraction)
    {
        int index = decoractionsList.Count;
        Vector2 position = new Vector2((index % 5) * 0.4f  - boxCollider.size.x/2
            , boxCollider.size.y / 2 - index / 5 * 0.4f);
        GameObject instantiatedDecoration = Instantiate(decoraction, decoractionsContainer);
        instantiatedDecoration.transform.localPosition = position;
        decoractionsList.Add(instantiatedDecoration);
    }
}
