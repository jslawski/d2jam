using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaunchField : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 25f;

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
        this._controlsDisabled = false;
    }

    public void DisableControls()
    {
        ControlsManager.RemovePerformedAction(ControlsManager.GetPlayerMapActions().PositivePolarity, this.LaunchPositive);
        ControlsManager.RemovePerformedAction(ControlsManager.GetPlayerMapActions().NegativePolarity, this.LaunchNegative);
        this._controlsDisabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (this._controlsDisabled == true)
        {
            return;
        }
    
        if (ControlsManager.IsInProgress(ControlsManager.GetPlayerMapActions().Left) == true)
        {
            this.RotateClockwise();
        }
        else if (ControlsManager.IsInProgress(ControlsManager.GetPlayerMapActions().Right) == true)
        {
            this.RotateCounterClockwise();
        }
    }

    private void RotateClockwise()
    {
        this._ringTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, this._ringTransform.localRotation.eulerAngles.z - (this._rotationSpeed * Time.deltaTime));
    }

    private void RotateCounterClockwise()
    {
        this._ringTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, this._ringTransform.localRotation.eulerAngles.z + (this._rotationSpeed * Time.deltaTime));
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
}
