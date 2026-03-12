using UnityEngine;

public class ball : MonoBehaviour
{
    public GameObject[] goals;
    public float goalgravity = 1;
    public Rigidbody body;
    private Vector3 fnet;
    void Start()
    {
        goals = GameObject.FindGameObjectsWithTag("goal");
    }

    // Update is called once per frame
    void Update()
    {
        fnet = Vector3.zero;
        foreach(GameObject goal in goals)
        {
            fnet += goalgravity*(goal.transform.position-gameObject.transform.position)/Mathf.Pow(Vector3.Distance(gameObject.transform.position,goal.transform.position),2);
        }
        body.linearVelocity += (fnet/body.mass)*Time.deltaTime;
    }
}
