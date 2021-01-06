using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRotator : MonoBehaviour
{
    public float deg = 0.9f;

    void Start()
    {
        
    }

    void Update()
    {
        this.transform.Rotate(new Vector3(0, deg, 0), Space.Self);
    }
}
