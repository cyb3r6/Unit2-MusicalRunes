using MusicalRunes;
using UnityEngine;
using UnityEngine.UI;

public abstract class Powerup : MonoBehaviour
{
    [SerializeField] private Button powerupButton;
    [SerializeField] private RectTransform cooldownBar;
    [SerializeField] protected PowerupConfig powerupConfig;

    private float cooldownBarHeight;
    private int currentCooldown;
    protected int currentLevel;

    private int cooldownDuration => powerupConfig.GetCooldown(currentLevel);

    public void SetButtonInteractable(bool interactable)
    {
        powerupButton.interactable = IsAvailable && interactable;
    }

    private void ResetButtonInteractable()
    {
        SetButtonInteractable(true);
    }

    protected virtual bool IsAvailable => currentLevel > 0 && currentCooldown <= 0;

    public virtual void Start()
    {
        cooldownBarHeight = cooldownBar.sizeDelta.y;

        // TODO: Load cooldown from save
        SetCooldownBarHeight();

        powerupButton.onClick.AddListener(OnClick);

        var gameManager = GameManager.Instance;
        currentLevel = gameManager.GetPowerupLevel(powerupConfig.powerupType);

        gameManager.sequenceCompleted += OnSequenceCompleted;
        gameManager.runeActivated += OnRuneActivated;
        gameManager.powerupUpgraded += OnPowerupUpgraded;

        ResetButtonInteractable();
    }

    protected abstract void PerformPowerupEffect();

    private void OnPowerupUpgraded(PowerupType upgradedPowerup, int newLevel)
    {
        if (upgradedPowerup != powerupConfig.powerupType) return;

        currentLevel = newLevel;
        ResetButtonInteractable();
    }

    private void OnClick()
    {
        Debug.Log("Powerup Button Clicked");
        Debug.Assert(IsAvailable, "Trying to activate a unavailable powerup", gameObject);
        ResetCooldown();
        SetButtonInteractable(false);

        PerformPowerupEffect();
    }

    private void ResetCooldown()
    {
        currentCooldown = cooldownDuration;
        SetCooldownBarHeight();
    }

    protected virtual void OnSequenceCompleted()
    {
        if (powerupConfig.decreaseCooldownOnRuneActivation) return;

        DecreaseCooldown();
    }

    protected virtual void OnRuneActivated()
    {
        if (!powerupConfig.decreaseCooldownOnRuneActivation) return;

        DecreaseCooldown();
    }

    private void DecreaseCooldown()
    {
        if (IsAvailable) return;

        currentCooldown--;
        currentCooldown = Mathf.Max(0, currentCooldown);

        SetCooldownBarHeight();

        ResetButtonInteractable();
    }

    private void SetCooldownBarHeight()
    {
        var fraction = (float)currentCooldown / cooldownDuration;
        cooldownBar.sizeDelta = new Vector2(cooldownBar.sizeDelta.x, fraction * cooldownBarHeight);
    }

    private void OnDestroy()
    {
        var gameManager = GameManager.Instance;
        gameManager.sequenceCompleted -= OnSequenceCompleted;
        gameManager.runeActivated -= OnRuneActivated;
        gameManager.powerupUpgraded -= OnPowerupUpgraded;
    }
}