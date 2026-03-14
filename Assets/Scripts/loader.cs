using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;



public class loader : MonoBehaviour
{
    public Button back;
    public Button next;

    private TMP_Text t1;
    private TMP_Text t2;
    private TMP_Text t3;
    private TMP_Text t4;

    private string preset = "Bot";

    public GameObject n1;
    public GameObject n2;
    public GameObject n3;
    public GameObject n4;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        t1 = n1.gameObject.GetComponent<TMP_Text>();
        t2 = n2.gameObject.GetComponent<TMP_Text>();
        t3 = n3.gameObject.GetComponent<TMP_Text>();
        t4 = n4.gameObject.GetComponent<TMP_Text>();
        t1.text = preset + 1;
        t2.text = preset + 1;
        t3.text = preset + 1;
        t4.text = preset + 1;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
