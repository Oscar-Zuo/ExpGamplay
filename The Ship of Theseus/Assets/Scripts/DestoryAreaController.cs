using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryAreaController : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemController item_controller = collision.gameObject.GetComponent<ItemController>();
        if (item_controller)
            Destroy(collision.gameObject);
    }
}
