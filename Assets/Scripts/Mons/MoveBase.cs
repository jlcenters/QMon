using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * This has the same function as the Monster Base, and is stored in the same location
 */
[CreateAssetMenu(fileName = "MoveBase", menuName = "Monster/Create new Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string moveName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] MonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;
    [SerializeField] MoveCategory category;
    [SerializeField] int priority;

    //Properties
    public int Power
    {
        get { return power; }
    }
    public string MoveName
    {
        get { return moveName; }
    }
    public string Description
    {
        get { return description; }
    }
    public int Accuracy
    {
        get { return accuracy; }
    }
    public int PP
    {
        get { return pp; }
    }
    public MonType Type
    {
        get { return type; }
    }
    public bool IsElemental
    {
        get
        {
            if(type == MonType.Fire || type == MonType.Water || type == MonType.Grass || type == MonType.Ghost || type == MonType.Special)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public MoveEffects Effects
    {
        get { return effects; }
    }
    public MoveTarget Target
    {
        get { return target; }
    }
    public MoveCategory Category
    {
        get { return category; }
    }
    public int Priority
    {
        get { return priority; }
    }
}



//Does the move damage or affect one's status?
public enum MoveCategory
{
    Attack,
    Status
}


//status conditions class
[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;

    public List<StatBoost> Boosts 
    { 
        get
        {
            return boosts;
        } 
    }
}

//Dictionaries are not able to be serializable, so this class is a workaround for that
[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}



//Who is receiving the stat boost?
public enum MoveTarget
{
    Foe,
    Self
}
