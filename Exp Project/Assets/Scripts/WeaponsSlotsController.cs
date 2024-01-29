using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct WeaponObject
{
    public GameObject weapon;
    public Transform weaponPivots;
    public WeaponObject(GameObject _weapon, Transform _weaponPrivots)
    {
        weapon = _weapon;
        weaponPivots = _weaponPrivots;
    }
}

public class WeaponsSlotsController : MonoBehaviour
{

    public List<WeaponObject> weaponSlots;
    private bool lastFrameFireInput;

    // Start is called before the first frame update
    void Start()
    {
        lastFrameFireInput = false;
        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot.weapon)
            {
                weaponSlot.weapon.transform.position = weaponSlot.weaponPivots.position;
                weaponSlot.weapon.transform.rotation = weaponSlot.weaponPivots.rotation;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool fireInput = Input.GetMouseButton(0);
        if (fireInput && !lastFrameFireInput)
        {
            foreach (var weaponSlot in weaponSlots)
            {
                if (weaponSlot.weapon)
                    weaponSlot.weapon.GetComponent<BasicWeaponController>().StartFire();
            }
        }
    }

    public void AddWeapon(GameObject weaponObject)
    {
        WeaponObject weapon;
        for (int i = 0; i < weaponSlots.Count; i++)
        {
            if (weaponSlots[i].weapon != null)
                continue;

            weapon = weaponSlots[i];
            weapon.weapon = Instantiate(weaponObject, weapon.weaponPivots.position, weapon.weaponPivots.rotation, weapon.weaponPivots);
            weaponSlots[i] = weapon;

            return;
        }

        int index = Random.Range(0, weaponSlots.Count);
        weapon = weaponSlots[index];
        weapon.weapon = Instantiate(weaponObject, weapon.weaponPivots);
        weaponSlots[index] = weapon;

    }
}
