using UnityEngine;

public class goal : MonoBehaviour
{
    public SpriteRenderer sprite;
    public GameObject cam;
    public int team;
    private logic gameLogic;

    void Start()
    {
        gameLogic = FindFirstObjectByType<logic>();

        switch (team)
        {
            case 1:
                sprite.color = Color.red;
                break;
            case 2:
                sprite.color = Color.blue;
                break;
            case 3:
                sprite.color = Color.green;
                break;
            case 4:
                sprite.color = Color.yellow;
                break;
        }
    }

    void Update()
    {
        transform.LookAt(cam.transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            // Score for the OPPOSITE team
            switch (team)
            {
                case 1:
                    gameLogic.score(2);
                    break;
                case 2:
                    gameLogic.score(1);
                    break;
                case 3:
                    gameLogic.score(4);
                    break;
                case 4:
                    gameLogic.score(3);
                    break;
            }
        }
    }

    public int getTeam() { return team; }
}