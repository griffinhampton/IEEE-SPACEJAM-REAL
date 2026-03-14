using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private string handsUp = "HandsUp";

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction shootAction;
    private InputAction handsUpAction;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public float SprintValue { get; private set; }
    public bool ShootTriggered { get; private set; }
    public bool HandsUpPressed { get; private set; }

    [Header("Shot specifiers")]
    public float vertangle;
    public float horizangle;
    public float initialvelocity;
    public float step;
    public float max;
    public float gravity;

    [Header("Prefabs")]
    public GameObject node;
    public GameObject ball;
    public GameObject ballchild;

    [Header("Shooting mechanics")]
    public float angledelta = .5f;
    public float powerdelta = .5f;
    public float spawntime;

    [Header("Movement Stuff")]
    public float speed;
    public float jumppower = 10;
    public Rigidbody body;
    public GameObject maincamera;
    public GameObject model;
    private PlayerInput _playerInput;
    private List<GameObject> myNodes = new List<GameObject>(); 
    private bool _isAiming = false;
    private int clicks = -1;
    private bool increase;
    private float initangle;
    private bool possession = false;

    public void ResetJump()
    {
        JumpTriggered = false;
    }

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        InputActionAsset inputAsset = _playerInput.actions;

        moveAction = inputAsset.FindActionMap(actionMapName).FindAction(move);
        lookAction = inputAsset.FindActionMap(actionMapName).FindAction(look);
        jumpAction = inputAsset.FindActionMap(actionMapName).FindAction(jump);
        sprintAction = inputAsset.FindActionMap(actionMapName).FindAction(sprint);
        shootAction = inputAsset.FindActionMap(actionMapName).FindAction("Shoot");
        handsUpAction = inputAsset.FindActionMap(actionMapName).FindAction(handsUp);

        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => LookInput = Vector2.zero;

        jumpAction.performed += context => JumpTriggered = true;
        jumpAction.canceled += context => JumpTriggered = false;

        sprintAction.performed += context => SprintValue = context.ReadValue<float>();
        sprintAction.canceled += context => SprintValue = 0.0f;

        shootAction.performed += context => {
            if (possession) {
                clicks++;
                ShootTriggered = true; 
            }
        };
        shootAction.canceled += context => ShootTriggered = false;

        handsUpAction.performed += context => HandsUpPressed = true;
        handsUpAction.canceled += context => HandsUpPressed = false;

        inputAsset.FindActionMap(actionMapName).Enable();
    }

    void FixedUpdate()
    {
        if (!_isAiming)
        {
            movement();
        }
    }

    void Update()
    {
        foreach (GameObject n in myNodes)
        {
            if (n != null) Destroy(n);
        }
        myNodes.Clear();

        if (clicks >= 0)
        {
            Shoot();
        }
    }

    void movement()
    {
        if (body == null) return;

        // Always use the closest enabled main camera to this player
        Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
        Camera closestMainCam = null;
        float minDist = float.MaxValue;
        foreach (Camera cam in cameras)
        {
            if (cam.enabled && cam.CompareTag("MainCamera"))
            {
                float dist = Vector3.Distance(transform.position, cam.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestMainCam = cam;
                }
            }
        }
        if (closestMainCam == null) return;
        maincamera = closestMainCam.gameObject;

        bool isGrounded = Mathf.Abs(body.linearVelocity.y) < 0.1f;

        if (JumpTriggered && isGrounded)
        {
            body.AddForce(Vector3.up * jumppower, ForceMode.Impulse);
            JumpTriggered = false;
        }

        if (MoveInput.magnitude > 0.2f && isGrounded)
        {
            float theta = Mathf.Atan2(-1 * MoveInput.y, MoveInput.x);
            Vector3 camRight = maincamera.transform.right;
            Vector3 right = new Vector3(camRight.x, 0, camRight.z).normalized;

            Vector3 cambasis = right * MoveInput.magnitude;
            cambasis = new Vector3(cambasis.magnitude * Mathf.Sin(theta), 0, cambasis.magnitude * Mathf.Cos(theta));

            body.linearVelocity = new Vector3(cambasis.x * speed, body.linearVelocity.y, cambasis.z * speed);
            gameObject.transform.eulerAngles = new Vector3(0, theta * 180 / Mathf.PI, 0);
        }
    }

    public void ShootPreview()
    {
        float dx = initialvelocity * Mathf.Cos(vertangle) * Mathf.Sin(horizangle);
        float dz = initialvelocity * Mathf.Cos(vertangle) * Mathf.Cos(horizangle);
        float ay = Physics.gravity.y * .5f;
        float vy = initialvelocity * Mathf.Sin(vertangle);
        
        if (step <= 0) step = .05f;

        for (float t = 0; t < max; t += step)
        {
            GameObject n = Instantiate(node, gameObject.transform.position + new Vector3(dx * t, ay * t * t + vy * t, dz * t), Quaternion.identity);
            myNodes.Add(n); 
        }
    }

    public void Shoot()
    {
        ShootPreview();
        switch (clicks)
        {
            case 0:
                _isAiming = true;
                horizangle = transform.rotation.eulerAngles.y;
                initangle = transform.rotation.eulerAngles.y;
                initialvelocity = 10;
                clicks++;
                break;

            case -1: 
                horizangle = increase ? horizangle + angledelta * Time.deltaTime : horizangle - angledelta * Time.deltaTime;
                if (Mathf.Abs(horizangle - initangle) > Mathf.PI / 4)
                {
                    horizangle = Mathf.Clamp(horizangle, initangle - Mathf.PI / 4, initangle + Mathf.PI / 4);
                    increase = !increase;
                }
                break;

            case 1: 
                initialvelocity = increase ? initialvelocity + powerdelta * Time.deltaTime : initialvelocity - powerdelta * Time.deltaTime;
                if (initialvelocity - 10 > 5 || initialvelocity - 10 < -5)
                {
                    initialvelocity = Mathf.Clamp(initialvelocity, 5, 15);
                    increase = !increase;
                }
                horizangle = transform.rotation.eulerAngles.y * Mathf.PI / 180;
                break;

            case 2: 
                float dx = initialvelocity * Mathf.Cos(vertangle) * Mathf.Sin(horizangle);
                float dz = initialvelocity * Mathf.Cos(vertangle) * Mathf.Cos(horizangle);
                float ay = Physics.gravity.y * .5f;
                float vy = initialvelocity * Mathf.Sin(vertangle);
                
                GameObject temp = Instantiate(ball, gameObject.transform.position + new Vector3(dx * spawntime, ay * spawntime * spawntime + vy * spawntime, dz * spawntime), Quaternion.identity);
                Rigidbody ballRb = temp.GetComponent<Rigidbody>();
                ballRb.linearVelocity = new Vector3(dx, vy, dz);

                _isAiming = false; 
                clicks = -1;
                possession = false;
                ballchild.SetActive(false);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball") && !possession)
        {
            possession = true;
            ballchild.SetActive(true);
            Destroy(other.gameObject);
        }
    }

    public bool HasBall() { return possession; }
}