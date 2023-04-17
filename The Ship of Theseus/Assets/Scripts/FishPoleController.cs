using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishPoleController : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.tag.Equals("Player"))
            return;
        CharacterController controller = other.GetComponent<CharacterController>();
        if (controller == null || controller.ItemList.Count>0)
            return;
        controller.is_using_fishing_pole_ = true;
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (!other.tag.Equals("Player"))
            return;
        CharacterController controller = other.GetComponent<CharacterController>();
        if (controller == null)
            return;
        controller.is_using_fishing_pole_ = false;
    }
}