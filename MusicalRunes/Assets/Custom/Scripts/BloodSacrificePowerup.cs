using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicalRunes
{
    public class BloodSacrificePowerup : Powerup
    {
        [SerializeField] private float animationDuration = 3;
        [SerializeField] private float perRuneAnimationDelay = .2f;
        [SerializeField] private Announcer announcer;

        protected override bool IsAvailable => base.IsAvailable && GameManager.Instance.RemainingLives > 1;

        protected override void PerformPowerupEffect()
        {
            StartCoroutine(AnimatePowerup());
        }

        private IEnumerator AnimatePowerup()
        {
            var gameManager = GameManager.Instance;

            gameManager.isRuneChoosingTime = false;
            gameManager.SetPlayerInteractivity(false);
            announcer.ShowBloodSacrificeText();

            var startingTime = Time.time;
            float lastRuneAnimationTime = 0;

            List<bool> runeStates = new List<bool>(gameManager.BoardRunes.Count);
            for (var index = 0; index < gameManager.BoardRunes.Count; index++)
                runeStates.Add(false);

            while (Time.time - startingTime < animationDuration)
            {
                if (Time.time - lastRuneAnimationTime > perRuneAnimationDelay)
                {
                    var index = Random.Range(0, gameManager.BoardRunes.Count);
                    var targetColor = runeStates[index] ? Color.white : Color.red;
                    gameManager.BoardRunes[index].LerpToColor(targetColor);

                    runeStates[index] = !runeStates[index];
                    lastRuneAnimationTime = Time.time;
                }

                yield return new WaitForEndOfFrame();
            }

            foreach (var boardRune in gameManager.BoardRunes)
                boardRune.LerpToColor(Color.white);

            gameManager.BloodSacrifice();
        }
    }
}