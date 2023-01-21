using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    [SerializeField] protected float forwardSpeed = 7, rotationSpeed = 180, backwardSpeed = 5;
    [SerializeField] protected int maxHealth = 5;
    protected int health;
    public float invincibleTime = 1;
    protected Vector2 speed;
    Vector3 oldPosition;
    bool invincible = false;
    private Rigidbody2D rb2D;
    [SerializeField] private WeaponsSlotsController weaponsSlotsController;

    public Vector2 Speed { get => speed; set => speed = value; }

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        oldPosition=transform.position;
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 translation;
        if (Input.GetAxis("Vertical") > 0)
        {
            translation = Input.GetAxis("Vertical") * Time.fixedDeltaTime * transform.up * forwardSpeed;
            rb2D.MovePosition(rb2D.position + translation);
        }
            
        else if (Input.GetAxis("Vertical") < 0)
        {
            translation = Input.GetAxis("Vertical") * Time.fixedDeltaTime * transform.up * backwardSpeed;
            rb2D.MovePosition(rb2D.position + translation);
        }

        float rotation = -Input.GetAxis("Horizontal") * Time.fixedDeltaTime;
        // rotate tank
        rb2D.MoveRotation(rb2D.rotation + rotation * rotationSpeed);

        // Get speed for bullets
        speed = transform.up * Vector3.Distance(oldPosition, transform.position);
        oldPosition = transform.position;
    }

    public void OnHit(int Damage)
    {
        // not in invincible
        if (!invincible)
        {
            health -= Damage;
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
            if (itemController.ItemType == EItemType.Weapon)
            {
                weaponsSlotsController.AddWeapon(itemController.ItemObject);
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
}
