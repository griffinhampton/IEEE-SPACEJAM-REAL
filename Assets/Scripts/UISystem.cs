using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class UISystem : MonoBehaviour
{
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject character;
    public GameObject effects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pause.SetActive(false);
        effects.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        var gamepad = Gamepad.current;

        if (gamepad.startButton.wasPressedThisFrame)
        {
            pause.SetActive(true);
            effects.SetActive(true);
            Time.timeScale = 0;
        }   
    }
}
