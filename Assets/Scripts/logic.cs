using JetBrains.Annotations;
using UnityEngine;

public class logic : MonoBehaviour
{
    [Header("Scoring")]
    public int p1score=0;
    public int p2score=0; 
    public int p3score=0;
    public int p4score=0; 
    public int players;

    [Header("Background rotation")]
    public GameObject[] stars;
    public float maxrot;
    public GameObject directionallight;
    private Vector3 rotationdelta;

    [Header("Ball handling")]
    public GameObject ball;
    public Vector3 ballspawn;
    private GameObject[] balls;
    public PlanetPop pop;
    void Start()
    {
        pop = FindFirstObjectByType<PlanetPop>();

        stars = GameObject.FindGameObjectsWithTag("stars");
        rotationdelta = new Vector3(Random.value*maxrot-maxrot/2,Random.value*maxrot-maxrot/2,Random.value*maxrot-maxrot/2);
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject star in stars){
            star.transform.Rotate(rotationdelta*Time.deltaTime);
        }
        balls = GameObject.FindGameObjectsWithTag("ball");
        if(balls.Length>1){
            for(int i = 1; i<balls.Length;i++){
                Destroy(balls[i]);
            }
        }else if(balls.Length==1 && balls[0].transform.position.magnitude > 25){
            Destroy(balls[0]);
            spawnball();
        }
    }

    public void spawnball()
    {
        Instantiate(ball,ballspawn+new Vector3(Random.value*2,0,Random.value*2),Quaternion.identity);
    }

    public void score(int player)
    {
        spawnball();
        switch (player)
        {
            case 1:
                p1score++;
                pop.Update_UI(1, p1score);
                break;
            case 2:
                p2score++;
                pop.Update_UI(2, p2score);
                break;
            case 3:
                p3score++;
                pop.Update_UI(3, p3score);
                break;
            case 4:
                p4score++;
                pop.Update_UI(4, p4score);
                break;
        }
    }
}
