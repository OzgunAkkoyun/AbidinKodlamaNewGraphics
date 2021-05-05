using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceMap : MonoBehaviour
{
    public MapGenerator map;
    public PathGenarator pathGenarator;
    public List<MapGenerator.Coord> posibleVisitNodes;

    public void SetVisitPoints()
    {
        var ifandWaitCoords = pathGenarator.Path.FindAll(v =>v.whichCoord == AnimalsInIfPath.isAnimalCoord || v.whichDirt != null);

        for (int i = 0; i < ifandWaitCoords.Count; i++)
        {
            posibleVisitNodes.Add(ifandWaitCoords[i]);
        }
        //posibleVisitNodes.Add(map.currentMap.targetPoint);
    }
}
