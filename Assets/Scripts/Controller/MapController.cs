using PathFinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{   
    [SerializeField]
    private NeighboursData neighboursData;

    private List<HexTerrain> hexTerrainsSelect;
    private int[] hexTerrainsTypeList;   
    private int mapWidth = 8;
    private int mapHeigth = 8;    
    private HexTerrain[,] hexagonArray;
    private float tileXOffset = 0.99f;
    private float tileZOffset = -0.764f;
    private float mapXMin;
    private float mapXMax;
    private float mapZMin;
    private float mapZMax;

    void Start()
    {       
        ConfigureMap();
        CreateMap();
    }

    private void ConfigureMap()
    {
        neighboursData = Instantiate(neighboursData);
        hexTerrainsSelect = new List<HexTerrain>();
        hexagonArray = new HexTerrain[mapWidth, mapHeigth];              
        hexTerrainsTypeList = new int[] {
            HexTerrain.DESERT,HexTerrain.DESERT,HexTerrain.DESERT,HexTerrain.DESERT,HexTerrain.WATER,HexTerrain.FOREST,HexTerrain.GRASS,HexTerrain.MOUNTAIN,
            HexTerrain.DESERT,HexTerrain.DESERT,HexTerrain.FOREST,HexTerrain.FOREST,HexTerrain.WATER,HexTerrain.FOREST,HexTerrain.FOREST,HexTerrain.MOUNTAIN,
            HexTerrain.GRASS,HexTerrain.DESERT,HexTerrain.DESERT,HexTerrain.WATER,HexTerrain.WATER,HexTerrain.DESERT,HexTerrain.MOUNTAIN,HexTerrain.MOUNTAIN,
            HexTerrain.WATER,HexTerrain.FOREST,HexTerrain.GRASS,HexTerrain.FOREST,HexTerrain.FOREST,HexTerrain.GRASS,HexTerrain.FOREST,HexTerrain.FOREST,
            HexTerrain.GRASS,HexTerrain.GRASS,HexTerrain.GRASS,HexTerrain.MOUNTAIN,HexTerrain.MOUNTAIN,HexTerrain.GRASS,HexTerrain.FOREST,HexTerrain.FOREST,
            HexTerrain.MOUNTAIN,HexTerrain.FOREST,HexTerrain.MOUNTAIN,HexTerrain.FOREST,HexTerrain.FOREST,HexTerrain.WATER,HexTerrain.WATER,HexTerrain.FOREST,
            HexTerrain.MOUNTAIN,HexTerrain.MOUNTAIN,HexTerrain.DESERT,HexTerrain.DESERT,HexTerrain.GRASS,HexTerrain.GRASS,HexTerrain.WATER,HexTerrain.GRASS,
            HexTerrain.GRASS,HexTerrain.FOREST,HexTerrain.GRASS,HexTerrain.DESERT,HexTerrain.DESERT,HexTerrain.WATER,HexTerrain.WATER,HexTerrain.WATER
        };       
    }   
    
    private void CreateMap()
    {
        mapXMin = -mapWidth / 2;
        mapXMax = mapWidth / 2;
        mapZMin = -mapHeigth / 2;
        mapZMax = mapHeigth / 2;
        int idHexTerrain = 0;
        var countColumn = 0;
        var countRow = 0;

        for (float x = mapXMin; x < mapXMax; x++)
        {
            for (float z = mapZMin; z < mapZMax; z++)
            {
                var typeTerrain = hexTerrainsTypeList[idHexTerrain];              
                HexTerrain hexTerrain = Instantiate(HexTerrainFactory.Instance.CreateHexTerrain(hexTerrainsTypeList[idHexTerrain], transform.position),transform);
                hexTerrain.GetComponent<HexTerrain>().SELECT += OnHexTerrainSelect;
                hexTerrain.Type = typeTerrain;
                Vector3 pos;

                if(z % 2 == 0)               
                    pos = new Vector3(x * tileXOffset, 0, z * tileZOffset);
                else 
                    pos = new Vector3(x * tileXOffset + tileXOffset / 2, 0, z * tileZOffset);                

                hexTerrain.Column = countColumn;
                hexTerrain.Row = countRow;
                hexTerrain.transform.position = pos;
                hexTerrain.name = "HexTerrain Column: " + countColumn + " Row: " + countRow;                
                hexagonArray[countColumn, countRow] = hexTerrain;                      
                idHexTerrain++;
                countRow++;
            }
            countColumn++;
            countRow = 0;
        }
        AddNeighbours();
    }

    private void OnHexTerrainSelect(object sender, EventArgs args)
    {
        if (hexTerrainsSelect.Contains((sender as HexTerrain)) && hexTerrainsSelect.Count == 1) return;

        if (hexTerrainsSelect.Count == 2) ResetMap();

        if (hexTerrainsSelect.Count < 2) { 
            hexTerrainsSelect.Add((sender as HexTerrain));
            (sender as HexTerrain).SelectedInit();
            if (hexTerrainsSelect.Count == 2)
            {                
                var listReturn = AStar.GetPath(hexTerrainsSelect[0], hexTerrainsSelect[1]);
                if (listReturn != null)
                    ShowWay(listReturn);
            }
        }       
    }

    private void ResetMap()
    {
        hexTerrainsSelect.Clear();
        foreach (var hexTerrain in hexagonArray)
        {
            hexTerrain.Reset();
        }
    }

    private void ShowWay(IList<IAStarNode> hexTerrains)
    {
        foreach (HexTerrain hex in hexTerrains)
        {
            if (hex != hexTerrainsSelect[0] && hex != hexTerrainsSelect[1])
                hex.SelectedWay();

            hex.ChangeYPosition(false);
        }
    }

    private void AddNeighbours()
    {       
        var countColumn = 0;
        var countRow = 0;
        for (float x = 0; x < 8; x++)
        {
            for (float z = 0; z < 8; z++)
            {               
                hexagonArray[countColumn, countRow].AddNeighbours(GetNeighbours(countColumn, countRow));               
                countRow++;
            }
            countColumn++;
            countRow = 0;
        }       
    }   

    private List<IAStarNode> GetNeighbours(int posX, int posY)
    {
        List<IAStarNode> neighbours = new List<IAStarNode>(6);
        int markerX = posX;
        int markerY = posY;
        CheckLeft(neighbours, markerX, markerY);
        CheckRight(neighbours, markerX, markerY);
        CheckUp(neighbours, markerX, markerY);
        CheckDown(neighbours, markerX, markerY);

        return neighbours;
    }

    private void CheckRight(List<IAStarNode> neighbours, int markerX, int markerY)
    {       
        if (markerX + 1 < mapWidth)
        {
            neighbours.Add(hexagonArray[markerX + 1, markerY]);
        }
    }

    private void CheckLeft(List<IAStarNode> neighbours, int markerX, int markerY)
    {       
        if (markerX - 1 >= 0)
        {
            neighbours.Add(hexagonArray[markerX - 1, markerY]);
        }
    }

    private void CheckDown(List<IAStarNode> neighbours, int markerX, int markerY)
    {       
        if (markerY + 1 <= mapHeigth)
        {
            //left
            if (markerY < 7)
            {
                var markerXPos = markerX;
                if ((markerY % 2 == 0) && markerX > 0)
                    markerXPos = markerX - 1;

                neighbours.Add(hexagonArray[markerXPos, markerY + 1]);
            }
            //right
            if (markerY + 1 < mapHeigth && markerX + 1 <= mapWidth && markerY < 7)
            {
                var markerXPos = markerX;
                if ((markerY % 2 == 1) && markerY > 0 && markerX < 7)
                    markerXPos = markerX + 1;

                neighbours.Add(hexagonArray[markerXPos, markerY + 1]);
            }
        }
    }

    private void CheckUp(List<IAStarNode> neighbours, int markerX, int markerY)
    {       
        if ((markerY - 1) >= 0)
        {
            //left
            if (markerY - 1 >= 0)
            {
                var markerXPos = markerX;
                if ((markerY % 2 == 0) && markerX > 0)
                    markerXPos = markerX - 1;

                neighbours.Add(hexagonArray[markerXPos, markerY - 1]);
            }
            //right
            if (markerY - 1 <= mapHeigth && markerX + 1 <= mapWidth)
            {
                var markerXPos = markerX;
                if (markerY % 2 == 1 && markerX < 7)
                    markerXPos = markerX + 1;

                neighbours.Add(hexagonArray[markerXPos, markerY - 1]);
            }
        }
    }
}
