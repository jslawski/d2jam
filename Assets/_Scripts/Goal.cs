using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField]
    private AudioClip _targetReachedSound;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(other.gameObject);

            UIManager.instance.DisplayCurrentPointValues();
            ScoreManager.instance.UpdateLatestHighScore();

            AudioChannelSettings channelSettings = new AudioChannelSettings(false, 0.8f, 1.2f, 1.0f, "SFX", this.gameObject.transform);
            AudioManager.instance.Play(this._targetReachedSound, channelSettings);
        }
    }
}
