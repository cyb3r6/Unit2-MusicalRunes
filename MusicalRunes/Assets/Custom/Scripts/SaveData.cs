using System;
using System.Collections.Generic;
using MusicalRunes;
using UnityEngine;

[Serializable]
public class SaveData
{
    [Serializable]
    private class UpgradableSaveData
    {
        public PowerupType Type;
        public int Level;
    }

    public int coinsAmount;
    public int highScore;

    [SerializeField] private List<UpgradableSaveData> upgradableSaveData;

    private Dictionary<PowerupType, UpgradableSaveData> upgradableLevels;

    public int GetUpgradableLevel(PowerupType powerupType)
    {
        return upgradableLevels[powerupType].Level;
    }

    public void SetUpgradableLevel(PowerupType powerupType, int level)
    {
        upgradableLevels[powerupType].Level = level;
    }

    public SaveData()
    {
        upgradableSaveData = new List<UpgradableSaveData>();
        upgradableLevels = new Dictionary<PowerupType, UpgradableSaveData>();
    }

    public SaveData(bool createDefaults) : this()
    {
        foreach (PowerupType upgradableType in Enum.GetValues(typeof(PowerupType)))
        {
            upgradableLevels[upgradableType] = new UpgradableSaveData
            {
                Type = upgradableType,
                Level = 0
            };
        }
    }

    public string Serialize()
    {
        upgradableSaveData.Clear();
        foreach (var pair in upgradableLevels)
        {
            upgradableSaveData.Add(pair.Value);
        }

        return JsonUtility.ToJson(this);
    }

    public static SaveData Deserialize(string jsonString)
    {
        SaveData newSaveData = JsonUtility.FromJson<SaveData>(jsonString);

        foreach (var data in newSaveData.upgradableSaveData)
        {
            newSaveData.upgradableLevels.Add(data.Type, data);
        }

        return newSaveData;
    }
}