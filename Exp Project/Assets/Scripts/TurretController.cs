using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float rotateSpeed = 270f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePostion = Input.mousePosition;
        Vector2 lookAtVector = Camera.main.ScreenToWorldPoint(mousePostion) - transform.position;
        float angle = Vector2.SignedAngle(transform.parent.up, lookAtVector);
        Quaternion lookAtQuat = Quaternion.Euler(0, 0, angle);
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, lookAtQuat, rotateSpeed * Time.fixedDeltaTime);
    }
}
