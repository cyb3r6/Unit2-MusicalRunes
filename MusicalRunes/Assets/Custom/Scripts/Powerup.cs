using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Powerup : MonoBehaviour
{
    [SerializeField] private int cooldownDuration = 3;
    [SerializeField] private bool decreaseCooldownOnRuneActivation;

    [SerializeField] private Button powerupButton;
    [SerializeField] private RectTransform cooldownBar;

    private float cooldownBarHeight;
    private int currentCooldown;

    public bool Interactable
    {
        get => powerupButton.interactable;
        set => powerupButton.interactable = IsAvailable && value;
    }

    protected virtual bool IsAvailable => currentCooldown <= 0;

    public virtual void Start()
    {
        cooldownBarHeight = cooldownBar.sizeDelta.y;

        // TODO: Load cooldown from save
        SetCooldownBarHeight();

        powerupButton.onClick.AddListener(OnClick);
        GameManager.Instance.sequenceCompleted += OnSequenceCompleted;
        GameManager.Instance.runeActivated += OnRuneActivated;
    }

    protected abstract void PerformPowerupEffect();

    private void OnClick()
    {
        Debug.Assert(IsAvailable, "Trying to activate a unavailable powerup", gameObject);
        ResetCooldown();
        Interactable = false;

        PerformPowerupEffect();
    }

    private void ResetCooldown()
    {
        currentCooldown = cooldownDuration;
        SetCooldownBarHeight();
    }

    protected virtual void OnSequenceCompleted()
    {
        if (decreaseCooldownOnRuneActivation) return;

        DecreaseCooldown();
    }

    protected virtual void OnRuneActivated()
    {
        if (!decreaseCooldownOnRuneActivation) return;

        DecreaseCooldown();
    }

    private void DecreaseCooldown()
    {
        if (IsAvailable) return;

        currentCooldown--;
        currentCooldown = Mathf.Max(0, currentCooldown);

        SetCooldownBarHeight();

        Interactable = IsAvailable;
    }

    private void SetCooldownBarHeight()
    {
        var fraction = (float)currentCooldown / cooldownDuration;
        cooldownBar.sizeDelta = new Vector2(cooldownBar.sizeDelta.x, fraction * cooldownBarHeight);
    }

    private void OnDestroy()
    {
        GameManager.Instance.sequenceCompleted -= OnSequenceCompleted;
        GameManager.Instance.runeActivated -= OnRuneActivated;
    }
}
