using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingController : MonoBehaviour
{
    public bool is_floating_;
    public float height_ = 1;
    public float horizontal_speed_ = 1.5f;
    public float vertical_speed_ = 5f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!is_floating_)
            return;
        Vector2 offset= Vector2.zero;
        offset.x -= horizontal_speed_ * Time.deltaTime;
        offset.y += height_ * Time.deltaTime * Mathf.Sin(Time.time * vertical_speed_);
        transform.Translate(offset);
    }
}
