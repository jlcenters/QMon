using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    //Constructor
    public Monster(MonsterBase _base, int lvl)
    {
        monBase = _base;
        level = lvl;

        Init();
    }
    //current stats which reference the base
    public int Level {
        get {
            return level;
        }
    }
    public int Hp { get; set; }
    public int Xp { get; set; }
    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoost { get; private set; }
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();



    //Stats Properties and Methods
    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }
    public int Block
    {
        get { return GetStat(Stat.Block); }
    }
    public int Element
    {
        get { return GetStat(Stat.Element); }
    }
    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }
    public int MaxHp { get; private set; }
    //TODO: new formulas or new base stats; current stats do not match with formulas
    //TODO: on level up; check monster for special bonuses to stats
    //TODO - STRETCH: after lv 10,15,20, give a one-time bonus to stats



    //Initializes all public stats
    public void Init()
    {
        //grabbing available moves
        Moves = new List<Move>();
        foreach (var move in MonBase.LearnableMoves)
        {
            //if current is the minimum level required to learn the move, store in list
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }

            //if list contains 4 or more moves, break loop
            if (Moves.Count >= 4)
            {
                break;
            }
        }

        Xp = MonBase.GetXpForLevel(Level);
        CalculateStats();
        Hp = MaxHp;
        ResetStatBoost();
        StatusChanges = new Queue<string>();
    }

    //Initializing stats referencing the base
    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, MonBase.Attack + (1 * Level));
        Stats.Add(Stat.Block, MonBase.Block + (1 * Level));
        Stats.Add(Stat.Element, MonBase.Element + (1 * Level));
        Stats.Add(Stat.Speed, MonBase.Speed + (1 * Level));

        MaxHp = MonBase.MaxHp + (2 * Level);
    }

    //encapsulating the getter for all stats, so that it's more efficient to apply stat changes in the future
    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        //stat boosts
        int boost = StatBoost[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        //if boost lvl is positive, stat is boosted; else, stat is decreased
        if (boost >= 0)
        {
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        }
        else
        {
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);
        }
        return statVal;
    }



    //during status effect move, apply boosts to separate object which will be referenced to when grabbing the Get Stat method each turn
    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach(var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            //add boost value to dictionary
            StatBoost[stat] = Mathf.Clamp(StatBoost[stat] + boost, -6, 6);
            //add to queue each status change to be displayed in UI
            if(boost > 0)
            {
                StatusChanges.Enqueue($"{MonBase.MonName}'s {stat} rose!");
            }
            else
            {
                if(StatusChanges == null)
                {
                    StatusChanges = new Queue<string>();
                }
                StatusChanges.Enqueue($"{MonBase.MonName}'s {stat} fell!");

            }
        }
    }



    //resets Stat Boost dictionary for starting and ending battles
    void ResetStatBoost()
    {
        StatBoost = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0 },
            {Stat.Block, 0 },
            {Stat.Element, 0 },
            {Stat.Speed, 0 }
        };
    }



    //attack/defense logic; returns data from move for UI
    public DamageDetails TakeDamage(Move move, Monster attacker)
    {
        //62.5% chance of landing critical
        float critical = 1f;
        if(Random.value * 100f <= 6.25)
        {
            critical = 2f;
        }

        //store info in Damage Details object to reference in Battle System
        var details = new DamageDetails()
        {
            TypeEffectiveness = TypeChart.GetEffectiveness(move.Base.Type, this.MonBase.Type),
            Critical = critical,
            Fainted = false
        };

        //if move is special, use elemental dmg and check type effectiveness
        float attackType = (move.Base.IsElemental) ? attacker.Element * TypeChart.GetEffectiveness(move.Base.Type, this.MonBase.Type) : attacker.Attack;
        float defenseType = (move.Base.IsElemental) ? Block : Block;

        //adds attack to move's base damage, using level and crit success as multipliers, divide damage by defense
        float damage = (attackType + move.Base.Power) * attacker.Level * critical / defenseType;
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
        //Usable Moves will store all moves which have not been exhausted yet
        var usableMoves = Moves.Where(x => x.PP > 0).ToList();

        int r = Random.Range(0, usableMoves.Count);

        return usableMoves[r];
    }



    public bool LeveledUp()
    {
        //if xp went to 
        if(Xp > monBase.GetXpForLevel(Level + 1))
        {
            level++;
            return true;
        }

        return false;
    }



    public void OnBattleOver()
    {
        ResetStatBoost();
    }







    /* POKEMON BULBAPEDIA FORMULAS: 
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
