using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Monster
{
    //list references
    [SerializeField] MonsterBase monBase;
    [SerializeField] int level;
    //reference to base stats
    public MonsterBase MonBase { 
        get {
            return monBase;
        } 
    }

    //current stats compared to base
    public int Level {
        get {
            return level;
        }
    }
    public int Hp { get; set; }
    public int Xp { get; set; }

    //list of usable moves
    public List<Move> Moves { get; set; }

    
    public void Init()
    {
        //grabs references to base and stats
        Hp = MaxHp;

        //grabbing available moves
        Moves = new List<Move>();
        foreach (var move in MonBase.LearnableMoves)
        {
            //if current is the minimum level required to learn the move, store in list
            if(move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }

            //if list contains 4 or more moves, break loop
            if(Moves.Count >= 4)
            {
                break;
            }
        }
    }


    //TODO: new formulas or new base stats; current stats do not match with formulas
    //TODO: on level up; check monster for special bonuses to stats
    //TODO - STRETCH: after lv 10,15,20, give a one-time bonus to stats
    //stats differ by level
    public int Attack
    {
        get { return MonBase.Attack + (1 * Level); }
    }
    public int Block
    {
        get { return MonBase.Block + (1 * Level); }
    }
    public int SpBlock
    {
        get { return MonBase.SpBlock + (1 * Level); }
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



    public DamageDetails TakeDamage(Move move, Monster attacker)
    {
        //62.5% chance of landing critical
        float critical = 1f;
        if(Random.value * 100f <= 6.25)
        {
            critical = 2f;
        }

        //multiply type effectiveness to modifiers
        float type = TypeChart.GetEffectiveness(move.Base.Type, this.MonBase.Type);

        //store info in Damage Details object to reference in Battle System
        var details = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };


        //if move is special, use elemental dmg and special block
        float attackType = (move.Base.IsSpecial) ? attacker.Element : attacker.Attack;
        float defenseType = (move.Base.IsSpecial) ? SpBlock : Block;


        //checks dmg stats and level, multiplies by type effectiveness and if critical
        float damage = ((attackType + move.Base.Power) * attacker.Level) * type * critical / defenseType;
        Hp -= (int)damage;
        if(Hp <= 0)
        {
            Hp = 0;
            details.Fainted = true;
        }

        return details;
    }

    //TODO: depending on ailments and hp, prepare different moves or different items
    //Grabs random number between 1 and the size of the monster's move list
    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);

        return Moves[r];
    }



    /*
     * POKEMON BULBAPEDIA FORMULAS: 
     * 
     * STATS: Mathf.FloorToInt((MonBase.Attack * Level) / 100f) + 5;   //MonBase.MaxHP & + 10 for hp
     * 
     * 
     * TAKING DMG: float type = TypeChart.GetEffectiveness(move.Base.Type, this.MonBase.Type);
        float mod = Random.Range(0.85f, 1f) * type * critical;
        float atk = (2 * attacker.Level + 10) / 250f;
        float def = atk * move.Base.Power * ((float)attackType / defenseType) + 2;
        int damage = Mathf.FloorToInt(def * mod);
     * 
     */
}



//Details can be sent to the Battle System to check 
public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}
