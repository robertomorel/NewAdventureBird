using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MSJoystickController : MonoBehaviour
{

    public MSJoystick joystick;
    public static Vector2 joystickInput;//é possível acessar através de MSJoystickController.joystickInput

    void Update()
    {
        if (joystick)
        {
            joystickInput = new Vector2(joystick.joystickX, joystick.joystickY);
        }
    }
}
