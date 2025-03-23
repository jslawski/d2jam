using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ResetCollectibles()
    { 
        Collectible[] allCollectibles = GetComponentsInChildren<Collectible>(true);

        for (int i = 0; i < allCollectibles.Length; i++)
        {
            allCollectibles[i].ResetCollectible();
        }
    }
}
