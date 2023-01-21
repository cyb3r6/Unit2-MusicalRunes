using UnityEngine;

namespace MusicalRunes
{
    public class LocaleButton : MonoBehaviour
    {
        [SerializeField] private Locale locale;

        public void OnClick()
        {
            Localization.ChangeLocale(locale);
        }
    }
}