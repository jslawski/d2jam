using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MagnetScanner : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve _forceCurve;

    [SerializeField]
    private LayerMask _magnetLayerMask;

    private Train _train;

    private int _numRaycastsPerSide = 20;
    private float _maxRaycastAnglePerSide = 120f;

    private List<RaycastHit> _currentMagnets;

    public bool debug = true;

    private void Awake()
    {
        this._currentMagnets = new List<RaycastHit>();
        this._train = GetComponent<Train>();
    }

    public Vector3 GetMagnetizedForce()
    {
        Vector3 totalForceVector = Vector3.zero;
        
        this.ScanForCurrentMagnets();
        
        /*
        if (this._currentMagnets.Count > 0)
        {
            Debug.LogError("Magnet Points: " + this._currentMagnets.Count);
        }
        */
        for (int i = 0; i < this._currentMagnets.Count; i++)
        {
            Magnet magnetComponent = this.GetMagnetComponent(this._currentMagnets[i].collider.gameObject);
            if (GlobalVariables.CURRENT_POLARITY == magnetComponent.polarity)
            {
                totalForceVector -= this._currentMagnets[i].normal.normalized * this.CalculateMagnetForce(this._currentMagnets[i].point);
            }
            else
            {
                totalForceVector += this._currentMagnets[i].normal.normalized * this.CalculateMagnetForce(this._currentMagnets[i].point);
            }            
        }

        return totalForceVector;
    }

    private Magnet GetMagnetComponent(GameObject magnetObject)
    {
        if (magnetObject == null)
        {
            return null;
        }
    
        Magnet returnMagnet = magnetObject.GetComponent<Magnet>();

        if (returnMagnet == null)
        {
            if (magnetObject.transform.parent != null)
            {
                return this.GetMagnetComponent(magnetObject.transform.parent.gameObject);
            }
        }
        else
        {
            return returnMagnet;
        }

        return null;
    }

    private float CalculateMagnetForce(Vector3 impactPoint)
    {
        float forceDistance = Vector3.Distance(impactPoint, this._train.trainTransform.position);
        float forceT = forceDistance / GlobalVariables.RAYCAST_DISTANCE;
        float forcePercentage = 1.0f - this._forceCurve.Evaluate(forceT);

        return (GlobalVariables.MAGNETIC_FORCE * forcePercentage);
    }

    private void ScanForCurrentMagnets()
    {
        this._currentMagnets.Clear();

        float angleIncrement = this._maxRaycastAnglePerSide / (float)this._numRaycastsPerSide;

        RaycastHit hitInfo = new RaycastHit();

        bool raycastResult = Physics.Raycast(this._train.trainTransform.position, this._train.trainTransform.up, out hitInfo, GlobalVariables.RAYCAST_DISTANCE, this._magnetLayerMask);

        if (raycastResult == true)
        {
            this._currentMagnets.Add(hitInfo);
        }

        for (float i = angleIncrement; i < this._maxRaycastAnglePerSide; i += angleIncrement)
        {
            Vector3 leftVector = Quaternion.Euler(0.0f, 0.0f, i) * this._train.trainTransform.up;
            RaycastHit leftHitInfo = new RaycastHit();
            bool leftRaycastResult = Physics.Raycast(this._train.trainTransform.position, leftVector, out leftHitInfo, GlobalVariables.RAYCAST_DISTANCE, this._magnetLayerMask);

            Vector3 rightVector = Quaternion.Euler(0.0f, 0.0f, -i) * this._train.trainTransform.up;
            RaycastHit rightHitInfo = new RaycastHit();
            bool rightRaycastResult = Physics.Raycast(this._train.trainTransform.position, rightVector, out rightHitInfo, GlobalVariables.RAYCAST_DISTANCE, this._magnetLayerMask);

            if (leftRaycastResult == true)
            {
                this._currentMagnets.Add(leftHitInfo);
            }

            if (rightRaycastResult == true)
            {
                this._currentMagnets.Add(rightHitInfo);
            }
        }

        if (this.debug == true)
        {
            this.DrawRaycasts();
        }
    }

    private void DrawRaycasts()
    {
        Vector3 currentDirection = this._train.trainTransform.up;

        float angleIncrement = this._maxRaycastAnglePerSide / (float)this._numRaycastsPerSide;

        Debug.DrawRay(this._train.trainTransform.position, currentDirection * GlobalVariables.RAYCAST_DISTANCE, Color.red);

        for (float i = angleIncrement; i < this._maxRaycastAnglePerSide; i += angleIncrement)
        {
            Vector3 leftVector = Quaternion.Euler(0.0f, 0.0f, i) * this._train.trainTransform.up;
            Debug.DrawRay(this._train.trainTransform.position, leftVector * GlobalVariables.RAYCAST_DISTANCE, Color.yellow);

            Vector3 rightVector = Quaternion.Euler(0.0f, 0.0f, -i) * this._train.trainTransform.up;
            Debug.DrawRay(this._train.trainTransform.position, rightVector * GlobalVariables.RAYCAST_DISTANCE, Color.green);
        }
    }
}
