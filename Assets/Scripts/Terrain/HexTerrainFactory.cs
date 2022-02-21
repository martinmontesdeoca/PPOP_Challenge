using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTerrainFactory : MonoBehaviour
{
    [SerializeField]
    private TerrainConfiguration terrainConfiguration;

    private static HexTerrainFactory _instance;
    public static HexTerrainFactory Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("HexTerrainFactory is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        terrainConfiguration = Instantiate(terrainConfiguration);       
    }

    public HexTerrain CreateHexTerrain(int id, Vector3 position)
    {
        return terrainConfiguration.GetHexTerrainFromId(id, position);
    }    
}
