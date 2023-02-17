using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // Start is called before the first frame update

    public float walking_acceleration = 3.0f;
    public float swining_acceleration = 1.5f;
    public float max_walking_speed = 3.0f;
    public float max_swining_speed = 2.0f;
    public bool is_on_boat = true;
    public float max_health = 100;

    protected float health;
    private Rigidbody2D rb2D;

    public float Health 
    { 
        get => health; set 
        {
            health = Mathf.Clamp(value, 0, max_health);
            UpdateHealth();
        } 
    }

    void UpdateHealth()
    {
        // TODO:: generate holes
    }

    void Start()
    {
        rb2D= GetComponent<Rigidbody2D>();
        Physics2D.gravity = Vector2.zero;
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
        float acceleration = is_on_boat ? walking_acceleration : swining_acceleration;
        float max_speed = is_on_boat ? max_walking_speed : max_swining_speed;

        float vertical_input, horizontal_input;
        vertical_input = Input.GetAxis("Vertical");
        if (vertical_input != 0)
        {
            rb2D.AddForce(new Vector2(0, 1) * vertical_input * acceleration * Time.fixedDeltaTime, ForceMode2D.Impulse);
            rb2D.velocity = rb2D.velocity.normalized * Mathf.Clamp(rb2D.velocity.magnitude, 0, max_speed);
        }

        horizontal_input = Input.GetAxis("Horizontal");
        if (horizontal_input != 0)
        {
            rb2D.AddForce(new Vector2(1, 0) * horizontal_input * acceleration * Time.fixedDeltaTime, ForceMode2D.Impulse);
            rb2D.velocity = rb2D.velocity.normalized * Mathf.Clamp(rb2D.velocity.magnitude, 0, max_speed);
        }
    }
}
