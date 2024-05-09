using UnityEngine;

public static class Prefs 
{
    public static int coins
    {
        set => PlayerPrefs.SetInt(PrefConst.COIN_KEY, value);
        get => PlayerPrefs.GetInt(PrefConst.COIN_KEY, 0);
    }
    public static string playerData
    {
        set => PlayerPrefs.SetString(PrefConst.PLAYER_DATA_KEY, value);
        get => PlayerPrefs.GetString(PrefConst.PLAYER_DATA_KEY);
    }
    public static string playerWeaponData
    {
        set => PlayerPrefs.SetString(PrefConst.PLAYER_WEAPON_DATA_KEY, value);
        get => PlayerPrefs.GetString(PrefConst.PLAYER_WEAPON_DATA_KEY);
    }
    public static string enemyData
    {
        set => PlayerPrefs.SetString(PrefConst.ENEMY_DATA_KEY, value);
        get => PlayerPrefs.GetString(PrefConst.ENEMY_DATA_KEY);
    }
    public static bool IsEnoughCoin(int coinToCheck)
    {
        return coins >= coinToCheck;
    }
}
