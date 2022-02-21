using PathFinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Neighbours Data")]
public class NeighboursData : ScriptableObject
{   
    public static IEnumerable<IAStarNode> neighbours;

    void Awake()
    {
        neighbours = new List<IAStarNode>();
    }
}
