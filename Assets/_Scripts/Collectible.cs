using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public void GrabCollectible()
    {
        this.gameObject.SetActive(false);
        ScoreManager.instance.IncrementCollectiblesGrabbed();
    }

    public void ResetCollectible()
    {
        this.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            this.GrabCollectible();
        }
    }
}
