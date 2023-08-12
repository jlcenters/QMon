using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{

    //reference to base stats
    public MoveBase Base { get; set; }

    //current PP out of base's total
    public int PP { get; set; }

    public Move(MoveBase mBase)
    {
        Base = mBase;
        PP = mBase.PP;
    }
}
