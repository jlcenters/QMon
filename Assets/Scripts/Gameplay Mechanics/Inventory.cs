using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] int qballs;

    public int Qballs { get { return qballs; } set { qballs = value; } }


}
