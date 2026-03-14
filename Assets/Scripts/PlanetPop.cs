using UnityEngine;
using System.Collections.Generic;
using TMPro;


public class PlanetPop : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject first;
    public GameObject second;
    public GameObject third;
    private List<GameObject> planets;
    [Header("Player")]
    public GameObject p1;
    public GameObject p2;
    // public GameObject p3;
    // public GameObject p4;


    [SerializeField] int playercount = 0;
    private GameObject Active;
    private Transform child;
    private TMP_Text text;
    private int player;
    private int left_side;
    private Transform newer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        planets = new List<GameObject> { first, second, third };
        //Random.InitState(); //gotta put something for the int
        //determines what parts of the player ui is being shown
        //need to make a class that is not destroyed from main menu to this scene
        
        
    }
    public void Playercounter()
    {
        //gamelogic = FindFirstObjectByType<logic>();
        playercount = playercount + 1;
        switch (playercount)
        {
            case 1:
                p1.SetActive(true);
                p2.SetActive(false);
                // p3.SetActive(false);
                // p4.SetActive(false);
                break;
            case 2:
                p1.SetActive(true);
                p2.SetActive(true);

                //p3.SetActive(false);
                //p4.SetActive(false);
                break;
                /*   case 3:
                       p4.SetActive(false);
                       break;
                   case 4:
                       break;   */

        }
    }
    //private int i = 1;

    //have this called each time in logic 
    public void Update_UI(int player, int score)
    {
        switch (player)
        {
            case 1:
                Active = p1;
                left_side = 1;
                break;
            case 2:
                Active = p2;
                left_side = -1;
                break;
          /*  case 3:
                Active = p3;
                left_side = 1;
                break;
            case 4:
                Active = p4;
                left_side = -1;
                break;  */
        }
        //change the text score
        child = Active.transform.GetChild(0);
        text = child.gameObject.GetComponent<TMP_Text>();
        Debug.Log("child name: " + child.name);
        text.text = score + "";


        //add the planet
        Transform parent = Active.gameObject.GetComponent<Transform>();
        float xcoord = parent.position.x;

        //choose a random planet design of the list of planet prefabs
        if (score > 3)
        {
            GameObject planet = Instantiate(planets[Random.Range(0, 2)], parent);
            planet.transform.SetParent(parent, true);

            planet.transform.localPosition = new Vector3(((80 * score) * left_side), 0, 0);
            Debug.Log("x coordinate: " + xcoord + 80 * score);
        }
    }
}
