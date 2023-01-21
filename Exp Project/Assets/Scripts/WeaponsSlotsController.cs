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

    public void AddWeapon(GameObject weapon)
    {
        bool added = false;
        for (int i=0; i<weaponSlots.Count; i++)
        {
            if (weaponSlots[i].weapon==null)
            {
                WeaponObject weaponObject = weaponSlots[i];
                weaponObject.weapon = Instantiate(weapon, weaponObject.weaponPivots);
                weaponSlots[i] = weaponObject;
                added= true;
                break;
            }
        }
        if (!added)
        {
            int index = Random.Range(0, weaponSlots.Count);
            WeaponObject weaponObject = weaponSlots[index];
            weaponObject.weapon = Instantiate(weapon, weaponObject.weaponPivots);
            weaponSlots[index] = weaponObject;
        }
    }
}
