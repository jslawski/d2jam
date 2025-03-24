using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Polarity { Positive, Negative, None }

public class Magnet : MonoBehaviour
{
    [Header("Step 1: Choose a polarity")]
    [Header("Step 2: Add your 3D model as a child of this prefab")]
    [Header("Step 3: Add a MeshCollider to the object that contains the MeshRenderer")]
    public Polarity polarity = Polarity.None;

    public MeshRenderer[] _meshRenderers;

    private void Awake()
    {
        this._meshRenderers = GetComponentsInChildren<MeshRenderer>();   
    }

    private void Update()
    {
        if (this.polarity == GlobalVariables.CURRENT_POLARITY)
        {
            this._meshRenderers[1].material.SetVector("_Scroll_Speed", new Vector3(0.0f, 2.0f));
        }
        else
        {
            this._meshRenderers[1].material.SetVector("_Scroll_Speed", new Vector3(0.0f, -2.0f));
        }
    }
}
