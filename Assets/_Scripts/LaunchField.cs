using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaunchField : MonoBehaviour
{
    private PlayerControls _playerControls;

    [SerializeField]
    private float _rotationSpeed = 25f;

    [SerializeField]
    private Transform _ringTransform;

    private void Awake()
    {
        this._playerControls = new PlayerControls();
    }
    

    private void OnEnable()
    {
        this._playerControls.Enable();
    }

    private void OnDisable()
    {
        this._playerControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this._playerControls.PlayerMap.Left.inProgress == true)
        {
            this.RotateClockwise();
        }
        else if (this._playerControls.PlayerMap.Right.inProgress == true)
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
}
