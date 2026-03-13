using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public Button cont;
    public Button exit;
    public GameObject effects;
    [SerializeField] private GameObject character;
    //public Button menu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cont.onClick.AddListener(() => disable());
        //menu.onClick.AddListener(() => SceneManager.LoadScene("Main Menu"));
        exit.onClick.AddListener(() => Application.Quit());
    }

    // Update is called once per frame
    private void disable()
    {
        gameObject.SetActive(false);
        effects.SetActive(false);
        character.gameObject.GetComponent<PlayInputHandler>().enabled = true;

    }
}
