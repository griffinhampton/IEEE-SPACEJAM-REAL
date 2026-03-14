using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public NavMeshAgent mesh;
    public GameObject ballchild;
    public GameObject ball;
    public float spawntime;
    private bool possession;
    private GameObject[] balls;
    private GameObject[] players;

    void Start()
    {
        //players=GameObject.FindGameObjectsWithTag("Player");
    }

    void Update()
    {
        balls = GameObject.FindGameObjectsWithTag("ball");
        if(balls.Length == 1)
        {
            mesh.destination = balls[0].transform.position;
        }else if (possession)
        {
            mesh.destination = GameObject.FindGameObjectWithTag("goal").transform.position;
            if ((GameObject.FindGameObjectWithTag("goal").transform.position - gameObject.transform.position).magnitude < 10)
            {
                Vector3 unitvector = (GameObject.FindGameObjectWithTag("goal").transform.position - gameObject.transform.position)/(GameObject.FindGameObjectWithTag("goal").transform.position - gameObject.transform.position).magnitude;
                GameObject temp = Instantiate(ball,gameObject.transform.position+unitvector*spawntime,Quaternion.identity);
                Rigidbody rb = temp.GetComponent<Rigidbody>();
                rb.linearVelocity = unitvector*10;
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
