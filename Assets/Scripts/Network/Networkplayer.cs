using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewEmptyCSharpScript : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody3D;
    [SerializeField] ConfigurableJoint mainJoint;

    // Local reference to the input script
    private PlayInputHandler inputHandler;

    // Input 
    Vector2 moveInputVector = Vector2.zero;
    bool isJumpButtonPressed = false;

    // Controller Settings
    float maxSpeed = 3;

    // States
    bool isGrounded = false;

    // Raycast
    RaycastHit[] raycastHits = new RaycastHit[10];

    void Start()
    {

        GameObject manager = GameObject.Find("Player Multiplayer Manager");
        if (manager != null)
        {
            transform.position = manager.transform.position;
        }
        inputHandler = GetComponent<PlayInputHandler>();
    }

    void Update()
    {
        if (inputHandler == null)
        {
            inputHandler = GetComponent<PlayInputHandler>();
        }
        if (inputHandler != null)
        {
            moveInputVector = inputHandler.MoveInput;
        
            if (inputHandler.JumpTriggered)
            {
                isJumpButtonPressed = true;
                inputHandler.ResetJump();
            }
        }
        else
        {
            // This is helpful for debugging multiplayer spawning
            Debug.LogWarning("PlayInputHandler component missing from " + gameObject.name);
        }
    }

    void FixedUpdate()
    {
        isGrounded = false;

        // Ground Check
        int numberOfHits = Physics.SphereCastNonAlloc(rigidbody3D.position, 0.5f, transform.up * -1, raycastHits, 0.5f);

        for (int i = 0; i < numberOfHits; i++)
        {
            if(raycastHits[i].transform.root == transform)
                continue;

            isGrounded = true;
                break;
        }

        if (!isGrounded)
            rigidbody3D.AddForce(Vector3.down * 10);
        
        float inputMagnituded = moveInputVector.magnitude;

        if (inputMagnituded != 0)
        {
            // Note: Fixed the typo in 'inputMagnituded' logic from your original snippet
            Quaternion desiredRotation = Quaternion.LookRotation(new Vector3(moveInputVector.x, 0, moveInputVector.y*-1), Vector3.up);

            mainJoint.targetRotation = Quaternion.RotateTowards(mainJoint.targetRotation, desiredRotation, Time.fixedDeltaTime * 300);

            Vector3 localVelocityfyVsForward = transform.forward * Vector3.Dot(transform.forward, rigidbody3D.linearVelocity);
            
            float localForwardVelocity = localVelocityfyVsForward.magnitude;
            
            if(localForwardVelocity < maxSpeed)
            {
                rigidbody3D.AddForce(transform.forward * inputMagnituded * 10);
            }
        }

        if(isGrounded && isJumpButtonPressed)
        {
            rigidbody3D.AddForce(Vector3.up * 20, ForceMode.Impulse);
            isJumpButtonPressed = false;
        }
    }
}