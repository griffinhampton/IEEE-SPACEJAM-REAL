using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "Gameplay";

    [Header("Action Name References")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string look = "Look";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction shootAction;

    public Vector2 MoveInput { get; private set;}
    public Vector2 LookInput { get; private set;}
    public bool JumpTriggered { get; private set;}
    public float SprintValue {get; private set;}
    public bool ShootTriggered { get; private set;}

    [Header("Shot Variables")]
    public float vertangle;
    public float horizangle;
    public float initialvelocity;
    public GameObject node;
    public float step;
    public float max;
    public float gravity;
    private GameObject[] nodes;
    public GameObject ball;
    private int clicks = 0;
    private bool increase;
    public float angledelta = .5f;
    public float powerdelta = .5f;
    public float spawntime;
    public void ResetJump()
    {
        JumpTriggered = false;
    }

    public static PlayInputHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }


        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        lookAction = playerControls.FindActionMap(actionMapName).FindAction(look);
        jumpAction = playerControls.FindActionMap(actionMapName).FindAction(jump);
        sprintAction = playerControls.FindActionMap(actionMapName).FindAction(sprint);
        shootAction = playerControls.FindActionMap(actionMapName).FindAction("Shoot");
        RegisterInputActions();
    }

    void Update()
    {
        //Debug.Log(ShootTriggered);
        //Physics.gravity = new Vector3(0,gravity,0);
        nodes = GameObject.FindGameObjectsWithTag("node");
        foreach(GameObject node in nodes)
        {
            Destroy(node);
        }
        if (ShootTriggered)
        {
            clicks++;
            ShootTriggered = false;
        }
        if(clicks>=0){
            Shoot();
        }
    }

    void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => LookInput = Vector2.zero;

        jumpAction.performed += context => JumpTriggered = true;

        sprintAction.performed += context => SprintValue = context.ReadValue<float>();
        sprintAction.canceled += context => SprintValue = 0.0f;

        shootAction.performed += context => ShootTriggered = true;
    }

    public void ShootPreview()
    {
        ShootTriggered = false;
        float dx = initialvelocity*Mathf.Cos(vertangle)*Mathf.Sin(horizangle);
        float dz = initialvelocity*Mathf.Cos(vertangle)*Mathf.Cos(horizangle);
        float ay = Physics.gravity.y*.5f;
        float vy = initialvelocity*Mathf.Sin(vertangle);
        if (step < 0)
        {
            step = .05f;
        }
        for(float t = 0; t < max; t += step)
        {
            Instantiate(node,gameObject.transform.position + new Vector3(dx*t,ay*t*t + vy*t,dz*t),Quaternion.identity);
        }
    }

    //[ContextMenu("Shoot ball")]
    public void Shoot()
    {
        ShootPreview();
        switch (clicks){
            case 0:
                playerControls.FindActionMap(actionMapName).Disable();
                playerControls.FindActionMap(actionMapName).FindAction("Shoot").Enable();
                horizangle = transform.rotation.eulerAngles.y;
                initialvelocity = 10;
                clicks++;
                break;
            case 1:
                horizangle = increase?horizangle+angledelta*Time.deltaTime:horizangle-angledelta*Time.deltaTime;
                if (horizangle - transform.rotation.eulerAngles.y > Mathf.PI / 2 || horizangle - transform.rotation.eulerAngles.y < Mathf.PI / -2)
                {
                    Mathf.Clamp(horizangle,transform.rotation.eulerAngles.y-Mathf.PI/2,transform.rotation.eulerAngles.y+Mathf.PI/2 );
                    increase = !increase;
                }
                break;
            case 2:
                initialvelocity = increase?initialvelocity+powerdelta*Time.deltaTime:initialvelocity-powerdelta*Time.deltaTime;
                if (initialvelocity - 10 > 5 || initialvelocity - 10 < -5)
                {
                    Mathf.Clamp(initialvelocity,5,15);
                    increase = !increase;
                }
                break;
            case 3:
                float dx = initialvelocity*Mathf.Cos(vertangle)*Mathf.Sin(horizangle);
                float dz = initialvelocity*Mathf.Cos(vertangle)*Mathf.Cos(horizangle);
                float ay = Physics.gravity.y*.5f;
                float vy = initialvelocity*Mathf.Sin(vertangle);
                GameObject temp = Instantiate(ball,gameObject.transform.position+new Vector3(dx*spawntime,ay*spawntime*spawntime + vy*spawntime,dz*spawntime),Quaternion.identity);
                Rigidbody test = temp.GetComponent<Rigidbody>();
                test.linearVelocity = new Vector3(initialvelocity*Mathf.Cos(vertangle)*Mathf.Sin(horizangle),initialvelocity*Mathf.Sin(vertangle),initialvelocity*Mathf.Cos(vertangle)*Mathf.Cos(horizangle));
                playerControls.FindActionMap(actionMapName).Enable();
                clicks=-1;
                break;
        }
    }

    private void OnEnable()
    {
        playerControls.FindActionMap(actionMapName).Enable();
    }

    private void OnDisable()
    {
        if (moveAction == null) return;
        playerControls.FindActionMap(actionMapName).Disable();
    }
}
