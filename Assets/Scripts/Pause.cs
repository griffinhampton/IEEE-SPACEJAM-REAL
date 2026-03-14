using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.UI;

using System.Collections;

public class Pause : MonoBehaviour
{
    public Button cont;
    public Button menu;
    public Button exit;
   
    //[SerializeField] private GameObject character;

    private void Awake()
    {
        //effects.SetActive(true);
        Time.timeScale = 0;
        cont.onClick.AddListener(() => disable());
        menu.onClick.AddListener(() => SceneManager.LoadScene("Direct Menu"));
        exit.onClick.AddListener(() => Application.Quit());
  
    }

    private void disable()
    {
        gameObject.SetActive(false);
        //if (effects != null)
          //  effects.SetActive(false);
        Time.timeScale = 1;
    }
}
