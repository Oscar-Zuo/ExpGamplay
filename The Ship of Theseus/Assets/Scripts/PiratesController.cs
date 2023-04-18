using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PiratesController : MonoBehaviour
{
    // Start is called before the first frame update
    public float cannon_accuracy_ = 0f;
    [SerializeField] GameObject cannon_ball_object_;
    [SerializeField] UnityEngine.Transform cannon_muzzle_;
    public float speed_ = 3.0f;
    public float shot_interval_ = 10.0f;
    public int max_health_ = 2;
    public int next_state_ = 0;
    public float rotation_speed_ = Mathf.PI / 2;

    int health_;

    public int Health { get => health_; set
        {
            health_ = Mathf.Clamp(value, 0, max_health_);
            if (health_<=0)
            {
                Destroy(gameObject);
                for (int i = 0; i < Random.Range(0, 3); i++)
                {
                    Instantiate(GameManager.instance_.plank_, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0), Quaternion.identity);
                }
                for (int i = 0; i < Random.Range(0, 3); i++)
                {
                    Instantiate(GameManager.instance_.nails_, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0), Quaternion.identity);
                }
            }
        }
    }
    void Start()
    {
        //if (0 < path_list_.Count)
        //{
        //    transform.position = path_list_[0].position;
        //}

        StartCoroutine(Cannon());
        Health = max_health_;
    }

    // Update is called once per frame
    void Update()
    {
       
        
    }

    IEnumerator Moving()
    {
        var path_list = GameManager.instance_.prirates_path_list_;
        if (Vector2.Distance(transform.position, path_list[next_state_].position) <= 0.05)
        {
            Vector2 direction = (path_list[(next_state_ + 1) % path_list.Count].position - transform.position).normalized;
            float angle = Vector2.Angle(direction, -transform.right);
            if (angle <= 0.01)
                transform.right = -direction;
            else
            {
                transform.Rotate(transform.up, rotation_speed_ * Time.deltaTime * Mathf.Sign(Vector3.Dot(transform.up, Vector3.Cross(direction, -transform.right))));
                yield return new WaitForEndOfFrame();
            }
            next_state_ = (next_state_ + 1) % path_list.Count;
        }

        transform.position = Vector3.MoveTowards(transform.position, path_list[next_state_].position, speed_ * Time.deltaTime);
        yield return new WaitForEndOfFrame();
    }

    IEnumerator Cannon()
    {
        while(true)
        {
            yield return new WaitForSeconds(shot_interval_);
            ShotCannon();
        }
    }

    void ShotCannon()
    {
        if (!cannon_ball_object_)
            return;

        GameObject cannon_ball = Instantiate(cannon_ball_object_, cannon_muzzle_.position, Quaternion.identity);
        BombController cannon_ball_controller = cannon_ball.GetComponent<BombController>();
        if (cannon_ball_controller)
        {
            Vector2 land_postition = GameManager.instance_.ship_controller_.bomb_landing_location_list_[Random.Range(0, GameManager.instance_.ship_controller_.bomb_landing_location_list_.Count-1)].position +
                new Vector3(Random.Range(-cannon_accuracy_, cannon_accuracy_), Random.Range(-cannon_accuracy_, cannon_accuracy_), 0);
            cannon_ball_controller.StartTossing(land_postition);
        }
    }
}
