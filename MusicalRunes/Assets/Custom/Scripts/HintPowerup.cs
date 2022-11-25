using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HintPowerup : Powerup
{
    [SerializeField] private int runeHintAmount;

    private List<int> selectedRuneIndexes;
    private bool isActive;

    protected override void PerformPowerupEffect()
    {
        isActive = true;

        GameManager manager = GameManager.Instance;
        manager.SetPlayerInteractivity(false);

        selectedRuneIndexes = Enumerable.Range(0, manager.BoardRunes.Count)
            .OrderBy(index => index == manager.CurrentRuneIndex ? 2 : Random.value)
            .Take(runeHintAmount)
            .ToList();

        StartCoroutine(AnimateHintPowerUp());
    }

    protected override void OnRuneActivated()
    {
        base.OnRuneActivated();

        if (!isActive) return;

        var runes = GameManager.Instance.BoardRunes;
        foreach (var runeIndex in selectedRuneIndexes)
            runes[runeIndex].SetHintVisual(false);

        isActive = false;
        selectedRuneIndexes = null;
    }

    private IEnumerator AnimateHintPowerUp()
    {
        var runes = GameManager.Instance.BoardRunes;
        for (var index = 0; index < selectedRuneIndexes.Count; index++)
        {
            var runeIndex = selectedRuneIndexes[index];
            var rune = runes[runeIndex];

            if (index == selectedRuneIndexes.Count - 1)
                yield return rune.SetHintVisual(true);
            else
                rune.SetHintVisual(true);
        }

        GameManager.Instance.SetPlayerInteractivity(true);
    }
}