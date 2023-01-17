using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    public float speedfactor = 5.0f;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 playerPos = player.transform.position;
        playerPos.z = transform.position.z;

        // TODO::Camera should have connection with cursor
        transform.position = Vector3.Lerp(transform.position, playerPos, Time.deltaTime * speedfactor);
        //transform.position = playerPos;
    }
}
