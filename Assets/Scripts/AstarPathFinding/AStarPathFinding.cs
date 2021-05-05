using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class AStarPathFinding : MonoBehaviour
{
    public MapGenerator map;
    public PathGenarator pathGenarator;
    public HintButton hint;
    
    void FindPath(Coord startPos,Coord endPos)
    {
        Coord startNode = startPos;
        Coord targetNode = endPos;

        List<Coord> openSet = new List<Coord>();
        List<Coord> closedSet = new List<Coord>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Coord currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            if (currentNode.x == targetNode.x && currentNode.y == targetNode.y)
            {
                hint.shortestPath = RetracePath(currentNode);
                return;
            }

            foreach (Coord neighbour in GetAvailableNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }
    int GetDistance(Coord nodeA, Coord nodeB)
    {
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
    List<Coord> RetracePath(Coord targetNode)
    {
        List<Coord> path = new List<Coord>();
        Coord currentNode = targetNode;

        while (!CheckIsNull(currentNode.parent))
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    bool CheckIsNull(object obj)
    {
        bool isNull = obj == null || (obj is UnityEngine.Object && ((obj as UnityEngine.Object) == null));
        return isNull;
    }
    private List<Coord> GetAvailableNeighbours(Coord cell)
    {
        var neighbours = cell.GetNeighbours();
        List<Coord> availableCells = new List<Coord>();
        foreach (var neighbour in neighbours)
        {

            if (CellinBounds(neighbour))
            {
                if (CellUnavaliableNeighboursGet(neighbour, cell))
                {
                    //Log("Cell UnavaliableNeighbours");
                }
                else
                {
                    availableCells.Add(neighbour);
                }
            }

        }
        return availableCells;
    }

    public bool CellinBounds(Coord neighbours)
    {
        if (neighbours.x < 0 || neighbours.x >= 5 || neighbours.y < 0 || neighbours.y >= 5)
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    public bool CellUnavaliableNeighboursGet(Coord neighbours, Coord cell)
    {
        if (map.allObstacleCoord.Contains(neighbours) || cell.UnavaliableNeighbours.Contains(neighbours))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
