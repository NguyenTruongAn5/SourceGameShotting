using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon Starts", menuName = "TRUONGAN/SHOTTING_DOWN/Create Weapon Start")]
public class WeaponStats : Starts
{
    [Header("Base Starts: ")]
    public int bullets;
    public float firerate;
    public float reloadTime;
    public float damage;

    [Header("Upragre: ")]
    public int level;
    public int maxLevel;
    public int bulletsUp;
    public float firerateUp;
    public float reloadTimeUp;
    public float damageUp;
    public int upgradePrice;
    public int upgragePriceUp;

    [Header("Limit: ")]
    public float minFirerate = 0.1f;
    public float minReloadTime = 0.01f;

    public int BulletsUpInfo { get => bulletsUp * (level + 1); }
    public float FirerateUpInfo { get => firerateUp * Helper.GetUpgradeFormula(level + 1); }
    public float ReloadUpInfo { get => reloadTime * Helper.GetUpgradeFormula(level + 1); }
    public float DamageUpInfo { get => damageUp * Helper.GetUpgradeFormula(level + 1); }
    public override bool IsMaxLevel()
    {
        return level >= maxLevel;
    }
    public override void Load()
    {
        if (!string.IsNullOrEmpty(Prefs.playerWeaponData))
        {
            JsonUtility.FromJsonOverwrite(Prefs.playerWeaponData, this);
        }
    }
    public override void Save()
    {
        Prefs.playerWeaponData = JsonUtility.ToJson(this);
    }
    public override void Upgrade(Action OnSucess = null, Action OnFailed = null)
    {
        if(Prefs.IsEnoughCoin(upgradePrice) && !IsMaxLevel())
        {
            Prefs.coins -= upgradePrice;
            level++;
            bullets += bulletsUp * level;
            firerate -= firerateUp * Helper.GetUpgradeFormula(level);
            firerate = Mathf.Clamp(firerate, minFirerate, firerate);
            reloadTime -= reloadTimeUp * Helper.GetUpgradeFormula(level);
            reloadTime = Mathf.Clamp(reloadTime, minReloadTime, reloadTime);
            damage += damageUp * Helper.GetUpgradeFormula(level);
            upgradePrice += upgragePriceUp * level;
            Save();
            OnSucess?.Invoke();
            return;
        }
        OnFailed?.Invoke();
    }
}
