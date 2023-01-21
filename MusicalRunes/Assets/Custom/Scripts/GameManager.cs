using System;
using System.Collections;
using System.Collections.Generic;
using MusicalRunes;
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
    [SerializeField] private float maxTimeToChooseRune = 5;
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
    [SerializeField] private TMP_Text timeText;

    public Action<int> coinsChanged;
    public Action sequenceCompleted;
    public Action runeActivated;

    public delegate void OnPowerupUpgradedDelegate(PowerupType upgradedPowerup, int newLevel);
    public OnPowerupUpgradedDelegate powerupUpgraded;

    public int CoinsAmount
    {
        get => saveData.coinsAmount;
        private set
        {
            saveData.coinsAmount = value;
            coinsAmountText.text = CoinsAmount.ToString();

            coinsChanged?.Invoke(value);
        }
    }

    public int HighScore
    {
        get => saveData.highScore;
        private set
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
    private float remainingRuneChooseTime;
    private bool isRuneChoosingTime;

    private SaveData saveData;

    public int GetPowerupLevel(PowerupType powerupType)
    {
        return saveData.GetUpgradableLevel(powerupType);
    }

    public void UpgradePowerup(PowerupType powerupType, int price)
    {
        if (price > CoinsAmount)
            throw new Exception("Trying to buy upgrade with insufficient coins");

        CoinsAmount -= price;

        var newLevel = GetPowerupLevel(powerupType) + 1;
        saveData.SetUpgradableLevel(powerupType, newLevel);
        Save();

        powerupUpgraded?.Invoke(powerupType, newLevel);
    }

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
        isRuneChoosingTime = false;
        if (resetPlayIndex) currentPlayIndex = 0;

        return StartCoroutine(PlaySequencePreviewCoroutine(startDelay));
    }

    private IEnumerator FailedSequence(bool choseWrongRune = true)
    {
        isRuneChoosingTime = false;
        SetPlayerInteractivity(false);

        if (choseWrongRune)
            announcer.ShowWrongRuneText();
        else
            announcer.ShowTimeoutText();

        yield return new WaitForSeconds(2);

        if (currentRound > HighScore)
        {
            HighScore = currentRound;
            announcer.ShowHighScoreText(HighScore);

            yield return new WaitForSeconds(3);
        }

        Reset();
        currentRound = 0;

        yield return PlaySequencePreview(2);
    }

    private void CorrectRuneSelected()
    {
        remainingRuneChooseTime = maxTimeToChooseRune;
        runeActivated?.Invoke();
        CoinsAmount += coinsPerRune;
        currentPlayIndex++;

        if (currentPlayIndex >= currentRuneSequence.Count)
            CompletedSequence();
        else
            Save();
    }

    private void CompletedSequence()
    {
        CoinsAmount += coinsPerRound;
        currentRound++;

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

        isRuneChoosingTime = true;
        remainingRuneChooseTime = maxTimeToChooseRune;
    }

    public void SetPlayerInteractivity(bool interactable)
    {
        foreach (var rune in BoardRunes)
            rune.Interactable = interactable;

        foreach (var powerup in powerups)
            powerup.SetButtonInteractable(interactable);
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

    private void Update()
    {
        if (!isRuneChoosingTime) return;

        remainingRuneChooseTime -= Time.deltaTime;
        remainingRuneChooseTime = Mathf.Max(0, remainingRuneChooseTime);
        timeText.text = remainingRuneChooseTime.ToString("F1");

        if (Mathf.Approximately(remainingRuneChooseTime, 0))
            StartCoroutine(FailedSequence(false));
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
        coinsAmountText.text = CoinsAmount.ToString();
        timeText.text = maxTimeToChooseRune.ToString("F1");
    }

    private void LoadSaveData()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            string serializedSaveData = PlayerPrefs.GetString(saveKey);
            saveData = SaveData.Deserialize(serializedSaveData);

            return;
        }

        saveData = new SaveData(true);
    }

    private void Save()
    {
        string serializedSaveData = saveData.Serialize();
        PlayerPrefs.SetString(saveKey, serializedSaveData);
    }
}
