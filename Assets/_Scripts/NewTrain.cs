using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class NewTrain : MonoBehaviour
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

    private float _maxTurnAngle = 90.0f;

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
        Vector3 finalVector = this.trainTransform.up * GlobalVariables.TRAIN_SPEED;

        if (this.latchedMagnet != null)
        {
            //Do latched movement logic
            finalVector = Vector3.zero;
        }
        else
        {
            Vector3 magnetizedVector = this._magnetScanner.GetMagnetizedForce();
            Vector3 engineVector = this.trainTransform.up * GlobalVariables.TRAIN_SPEED;

            Vector3 compositeVector = magnetizedVector + engineVector;
            Vector3 clampedDirection = compositeVector;// this.ClampMaxAngle(compositeVector);

            finalVector = clampedDirection.normalized * GlobalVariables.TRAIN_SPEED;

            this._rigidbody.AddForce(finalVector);

            //this._rigidbody.velocity = finalVector;

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
            Debug.LogError("CLAMPED!");
            return (Quaternion.Euler(0.0f, 0.0f, -this._maxTurnAngle) * this.trainTransform.up);
        }
        else
        {
            Debug.LogError("CLAMPED!");
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
    }

    private void ChangeToNegative(InputAction.CallbackContext context)
    {
        GlobalVariables.CURRENT_POLARITY = Polarity.Negative;
        this._pulseMaterial.SetColor("_Color", Color.blue);

        this._tracksSpriteRenderer.DOKill();
        this._tracksSpriteRenderer.DOColor(this._negativeColor, 0.5f);
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

    private void LatchToMagnet(GameObject latchedMagnet, Collision collisionInfo)
    {
        this.latchedMagnet = latchedMagnet;

        //this.trainTransform.position = collisionInfo.GetContact(0).point + (collisionInfo.GetContact(0).normal * collisionInfo.GetContact(0).separation);

        Debug.LogError(Vector3.SignedAngle(this.trainTransform.up, collisionInfo.GetContact(0).normal, Vector3.forward));

        this.trainTransform.up = Vector3.Cross(Vector3.forward, collisionInfo.GetContact(0).normal).normalized;
    }

    public Vector3 GetLatchedMagnetNormal()
    {
        RaycastHit hitInfo = new RaycastHit();

        bool raycastResult = Physics.Raycast(this.trainTransform.position, this.trainTransform.up, out hitInfo, GlobalVariables.RAYCAST_DISTANCE, this._magnetLayerMask);

        return Vector3.zero;
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

/*
 void FixedUpdate()
    {
        if (this.latchedMagnet != null)
        {
            //Do latched movement logic
            Vector3 finalVector = this.trainTransform.up * GlobalVariables.TRAIN_SPEED;
            finalVector = Vector3.zero;
        }
        else
        {
            Vector3 magnetizedVector = this._magnetScanner.GetMagnetizedForce();
            Vector3 engineVector = this.trainTransform.up * GlobalVariables.ENGINE_FORCE;

            Vector3 compositeVector = magnetizedVector + engineVector;
            Vector3 clampedDirection = this.ClampMaxAngle(compositeVector);
            Vector3 finalVector = clampedDirection.normalized * compositeVector.magnitude;

            this._rigidbody.AddForce(finalVector);

            this.trainTransform.up = this._rigidbody.velocity.normalized;

            if (this._rigidbody.velocity.magnitude > GlobalVariables.MAX_VELOCITY)
            {
                this._rigidbody.velocity = this._rigidbody.velocity.normalized * GlobalVariables.MAX_VELOCITY;
            }
        }

        ScoreManager.instance.IncrementTimer();
    }
 */ 