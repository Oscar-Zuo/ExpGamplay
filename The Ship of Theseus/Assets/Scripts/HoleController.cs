using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleController : InteractableController
{
    // Start is called before the first frame update

    public Sprite spirte_;

    private GameObject player_in_interacting_ = null;
    private AudioSource audio_source_;

    void Start()
    {
        audio_source_ = GetComponent<AudioSource>();
        if (!is_activated_)
        {
            DeactivateHole();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateHole()
    {
        GetComponent<SpriteRenderer>().sprite = spirte_;
        GetComponent<Collider2D>().enabled = true;
        is_activated_ = true;
        audio_source_.Play();
    }

    public void DeactivateHole()
    {
        GetComponent<SpriteRenderer>().sprite = null;
        GetComponent<Collider2D>().enabled = false;
        is_activated_ = false;
        audio_source_.Stop();
    }

    override public bool StartInteract(GameObject player)
    {
        if (!is_activated_ || player_in_interacting_)
            return false;

        bool has_plank = false;
        var player_items = player.GetComponent<CharacterController>().ItemList;
        foreach (var item in player_items)
        {
            var item_controller = item.GetComponent<ItemController>();
            if (item_controller && item_controller.ItemName == "RepairKit")
            {
                has_plank = true;
                break;
            }
        }
        if (!has_plank)
            return false;

        player_in_interacting_ = player;
        return true;
    }

    override public void StopInteract(GameObject player)
    {
        player_in_interacting_ = null;
    }

    override public void FinishInteract(GameObject player)
    {
        DeactivateHole();
        GameManager.instance_.ship_controller_.PackOneHole();
        var player_items = player.GetComponent<CharacterController>().ItemList;
        foreach ( var item in player_items )
        {
            var item_controller = item.GetComponent<ItemController>();
            if (item_controller && item_controller.ItemName == "RepairKit")
            {
                player_items.Remove(item);
                Destroy(item);
                break;
            }
        }

        player_in_interacting_ = null;
    }
}
