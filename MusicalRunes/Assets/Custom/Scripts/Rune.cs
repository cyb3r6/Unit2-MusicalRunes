using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Rune : MonoBehaviour
{
    private static readonly Color hintColor = new Color(1, 1, 1, .6f);

    [SerializeField] private Color activationColor;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Button button;
    [SerializeField] private Image runeImage;
    [SerializeField] private float colorTransitionDuration = 0.35f;
    [SerializeField] private float minActivationDuration = 0.5f;

    public bool Interactable
    {
        get => button.interactable;
        set => button.interactable = value;
    }

    private int index;
    private GameManager gameManager;

    private Coroutine animationCoroutine;

    public void Setup(int runeIndex, GameManager manager)
    {
        index = runeIndex;
        gameManager = manager;
    }

    public void OnClick()
    {
        gameManager.OnRuneActivated(index);
        ActivateRune();
    }

    public Coroutine ActivateRune()
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(ActivateRuneCoroutine());

        return animationCoroutine;
    }

    private IEnumerator ActivateRuneCoroutine()
    {
        audioSource.Play();

        yield return LerpToColor(Color.white, activationColor);

        yield return new WaitForSeconds(minActivationDuration);

        var duration = audioSource.clip.length;
        while (audioSource.isPlaying)
            yield return new WaitForSeconds(duration - audioSource.time);

        yield return LerpToColor(activationColor, Color.white);
    }

    public Coroutine SetHintVisual(bool state)
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);

        if (state)
            animationCoroutine = StartCoroutine(LerpToColor(Color.white, hintColor));
        else
            animationCoroutine = StartCoroutine(LerpToColor(hintColor, Color.white));

        return animationCoroutine;
    }

    private IEnumerator LerpToColor(Color start, Color end)
    {
        float elapsedTime = 0;
        float startTime = Time.time;

        while (elapsedTime < colorTransitionDuration)
        {
            runeImage.color = Color.Lerp(start, end, elapsedTime / colorTransitionDuration);
            elapsedTime = Time.time - startTime;
            yield return null;
        }
    }
}