using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public void SetMagneticForce(Slider targetSlider)
    {
        GlobalVariables.MAGNETIC_FORCE = targetSlider.value;
    }

    public void SetRaycastDistance(Slider targetSlider)
    {
        GlobalVariables.RAYCAST_DISTANCE = targetSlider.value;
    }

    public void SetEngineForce(Slider targetSlider)
    {
        GlobalVariables.ENGINE_FORCE = targetSlider.value;
    }

    public void SetMaxVelocity(Slider targetSlider)
    {
        GlobalVariables.MAX_VELOCITY = targetSlider.value;
    }
}
