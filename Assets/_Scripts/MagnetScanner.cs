using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetScanner : MonoBehaviour
{
    [SerializeField]
    private Transform _trainTransform;

    private float _maxRaycastDistance = 1.0f;

    private int _numRaycastsPerSide = 20;
    private float _maxRaycastAnglePerSide = 120f;

    private List<RaycastHit> _currentMagnets;

    private void Awake()
    {
        this._currentMagnets = new List<RaycastHit>();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.DrawRaycasts();
    }

    private void DrawRaycasts()
    {
        Vector3 currentDirection = this._trainTransform.up;

        float angleIncrement = this._maxRaycastAnglePerSide / (float)this._numRaycastsPerSide;

        Debug.DrawRay(this._trainTransform.position, currentDirection * this._maxRaycastDistance, Color.red);

        for (float i = 0; i < this._maxRaycastAnglePerSide; i += angleIncrement)
        {
            Vector3 leftVector = Quaternion.Euler(0.0f, 0.0f, -i) * this._trainTransform.up;
            Debug.DrawRay(this._trainTransform.position, leftVector * this._maxRaycastDistance, Color.yellow);

            Vector3 rightVector = Quaternion.Euler(0.0f, 0.0f, i) * this._trainTransform.up;
            Debug.DrawRay(this._trainTransform.position, rightVector * this._maxRaycastDistance, Color.green);
        }
    }
}
