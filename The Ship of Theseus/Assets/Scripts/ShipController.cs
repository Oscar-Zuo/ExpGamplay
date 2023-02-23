using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShipController : MonoBehaviour
{
    public float max_health = 100;
    public List<GameObject> hole_list;
    public float decrease_health_speed = 5.0f;

    protected float water_surface_;
    protected int activated_holes_num = 0;
    protected float health;
    public float Health
    {
        get => health;
        set
        {
            health = Mathf.Clamp(value, 0, max_health);
            UpdateHealth();
        }
    }

    public float WaterSurface { get => water_surface_; set => water_surface_ = value; }

    // Start is called before the first frame update
    void Start()
    {
        health = max_health;
        water_surface_ = transform.position.y - GetComponent<SpriteRenderer>().bounds.size.y / 2;
    }

    void UpdateHealth()
    {
        if (health <= 0)
        {
            // TODO:: call game over function
        }

        int holes_to_generate_num = (int)((max_health - health) / max_health * (hole_list.Count + 1)) - activated_holes_num;
        if (holes_to_generate_num> 0)
        {
            GenerateHoles(holes_to_generate_num);
        }
    }

    void GenerateHoles(int num)
    {
        List<HoleController> inactive_hole_list = new List<HoleController>();

        foreach (GameObject hole in hole_list)
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
            activated_holes_num++;
        }
    }

    public void PackOneHole()
    {
        health += max_health / hole_list.Count;
        --activated_holes_num;
    }

    // Update is called once per frame
    void Update()
    {
        Health -= decrease_health_speed * Time.deltaTime;
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
