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
    [SerializeField] private int increaseBoardSizeEveryXSequences = 5;
    [SerializeField] private RectTransform runesHolder;
    [SerializeField] private List<Rune> availableRunePrefabs;
    public List<Rune> BoardRunes { get; private set; }
    private List<Rune> instantiatedBoardRunes;

    [Header("Coins Settings")]
    [SerializeField] private int coinsPerRune = 1;
    [SerializeField] private int coinsPerRound = 10;

    [Header("Preview Settings")]
    [SerializeField] private GameObject[] spinlights;
    [SerializeField] private float delayBetweenRunePreview = .3f;

    [Header("Power Up Settings")]
    [SerializeField] private List<Powerup> powerups;

    [Header("UI References")]
    [SerializeField] private Announcer announcer;
    [SerializeField] private TMP_Text coinsAmountText;
    [SerializeField] private TMP_Text highScoreText;

    public Action<int> coinsChanged;
    public Action sequenceCompleted;
    public Action runeActivated;

    private int coinsAmount
    {
        get => saveData.coinsAmount;
        set
        {
            saveData.coinsAmount = value;
            coinsAmountText.text = coinsAmount.ToString();

            coinsChanged?.Invoke(value);
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
    public int CurrentRuneIndex => currentRuneSequence[currentPlayIndex];
    private int currentPlayIndex;
    private int currentRound;

    private SaveData saveData;

    public void OnRuneActivated(int index)
    {
        if (CurrentRuneIndex == index)
            CorrectRuneSelected();
        else
            StartCoroutine(FailedSequence());
    }

    public void PurgeBoardRune(int removedIndex)
    {
        var rune = BoardRunes[removedIndex];

        for (int i = BoardRunes.Count - 1; i > removedIndex; i--)
        {
            BoardRunes[i].Setup(i - 1, this);
        }

        for (int i = currentRuneSequence.Count - 1; i >= 0; i--)
        {
            var index = currentRuneSequence[i];
            if (index == removedIndex) currentRuneSequence.RemoveAt(i);
            else if (index > removedIndex) currentRuneSequence[i]--;
        }

        Destroy(runesHolder.GetChild(removedIndex).gameObject);

        availableRunePrefabs.Add(instantiatedBoardRunes[removedIndex]);
        instantiatedBoardRunes.RemoveAt(removedIndex);
        BoardRunes.RemoveAt(removedIndex);
    }

    public Coroutine PlaySequencePreview(float startDelay = 1, bool resetPlayIndex = true)
    {
        if (resetPlayIndex) currentPlayIndex = 0;

        return StartCoroutine(PlaySequencePreviewCoroutine(startDelay));
    }

    private IEnumerator FailedSequence()
    {
        SetPlayerInteractivity(false);

        announcer.ShowWrongRuneText();

        yield return new WaitForSeconds(2);

        if (currentRound > highScore)
        {
            highScore = currentRound;
            announcer.ShowHighScoreText(highScore);

            yield return new WaitForSeconds(3);
        }

        Reset();
        currentRound = 0;

        yield return PlaySequencePreview(2);
    }

    private void CorrectRuneSelected()
    {
        runeActivated?.Invoke();
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

        // CHALLENGE
        if (currentRound % increaseBoardSizeEveryXSequences == 0)
            AddRandomRuneToBoard();

        sequenceCompleted?.Invoke();

        currentRuneSequence.Add(Random.Range(0, BoardRunes.Count));

        Save();
        PlaySequencePreview();
    }

    private IEnumerator PlaySequencePreviewCoroutine(float startDelay = 1)
    {
        SetPlayerInteractivity(false);
        announcer.Clear();

        yield return new WaitForSeconds(1);

        EnablePreviewFeedback();

        yield return new WaitForSeconds(startDelay);

        foreach (var runeIndex in currentRuneSequence)
        {
            yield return BoardRunes[runeIndex].ActivateRune();
            yield return new WaitForSeconds(delayBetweenRunePreview);
        }

        DisablePreviewFeedback();
        SetPlayerInteractivity(true);
    }

    public void SetPlayerInteractivity(bool interactable)
    {
        foreach (var rune in BoardRunes)
            rune.Interactable = interactable;

        foreach (var powerup in powerups)
            powerup.Interactable = interactable;
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

        PlaySequencePreview(2);
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
        rune.Setup(BoardRunes.Count, this);
        BoardRunes.Add(rune);
    }

    private void InitializeBoard()
    {
        BoardRunes = new List<Rune>(initialBoardSize);
        instantiatedBoardRunes = new List<Rune>();

        for (int i = 0; i < initialBoardSize; i++)
            AddRandomRuneToBoard();
    }

    private void InitializeSequence()
    {
        currentRuneSequence = new List<int>(initialSequenceSize);
        for (int i = 0; i < initialSequenceSize; i++)
            currentRuneSequence.Add(Random.Range(0, BoardRunes.Count));
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
