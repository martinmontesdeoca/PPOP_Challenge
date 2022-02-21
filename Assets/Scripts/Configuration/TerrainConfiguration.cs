using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Terrain configuration")]
public class TerrainConfiguration : ScriptableObject
{
    [SerializeField]
    private HexTerrain[] hexTerrainsAssets;
    private Dictionary<int, HexTerrain> hexTerrainsDictionary;

    void Awake()
    {
        hexTerrainsDictionary = new Dictionary<int, HexTerrain>();
        foreach (var hexTerrain in hexTerrainsAssets)
        {
            hexTerrainsDictionary.Add(hexTerrain.Id, hexTerrain);
        }
    }

    public HexTerrain GetHexTerrainFromId(int id, Vector3 position)
    {
        if (!hexTerrainsDictionary.TryGetValue(id, out var hexTerrain))
        {
            throw new Exception("The HexTerrain id not exits");
        }

        return hexTerrain;
    }
}
