using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Train : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private LineRenderer _lineRenderer;

    public Transform trainTransform;

    private MagnetScanner _magnetScanner;

    public Polarity currentPolarity = Polarity.Positive;

    private int _currentPositionIndex = 0;

    private float _timeBetweenVertices = 0.02f;

    [SerializeField]
    private LaunchField _launchField;

    [SerializeField]
    private SpriteRenderer _sprite;

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

    public void Launch(LaunchField launcher, Polarity initialPolarity)
    {
        this._rigidbody.AddForce(this.trainTransform.up * GlobalVariables.ENGINE_FORCE);
        this._launchField = launcher;
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
    }

    void FixedUpdate()
    {
        Vector3 magnetizedVector = this._magnetScanner.GetMagnetizedForce();
        Vector3 engineVector = this.trainTransform.up * GlobalVariables.ENGINE_FORCE;

        Vector3 finalVector = magnetizedVector + engineVector;

        this._rigidbody.AddForce(finalVector);

        this.trainTransform.up = this._rigidbody.velocity.normalized;

        if (this._rigidbody.velocity.magnitude > GlobalVariables.MAX_VELOCITY)
        {
            this._rigidbody.velocity = this._rigidbody.velocity.normalized * GlobalVariables.MAX_VELOCITY;
        }

        ScoreManager.instance.IncrementTimer();
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
        this.currentPolarity = Polarity.Positive;
        this._sprite.color = Color.red;
    }

    private void ChangeToNegative(InputAction.CallbackContext context)
    {
        this.currentPolarity = Polarity.Negative;
        this._sprite.color = Color.blue;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Killzone")
        {        
            ScoreManager.instance.ResetCurrentScore();

            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        this.DisableControls();
        this._launchField.EnableControls();
        CollectibleManager.instance.ResetCollectibles();
    }
}
