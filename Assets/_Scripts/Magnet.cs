using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Polarity { Positive, Negative, None }

public class Magnet : MonoBehaviour
{
    public Polarity polarity = Polarity.None;

    public float magnetForce = 0.5f;
}
