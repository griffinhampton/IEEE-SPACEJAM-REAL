using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class Pause : MonoBehaviour
{
    public Button cont;
    public Button menu;
    public Button exit;
    private Button selected;
    public GameObject effects;
    [SerializeField] private GameObject character;
    //private Vector2 input;
    private int select = 0;
    private List<Button> choices;
    private float vert;

    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset menuNav;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "MenuNav";

    [Header("Action Name References")]
    [SerializeField] private string swtch = "Navigate";
    [SerializeField] private string click = "Select";

    private InputAction swtchAction;
    private InputAction clickAction;

    public Vector2 swtchInput { get; private set; }
    public bool clickInput { get; private set; }

    public static Pause Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        InputActionAsset inputAsset;

        if (TryGetComponent<PlayerInput>(out var pInput))
        {
            inputAsset = pInput.actions;
        }
        else {

            inputAsset = Instantiate(menuNav);
        }
 

        swtchAction = inputAsset.FindActionMap(actionMapName).FindAction(swtch);
        clickAction = inputAsset.FindActionMap(actionMapName).FindAction(click);
        if (Pause.Instance == null)
        {
            return;
        }
        swtchAction.performed += context => swtchInput = context.ReadValue<Vector2>();
        swtchAction.canceled += context => swtchInput = Vector2.zero;
        clickAction.performed += context => clickInput = true;
        clickAction.canceled += context => clickInput = false;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cont.onClick.AddListener(() => disable());
        menu.onClick.AddListener(() => SceneManager.LoadScene("Main Menu"));
        exit.onClick.AddListener(() => Application.Quit());
        choices = new List<Button> { cont, menu, exit };
    }

    void Update()
    {              /*
        Debug.Log(swtchInput);
        if (clickInput)
        {
            choices[select].onClick.Invoke();
        }
        if (swtchInput.y != 0)
        {
            Debug.Log("moved the stick girly" + swtchInput.y);
            if ((swtchInput.y > 0) && (select != 0))
            {
                select = select - 1;
            }
            else if ((swtchInput.y < 0) && (select != 2))
            {
                select = select + 1;
            }
        }
        EventSystem.current.SetSelectedGameObject(choices[select].gameObject);   */
    }

    private void OnEnable() { 
        menuNav.FindActionMap(actionMapName).Enable();
    }

    // Update is called once per frame
    private void disable()
    {
        gameObject.SetActive(false);
        effects.SetActive(false);
        Time.timeScale = 1;

    }
}
