using System;
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
    [NonSerialized] public List<GameObject> interactable_list_ = new List<GameObject>();
    public float cross_hair_speed_ = 10.0f;
    public float max_toss_range_ = 10.0f;
    public List<GameObject> ItemList { get => item_list_; set => item_list_ = value; }
    public bool is_using_fishing_pole_ = false;

    [SerializeField] protected GameObject ui_;
    [SerializeField] protected Collision2D interact_collision_;
    [SerializeField] protected GameObject crosshair_;
    [SerializeField] protected GameObject hook_object_;
    protected GameObject interacting_object_;
    protected bool is_tossing_ = false;
    private List<GameObject> item_list_ = new List<GameObject>();
    [SerializeField] private Transform items_parent_object_;
    private Rigidbody2D rb2D_;
    private Coroutine interact_timer_;
    private bool last_interact_input_ = false;
    private Animator animator;

    void Start()
    {
        rb2D_ = GetComponent<Rigidbody2D>();
        if (crosshair_ != null)
        {
            crosshair_.GetComponent<SpriteRenderer>().enabled = false;
        }

        ui_.SetActive(false);

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
        if (is_tossing_)
            return;

        float acceleration = is_on_boat_ ? walking_acceleration_ : swining_acceleration_;
        float max_speed = is_on_boat_ ? max_walking_speed_ : max_swining_speed_;

        // process move input
        float vertical_input, horizontal_input;
        bool is_joystick_connected = Input.GetJoystickNames().Length > 0;
        vertical_input = is_joystick_ ? is_joystick_connected? Input.GetAxis("JoystickVertical") : Input.GetAxis("Character2Vertical") : Input.GetAxis("Vertical");
        if (vertical_input != 0)
        {
            rb2D_.AddForce(new Vector2(0, 1) * vertical_input * acceleration * Time.fixedDeltaTime, ForceMode2D.Impulse);
            rb2D_.velocity = rb2D_.velocity.normalized * Mathf.Clamp(rb2D_.velocity.magnitude, 0, max_speed);
        }

        horizontal_input = is_joystick_? is_joystick_connected? Input.GetAxis("JoystickHorizontal") : Input.GetAxis("Character2Horizontal") : Input.GetAxis("Horizontal");
        if (horizontal_input != 0)
        {
            rb2D_.AddForce(new Vector2(1, 0) * horizontal_input * acceleration * Time.fixedDeltaTime, ForceMode2D.Impulse);
            rb2D_.velocity = rb2D_.velocity.normalized * Mathf.Clamp(rb2D_.velocity.magnitude, 0, max_speed);
        }

        // animator setting
        if (rb2D_.velocity.magnitude >= 0.05)
            animator.SetBool("IsMoving", true);
        else
            animator.SetBool("IsMoving", false);
        animator.SetFloat("Horizontal", rb2D_.velocity.y);
        animator.SetFloat("Vertical", rb2D_.velocity.x);
    }

    private void Update()
    {
        // process interact input
        bool is_joystick_connected = Input.GetJoystickNames().Length > 0;
        bool interact_input = is_joystick_ ? (is_joystick_connected ? Input.GetKey(KeyCode.Joystick1Button0) : Input.GetKey(KeyCode.KeypadPeriod)) : Input.GetKey(KeyCode.F);
        if (interact_input && !last_interact_input_ && !interacting_object_)
        {
            Interact();
        }
        else if (interacting_object_ && !interact_input)
        {
            StopInteract();
        }
        last_interact_input_ = interact_input;

        bool toss_input = is_joystick_ ? (is_joystick_connected ? Input.GetKey(KeyCode.Joystick1Button1) : Input.GetKey(KeyCode.KeypadEnter)) : Input.GetKey(KeyCode.Space);
        if (toss_input && !is_tossing_)
        {
            StartTossing();
        }
        if (!toss_input && is_tossing_)
        {
            FinishTossing();
        }

        if (is_tossing_)
        {
            float current_cross_hair_speed = cross_hair_speed_;
            if (CanUseFishPole())
                current_cross_hair_speed /= 2;
            float vertical_input, horizontal_input;

            vertical_input = is_joystick_ ? is_joystick_connected ? Input.GetAxis("JoystickVertical") : Input.GetAxis("Character2Vertical") : Input.GetAxis("Vertical");
            if (vertical_input != 0)
            {
                crosshair_.transform.Translate(Vector2.up * vertical_input * Time.deltaTime * current_cross_hair_speed);
            }

            horizontal_input = is_joystick_ ? is_joystick_connected ? Input.GetAxis("JoystickHorizontal") : Input.GetAxis("Character2Horizontal") : Input.GetAxis("Horizontal");
            if (horizontal_input != 0)
            {
                crosshair_.transform.Translate(Vector2.right * horizontal_input * Time.deltaTime * current_cross_hair_speed);
            }

            if (Vector2.Distance(transform.position, crosshair_.transform.position) > max_toss_range_)
            {
                Vector3 direction = (crosshair_.transform.position - transform.position).normalized;
                crosshair_.transform.position = transform.position + direction * max_toss_range_;
            }
        }
    }

    public void ReorderItemList()
    {
        ItemList.RemoveAll(s => s == null);
        for (int i = 0; i < ItemList.Count; ++i)
            ItemList[i].transform.localPosition = new Vector2(0, 0.5f * i);
    }

    bool CanUseFishPole()
    {
        return is_using_fishing_pole_ && item_list_.Count == 0;
    }

    void Interact()
    {
        if (interacting_object_)
            interacting_object_ = null;
        if (interact_timer_ != null)
            interact_timer_ = null;

        ItemList.RemoveAll(s => s == null);
        foreach (var interactable in interactable_list_)
        {
            InteractableController interactableController = interactable.GetComponent<InteractableController>();
            if (interactableController == null) continue;
            if (interactableController.StartInteract(gameObject))
            {
                interacting_object_ = interactable;
                interact_timer_ = StartCoroutine(Hold(interactableController.last_time_));
                break;
            }
        }
        ReorderItemList();
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

        interact_timer_ = null;
        interacting_object_.GetComponent<InteractableController>().FinishInteract(gameObject);
        interacting_object_ = null;

        // UI function here
    }

    public void InteractableObjectInRange(GameObject interactable)
    {
        InteractableController interactable_controller = interactable.GetComponent<InteractableController>();
        if (interactable_controller != null)
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

    public void EnterBoat()
    {
        animator.SetBool("OnBoat", true);
    }

    public void LeaveBoat()
    {
        animator.SetBool("OnBoat", false);
    }

    IEnumerator Hold(float hold_time)
    {
        float wait_time = 0;
        var slider = ui_.GetComponent<UnityEngine.UI.Slider>();
        ui_.SetActive(true);

        while (wait_time < hold_time)
        {
            wait_time += Time.deltaTime;
            if (slider != null)
                slider.value = wait_time / hold_time;
            yield return null;
        }
        ui_.SetActive(false);
        FinishInteract();
    }

    public void PickupItem(GameObject item)
    {
        if (item == null) return;
        if (interactable_list_.Contains(item))
            interactable_list_.Remove(item);
        item.transform.parent = items_parent_object_;
        item.transform.localPosition = new Vector2(0, 0.5f * (item_list_.Count -1));
        item.GetComponent<ItemController>().is_activated_ = false;
        item.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        item_list_.Add(item);
    }

    void DropItem(GameObject item)
    {
        ItemController itemController = item.GetComponent<ItemController>();
        itemController.is_activated_ = true;
        itemController.Landed(false);
        item.transform.parent = null;
        item.transform.position = transform.position;
    }

    void StartTossing()
    {
        is_tossing_ = true;
        crosshair_.transform.localPosition = Vector2.zero;
        crosshair_.GetComponent<SpriteRenderer>().enabled = true;
        if (CanUseFishPole())
            crosshair_.GetComponent<SpriteRenderer>().color = Color.blue;
        else
            crosshair_.GetComponent<SpriteRenderer>().color = Color.red;
    }

    void TossOrDropItem()
    {
        GameObject item = null;
        while (!item && item_list_.Count > 0)
        {
            item = item_list_[item_list_.Count - 1];
            item_list_.Remove(item);
        } 

        if (!item)
            return;
        if (Vector2.Distance(transform.position, crosshair_.transform.position) <= 0.1)
        {
            DropItem(item);
        }
        else
        {
            item.GetComponent<ItemController>().is_activated_ = true;
            item.transform.parent = null;
            item.GetComponent<ItemController>().StartTossing(crosshair_.transform.position);
        }
    }

    void FinishTossing()
    {
        if (CanUseFishPole())
        {
            GameObject hook= Instantiate(hook_object_, transform.position, Quaternion.identity);
            HookController hook_controller = hook.GetComponent<HookController>();
            hook_controller.StartTossing(crosshair_.transform.position);
            hook_controller.start_position_ = transform.position;
        }
        else
            TossOrDropItem();

        is_tossing_ = false;
        crosshair_.transform.localPosition = Vector2.zero;
        crosshair_.GetComponent<SpriteRenderer>().enabled = false;
    }
}
