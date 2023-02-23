using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : InteractableController
{
    [SerializeField] protected string item_name_;

    public string ItemName { get => item_name_; set => item_name_ = value; }

    override public void FinishInteract(GameObject player)
    {
        player.GetComponent<CharacterController>().PickupItem(gameObject);
        FloatingController floating_controller = GetComponent<FloatingController>();
        if (floating_controller)
        {
            floating_controller.Deactivate();
        }
    }

    override public bool StartInteract(GameObject player)
    {
        return is_activated_;
    }

    override public void StopInteract(GameObject player)
    {
        return;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
