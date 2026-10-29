using UnityEngine;
using UnityEngine.InputSystem;

public class MouseController : MonoBehaviour
{
    void Start()
    {
        TravarMouse();
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                DestravarMouse();
            }
            else
            {
                TravarMouse();
            }
        }
    }

    void TravarMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void DestravarMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}