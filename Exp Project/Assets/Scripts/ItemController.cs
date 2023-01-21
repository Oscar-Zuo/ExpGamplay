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
    public EItemType ItemType { get => itemType; set => itemType = value; }
    public GameObject ItemObject { get => itemObject; set => itemObject = value; }
    public float LifeTime { get => lifeTime; set => lifeTime = value; }

    void Start()
    {
        SelfDestruct();
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
