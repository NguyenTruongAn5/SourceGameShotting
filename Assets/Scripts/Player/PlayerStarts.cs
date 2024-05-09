using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Player Starts", menuName = "TRUONGAN/SHOTTING_DOWN/Create Player Start")]
public class PlayerStarts : ActorStarts
{
    [Header("Level Up Base: ")]
    public int level;
    public int maxLevel;
    public float Xp;
    public float levelUpXpRequired;

    [Header("Level Up: ")]
    public float levelUpXpRequiredUp;
    public float hpUp;

    public override bool IsMaxLevel()
    {
        return level >= maxLevel;
    }
    public override void Load()
    {
        if (!string.IsNullOrEmpty(Prefs.playerData))
        {
            JsonUtility.FromJsonOverwrite(Prefs.playerData, this);
        }
    }
    public override void Save()
    {
        Prefs.playerData = JsonUtility.ToJson(this);
    }
    public override void Upgrade(Action OnSucess = null, Action OnFailed = null)
    {
        while (Xp >= levelUpXpRequired && !IsMaxLevel())
        {
            level++;
            Xp -= levelUpXpRequired;

            hp += hpUp * Helper.GetUpgradeFormula(level);
            levelUpXpRequired += levelUpXpRequiredUp * Helper.GetUpgradeFormula(level);
            Save();
            OnSucess?.Invoke();
        }
        if (Xp < levelUpXpRequired || IsMaxLevel())
        {
            OnFailed?.Invoke();
        }
    }
}
