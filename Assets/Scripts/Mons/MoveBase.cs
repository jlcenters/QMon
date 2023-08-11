using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
