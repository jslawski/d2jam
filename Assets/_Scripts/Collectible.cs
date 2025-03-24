using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField]
    private AudioClip _collectibleGetSound;

    public void GrabCollectible()
    {
        AudioChannelSettings channelSettings = new AudioChannelSettings(false, 0.8f, 1.2f, 1.0f, "SFX", this.gameObject.transform);
        AudioManager.instance.Play(this._collectibleGetSound, channelSettings);

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
