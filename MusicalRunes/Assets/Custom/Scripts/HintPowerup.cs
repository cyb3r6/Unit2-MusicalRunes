using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MusicalRunes;
using Random = UnityEngine.Random;

public class HintPowerup : Powerup
{
    private List<int> selectedRuneIndexes;
    private bool isActive;

    private int RuneHintAmount => ((HintPowerupConfig)powerupConfig).GetHintAmount(currentLevel);

    protected override void PerformPowerupEffect()
    {
        isActive = true;

        GameManager manager = GameManager.Instance;
        manager.SetPlayerInteractivity(false);

        selectedRuneIndexes = Enumerable.Range(0, manager.BoardRunes.Count)
            .OrderBy(index => index == manager.CurrentRuneIndex ? 2 : Random.value)
            .ToList();

        selectedRuneIndexes.RemoveAt(selectedRuneIndexes.Count - 1);
        selectedRuneIndexes = selectedRuneIndexes.GetRange(0, Math.Min(RuneHintAmount, selectedRuneIndexes.Count));

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