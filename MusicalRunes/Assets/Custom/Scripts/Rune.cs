using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Rune : MonoBehaviour
{
    [SerializeField] private Color activationColor;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Button button;
    [SerializeField] private Image runeImage;
    [SerializeField] private float activationTransitionDuration = 0.3f;
    [SerializeField] private float minActivationDuration = 0.5f;

    private int index;
    private GameManager gameManager;

    public void Setup(int runeIndex, GameManager manager)
    {
        index = runeIndex;
        gameManager = manager;
    }

    public void OnClick()
    {
        gameManager.OnRuneActivated(index);
        StartCoroutine(ActivateRune());
    }

    public void DisableInteraction()
    {
        button.interactable = false;
    }

    public void EnableInteraction()
    {
        button.interactable = true;
    }

    public IEnumerator ActivateRune()
    {
        audioSource.Play();
        float elapsedTime = 0;
        float startTime = Time.time;

        while (elapsedTime < activationTransitionDuration)
        {
            runeImage.color = Color.Lerp(Color.white, activationColor, elapsedTime / activationTransitionDuration);
            elapsedTime = Time.time - startTime;
            yield return null;
        }

        yield return new WaitForSeconds(minActivationDuration);

        var duration = audioSource.clip.length;
        while (audioSource.isPlaying)
            yield return new WaitForSeconds(duration - audioSource.time);

        elapsedTime = 0;
        startTime = Time.time;
        while (elapsedTime < activationTransitionDuration)
        {
            runeImage.color = Color.Lerp(activationColor, Color.white, elapsedTime / activationTransitionDuration);
            elapsedTime = Time.time - startTime;
            yield return null;
        }
    }
}