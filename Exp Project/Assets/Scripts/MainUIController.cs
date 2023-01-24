using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UIElements;

public class MainUIController : MonoBehaviour
{
    // Start is called before the first frame update
    UnityEngine.UIElements.Label health;
    //TankController playerController;
    void Start()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        health = rootVisualElement.Q<UnityEngine.UIElements.Label>("Health");
        //playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<TankController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health != null)
        {
            health.text = GameManager.instance.playerController.Health.ToString();

            if (GameManager.instance.playerController.Health <= 2)
                health.style.color = Color.red;
            else
                health.style.color = Color.green;
        }
    }
}
