using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int initialSequenceSize = 3;
    public List<Rune> boardRunes;

    public int coinsPerRune = 1;
    public TMP_Text coinsAmountText;

    private double coinsAmount;

    private List<int> currentRuneSequence;
    private int currentPlayIndex;

    public void OnRuneActivated(int index)
    {
        // TODO: Prevent rune clicks when sequence finished.
        if (currentPlayIndex >= currentRuneSequence.Count) return;

        if (currentRuneSequence[currentPlayIndex] == index)
            CorrectRuneSelected();
        else
            FailedSequence();
    }

    private void FailedSequence()
    {
        Debug.Log("FailedSequence");
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
        Debug.Log("CompletedSequence");
        currentRuneSequence.Add(Random.Range(0, boardRunes.Count));
        currentPlayIndex = 0;
        PlaySequencePreview();
    }

    private void PlaySequencePreview()
    {
        // TODO: Animate each rune in turn

        string sequence = "Sequence: ";
        foreach (var index in currentRuneSequence)
        {
            sequence += $"{index}, ";
        }
        Debug.Log(sequence);
    }

    private void Awake()
    {
        SetCoins(0);
        InitializeSequence();
        PlaySequencePreview();
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
