using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableController : MonoBehaviour
{
    public bool is_activated_ = false;
    public float last_time_ = 0;

    virtual public bool StartInteract(GameObject player)
    {
        return false;
    }

    virtual public void StopInteract(GameObject player) { }

    virtual public void FinishInteract(GameObject player) { }

    virtual protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            CharacterController playerController = other.GetComponent<CharacterController>();
            playerController.InteractableObjectInRange(gameObject);
        }
    }

    virtual protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            CharacterController playerController = other.GetComponent<CharacterController>();
            playerController.InterableObjectLeaveRange(gameObject);
        }
    }
}
