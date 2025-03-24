using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(other.gameObject);

            UIManager.instance.DisplayCurrentPointValues();
            ScoreManager.instance.UpdateLatestHighScore();
        }
    }
}
