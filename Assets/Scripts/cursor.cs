using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;



public class GamepadCursor : MonoBehaviour
{
    public float cursorSpeed = 1000f;
    private Vector2 mouse;
    private Vector2 thumb1;
    private Vector2 thumb2;


    void Awake()
    {
    }
    void Update()
    {
        //get x button


            //get thumbstick
            thumb1 = Gamepad.current.leftStick.ReadValue();
        thumb2 = Gamepad.current.rightStick.ReadValue();

        mouse = Mouse.current.position.ReadValue();
        Vector2 newPosition = mouse + (thumb1 + thumb2) * cursorSpeed * Time.deltaTime;

        Mouse.current.WarpCursorPosition(newPosition);
    
    }
}
