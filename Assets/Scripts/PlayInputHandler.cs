using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
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

    public Vector2 MoveInput { get; private set;}
    public Vector2 LookInput { get; private set;}
    public bool JumpTriggered { get; private set;}
    public float SprintValue {get; private set;}
    public bool ShootTriggered { get; private set;}
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
    public Rigidbody body;
    public GameObject maincamera;
    public GameObject model;
    /*public float strafeSpeed;
    public float jumpForce;
    public float turnSpeed = 2f;
    public float brakingForce = 5f;
    

    public Rigidbody hips;
    [Header("Hands")]
    public Rigidbody leftHand;
    public Rigidbody rightHand;
    public float handJumpForce = 5f;
    public float handShootForce = 20f;
    public bool isGrounded;

    [Header("Legs")]
    public Rigidbody leftLeg;
    public Rigidbody rightLeg;
    public float legStepForce = 50f;
    public float legLiftForce = 40f;
    public float stepRate = 10f;
    private float stepCycle = 0f;

    private static int playerCount = 0;*/


    private GameObject[] nodes;
    private int clicks = -1;
    private bool increase;
    private float initangle;
    private bool possession = false;

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
        }

        /*playerCount++;
        if (playerCount % 2 == 0)
        {
            SetTeamColor(Color.blue);
        }*/

        /*if (hips == null)
        {
            hips = GetComponent<Rigidbody>();
        }*/

        InputActionAsset inputAsset;
        if (TryGetComponent<PlayerInput>(out var pInput))
        {
            inputAsset = pInput.actions;
        }
        else
        {
            inputAsset = Instantiate(playerControls);
        }

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

        sprintAction.performed += context => SprintValue = context.ReadValue<float>();
        sprintAction.canceled += context => SprintValue = 0.0f;

        shootAction.performed += context => ShootTriggered = true;
        shootAction.canceled += context => ShootTriggered = false;

        handsUpAction.performed += context => HandsUpPressed = true;
        handsUpAction.canceled += context => HandsUpPressed = false;

        inputAsset.FindActionMap(actionMapName).Enable();
    }

    void Update()
    {
        nodes = GameObject.FindGameObjectsWithTag("node");
        foreach(GameObject node in nodes)
        {
            Destroy(node);
        }
        if (ShootTriggered && possession)
        {
            Debug.Log(ShootTriggered+" "+possession);
            clicks++;
            ShootTriggered = false;
        }
        if(clicks>=0){
            Shoot();
            //Debug.Log(clicks);
        }
        movement();

        /*if (hips != null && hips.position.y < -20f)
        {
            transform.position = new Vector3(0, 3, 0);
            hips.position = new Vector3(0, 3, 0);
            hips.linearVelocity = Vector3.zero;
            hips.angularVelocity = Vector3.zero;
        }*/
    }

    /*private void FixedUpdate()
    {
        if (hips == null)
        {
            return;
        }
        isGrounded = false;
        RaycastHit[] hits = Physics.RaycastAll(hips.position, Vector3.down, 1.1f);
        foreach (var hit in hits)
        {
            if (!hit.collider.isTrigger && !hit.transform.IsChildOf(transform))
            {
                isGrounded = true;
                break;
            }
        }

        if (JumpTriggered && isGrounded)
        {
            hips.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (leftHand != null) leftHand.AddForce(Vector3.up * handJumpForce, ForceMode.Impulse);
            if (rightHand != null) rightHand.AddForce(Vector3.up * handJumpForce, ForceMode.Impulse);
            JumpTriggered = false;
        }

        if (clicks >= 0 || HandsUpPressed)
        {
            if (leftHand != null) leftHand.AddForce(Vector3.up * handShootForce, ForceMode.Force);
            if (rightHand != null) rightHand.AddForce(Vector3.up * handShootForce, ForceMode.Force);
        }

        // --- ROTATION ---
        if (Mathf.Abs(LookInput.x) > 0.1f)
        {
            // Use time-based rotation instead of multiplier 
            // Default turnSpeed was 2, which is tiny. We'll use 150 as a hardcoded fast turn or assume user increases it.
            // But to "overhaul", we force a good value if it's small.
            float effectiveTurnSpeed = turnSpeed > 10 ? turnSpeed : 180f; 
            
            float turnAmount = LookInput.x * effectiveTurnSpeed * Time.fixedDeltaTime;
            Quaternion turnOffset = Quaternion.Euler(0, turnAmount, 0);
            hips.MoveRotation(turnOffset * hips.rotation);
        }

        // --- MOVEMENT ---
        Vector3 flatForward = Vector3.ProjectOnPlane(hips.transform.forward, Vector3.up).normalized;
        Vector3 flatRight = Vector3.ProjectOnPlane(hips.transform.right, Vector3.up).normalized;

        Vector3 moveDir = (flatForward * MoveInput.y + flatRight * MoveInput.x);
        
        // Analog control
        if (moveDir.sqrMagnitude > 1) moveDir.Normalize();

        // Calculate Target Velocity
        // Default speed variable might be small? 
        float targetSpeedVal = speed > 0 ? speed : 8f; 
        Vector3 targetVelocity = moveDir * targetSpeedVal * (1f + SprintValue);

        // Get Current Horizontal Velocity
        Vector3 currentVel = hips.linearVelocity;
        Vector3 horizontalVel = new Vector3(currentVel.x, 0, currentVel.z);

        // Calculate Velocity Error
        Vector3 velocityError = targetVelocity - horizontalVel;
        
        // Acceleration Factor (P-Controller Gain)
        // High value = snappy (no sliding), Low value = slippery
        // We use a high default if not set
        float effectiveAccel = 20f; 

        // Apply corrective force
        if (isGrounded)
        {
            hips.AddForce(velocityError * effectiveAccel, ForceMode.Acceleration);
        }
        else
        {
            // Air control (reduced)
            hips.AddForce(velocityError * effectiveAccel * 0.2f, ForceMode.Acceleration);
        }

        // Leg Animation
        if (moveDir.sqrMagnitude > 0.01f && isGrounded)
        {
            stepCycle += Time.fixedDeltaTime * stepRate * (1 + SprintValue);
            if(stepCycle > Mathf.PI * 2) stepCycle -= Mathf.PI * 2;

            float sineValue = Mathf.Sin(stepCycle);
            Vector3 forceDir = moveDir.normalized;

            if (sineValue > 0)
            {
                if (rightLeg != null)
                {
                    Vector3 lift = Vector3.up * legLiftForce * sineValue;
                    Vector3 move = forceDir * legStepForce;
                    rightLeg.AddForce(move + lift, ForceMode.Force);
                }
            }
            else
            {
                if (leftLeg != null) 
                {
                    Vector3 lift = Vector3.up * legLiftForce * -sineValue;
                    Vector3 move = forceDir * legStepForce;
                    leftLeg.AddForce(move + lift, ForceMode.Force);
                }
            }
        }
    }*/


    void movement()
    {
        //returns the angle from the Z axis from the controller
        float theta = Mathf.Atan2(-1*MoveInput.y,MoveInput.x);
        //defines a 2D unit vector along the cameras right vector, with the magnitude of the Movement input
        Vector3 right = new Vector3(maincamera.transform.right.x,0,maincamera.transform.right.z);
        right.Normalize();
        Vector3 cambasis = right*MoveInput.magnitude;
        //rotates cambasis by the angle of the input
        cambasis = new Vector3(cambasis.magnitude*Mathf.Sin(theta),0,cambasis.magnitude*Mathf.Cos(theta));
        body.linearVelocity = new Vector3(cambasis.x*speed,body.linearVelocity.y,cambasis.z*speed);
        //changes the rotation of the parent object to point to the movement vector
        gameObject.transform.eulerAngles = new Vector3(0,theta*180/Mathf.PI,0);
        Debug.Log(cambasis.magnitude+" "+theta*180/Mathf.PI);
        //matches parent object rotation with model render
        //model.transform.rotation = transform.rotation;
    }

    public void ShootPreview()
    {
        ShootTriggered = false;
        float dx = initialvelocity*Mathf.Cos(vertangle)*Mathf.Sin(horizangle);
        float dz = initialvelocity*Mathf.Cos(vertangle)*Mathf.Cos(horizangle);
        float ay = Physics.gravity.y*.5f;
        float vy = initialvelocity*Mathf.Sin(vertangle);
        if (step <= 0)
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
                initangle = transform.rotation.eulerAngles.y;
                initialvelocity = 10;
                clicks++;
                break;
            case -1:
                horizangle = increase?horizangle+angledelta*Time.deltaTime:horizangle-angledelta*Time.deltaTime;
                if (horizangle - initangle > Mathf.PI / 4 || horizangle - initangle < Mathf.PI / -4)
                {
                    horizangle = Mathf.Clamp(horizangle,initangle-Mathf.PI/4,initangle+Mathf.PI/4);
                    increase = !increase;
                }
                break;
            case 1:
                initialvelocity = increase?initialvelocity+powerdelta*Time.deltaTime:initialvelocity-powerdelta*Time.deltaTime;
                if (initialvelocity - 10 > 5 || initialvelocity - 10 < -5)
                {
                    initialvelocity = Mathf.Clamp(initialvelocity,5,15);
                    increase = !increase;
                }
                horizangle = transform.rotation.eulerAngles.y*Mathf.PI/180;
                break;
            case 2:
                float dx = initialvelocity*Mathf.Cos(vertangle)*Mathf.Sin(horizangle);
                float dz = initialvelocity*Mathf.Cos(vertangle)*Mathf.Cos(horizangle);
                float ay = Physics.gravity.y*.5f;
                float vy = initialvelocity*Mathf.Sin(vertangle);
                GameObject temp = Instantiate(ball,gameObject.transform.position+new Vector3(dx*spawntime,ay*spawntime*spawntime + vy*spawntime,dz*spawntime),Quaternion.identity);
                Rigidbody test = temp.GetComponent<Rigidbody>();
                test.linearVelocity = new Vector3(initialvelocity*Mathf.Cos(vertangle)*Mathf.Sin(horizangle),initialvelocity*Mathf.Sin(vertangle),initialvelocity*Mathf.Cos(vertangle)*Mathf.Cos(horizangle));
                playerControls.FindActionMap(actionMapName).Enable();
                clicks=-1;
                possession = false;
                ballchild.SetActive(false);
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

    void SetTeamColor(Color color)
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.material.color = color;
        }
    }

    void OnCollisionEnter(Collision collider){
        if(collider.gameObject.CompareTag("ball")){
            Debug.Log("grab ball");
            possession = true;
            ballchild.SetActive(true);
            Destroy(collider.gameObject);
        }
    }

    bool hasBall(){return possession;}
}
