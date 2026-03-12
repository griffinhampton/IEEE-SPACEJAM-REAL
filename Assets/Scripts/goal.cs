using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class goal : MonoBehaviour
{
    public SpriteRenderer sprite;
    public GameObject cam;
    public int team;
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.transform.position);
    }

    public int getTeam(){return team;}
}
