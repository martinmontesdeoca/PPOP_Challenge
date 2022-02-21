using PathFinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTerrain : MonoBehaviour, IAStarNode
{
    public const int GRASS = 0;
    public const int FOREST = 1;
    public const int DESERT = 2;
    public const int MOUNTAIN = 3;
    public const int WATER = 4;

    [SerializeField]
    private int id;
    public int Id => id;

    [SerializeField]
    protected int cost;

    private int column;
    private int row;
    private int type;
    private Color originalColor;
    private new Renderer renderer;

    public event EventHandler SELECT;

    public int Column
    {
        get { return column; }
        set { column = value; }
    }

    public int Row
    {
        get { return row; }
        set { row = value; }
    }

    public int Type
    {
        get { return type; }
        set { type = value; }
    }

    IEnumerable<IAStarNode> neighbours;
    public IEnumerable<IAStarNode> Neighbours => neighbours;   

    float IAStarNode.CostTo(IAStarNode neighbour)
    {
        return cost;       
    }

    float IAStarNode.EstimatedCostTo(IAStarNode target)
    {
        return cost + (target as HexTerrain).cost;
    }
  
    void Start()
    {       
        renderer = GetComponent<Renderer>();
        originalColor = renderer.material.color;
    }

    public void AddNeighbours(List<IAStarNode> neigh)
    {      
        NeighboursData.neighbours = neigh;
        neighbours = NeighboursData.neighbours;
    }

    private void OnMouseDown()
    {        
        SELECT?.Invoke(this,new EventArgs());
    }

    public void ChangeYPosition(bool reset)
    {
        var yPos = transform.position.y+0.1f;
        if (reset)
            yPos = 0;

        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
    }

    public void SelectedInit()
    {
        renderer.material.color = Color.green;
    }

    public void SelectedWay()
    {        
        renderer.material.color = Color.red;
    }

    public void Reset()
    {
        ChangeYPosition(true);
        renderer.material.color = originalColor;
    }
}
