using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * Monster Base is used like a reference
 * all of the base stats for each monster will be stored in the Resources section of the Assets folder
 * 
 * Everything that is consistent with every object is defined in the script, 
 * while everything consistent with each monster will be defined in the editor
 */
[CreateAssetMenu(fileName = "MonsterBase", menuName = "Monster/Create new Monster")]
public class MonsterBase : ScriptableObject
{
    [SerializeField] string monName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] MonType type;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    //Base Stats
    [SerializeField] int maxHp;
    [SerializeField] int baseXp;
    [SerializeField] GrowthRate growthRate;

    [SerializeField] int attack;
    [SerializeField] int block;
    [SerializeField] int element;
    [SerializeField] int speed;

    //between 0 and 255; lower the catch rate harder it is to catch
    [SerializeField] int catchRate = 255;

    [SerializeField] List<LearnableMove> learnableMoves;

    //checks to see if mon is player's current active
    public bool isFirst;

    public static int maxMoves = 4;



    //checks required xp to level up
    public int GetXpForLevel(int level)
    {
        if(growthRate == GrowthRate.Fast)
        {
            return 4 * Mathf.FloorToInt(Mathf.Pow(level, 3f)) / 7;
        }
        else if(growthRate == GrowthRate.Medium)
        {
            return Mathf.FloorToInt(Mathf.Pow(level, 3f));
        }
        //no slow rate yet, so will return error code
        return -1;
    }



    //Properties
    public string MonName
    {
        get { return monName; }
    }
    public string Description
    {
        get { return description; }
    }
    public MonType Type
    {
        get { return type; }
    }
    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }
    public Sprite BackSprite
    {
        get { return backSprite; }
    }
    public int MaxHp
    {
        get { return maxHp; }
    }
    public int BaseXp
    {
        get { return baseXp; }
    }
    public GrowthRate GrowthRate => growthRate;
    public int Attack
    {
        get { return attack; }
    }
    public int Block
    {
        get { return block; }
    }
    public int Element
    {
        get { return element; }
    }
    public int Speed
    {
        get { return speed; }
    }
    public int CatchRate => catchRate;
    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }
}



//will appear in Inspector
[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
}



//determines typing of monster
public enum MonType
{
    None,
    Fire,
    Water,
    Grass,
    Normal,
    Ghost,
    Special
}



//list of monster stats
public enum Stat
{
    Attack,
    Block,
    Element,
    Speed
}



//Determines Type Effectiveness
public class TypeChart
{
    static float[][] chart =
    {   //           fIR WAT GRS NRM GST SPC
        new float[] {1f, .5f, 2f, 1f, 1f, 1f}, //FIRE
        new float[] {2f, 1f, .5f, 1f, 1f, 1f}, //WATER
        new float[] {.5f, 2f, 1f, 1f, 1f, 1f}, //GRASS
        new float[] {1f, 1f, 1f, 1f, 1f, 1f}, //NORMAL
        new float[] {1f, 1f, 1f, 1f, 1f, 1f}, //GHOST
        new float[] {1f, 1f, 1f, 1f, 1f, 1f } //SPECIAL

        //KEY: 1- normal effectiveness, .5- not very effective, 2- super effective, 0- no effect
    };



    public static float GetEffectiveness(MonType attacker, MonType defender)
    {
        if(attacker == MonType.None || defender == MonType.None)
        {
            return 1;
        }

        //type None has an int value of 0 in the enum, so type - 1 will be its array location
        int row = (int)attacker - 1;
        int col = (int)defender - 1;

        //will return float of effectiveness based on chart
        return chart[row][col];
    }
}



//How fast does the mon gain XP upon leveling up?
public enum GrowthRate
{
    Slow,
    Medium,
    Fast
}
