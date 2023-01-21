using System;
using TMPro;
using UnityEngine;

public class Announcer : MonoBehaviour
{
    [SerializeField] private string previewText = "Hey, Listen!";
    [SerializeField] private string sequenceInputText = "Show me what you got!";
    [SerializeField] private string wrongRuneText = "Oh no!!!";
    [SerializeField] private string timeoutText = "Timeout!";
    [SerializeField] private string highScoreText = "New High Score: {0}";

    [SerializeField] private float bounceAmplitude = 0.01f;
    [SerializeField] private float bounceFrequency = 5;

    [SerializeField] private TMP_Text announcerText;

    public void ShowPreviewText()
    {
        announcerText.text = previewText;
    }

    public void ShowSequenceInputText()
    {
        announcerText.text = sequenceInputText;
    }

    public void Clear()
    {
        announcerText.text = String.Empty;
    }

    public void ShowWrongRuneText()
    {
        announcerText.text = wrongRuneText;
    }

    public void ShowTimeoutText()
    {
        announcerText.text = timeoutText;
    }

    public void ShowHighScoreText(int highScore)
    {
        announcerText.text = String.Format(highScoreText, highScore);
    }

    private void Update()
    {
        transform.localScale = Vector3.one + Vector3.one * (Mathf.Sin(Time.time * bounceFrequency) * bounceAmplitude);
    }
}