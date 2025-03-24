using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public static float MAGNETIC_FORCE = 5.0f;
    public static float RAYCAST_DISTANCE = 1.0f; //1.5
    public static float ENGINE_FORCE = 10f;
    public static float MAX_VELOCITY = 6.0f; //6.0f

    public static bool MOUSE_ROTATION = false;

    public static bool PREVENT_SHOOTING = false;

    public static Polarity CURRENT_POLARITY = Polarity.Positive;
}
