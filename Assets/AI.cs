using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public NavMeshAgent mesh;
    public GameObject ballchild;
    public GameObject ball;
    public float spawntime;
    public float power;
    public int team;
    private bool possession;
    private GameObject[] balls;
    private GameObject goal;
    private GameObject[] players;

    void Start()
    {
        //players=GameObject.FindGameObjectsWithTag("Player");
        GameObject[] goals = GameObject.FindGameObjectsWithTag("goal");
        foreach(GameObject potgoal in goals)
        {
            potgoal.GetComponent<goal>();
            if (potgoal.GetComponent<goal>().getTeam()==team)
            {
                goal = potgoal;
            }
        }
    }

    void Update()
    {
        balls = GameObject.FindGameObjectsWithTag("ball");
        if(balls.Length == 1)
        {
            mesh.destination = balls[0].transform.position;
        }else if (possession)
        {
            mesh.destination = goal.transform.position;
            if ((goal.transform.position - gameObject.transform.position).magnitude < 10)
            {
                Vector3 unitvector = (goal.transform.position - gameObject.transform.position)/(GameObject.FindGameObjectWithTag("goal").transform.position - gameObject.transform.position).magnitude;
                GameObject temp = Instantiate(ball,gameObject.transform.position+unitvector*spawntime,Quaternion.identity);
                Rigidbody rb = temp.GetComponent<Rigidbody>();
                rb.linearVelocity = unitvector*power;
                possession = false;
                ballchild.SetActive(false);
            }
        }
    }

    void OnCollisionEnter(Collision collider){
        if(collider.gameObject.CompareTag("ball")){
            Debug.Log("grab ball");
            possession = true;
            ballchild.SetActive(true);
            Destroy(collider.gameObject);
        }
    }

    bool hasBall(){return possession;}
    void setBall(bool poss){possession = poss;}
}
