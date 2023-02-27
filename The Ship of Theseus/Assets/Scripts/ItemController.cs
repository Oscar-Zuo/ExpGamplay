using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : InteractableController
{
    [SerializeField] protected string item_name_;
    public bool is_tossing_ = false;
    public float object_speed_ = 5f;
    public float toss_height_ = 3f;

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

    virtual public void StartTossing(Vector2 position)
    {
        is_tossing_ = true;
        StartCoroutine(Toss(position));
    }

    protected IEnumerator Toss(Vector2 targetPosition)
    {
        float initital_distance = Vector2.Distance(transform.position, targetPosition);
        Vector2 delta_position = targetPosition - new Vector2(transform.position.x, transform.position.y);
        float angle = Vector2.Angle(delta_position, Vector2.right) / 180 * Mathf.PI;
        if (delta_position.y<0)
            angle = 2 * Mathf.PI - angle;
        float x_speed = Mathf.Cos(angle) * object_speed_;
        float y_speed = Mathf.Sin(angle) * object_speed_;
        float required_time = initital_distance / object_speed_;
        float total_time = 0;

        while (total_time<required_time)
        {
            total_time += Time.deltaTime;
            float delta_y = Mathf.Cos(total_time / required_time * Mathf.PI) * toss_height_ * Mathf.PI + y_speed;
            transform.position += new Vector3(x_speed * Time.deltaTime, delta_y * Time.deltaTime, 0);
            yield return null;
        }
        is_tossing_ = false;

        Landed(true);
    }

    virtual public void Landed(bool tossed)
    {
        FloatingController floatingController = GetComponent<FloatingController>();
        if (GameManager.instance_.ship_controller_.IsOnShip(transform.position))
        {
            if (floatingController != null)
            {
                floatingController.Deactivate();
            }
            GetComponent<SpriteRenderer>().sortingLayerName = "Default";
            transform.parent = GameManager.instance_.ship_controller_.gameObject.transform;
        }
        else
        {
            var pirate = GameManager.GetLandedPirateShip(transform.position);
            if (pirate != null)
            {
                if (floatingController != null)
                {
                    floatingController.Deactivate();
                }
                GetComponent<SpriteRenderer>().sortingLayerName = "Default";
                transform.parent = pirate.transform;
            }

            else if (floatingController != null)
            {
                floatingController.Activate();
                GetComponent<SpriteRenderer>().sortingLayerName = "BehindBoat";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
