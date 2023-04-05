using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance;

    [SerializeField]
    private TextMeshProUGUI debugAreaText = null;

    [SerializeField]
    private bool enableDebug = false;

    [SerializeField]
    private int maxLines = 100;

    void Awake()
    {
        Instance = this;
    }
    void OnEnable()
    {
        debugAreaText.enabled = enableDebug;
        enabled = enableDebug;

        LogInfo("started");
    }

    public void LogInfo(string message)
    {
        ClearLines();
        debugAreaText.text += $"{DateTime.Now.ToString("yyyy-dd-M HH:mm:ss")}: <color=\"white\">{message}</color>\n";
    }

    public void LogError(string message)
    {
        ClearLines();
        debugAreaText.text += $"{DateTime.Now.ToString("yyyy-dd-M HH:mm:ss")}: <color=\"red\">{message}</color>\n";
    }

    public void LogWarning(string message)
    {
        ClearLines();
        debugAreaText.text += $"{DateTime.Now.ToString("yyyy-dd-M HH:mm:ss")}: <color=\"yellow\">{message}</color>\n";
    }

    private void ClearLines()
    {
        if (debugAreaText.text.Split('\n').Count() >= maxLines)
        {
            debugAreaText.text = string.Empty;
        }
    }
}
