using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class logic : MonoBehaviour
{
    [Header("Scoring")]
    public int p1score = 0;
    public int p2score = 0;
    public int p3score = 0;
    public int p4score = 0;
    public int players;

    [Header("Win conditions")]
    public int winningScore = 7;
    private bool gameover = false;

    [Header("Background rotation")]
    public GameObject[] stars;
    public float maxrot = 10;
    public GameObject directionallight;
    private Vector3 rotationdelta;

    [Header("Ball handling")]
    public GameObject ball;
    public Vector3 ballspawn;
    private GameObject[] balls;
    public PlanetPop pop;

    [Header("UI")]
    public GameObject gameOverPanel;
    public TMP_Text winnerText;
    public TMP_Text p1ScoreText;
    public TMP_Text p2ScoreText;
    public TMP_Text p3ScoreText;
    public TMP_Text p4ScoreText;
    public TMP_Text restartText;

    [Header("Audio")]
    public AudioSource scoreSound;
    public AudioSource winSound;
    public AudioSource backgroundMusic;

    [Header("Scene Management")]
    public string nextScene = "";
    public string mainMenuScene = "MainMenu";

    private float respawnDelay = 1.5f;
    private bool ballPendingSpawn = false;
    private float ballSpawnTimer = 0f;

    void Start()
    {
        pop = FindFirstObjectByType<PlanetPop>();
        stars = GameObject.FindGameObjectsWithTag("stars");

        rotationdelta = new Vector3(
            Random.value * maxrot - maxrot / 2,
            Random.value * maxrot - maxrot / 2,
            Random.value * maxrot - maxrot / 2
        );

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (backgroundMusic != null)
        {
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }

        UpdateAllScoreUI();
        spawnball();
    }

    void Update()
    {
        foreach (GameObject star in stars)
        {
            if (star != null)
                star.transform.Rotate(rotationdelta * Time.deltaTime);
        }

        if (ballPendingSpawn)
        {
            ballSpawnTimer -= Time.deltaTime;
            if (ballSpawnTimer <= 0f)
            {
                ballPendingSpawn = false;
                Instantiate(ball, ballspawn + new Vector3(Random.value * 2, 0, Random.value * 2), Quaternion.identity);
            }
        }

        balls = GameObject.FindGameObjectsWithTag("ball");
        if (balls.Length > 1)
        {
            for (int i = 1; i < balls.Length; i++)
                Destroy(balls[i]);
        }
        else if (balls.Length == 1 && balls[0].transform.position.magnitude > 25)
        {
            Destroy(balls[0]);
            spawnball();
        }
        else if (balls.Length == 0 && !ballPendingSpawn && !gameover)
        {
            spawnball();
        }

        if (gameover && Input.GetKeyDown(KeyCode.R))
            resetGame();

        if (Input.GetKeyDown(KeyCode.Escape))
            GoToMainMenu();
    }

    public void spawnball()
    {
        if (gameover) return;
        balls = GameObject.FindGameObjectsWithTag("ball");
        if (balls.Length > 0) return;
        ballPendingSpawn = true;
        ballSpawnTimer = respawnDelay;
    }

    public void score(int player)
    {
        if (gameover) return;

        if (scoreSound != null) scoreSound.Play();

        balls = GameObject.FindGameObjectsWithTag("ball");
        foreach (GameObject b in balls) Destroy(b);

        switch (player)
        {
            case 1:
                p1score++;
                pop?.Update_UI(1, p1score);
                UpdateScoreText(p1ScoreText, p1score);
                if (p1score >= winningScore) winGame(1); else spawnball();
                break;
            case 2:
                p2score++;
                pop?.Update_UI(2, p2score);
                UpdateScoreText(p2ScoreText, p2score);
                if (p2score >= winningScore) winGame(2); else spawnball();
                break;
            case 3:
                p3score++;
                pop?.Update_UI(3, p3score);
                UpdateScoreText(p3ScoreText, p3score);
                if (p3score >= winningScore) winGame(3); else spawnball();
                break;
            case 4:
                p4score++;
                pop?.Update_UI(4, p4score);
                UpdateScoreText(p4ScoreText, p4score);
                if (p4score >= winningScore) winGame(4); else spawnball();
                break;
        }
    }

    void winGame(int player)
    {
        gameover = true;

        if (winSound != null) winSound.Play();
        if (backgroundMusic != null) backgroundMusic.Stop();

        GameObject[] allBalls = GameObject.FindGameObjectsWithTag("ball");
        foreach (GameObject b in allBalls) Destroy(b);

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (winnerText != null) winnerText.text = "Player " + player + " Wins!";
        if (restartText != null) restartText.text = "Press R to Restart\nPress ESC for Main Menu";
    }

    public void resetGame()
    {
        gameover = false;
        p1score = p2score = p3score = p4score = 0;

        pop?.Update_UI(1, 0);
        pop?.Update_UI(2, 0);
        pop?.Update_UI(3, 0);
        pop?.Update_UI(4, 0);

        UpdateAllScoreUI();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (backgroundMusic != null)
        {
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }

        spawnball();
    }

    void UpdateScoreText(TMP_Text t, int score)
    {
        if (t != null) t.text = score.ToString();
    }

    void UpdateAllScoreUI()
    {
        UpdateScoreText(p1ScoreText, p1score);
        UpdateScoreText(p2ScoreText, p2score);
        UpdateScoreText(p3ScoreText, p3score);
        UpdateScoreText(p4ScoreText, p4score);
    }

    public void GoToMainMenu()
    {
        if (!string.IsNullOrEmpty(mainMenuScene))
            SceneManager.LoadScene(mainMenuScene);
    }

    public void GoToNextScene()
    {
        if (!string.IsNullOrEmpty(nextScene))
            SceneManager.LoadScene(nextScene);
    }
}