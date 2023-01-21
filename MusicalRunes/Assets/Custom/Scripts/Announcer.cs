using System;
using MusicalRunes;
using TMPro;
using UnityEngine;

public class Announcer : MonoBehaviour, ILocalizable
{
    [SerializeField] private string previewTextId = "AnnouncerListen";
    [SerializeField] private string sequenceInputTextId = "AnnouncerPlayerTurn";
    [SerializeField] private string wrongRuneTextId = "AnnouncerFailedChoice";
    [SerializeField] private string timeoutTextId = "AnnouncerTimeout";
    [SerializeField] private string highScoreTextId = "AnnouncerHighScore";
    [SerializeField] private string failedByTimeoutTextId = "AnnouncerFailedByTimeout";
    [SerializeField] private string failedByRuneChoiceTextId = "AnnouncerFailedByRuneChoice";
    [SerializeField] private string bloodSacrificeTextId = "AnnouncerBloodSacrifice";

    [SerializeField] private float bounceAmplitude = 0.01f;
    [SerializeField] private float bounceFrequency = 5;

    [SerializeField] private TMP_Text announcerText;

    private string currentTextId;
    private bool mustFormat;
    private string formatParam;

    public void ShowPreviewText()
    {
        currentTextId = previewTextId;
        mustFormat = false;
        UpdateText();
    }

    public void ShowSequenceInputText()
    {
        currentTextId = sequenceInputTextId;
        mustFormat = false;
        UpdateText();
    }

    public void Clear()
    {
        currentTextId = String.Empty;
        mustFormat = false;
    }

    public void ShowWrongRuneText()
    {
        currentTextId = wrongRuneTextId;
        mustFormat = false;
        UpdateText();
    }

    public void ShowFailedByRuneChoiceText()
    {
        currentTextId = failedByRuneChoiceTextId;
        mustFormat = false;
        UpdateText();
    }

    public void ShowTimeoutText()
    {
        currentTextId = timeoutTextId;
        mustFormat = false;
        UpdateText();
    }

    public void ShowFailedByTimeoutText()
    {
        currentTextId = failedByTimeoutTextId;
        mustFormat = false;
        UpdateText();
    }

    public void ShowHighScoreText(int highScore)
    {
        currentTextId = highScoreTextId;
        mustFormat = true;
        formatParam = highScore.ToString();

        UpdateText();
    }

    public void ShowBloodSacrificeText()
    {
        currentTextId = bloodSacrificeTextId;
        mustFormat = false;
        UpdateText();
    }

    private void UpdateText()
    {
        if (currentTextId == String.Empty)
            announcerText.text = String.Empty;
        else if (mustFormat)
            announcerText.text = String.Format(Localization.GetLocalizedText(highScoreTextId), formatParam);
        else
            announcerText.text = Localization.GetLocalizedText(currentTextId);
    }

    public void LocaleChanged()
    {
        UpdateText();
    }

    private void Update()
    {
        transform.localScale = Vector3.one + Vector3.one * (Mathf.Sin(Time.time * bounceFrequency) * bounceAmplitude);
    }

    private void Awake()
    {
        Localization.RegisterWatcher(this);
    }

    private void OnDestroy()
    {
        Localization.DeregisterWatcher(this);
    }
}