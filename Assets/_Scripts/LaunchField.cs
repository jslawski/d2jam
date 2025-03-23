using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaunchField : MonoBehaviour
{
    private float _maxRotationSpeed = 175f;

    private float _currentRotationSpeed = 0.0f;

    private float _accelerationPerFrame = 35f;

    [SerializeField]
    private Transform _ringTransform;

    [SerializeField]
    private Transform _launcherTransform;

    [SerializeField]
    private GameObject _trainPrefab;

    private bool _controlsDisabled = false;

    [SerializeField]
    private Transform _worldTransform;    

    private void Awake()
    {
        ControlsManager.Setup();
        this.EnableControls();
    }    

    private void OnDisable()
    {
        ControlsManager.Cleanup();
    }

    public void EnableControls()
    {
        ControlsManager.AddPerformedAction(ControlsManager.GetPlayerMapActions().PositivePolarity, this.LaunchPositive);
        ControlsManager.AddPerformedAction(ControlsManager.GetPlayerMapActions().NegativePolarity, this.LaunchNegative);
        ControlsManager.AddPerformedAction(ControlsManager.GetPlayerMapActions().Left, this.ResetCurrentSpeed);
        ControlsManager.AddPerformedAction(ControlsManager.GetPlayerMapActions().Right, this.ResetCurrentSpeed);
        this._controlsDisabled = false;
    }

    public void DisableControls()
    {
        ControlsManager.RemovePerformedAction(ControlsManager.GetPlayerMapActions().PositivePolarity, this.LaunchPositive);
        ControlsManager.RemovePerformedAction(ControlsManager.GetPlayerMapActions().NegativePolarity, this.LaunchNegative);
        ControlsManager.RemovePerformedAction(ControlsManager.GetPlayerMapActions().Left, this.ResetCurrentSpeed);
        ControlsManager.RemovePerformedAction(ControlsManager.GetPlayerMapActions().Right, this.ResetCurrentSpeed);
        this._controlsDisabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (this._controlsDisabled == true)
        {
            return;
        }

        if (GlobalVariables.MOUSE_ROTATION == false)
        {
            if (ControlsManager.IsInProgress(ControlsManager.GetPlayerMapActions().Left) == true)
            {
                this.RotateClockwise();
            }
            else if (ControlsManager.IsInProgress(ControlsManager.GetPlayerMapActions().Right) == true)
            {
                this.RotateCounterClockwise();
            }
        }
        else
        {
            this.RotateToMousePosition();
        }
    }

    private void FixedUpdate()
    {
        if (ControlsManager.IsInProgress(ControlsManager.GetPlayerMapActions().Left) == true || ControlsManager.IsInProgress(ControlsManager.GetPlayerMapActions().Right) == true)
        {
            this.AccelerateRotation();
        }        
    }

    private void AccelerateRotation()
    {
        if (this._currentRotationSpeed < this._maxRotationSpeed)
        {
            this._currentRotationSpeed += this._accelerationPerFrame;
        }
    }

    private void RotateClockwise()
    {    
        this._ringTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, this._ringTransform.localRotation.eulerAngles.z - (this._currentRotationSpeed * Time.deltaTime));
    }

    private void RotateCounterClockwise()
    {
        this._ringTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, this._ringTransform.localRotation.eulerAngles.z + (this._currentRotationSpeed * Time.deltaTime));
    }

    private void LaunchPositive(InputAction.CallbackContext context)
    {
        this.GenerateTrain(Polarity.Positive);
        this.DisableControls();
    }

    private void LaunchNegative(InputAction.CallbackContext context)
    {
        this.GenerateTrain(Polarity.Negative);
        this.DisableControls();
    }

    private void GenerateTrain(Polarity initialPolarity)
    {
        GameObject currentTrainInstance = Instantiate(this._trainPrefab, this._launcherTransform.position, this._ringTransform.rotation);
        currentTrainInstance.GetComponent<Train>().Launch(this, initialPolarity);
    }

    private void ResetCurrentSpeed(InputAction.CallbackContext context)
    {
        this._currentRotationSpeed = 0.0f;
    }

    private void RotateToMousePosition()
    {
        Vector3 directionVector = Camera.main.ScreenToViewportPoint(Input.mousePosition) - (Vector3.one * 0.5f);

        this._ringTransform.up = new Vector3(-directionVector.x, -directionVector.y);
    }
}
