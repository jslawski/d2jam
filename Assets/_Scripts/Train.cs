using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Train : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private LineRenderer _lineRenderer;

    public Transform trainTransform;

    private MagnetScanner _magnetScanner;

    private int _currentPositionIndex = 0;

    private float _timeBetweenVertices = 0.02f;

    [SerializeField]
    private LaunchField _launchField;

    [SerializeField]
    private SpriteRenderer _sprite;

    [SerializeField]
    private Material _pulseMaterial;

    private SpriteRenderer _tracksSpriteRenderer;

    [SerializeField]
    private Color _neutralColor;
    [SerializeField]
    private Color _positiveColor;
    [SerializeField]
    private Color _negativeColor;

    private float _maxTurnAngle = 60.0f;

    [SerializeField]
    private AudioClip _crashSound;
    [SerializeField]
    private AudioClip _launchSound;

    public GameObject latchedMagnet;

    [SerializeField]
    private LayerMask _magnetLayerMask;

    private void Awake()
    {
        this._rigidbody = GetComponent<Rigidbody>();
        this._lineRenderer = GetComponentInChildren<LineRenderer>();
        this.trainTransform = GetComponent<Transform>();
        this._magnetScanner = GetComponent<MagnetScanner>();
    }

    public void EnableControls()
    {
        ControlsManager.AddPerformedAction(ControlsManager.GetPlayerMapActions().PositivePolarity, this.ChangeToPositive);
        ControlsManager.AddPerformedAction(ControlsManager.GetPlayerMapActions().NegativePolarity, this.ChangeToNegative);
    }

    public void DisableControls()
    {
        ControlsManager.RemovePerformedAction(ControlsManager.GetPlayerMapActions().PositivePolarity, this.ChangeToPositive);
        ControlsManager.RemovePerformedAction(ControlsManager.GetPlayerMapActions().NegativePolarity, this.ChangeToNegative);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(this.UpdateLineRenderer());
    }

    public void Launch(LaunchField launcher, Polarity initialPolarity, SpriteRenderer tracksSpriteRenderer)
    {
        this._rigidbody.velocity = this.trainTransform.up * GlobalVariables.TRAIN_SPEED;
        this._launchField = launcher;
        this._tracksSpriteRenderer = tracksSpriteRenderer;

        this.EnableControls();

        if (initialPolarity == Polarity.Positive)
        {
            this.ChangeToPositive(new InputAction.CallbackContext());
        }
        else if (initialPolarity == Polarity.Negative)
        {
            this.ChangeToNegative(new InputAction.CallbackContext());
        }

        ScoreManager.instance.ResetCurrentScore();
        UIManager.instance.HideCurrentPointValues();

        AudioChannelSettings channelSettings = new AudioChannelSettings(false, 0.8f, 1.2f, 1.0f, "SFX", this.gameObject.transform);
        AudioManager.instance.Play(this._launchSound, channelSettings);
    }

    void FixedUpdate()
    {
        Vector3 finalVector = Vector3.zero;
        
        if (this.latchedMagnet != null)
        {
            finalVector = this.GetLatchedVelocityDirection();
        }
        /*
        if (finalVector == Vector3.forward)
        {
            Debug.LogError("Unlatch");
            this.Unlatch();
        }
        */
        if (this.latchedMagnet != null)
        {
            this._rigidbody.velocity = finalVector.normalized * GlobalVariables.MAX_VELOCITY;
            //this.trainTransform.up = this._rigidbody.velocity.normalized;

            Debug.LogError("Latched Mode");
        }
        else
        {
            Vector3 magnetizedVector = this._magnetScanner.GetMagnetizedForce();
            Vector3 engineVector = this.trainTransform.up * GlobalVariables.ENGINE_FORCE;

            Vector3 compositeVector = magnetizedVector + engineVector;
            Vector3 clampedDirection = this.ClampMaxAngle(compositeVector);
            finalVector = clampedDirection.normalized * compositeVector.magnitude;

            this._rigidbody.AddForce(finalVector);

            this.trainTransform.up = this._rigidbody.velocity.normalized;

            if (this._rigidbody.velocity.magnitude > GlobalVariables.MAX_VELOCITY)
            {
                this._rigidbody.velocity = this._rigidbody.velocity.normalized * GlobalVariables.MAX_VELOCITY;
            }
        }

        ScoreManager.instance.IncrementTimer();
    }

    private Vector3 ClampMaxAngle(Vector3 newVector)
    {
        float currentAngle = Vector3.SignedAngle(this.trainTransform.up, newVector, Vector3.forward);

        if (Mathf.Abs(currentAngle) <= this._maxTurnAngle)
        {
            return newVector;
        }
        else if (currentAngle < 0)
        {
            return (Quaternion.Euler(0.0f, 0.0f, -this._maxTurnAngle) * this.trainTransform.up);
        }
        else
        {
            return (Quaternion.Euler(0.0f, 0.0f, this._maxTurnAngle) * this.trainTransform.up);
        }
    }

    private IEnumerator UpdateLineRenderer()
    {
        while (true)
        {
            if (this._currentPositionIndex > (this._lineRenderer.positionCount - 1))
            {
                this._lineRenderer.positionCount++;
            }

            this._lineRenderer.SetPosition(this._currentPositionIndex, this.trainTransform.position);

            if (this._currentPositionIndex != 0)
            {
                Vector3 previousPosition = this._lineRenderer.GetPosition(this._currentPositionIndex - 1);
                Vector3 currentPosition = this._lineRenderer.GetPosition(this._currentPositionIndex);
                float incrementalDistance = Vector3.Distance(previousPosition, currentPosition);
                ScoreManager.instance.IncrementDistanceTravelled(incrementalDistance);
            }

            this._currentPositionIndex++;

            yield return new WaitForSeconds(this._timeBetweenVertices);
        }
    }

    private void ChangeToPositive(InputAction.CallbackContext context)
    {
        GlobalVariables.CURRENT_POLARITY = Polarity.Positive;
        this._pulseMaterial.SetColor("_Color", Color.red);

        this._tracksSpriteRenderer.DOKill();
        this._tracksSpriteRenderer.DOColor(this._positiveColor, 0.5f);

        if (this.latchedMagnet != null)
        {
            this.Unlatch();
        }
    }

    private void ChangeToNegative(InputAction.CallbackContext context)
    {
        GlobalVariables.CURRENT_POLARITY = Polarity.Negative;
        this._pulseMaterial.SetColor("_Color", Color.blue);

        this._tracksSpriteRenderer.DOKill();
        this._tracksSpriteRenderer.DOColor(this._negativeColor, 0.5f);

        if (this.latchedMagnet != null)
        {
            this.Unlatch();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Killzone")
        {
            ScoreManager.instance.ResetCurrentScore();

            AudioChannelSettings channelSettings = new AudioChannelSettings(false, 0.8f, 1.2f, 1.0f, "SFX", this.gameObject.transform);
            AudioManager.instance.Play(this._crashSound, channelSettings);

            Destroy(this.gameObject);
        }

        if (collision.collider.tag == "Magnet")
        {
            this.LatchToMagnet(collision.collider.gameObject, collision);
        }
    }

    private void Unlatch()
    {
        this.latchedMagnet = null;
    }

    private void LatchToMagnet(GameObject latchedMagnet, Collision collisionInfo)
    {
        this.latchedMagnet = latchedMagnet;

        Vector3 xyNormal = new Vector3(collisionInfo.GetContact(0).normal.x, collisionInfo.GetContact(0).normal.y, 0.0f);

        Vector3 cross1 = Vector3.Cross(Vector3.forward, xyNormal).normalized;
        Vector3 cross2 = Vector3.Cross(xyNormal, Vector3.forward).normalized;

        //Debug.LogError("Train Rotation Z: " + this.trainTransform.rotation.eulerAngles.z);

        //Debug.LogError("Cross1: " + cross1 + "\nCross2: " + cross2);

        float dot1 = Vector3.Dot(this.trainTransform.up, cross1);
        float dot2 = Vector3.Dot(this.trainTransform.up, cross2);

        //Debug.LogError("Dot1: " + dot1 + "\nDot2: " + dot2);

        if (dot1 > dot2)
        {
            //Debug.LogError("Cross1");
            float angle = this.trainTransform.rotation.eulerAngles.z - Vector3.SignedAngle(this.trainTransform.up, cross1, -Vector3.forward);//(Mathf.Acos(dot1) * Mathf.Rad2Deg);

            //Debug.LogError("Angle: " + angle);

            this.trainTransform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
        }
        else
        {
            //Debug.LogError("Cross2");

            float angle = this.trainTransform.rotation.eulerAngles.z - Vector3.SignedAngle(this.trainTransform.up, cross2, -Vector3.forward);//(Mathf.Acos(dot1) * Mathf.Rad2Deg);

            //Debug.LogError("Angle: " + angle);
            
            this.trainTransform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
        }       
    }

    public Vector3 GetLatchedMagnetNormal()
    {
        RaycastHit hitInfo = new RaycastHit();



        bool raycastResult = Physics.Raycast(this.trainTransform.position, this.trainTransform.up, out hitInfo, GlobalVariables.RAYCAST_DISTANCE, this._magnetLayerMask);

        return Vector3.zero;
    }

    public Vector3 GetLatchedVelocityDirection()
    {
        Vector3 cross1 = Vector3.Cross(Vector3.forward, this.trainTransform.up).normalized;
        Vector3 cross2 = Vector3.Cross(this.trainTransform.up, Vector3.forward).normalized;

        RaycastHit hitInfo1 = new RaycastHit();
        bool raycastResult1 = Physics.Raycast(this.trainTransform.position, cross1, out hitInfo1, GlobalVariables.RAYCAST_DISTANCE, this._magnetLayerMask);

        RaycastHit hitInfo2 = new RaycastHit();
        bool raycastResult2 = Physics.Raycast(this.trainTransform.position, cross2, out hitInfo2, GlobalVariables.RAYCAST_DISTANCE, this._magnetLayerMask);

        Debug.LogError("Result1: " + raycastResult1 + "\nResult2: " + raycastResult2);

        if (raycastResult1 == true && hitInfo1.collider.gameObject == this.latchedMagnet)
        {
            Debug.LogError("RESULT 1");
            return Vector3.Cross(Vector3.forward, hitInfo1.normal).normalized;
        }
        else if (raycastResult2 == true && hitInfo2.collider.gameObject == this.latchedMagnet)
        {
            Debug.LogError("RESULT 2");
            return Vector3.Cross(-Vector3.forward, hitInfo2.normal).normalized;
        }
        else
        {
            Debug.LogError("FALLBACK");
            return Vector3.forward;
        }
    }

    private void OnDestroy()
    {
        if (this._tracksSpriteRenderer == null)
        {
            this._tracksSpriteRenderer.color = this._neutralColor;
        }

        this.DisableControls();
        this._launchField.EnableControls();

        if (CollectibleManager.instance != null)
        {
            CollectibleManager.instance.ResetCollectibles();
        }
    }
}