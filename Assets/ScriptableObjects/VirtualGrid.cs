using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VirtualGrid : ScriptableObject
{
    private List<Vector3Int> VirtualTileLocations;
    private List<Vector3> RealTileLocatons;

    public void AddTileLocation(Vector3Int vtl, Vector3 rtl) 
    { 
        VirtualTileLocations.Add(vtl);
        RealTileLocatons.Add(rtl);
    }
    public void AddVirtualTile(Vector3Int vtl)
    {
        VirtualTileLocations.Add(vtl);
    }
    public void AddRealTile(Vector3 rtl)
    {
        RealTileLocatons.Add(rtl);
    }
    public void ResetTileArrays()
    {
       VirtualTileLocations.Clear();
       RealTileLocatons.Clear();
    }
    public List<Vector3Int> GetVertualTilesList() 
    { 
        return VirtualTileLocations;
    }
    public List<Vector3> GetRealTilesList()
    {
        return RealTileLocatons;
    }
}
