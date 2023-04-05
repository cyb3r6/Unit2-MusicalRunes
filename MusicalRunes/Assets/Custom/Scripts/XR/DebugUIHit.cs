using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class DebugUIHit : MonoBehaviour
{
    [SerializeField]
    private XRRayInteractor rayInteractor;

    void Start()
    {
        //rayInteractor.hoverEntered.AddListener(DebugHit);
    }

    public void DebugHit()
    {
        Debug.Log($"Hover hit");
        DebugManager.Instance.LogInfo(rayInteractor.interactablesHovered[0].transform.name);
    }
}
