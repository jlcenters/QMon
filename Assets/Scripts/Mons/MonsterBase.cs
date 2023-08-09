using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] int maxXp;

    [SerializeField] int attack;
    [SerializeField] int block;
    [SerializeField] int element;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMove> learnableMoves;

    //checks to see if mon is player's current active
    public bool isFirst;

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
