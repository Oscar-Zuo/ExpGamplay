using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ShipController : MonoBehaviour
{
    public float max_health_ = 100;
    public List<GameObject> hole_list_;
    public float decrease_health_speed_ = 5.0f;
    public List<Transform> bomb_landing_location_list_;

    protected float water_surface_;
    protected int activated_holes_num_ = 0;
    protected float health_;
    public float Health
    {
        get => health_;
        set
        {
            health_ = Mathf.Clamp(value, 0, max_health_);
            UpdateHealth();
        }
    }

    public float WaterSurface { get => water_surface_; set => water_surface_ = value; }

    // Start is called before the first frame update
    void Start()
    {
        health_ = max_health_;
        water_surface_ = transform.position.y - GetComponent<SpriteRenderer>().bounds.size.y / 2;
    }

    void UpdateHealth()
    {
        if (health_ <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }

        int holes_to_generate_num = (int)((max_health_ - health_) / max_health_ * (hole_list_.Count + 1)) - activated_holes_num_;
        if (holes_to_generate_num> 0)
        {
            GenerateHoles(holes_to_generate_num);
        }
    }

    void GenerateHoles(int num)
    {
        List<HoleController> inactive_hole_list = new List<HoleController>();

        foreach (GameObject hole in hole_list_)
        {
            HoleController hole_controller = hole.GetComponent<HoleController>();
            if (hole_controller != null && !hole_controller.is_activated_)
            {
                inactive_hole_list.Add(hole_controller);
            }
        }

        for (int i = 0; i < num; i++)
        {
            if (inactive_hole_list.Count <= 0)
                break;
            HoleController temp = inactive_hole_list[UnityEngine.Random.Range(0, inactive_hole_list.Count - 1)];
            temp.ActivateHole();
            inactive_hole_list.Remove(temp);
            activated_holes_num_++;
        }
    }

    public void PackOneHole()
    {
        health_ += max_health_ / hole_list_.Count;
        --activated_holes_num_;
    }

    // Update is called once per frame
    void Update()
    {
        Health -= decrease_health_speed_ * Time.deltaTime;
    }

    public bool IsOnShip(Vector2 position)
    {
        Collider2D item_collider = GetComponent<PolygonCollider2D>();
        if (!item_collider)
            return false;

        return item_collider.OverlapPoint(position);
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
