using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    // Start is called before the first frame update
    public float explosiveLastTime = 0.5f;
    public float damageModifier = 1;
    public float knockBackForce = 0;
    float playerDamage;
    void Start()
    {
        playerDamage = GameManager.Instance.playerController.PlayerBaseDamage;
        Destroy(gameObject, explosiveLastTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag.Equals("Enemy"))
        {
            EnemyController controller = collider.GetComponent<EnemyController>();
            controller.GetHit(GetDamage(), knockBackForce, gameObject);
        }
        return;
    }

    public virtual float GetDamage()
    {
        return playerDamage * damageModifier;
    }
}
