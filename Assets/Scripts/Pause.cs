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

        var actionMap = inputAsset.FindActionMap(actionMapName);
        if (actionMap == null)
        {
            Debug.LogError($"Pause: Action map '{actionMapName}' not found in InputActionAsset. Check your asset and Inspector settings.");
            return;
        }
        swtchAction = actionMap.FindAction(swtch);
        if (swtchAction == null)
        {
            Debug.LogError($"Pause: Action '{swtch}' not found in action map '{actionMapName}'.");
            return;
        }
        clickAction = actionMap.FindAction(click);
        if (clickAction == null)
        {
            Debug.LogError($"Pause: Action '{click}' not found in action map '{actionMapName}'.");
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
    {
        // Only process if choices are set
        if (choices == null || choices.Count == 0)
            return;

        // Handle selection with controller
        if (clickInput)
        {
            choices[select].onClick.Invoke();
            clickInput = false; // Prevent multiple triggers
        }

        if (Mathf.Abs(swtchInput.y) > 0.5f)
        {
            int prevSelect = select;
            if (swtchInput.y > 0.5f && select > 0)
            {
                select--;
            }
            else if (swtchInput.y < -0.5f && select < choices.Count - 1)
            {
                select++;
            }
            if (prevSelect != select)
            {
                EventSystem.current.SetSelectedGameObject(choices[select].gameObject);
                // Prevent rapid navigation
                swtchInput = Vector2.zero;
            }
        }
    }

    private void OnEnable()
    {
        menuNav.FindActionMap(actionMapName).Enable();
    }

    private void disable()
    {
        gameObject.SetActive(false);
        effects.SetActive(false);
        Time.timeScale = 1;
    }
}
