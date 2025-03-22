using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Train : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private LineRenderer _lineRenderer;

    private Transform _transform;

    private int _currentPositionIndex = 0;

    private float _baseEngineVelocity = 5.0f;

    private const float MAX_VELOCITY = 10.0f;

    private float _timeBetweenVertices = 0.02f;

    private void Awake()
    {
        this._rigidbody = GetComponent<Rigidbody>();
        this._lineRenderer = GetComponentInChildren<LineRenderer>();
        this._transform = GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(this.UpdateLineRenderer());           
    }

    public void Launch()
    {        
        this._rigidbody.velocity = this._transform.up * this._baseEngineVelocity;        
    }

    void FixedUpdate()
    {    
        if (this._rigidbody.velocity.magnitude > MAX_VELOCITY)
        {
            this._rigidbody.velocity = this._rigidbody.velocity.normalized * MAX_VELOCITY;
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
            
            this._lineRenderer.SetPosition(this._currentPositionIndex, this.transform.position);
            this._currentPositionIndex++;

            yield return new WaitForSeconds(this._timeBetweenVertices);
        }
    }
}
