using UnityEngine;

public class foreachplayer : MonoBehaviour
    
{
    public PlanetPop pop;
    private int robotsRemoved = 0;
    public GameObject save;
    void Start()
    {
        pop = save.gameObject.GetComponent<PlanetPop>();
    }

    // Update is called once per frame
    void Update()
    {
        // Count all root objects named "player(Clone)" in the scene
        int playerCount = 0;
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.transform.parent == null && obj.name.StartsWith("player(Clone)"))
            {
                playerCount++;
            }
        }

        // Remove one robot for each new player, one at a time
        while (robotsRemoved < playerCount && transform.childCount > 0)
        {
            Transform robot = transform.GetChild(0);
            Destroy(robot.gameObject);
            pop.Playercounter();
            robotsRemoved++;
        }
    }
}
