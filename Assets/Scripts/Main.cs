using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    [Header("Buttons")]
    public Button start;
    public Button tut;
    public Button exit;

    [Header("Other Screen")]
    public GameObject screen;
    private void Start_button()
    {
        screen.SetActive(true);
        gameObject.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start.onClick.AddListener(() => Start_button());
        // tut.onClick.AddListener(() => SceneManager.LoadScene("Tutorial"));
        exit.onClick.AddListener(() => Application.Quit());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
