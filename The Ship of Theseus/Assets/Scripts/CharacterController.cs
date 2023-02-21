using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    float LastTime { get; set; }
    bool IsActivated{get; set; }
    bool StartInteract(GameObject player);
    void StopInteract(GameObject player);
    void FinishInteract(GameObject player);
}

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
    protected List<GameObject> interactable_list_ = new List<GameObject>();
    private Rigidbody2D rb2D_;

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
        foreach (var interactable in interactable_list_)
        {
            if (interactable == null) continue;
            IInteractable interactableController = interactable.GetComponent<IInteractable>();
            if (interactableController != null) continue;
            if (interactableController.StartInteract(gameObject))
            {
                interacting_object_ = interactable;
                break;
            }
        }
    }

    void StopInteract()
    {
        
    }

    public void InteractableObjectInRange(GameObject interactable)
    {
        IInteractable interactable_controller = interactable.GetComponent<IInteractable>();
        if (interactable_controller != null && interactable_controller.IsActivated)
        {
            interactable_list_.Add(interactable);
        }
    }

    public void InterableObjectLeaveRange(GameObject interactable)
    {
        if (interactable_list_.Contains(interactable))
        {
            interactable_list_.Remove(interactable);
        }
    }
}
