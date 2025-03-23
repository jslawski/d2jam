using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public static class ControlsManager
{
    private static PlayerControls playerControls;

    public static void Setup()
    {
        ControlsManager.playerControls = new PlayerControls();
        ControlsManager.playerControls.Enable();

        ControlsManager.playerControls.PlayerMap.Restart.performed += ControlsManager.RestartLevel;
    }

    public static void Cleanup()
    {
        ControlsManager.playerControls.Disable();
    }

    public static PlayerControls.PlayerMapActions GetPlayerMapActions()
    {
        return ControlsManager.playerControls.PlayerMap;
    }

    public static void AddPerformedAction(InputAction targetInputAction, System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> newAction)
    {
        targetInputAction.performed += newAction;
    }

    public static void RemovePerformedAction(InputAction targetInputAction, System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> removedAction)
    {
        targetInputAction.performed -= removedAction;
    }

    public static bool IsInProgress(InputAction targetInputAction)
    {
        return targetInputAction.inProgress;
    }

    private static void RestartLevel(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
