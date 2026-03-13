using UnityEngine;
using UnityEngine.InputSystem;
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
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            pause.SetActive(true);
            effects.SetActive(true);
            character.gameObject.GetComponent<PlayInputHandler>().enabled = false;
        }   
    }
}
