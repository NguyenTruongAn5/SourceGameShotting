using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Enemy Starts", menuName = "TRUONGAN/SHOTTING_DOWN/Create Enemy Start")]

public class EnemyStarts : ActorStarts
{
    [Header("Xp Bonus: ")]
    public float minXpBonus;
    public float maxXpBonus;

    [Header("Level Up: ")]
    public float hpUp;
    public float damageUp;
}
