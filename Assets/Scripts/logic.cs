using JetBrains.Annotations;
using UnityEngine;

public class logic : MonoBehaviour
{
    public int p1score=0;
    public int p2score=0; 
    public int p3score=0;
    public int p4score=0; 
    public int players;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
