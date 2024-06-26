using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicalRunes
{
    public enum Locale
    {
        en,
        pt
    }

    public static class Localization
    {
        private static Dictionary<Locale, Dictionary<string, string>> localizationTable;

        private static Locale currentLocale = Locale.en;

        private static Dictionary<string, string> CurrentLocalizationTable => localizationTable[currentLocale];

        private static readonly List<ILocalizable> watchers = new List<ILocalizable>();

        public static void ChangeLocale(Locale newLocale)
        {
            currentLocale = newLocale;

            foreach (var watcher in watchers)
                watcher.LocaleChanged();
        }

        public static void RegisterWatcher(ILocalizable newWatcher)
        {
            watchers.Add(newWatcher);
        }

        public static void DeregisterWatcher(ILocalizable removedWatcher)
        {
            watchers.Remove(removedWatcher);
        }

        public static string GetLocalizedText(string id)
        {
            return CurrentLocalizationTable[id];
        }

        private static void Load()
        {
            var source = Resources.Load<TextAsset>("LocalizationSource");

            var lines = source.text.Split('\n');
            var header = lines[0].Split(';');

            var localeOrder = new List<Locale>(header.Length - 1);
            localizationTable = new Dictionary<Locale, Dictionary<string, string>>(header.Length - 1);
            for (int i = 1; i < header.Length; i++)
            {
                var locale = (Locale)Enum.Parse(typeof(Locale), header[i]);
                localeOrder.Add(locale);
                localizationTable[locale] = new Dictionary<string, string>(lines.Length - 1);
            }

            for (var index = 1; index < lines.Length; index++)
            {
                var entry = lines[index].Split(';');
                var key = entry[0];

                for (var i = 0; i < localeOrder.Count; i++)
                {
                    var locale = localeOrder[i];
                    localizationTable[locale][key] = entry[i + 1];
                }
            }
        }

        static Localization()
        {
            Load();
        }
    }
}