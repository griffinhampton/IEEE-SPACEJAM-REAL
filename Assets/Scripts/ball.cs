using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor.Rendering;
using UnityEngine;

public class ball : MonoBehaviour
{
    public GameObject[] goals;
    public float goalgravity = 1;
    public Rigidbody body;
    private Vector3 fnet;
    private logic logicscript;
    void Start()
    {
        goals = GameObject.FindGameObjectsWithTag("goal");
        logicscript = GameObject.FindGameObjectWithTag("logic").GetComponent<logic>();
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("goal"))
        {
            logicscript.score(other.gameObject.GetComponent<goal>().getTeam());
            Destroy(gameObject);
        }
    }
}
