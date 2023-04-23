using UnityEngine;

public class CraftTableController : InteractableController
{
    // Should have used a recipe to manage how to craft, but I dont have time

    [SerializeField] GameObject repair_kit_object_;

    void Start()
    {

    }

    public override bool StartInteract(GameObject player)
    {
        if (player == null) { return false; }
        CharacterController player_controller = player.GetComponent<CharacterController>();
        if (player_controller == null) { return false; }

        bool has_nails = false, has_plank = false;
        player_controller.ItemList.RemoveAll(s => s == null);
        foreach (var item in player_controller.ItemList)
        {
            ItemController item_controller = item.GetComponent<ItemController>();
            if (item_controller == null) continue;

            if (item_controller.ItemName == "Nails")
                has_nails = true;
            else if (item_controller.ItemName == "Plank")
                has_plank = true;

            if (has_nails && has_plank)
                return true;
        }
        player_controller.ReorderItemList();
        return false;
    }

    public override void FinishInteract(GameObject player)
    {
        if (player == null) { return; }
        CharacterController player_controller = player.GetComponent<CharacterController>();
        if (player_controller == null) { return; }

        player_controller.ItemList.RemoveAll(s => s == null);
        foreach (var item in player_controller.ItemList)
        {
            ItemController item_controller = item.GetComponent<ItemController>();
            if (item_controller == null) continue;

            if (item_controller.ItemName == "Nails")
            {
                player_controller.ItemList.Remove(item);
                Destroy(item);
                break;
            }
        }

        player_controller.ItemList.RemoveAll(s => s == null);
        foreach (var item in player_controller.ItemList)
        {
            ItemController item_controller = item.GetComponent<ItemController>();
            if (item_controller == null) continue;

            if (item_controller.ItemName == "Plank")
            {
                player_controller.ItemList.Remove(item);
                Destroy(item);
                break;
            }
        }

        player_controller.PickupItem(Instantiate(repair_kit_object_));
        player_controller.ReorderItemList();
    }
}
