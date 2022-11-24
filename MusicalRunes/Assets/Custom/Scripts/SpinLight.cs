using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinLight : MonoBehaviour
{
    public float spinVelocity;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        transform.Rotate(0,0,spinVelocity * Time.deltaTime);
    }
}
