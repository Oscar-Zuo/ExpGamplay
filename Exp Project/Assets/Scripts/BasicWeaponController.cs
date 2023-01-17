using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BasicWeaponController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] protected float fireRate = 1.0f, fireAngleLimits = 45.0f;
    public GameObject bulletType;
    protected float angle;
    private bool isFiring;
    private Coroutine currentCoroutine;

    void Start()
    {
        isFiring = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePostion= Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePostion);
        worldPosition.z = transform.position.z;
        float angle = Vector2.SignedAngle(transform.parent.up, worldPosition - transform.position);
        Quaternion lookAtQuat = Quaternion.Euler(0, 0, Mathf.Clamp(angle, -fireAngleLimits, fireAngleLimits));
        transform.localRotation = Quaternion.Lerp(transform.localRotation, lookAtQuat, Time.deltaTime * 5);

        if (isFiring&&currentCoroutine==null)
        {
            Fire();
        }
    }

    virtual public void StartFire()
    {
        isFiring= true;
    }

    virtual public void StopFire()
    { 
        isFiring= false;
    }

    virtual protected void Fire()
    {
        IEnumerator EFire()
        {
            GameObject bullet = Instantiate(bulletType);
            float weaponLength = GetComponent<BoxCollider2D>().size.y;
            bullet.transform.position = transform.position + transform.up*weaponLength;
            bullet.transform.rotation = transform.rotation;
            yield return new WaitForSeconds(fireRate);
            currentCoroutine = null;
        }
        currentCoroutine = StartCoroutine(EFire());
    }
}
