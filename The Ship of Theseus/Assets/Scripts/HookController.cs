using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class HookController : ItemController
{
    // Start is called before the first frame update
    public Vector2 start_position_;
    public float pulling_back_speed_ = 5;
    private LineRenderer line_renderer_;
    private List<GameObject> collected_item_list_ = new List<GameObject>();
    private Collider2D collection_collider_;

    void Start()
    {
        is_activated_ = true;
        line_renderer_ = GetComponent<LineRenderer>();
        line_renderer_.SetPosition(0, start_position_);
    }

    // Update is called once per frame
    void Update()
    {
        line_renderer_.SetPosition(1, transform.position);
    }
    
    IEnumerator PullingBack()
    {
        while(true)
        {
            Vector3 direction = start_position_ - new Vector2(transform.position.x, transform.position.y);
            if (direction.magnitude <= 0.2)
            {
                DropAllItem();
                yield return new WaitForEndOfFrame();
                Destroy(gameObject);
            }
            direction.Normalize();
            transform.position += direction * pulling_back_speed_ * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    override public void Landed(bool tossed)
    {
        base.Landed(tossed);
        StartCoroutine(PullingBack());
    }

    override protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Item"))
            CollectItem(other.gameObject);
    }

    override protected void OnTriggerExit2D(Collider2D other){ } // No interaction with player

    void CollectItem(GameObject item)
    {
        if (item == null || !item.tag.Equals("Item")) return;
        ItemController itemController = item.GetComponent<ItemController>();
        if (itemController == null || !itemController.is_activated_)
            return;
        FloatingController floating_controller = item.GetComponent<FloatingController>();
        if (floating_controller)
            floating_controller.Deactivate();
        item.transform.parent = transform;
        item.transform.localPosition = new Vector2(0, 0.5f * collected_item_list_.Count);
        itemController.is_activated_ = false;
        item.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        collected_item_list_.Add(item);
    }

    void DropAllItem()
    {
        if (collected_item_list_.Count == 0)
            return;
        foreach (GameObject item in collected_item_list_)
        {
            ItemController itemController = item.GetComponent<ItemController>();
            itemController.is_activated_ = true;
            item.transform.parent = null;
            item.transform.position = transform.position;
            itemController.Landed(false);
        }
    }
}
