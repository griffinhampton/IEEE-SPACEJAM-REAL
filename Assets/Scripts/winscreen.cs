using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class winscreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("menu");
        }
    }
}
