using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewEmptyCSharpScript : MonoBehaviour
{
    [SerializeField]
    Rigidbody rigidbody3D;

    [SerializeField]
    ConfigurableJoint mainJoint;

    //Input 
    Vector2 moveInputVector = Vector2.zero;
    bool isJumpButtonPressed = false;

    //Contorller Settings
    float maxSpeed = 3;

    //States
    bool isGrounded = false;

    //Raycast
    RaycastHit[] raycastHits = new RaycastHit[10];

    void Start()
    {
        
    }

    void Update()
    {
        if (PlayInputHandler.Instance != null)
        {
            moveInputVector = PlayInputHandler.Instance.MoveInput;
        
            if (PlayInputHandler.Instance.JumpTriggered)
            {
                isJumpButtonPressed = true;
                PlayInputHandler.Instance.ResetJump();
            }
        }
    else
    {
        Debug.LogWarning("PlayInputHandler not found in scene!");
    }

    }

    void FixedUpdate()
    {
        isGrounded = false;

        //Ground Check
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

        Debug.Log("Input: " + moveInputVector + " | Grounded: " + isGrounded + " | Velocity: " + rigidbody3D.linearVelocity);
    }
}

