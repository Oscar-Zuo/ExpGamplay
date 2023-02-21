using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleController : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update

    public bool is_activated_ = false;
    public Sprite spirte_;
    public float last_time_;

    private GameObject player_in_interacting_ = null;

    public float LastTime { get => last_time_; set=>last_time_=value; }
    public bool IsActivated { get => is_activated_; set => is_activated_ = value; }
    void Start()
    {
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
    }

    public void DeactivateHole()
    {
        GetComponent<SpriteRenderer>().sprite = null;
        GetComponent<Collider2D>().enabled = false;
        is_activated_ = false;
    }

    bool IInteractable.StartInteract(GameObject player)
    {
        if (!is_activated_ || player_in_interacting_)
            return false;
        player_in_interacting_ = player;
        return true;
    }

    void IInteractable.StopInteract(GameObject player)
    {
        player_in_interacting_ = null;
    }

    void IInteractable.FinishInteract(GameObject player)
    {
        DeactivateHole();
        GameManager.instance.ship_controller_.PackOneHole();
        player_in_interacting_ = null;
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            CharacterController playerController = other.GetComponent<CharacterController>();
            playerController.InteractableObjectInRange(gameObject);
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            CharacterController playerController = other.GetComponent<CharacterController>();
            playerController.InterableObjectLeaveRange(gameObject);
        }
    }
}
