using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CurvedUI;

public class CurvedGraphicPosition : MonoBehaviour
{
    [SerializeField]
    private CurvedUIVertexEffect curvedUIVertexEffect;
    [SerializeField]
    private RectTransform rectTransform;
    
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        rectTransform.anchoredPosition3D = curvedUIVertexEffect.GraphicPositionLocalPosition;
    }
}
