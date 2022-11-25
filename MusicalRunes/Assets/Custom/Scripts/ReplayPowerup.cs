public class ReplayPowerup : Powerup
{
    protected override void PerformPowerupEffect()
    {
        GameManager.Instance.PlaySequencePreview();
    }
}