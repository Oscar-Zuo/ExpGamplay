using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // Start is called before the first frame update

    public float walking_acceleration_ = 3.0f;
    public float swining_acceleration_ = 1.5f;
    public float max_walking_speed_ = 3.0f;
    public float max_swining_speed_ = 2.0f;
    public bool is_on_boat_ = true;
    public bool is_joystick_ = false;

    [SerializeField] protected Collision2D interactCollision;
    protected GameObject interacting_object_;
    public List<GameObject> interactable_list_ = new List<GameObject>();
    private List<GameObject> item_list_ = new List<GameObject>();
    [SerializeField] private Transform items_parent_object_;
    private Rigidbody2D rb2D_;
    private Coroutine interact_timer_;

    public List<GameObject> ItemList { get => item_list_; set => item_list_ = value; }

    void Start()
    {
        rb2D_= GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
        float acceleration = is_on_boat_ ? walking_acceleration_ : swining_acceleration_;
        float max_speed = is_on_boat_ ? max_walking_speed_ : max_swining_speed_;

        // process move input
        float vertical_input, horizontal_input;
        vertical_input = is_joystick_ ? Input.GetAxis("JoystickVertical") : Input.GetAxis("Vertical");
        if (vertical_input != 0)
        {
            rb2D_.AddForce(new Vector2(0, 1) * vertical_input * acceleration * Time.fixedDeltaTime, ForceMode2D.Impulse);
            rb2D_.velocity = rb2D_.velocity.normalized * Mathf.Clamp(rb2D_.velocity.magnitude, 0, max_speed);
        }

        horizontal_input = is_joystick_ ? Input.GetAxis("JoystickHorizontal"): Input.GetAxis("Horizontal");
        if (horizontal_input != 0)
        {
            rb2D_.AddForce(new Vector2(1, 0) * horizontal_input * acceleration * Time.fixedDeltaTime, ForceMode2D.Impulse);
            rb2D_.velocity = rb2D_.velocity.normalized * Mathf.Clamp(rb2D_.velocity.magnitude, 0, max_speed);
        }
    }

    private void Update()
    {
        // process interact input
        bool interact_input = is_joystick_ ? Input.GetKey(KeyCode.Joystick1Button0) : Input.GetKey(KeyCode.F);
        if (interact_input && !interacting_object_)
        {
            Interact();
        }
        else if (interacting_object_ && !interact_input)
        {
            StopInteract();
        }
    }

    void Interact()
    {
        if (interacting_object_)
            interacting_object_ = null;
        if (interact_timer_ != null)
             interact_timer_ = null;

        foreach (var interactable in interactable_list_)
        {
            if (interactable == null) continue;
            InteractableController interactableController = interactable.GetComponent<InteractableController>();
            if (interactableController == null) continue;
            if (interactableController.StartInteract(gameObject))
            {
                interacting_object_ = interactable;
                interact_timer_ = StartCoroutine(Hold(interactableController.last_time_));
                break;
            }
        }
    }

    void StopInteract()
    {
        if (!interacting_object_)
            return;
        if (interact_timer_ == null)
            return;

        StopCoroutine(interact_timer_);
        interacting_object_.GetComponent<InteractableController>().StopInteract(gameObject);
        interact_timer_ = null;
        interacting_object_ = null;
    }

    void FinishInteract()
    {
        if (!interacting_object_)
            return;
        
        interact_timer_=null;
        interacting_object_.GetComponent<InteractableController>().FinishInteract(gameObject);
        interacting_object_=null;

        // UI function here
    }

    public void InteractableObjectInRange(GameObject interactable)
    {
        InteractableController interactable_controller = interactable.GetComponent<InteractableController>();
        if (interactable_controller != null && interactable_controller.is_activated_)
        {
            interactable_list_.Add(interactable);
        }
    }

    public void InterableObjectLeaveRange(GameObject interactable)
    {
        if (interactable_list_.Contains(interactable))
        {
            if (interacting_object_ == interactable)
                StopInteract();
            interactable_list_.Remove(interactable);
        }
    }

    IEnumerator Hold(float hold_time)
    {
        yield return new WaitForSeconds(hold_time);
        FinishInteract();
    }

    public void PickupItem(GameObject item)
    {
        if (item == null) return;
        item.transform.parent = items_parent_object_;
        item.transform.localPosition = Vector2.zero;
        item.GetComponent<ItemController>().is_activated_ = false;
        item_list_.Add(item);
    }
}
