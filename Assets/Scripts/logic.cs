using JetBrains.Annotations;
using UnityEngine;

public class logic : MonoBehaviour
{
    public int p1score=0;
    public int p2score=0; 
    public int p3score=0;
    public int p4score=0; 
    public int players;
    public GameObject[] stars;
    public float maxrot;
    private Vector3 rotationdelta;
    void Start()
    {
        stars = GameObject.FindGameObjectsWithTag("stars");
        rotationdelta = new Vector3(Random.value*maxrot-maxrot/2,Random.value*maxrot-maxrot/2,Random.value*maxrot-maxrot/2);
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject star in stars){
            Vector3 newrot = star.transform.eulerAngles + rotationdelta*Time.deltaTime;
            star.transform.eulerAngles = newrot;
        }
    }

    public void score(int player)
    {
        switch (player)
        {
            case 1:
                p1score++;
                break;
            case 2:
                p2score++;
                break;
            case 3:
                p3score++;
                break;
            case 4:
                p4score++;
                break;
        }
    }
}
