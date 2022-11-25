using UnityEngine;

public class PurgeRunePowerup : Powerup
{
    [SerializeField] private int minBoardSize;

    protected override bool IsAvailable => GameManager.Instance.BoardRunes.Count > minBoardSize && base.IsAvailable;

    protected override void PerformPowerupEffect()
    {
        var manager = GameManager.Instance;

        var runes = manager.BoardRunes;
        manager.PurgeBoardRune(Random.Range(0, runes.Count));
        manager.PlaySequencePreview();
    }
}