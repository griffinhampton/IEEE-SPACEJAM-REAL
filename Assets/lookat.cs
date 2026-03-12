using UnityEngine;

public class lookat : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject cam;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.transform.position);
    }
}
