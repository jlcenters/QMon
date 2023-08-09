using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    public MonsterBase MonBase { get; set; }
    public int Level { get; set; }
    public int Hp { get; set; }
    public int Xp { get; set; }
    public List<Move> Moves { get; set; }
    public Monster(MonsterBase mBase, int mLevel)
    {
        MonBase = mBase;
        Level = mLevel;
        Hp = MaxHp;

        Moves = new List<Move>();
        foreach (var move in MonBase.LearnableMoves)
        {
            if(move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }
            if(Moves.Count >= 4)
            {
                break;
            }
        }
    }


    //TODO: new formulas or new base stats; current stats do not match with formulas
    //TODO: on level up; check monster for special bonuses to stats
    //TODO - STRETCH: after lv 10,15,20, give a one-time bonus to stats
    public int Attack
    {
        get { return MonBase.Attack + (1 * Level); }
    }
    public int Block
    {
        get { return MonBase.Block + (1 * Level); }
    }
    public int Element
    {
        get { return MonBase.Element + (1 * Level); }
    }
    public int Speed
    {
        get { return MonBase.Speed + (1 * Level); }
    }
    public int MaxHp
    {
        get { return MonBase.MaxHp + (5 * Level); }
    }

    /*
     * Pokemon Bulbapedia formula: Mathf.FloorToInt((MonBase.Attack * Level) / 100f) + 5;   //MonBase.MaxHP & + 10 for hp
     */
}
