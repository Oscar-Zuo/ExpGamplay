using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType
{
    Weapon,
    Decoraction
}

public class ItemController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] protected EItemType itemType;
    [SerializeField] protected GameObject itemObject;
    [SerializeField] protected float lifeTime = 30;
    protected TankController playerController;

    public EItemType ItemType { get => itemType; set => itemType = value; }
    public GameObject ItemObject { get => itemObject; set => itemObject = value; }
    public float LifeTime { get => lifeTime; set => lifeTime = value; }

    virtual protected void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<TankController>();
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        lifeTime-=Time.deltaTime;
        if (lifeTime<=0)
        {
            Destroy(gameObject);
        }
    }

    virtual protected void ActivateItemEffects()
    {

    }
}
