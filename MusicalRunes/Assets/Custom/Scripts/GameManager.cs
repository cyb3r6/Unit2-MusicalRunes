using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private readonly string saveKey = "SaveKey";

    public static GameManager Instance { get; private set; }

    [Header("Runes Settings")]
    [SerializeField] private int initialSequenceSize = 3;
    [SerializeField] private int initialBoardSize = 4;
    [SerializeField] private RectTransform runesHolder;
    [SerializeField] private List<Rune> availableRunePrefabs;
    private List<Rune> boardRunes;
    private List<Rune> instantiatedBoardRunes;

    [Header("Coins Settings")]
    [SerializeField] private int coinsPerRune = 1;
    [SerializeField] private int coinsPerRound = 10;

    [Header("Preview Settings")]
    [SerializeField] private GameObject[] spinlights;
    [SerializeField] private float delayBetweenRunePreview = .3f;

    [Header("UI References")]
    [SerializeField] private Announcer announcer;
    [SerializeField] private TMP_Text coinsAmountText;
    [SerializeField] private TMP_Text highScoreText;

    private int coinsAmount
    {
        get => saveData.coinsAmount;
        set
        {
            saveData.coinsAmount = value;
            coinsAmountText.text = coinsAmount.ToString();
        }
    }

    private int highScore
    {
        get => saveData.highScore;
        set
        {
            saveData.highScore = value;
            Save();

            highScoreText.text = saveData.highScore.ToString();
        }

    }

    private List<int> currentRuneSequence;
    private int currentPlayIndex;
    private int currentRound;

    private SaveData saveData;

    public void OnRuneActivated(int index)
    {
        if (currentRuneSequence[currentPlayIndex] == index)
            CorrectRuneSelected();
        else
            StartCoroutine(FailedSequence());
    }

    private IEnumerator FailedSequence()
    {
        SetRunesInteractivity(false);

        announcer.ShowWrongRuneText();

        yield return new WaitForSeconds(2);

        if (currentRound > highScore)
        {
            highScore = currentRound;
            announcer.ShowHighScoreText(highScore);
            Save();
            yield return new WaitForSeconds(3);
        }

        Reset();
        currentPlayIndex = 0;
        currentRound = 0;

        yield return PlaySequencePreview(2);
    }

    private void CorrectRuneSelected()
    {
        coinsAmount += coinsPerRune;
        currentPlayIndex++;

        if (currentPlayIndex >= currentRuneSequence.Count)
            CompletedSequence();
        else
            Save();
    }

    private void CompletedSequence()
    {
        coinsAmount += coinsPerRound;
        currentRound++;
        Save();

        currentRuneSequence.Add(Random.Range(0, boardRunes.Count));
        currentPlayIndex = 0;

        StartCoroutine(PlaySequencePreview());
    }

    private IEnumerator PlaySequencePreview(float startDelay = 1)
    {
        SetRunesInteractivity(false);
        announcer.Clear();

        yield return new WaitForSeconds(1);

        EnablePreviewFeedback();

        yield return new WaitForSeconds(startDelay);

        foreach (var runeIndex in currentRuneSequence)
        {
            yield return boardRunes[runeIndex].ActivateRune();
            yield return new WaitForSeconds(delayBetweenRunePreview);
        }

        DisablePreviewFeedback();
        SetRunesInteractivity(true);
    }

    private void SetRunesInteractivity(bool interactable)
    {
        foreach (var rune in boardRunes)
        {
            if (interactable)
                rune.EnableInteraction();
            else
                rune.DisableInteraction();
        }
    }

    private void EnablePreviewFeedback()
    {
        foreach (var spinlight in spinlights)
            spinlight.SetActive(true);

        announcer.ShowPreviewText();
    }

    private void DisablePreviewFeedback()
    {
        foreach (var spinlight in spinlights)
            spinlight.SetActive(false);

        announcer.ShowSequenceInputText();
    }

    private void Awake()
    {
        if (Instance != null)
        {
            throw new Exception($"Multiple singleton instances! {Instance} :: {this}");
        }
        Instance = this;

        LoadSaveData();

        InitializeBoard();
        InitializeSequence();
        InitializeUI();

        StartCoroutine(PlaySequencePreview(2));
    }

    private void Reset()
    {
        for (int i = runesHolder.childCount - 1; i >= 0; i--)
            Destroy(runesHolder.GetChild(i).gameObject);

        availableRunePrefabs.AddRange(instantiatedBoardRunes);

        InitializeBoard();
        InitializeSequence();
    }

    private void AddRandomRuneToBoard()
    {
        var runePrefab = availableRunePrefabs[Random.Range(0, availableRunePrefabs.Count)];
        availableRunePrefabs.Remove(runePrefab);
        instantiatedBoardRunes.Add(runePrefab);

        var rune = Instantiate(runePrefab, runesHolder);
        rune.Setup(boardRunes.Count, this);
        boardRunes.Add(rune);
    }

    private void InitializeBoard()
    {
        boardRunes = new List<Rune>(initialBoardSize);
        instantiatedBoardRunes = new List<Rune>();

        for (int i = 0; i < initialBoardSize; i++)
            AddRandomRuneToBoard();
    }

    private void InitializeSequence()
    {
        currentRuneSequence = new List<int>(initialSequenceSize);
        for (int i = 0; i < initialSequenceSize; i++)
            currentRuneSequence.Add(Random.Range(0, boardRunes.Count));
    }

    private void InitializeUI()
    {
        highScoreText.text = saveData.highScore.ToString();
        coinsAmountText.text = coinsAmount.ToString();
    }

    private void LoadSaveData()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            string serializedSaveData = PlayerPrefs.GetString(saveKey);
            saveData = JsonUtility.FromJson<SaveData>(serializedSaveData);

            return;
        }

        saveData = new SaveData();
    }

    private void Save()
    {
        string serializedSaveData = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(saveKey, serializedSaveData);
    }
}
