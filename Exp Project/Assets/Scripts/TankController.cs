using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    [SerializeField] protected float backwardSpeed = 5, maxSpeed = 15, minSpeed = 3;
    [SerializeField] protected int maxHealth = 5;
    [SerializeField] protected float playerBaseDamage = 1;
    [SerializeField] protected float fireRateModifier = 1;
    protected int health;
    bool invincible = false;
    [SerializeField] private float invincibleTime = 1;
    [SerializeField] private BoxCollider2D boxCollider;
    private List<GameObject> decoractionsList = new List<GameObject>();
    [SerializeField] private Transform decoractionsContainer; 
    [SerializeField] private WeaponsSlotsController weaponsSlotsController;
    [SerializeField] private FlashingController flashingController;
    [SerializeField] protected float forwardSpeed = 7, rotationSpeed = 180;

    [SerializeField] private Rigidbody2D rb2D;

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
                GameManager.Instance.GameOver();
            }
        }
    }

    public float PlayerBaseDamage { get => playerBaseDamage; set => playerBaseDamage = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float FireRateModifier { get => fireRateModifier; set => fireRateModifier = value; }

    // Start is called before the first frame update
    void Start()
    {
        Health = MaxHealth;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 translation;
        float verticalInput = Input.GetAxis("Vertical");
        if (verticalInput > 0)
        {
            translation = verticalInput * Time.fixedDeltaTime * transform.up * ForwardSpeed;
            rb2D.MovePosition(rb2D.position + translation);
        }
            
        else if (Input.GetAxis("Vertical") < 0)
        {
            translation = backwardSpeed * Input.GetAxis("Vertical") * Time.fixedDeltaTime * transform.up;
            rb2D.MovePosition(rb2D.position + translation);
        }

        float rotation = -Input.GetAxis("Horizontal") * Time.fixedDeltaTime;
        // rotate tank
        rb2D.MoveRotation(rb2D.rotation + rotation * RotationSpeed);
    }

    public void OnHit(int Damage)
    {
        // not in invincible
        if (!invincible)
        {
            Health -= Damage;
            invincible = true;
            // player become invincible
            flashingController.StartFlashing();
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
        if (collision.gameObject.CompareTag("Item"))
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
                    AddDecoraction(itemController.gameObject);
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
