using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class ProjectileController : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed = 30, maxLifeTime = 3;
    public float damage = 1;
    [SerializeField] protected float knockBackForce = 1;
    [SerializeField] protected bool penetration = false;
    private float lifetime;
    GameObject player;

    public bool Penetration { get => penetration; set => penetration = value; }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        lifetime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        lifetime += Time.deltaTime;
        // if the bullet reached its lifetime, it will be destory to save resources
        if (lifetime>=maxLifeTime)
        {
            Destroy(gameObject);
        }
        Move();
    }

    public virtual void Move()
    {
        // transform.Translate(Vector3.up * Time.deltaTime * speed + transform.InverseTransformDirection(player.GetComponent<TankController>().Speed * 0.5f));
        transform.Translate(Vector3.up * Time.deltaTime * speed);
    }

    protected virtual void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag.Equals("Enemy"))
        {
            EnemyController controller = collider.GetComponent<EnemyController>();
            controller.GetHit(damage, knockBackForce,  gameObject);
            if (!penetration)
            {
                Destroy(gameObject);
            }
        }
        return;
    }

    public virtual float GetDamage()
    {
        return damage;
    }
}
