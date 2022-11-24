using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Runes Settings")]
    [SerializeField] private int initialSequenceSize = 3;
    [SerializeField] private int initialBoardSize = 4;
    [SerializeField] private RectTransform runesHolder;
    [SerializeField] private List<Rune> availableRunePrefabs;
    private List<Rune> boardRunes;
    private List<Rune> instantiatedBoardRunes;

    [Header("Coins Settings")]
    [SerializeField] private int coinsPerRune = 1;
    [SerializeField] private TMP_Text coinsAmountText;

    [Header("Preview Settings")]
    [SerializeField] private GameObject[] spinlights;
    [SerializeField] private float delayBetweenRunePreview = .3f;
    [SerializeField] private Announcer announcer;

    private double coinsAmount;

    private List<int> currentRuneSequence;
    private int currentPlayIndex;

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

        Reset();
        currentPlayIndex = 0;

        yield return PlaySequencePreview(2);
    }

    private void CorrectRuneSelected()
    {
        AddCoins(coinsPerRune);
        currentPlayIndex++;

        if (currentPlayIndex >= currentRuneSequence.Count)
            CompletedSequence();
    }

    private void CompletedSequence()
    {
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
        SetCoins(0);
        InitializeBoard();
        InitializeSequence();
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

    private void AddCoins(double addedValue)
    {
        SetCoins(coinsAmount + addedValue);
    }

    private void SetCoins(double newValue)
    {
        coinsAmount = newValue;
        coinsAmountText.text = coinsAmount.ToString();
    }
}
