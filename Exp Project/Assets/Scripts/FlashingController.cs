using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingController : MonoBehaviour
{
    // Start is called before the first frame update
    public Color flashingColor = Color.white;
    [SerializeField] private float flashingTime;
    [SerializeField] private float flashingLast = 1;
    public List<SpriteRenderer> spriteRanderers;
    public float FlashingTime { get => flashingTime; set => flashingTime = value; }
    public float FlashingLast { get => flashingLast; set => flashingLast = value; }

    void Start()
    {
        Material flashingMaterial = Resources.Load<Material>("Materials/HitMaterial");
        foreach (var randerer in spriteRanderers)
        {
            randerer.material = flashingMaterial;
            randerer.material.SetColor("_DisplayColor", flashingColor);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartFlashing()
    {
        IEnumerator EFlash()
        {
            float startTime=Time.time, lastTime=0;
            while(lastTime< flashingLast)
            {
                foreach (var randerer in spriteRanderers)
                {
                    randerer.material.SetInt("_Hit", 1);
                }
                yield return new WaitForSeconds(FlashingTime);
                foreach (var randerer in spriteRanderers)
                {
                    randerer.material.SetInt("_Hit", 0);
                }
                yield return new WaitForSeconds(FlashingTime);
                lastTime = Time.time - startTime;
            }
        }
        StartCoroutine(EFlash());
    }
}
