using JetBrains.Annotations;
using UnityEngine;
using TMPro;

public class logic : MonoBehaviour
{
    [Header("Scoring")]
    public int p1score=0;
    public int p2score=0; 
    public int p3score=0;
    public int p4score=0; 
    public int players;

    [Header("Win conditions")]
    public int winningScore = 7;
    private bool gameover = false;

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
    public GameObject gameOverPanel;
    public TMP_Text winnerText;
    void Start()
    {
        pop = FindFirstObjectByType<PlanetPop>();

        stars = GameObject.FindGameObjectsWithTag("stars");
        rotationdelta = new Vector3(Random.value*maxrot-maxrot/2,Random.value*maxrot-maxrot/2,Random.value*maxrot-maxrot/2);

        gameOverPanel.SetActive(false);
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

        if (gameover && Input.GetKeyDown(KeyCode.R)){
            resetGame();
        }
    }

    public void spawnball()
    {
        Instantiate(ball,ballspawn+new Vector3(Random.value*2,0,Random.value*2),Quaternion.identity);
    }

    public void score(int player)
    {
        if(gameover) return;
        spawnball();
        switch (player)
        {
            case 1:
                p1score++;
<<<<<<< HEAD
                pop.Update_UI(1, p1score);
                if(p1score>=winningScore) winGame(1);
                break;
            case 2:
                p2score++;
                pop.Update_UI(2, p2score);
                if(p2score>=winningScore)winGame(2);
                break;
            case 3:
                p3score++;
                pop.Update_UI(3, p3score);
                if(p3score>=winningScore) winGame(3);
                break;
            case 4:
                p4score++;
                pop.Update_UI(4, p4score);
                if(p4score>=winningScore)winGame(4);
=======
                //pop.Update_UI(1, p1score);
                break;
            case 2:
                p2score++;
                //pop.Update_UI(2, p2score);
                break;
            case 3:
                p3score++;
                //pop.Update_UI(3, p3score);
                break;
            case 4:
                p4score++;
                //pop.Update_UI(4, p4score);
>>>>>>> 6660ba5492958a418f5da7851ab62fa4a37aa066
                break;
        }
    }

    void winGame(int player)
    {
        gameover = true;

        Debug.Log("Player "+player+" wins!");

        GameObject[] balls = GameObject.FindGameObjectsWithTag("ball");
        foreach(GameObject ball in balls){
            Destroy(ball);
        }

        gameOverPanel.SetActive(true);
        winnerText.text = "Player "+player+" wins!";
    }

    public void resetGame()
    {
        gameover = false;
        p1score = 0;
        p2score = 0;
        p3score = 0;
        p4score = 0;
        pop.Update_UI(1, p1score);
        pop.Update_UI(2, p2score);
        pop.Update_UI(3, p3score);
        pop.Update_UI(4, p4score);
        gameOverPanel.SetActive(false);
        spawnball();
    }
}
