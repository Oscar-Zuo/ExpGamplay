using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponsSlotsController : MonoBehaviour
{
    [System.Serializable]
    public struct WeaponObject
    {
        public GameObject weapon;
        public Transform weaponPivots;
        public WeaponObject( GameObject _weapon, Transform _weaponPrivots)
        {
            weapon = _weapon;
            weaponPivots = _weaponPrivots;
        }
    }

    public List<WeaponObject> weaponsSlots;
    private bool lastFrameFireInput;

    // Start is called before the first frame update
    void Start()
    {
        lastFrameFireInput = false;
        foreach (var weaponSlot in weaponsSlots)
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
            foreach (var weaponSlot in weaponsSlots)
            {
                if (weaponSlot.weapon)
                    weaponSlot.weapon.GetComponent<BasicWeaponController>().StartFire();
            }
        }
    }

    public bool AddWeapon(GameObject weapon)
    {
        if (weapon == null) { return false; }
        for (int i=0; i<weaponsSlots.Count; i++)
        {
            if (weaponsSlots[i].weapon!=null)
            {
                WeaponObject weaponObject = weaponsSlots[i];
                weaponObject.weapon = Instantiate(weapon, weaponObject.weaponPivots);
                weaponsSlots[i] = weaponObject;
            }
        }
        return true;
    }
}
