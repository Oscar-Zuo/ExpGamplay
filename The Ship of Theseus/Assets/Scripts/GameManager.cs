using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update

    public ShipController ship_controller_;
    public List<GameObject> player_list_;
    public List<Transform> prirates_path_list_ = new List<Transform>();
    public GameObject plank_;
    public GameObject bomb_;
    public GameObject pirate_ship_;
    public GameObject nails_;
    public float plank_nails_spawn_rate_ = 0.15f;
    public float bomb_spawn_rate_ = 0.05f;
    public float pirates_spawn_interval_ = 20.0f;
    public int target_progress_ = 30;
    public GameObject progress_bar;

    public static GameManager instance_;

    int remaining_time;
    protected float screen_bound_x_ = 10, screen_bound_y_ = 4.5f;
    private int progress_ = 0;

    public static GameObject GetPirateShipByPosition(Vector2 position)
    {
        var pirate_boats = GameObject.FindGameObjectsWithTag("PirateBoat");
        foreach (var pirate_boat in pirate_boats)
        {
            Collider2D pirate_boat_collider = pirate_boat.GetComponent<PolygonCollider2D>();
            if (pirate_boat_collider && pirate_boat_collider.OverlapPoint(position))
            {
                return pirate_boat;
            }
        }
        return null;
    }

    public void IncreaseProgress(int num)
    {
        progress_ += num;
        if (progress_ >= target_progress_)
            SceneManager.LoadScene("Win");
        progress_bar.GetComponent<UnityEngine.UI.Slider>().value = (float)progress_ / target_progress_;
    }

    IEnumerator GenerateItems()
    {
        while (true)
        {
            if (Random.value < plank_nails_spawn_rate_ * Time.deltaTime)
            {
                Instantiate(plank_, new Vector2(screen_bound_x_, Random.Range(-screen_bound_y_, screen_bound_y_)), Quaternion.identity);
            }
            if (Random.value < plank_nails_spawn_rate_ * Time.deltaTime)
            {
                Instantiate(nails_, new Vector2(screen_bound_x_, Random.Range(-screen_bound_y_, screen_bound_y_)), Quaternion.identity);
            }
            if (Random.value < bomb_spawn_rate_ * Time.deltaTime)
            {
                Instantiate(bomb_, new Vector2(screen_bound_x_, Random.Range(-screen_bound_y_, screen_bound_y_)), Quaternion.identity);
            }
            yield return null;
        }
    }

    IEnumerator GeneratePirates()
    {
        while (true)
        {
            var pirate = Instantiate(pirate_ship_, new Vector2(screen_bound_x_, Random.Range(-screen_bound_y_, screen_bound_y_)), Quaternion.identity);
            pirate.GetComponent<PiratesController>().next_state_ = Random.Range(0, prirates_path_list_.Count - 1);
            yield return new WaitForSeconds(pirates_spawn_interval_);
        }
    }

    void Start()
    {
        instance_ = this;
        StartCoroutine(GenerateItems());
        StartCoroutine(GeneratePirates());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
