using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;
public class CreateMap : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject obstaclePrefab;
    public List<Coord> allTileCoords = new List<Coord>();
    public List<Coord> allObstacleCoords = new List<Coord>();
    List<Coord> openList = new List<Coord>();
    List<Coord> closedList = new List<Coord>();
    public Coord startPos = new Coord(0,0);
    public Coord endPos = new Coord(4,4);
    public List<Coord> Path;
    void Start()
    {
        CreateAllTiles();
        CreateObstacles();
        FindPath();
    }

    private void CreateObstacles()
    {
        for (int i = 0; i < 5; i++)
        {
            var rnd = UnityEngine.Random.Range(0, 5);
            var a =Instantiate(obstaclePrefab, new Vector3(i, 1, rnd), Quaternion.identity);
            a.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            allObstacleCoords.Add(new Coord(i,rnd));
        }
    }

    private void CreateAllTiles()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                var a = Instantiate(tilePrefab, new Vector3(i, 0, j),Quaternion.identity);
                a.transform.localScale = new Vector3(0.9f,0.9f,0.9f);
                allTileCoords.Add(new Coord(i, j));
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
    List<Coord> RetracePath(Coord startNode, Coord targetNode)
    {
        List<Coord> path = new List<Coord>();
        Coord currentNode = targetNode;
      
        while (currentNode.x != startNode.x && currentNode.y != startNode.y)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }
    void FindPath()
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
                Path = RetracePath(startNode, targetNode);
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
    //public List<Coord> PathFinding()
    //{
    //    openList.Add(startPos);

    //    while (openList.Count > 0)
    //    {
    //        var currentCoord = GetLowestFCostNode(openList);
    //        openList.Remove(currentCoord);
    //        closedList.Add(currentCoord);
    //        if (currentCoord == endPos)
    //        {
    //            //TODO: İf reached the endPOs
    //            return null;
    //        }

    //        foreach (var coord in GetAvailableNeighbours(currentCoord))
    //        {
    //            if (closedList.Contains(coord))
    //            {
    //                 continue;
    //            }

    //            if()
    //            {

    //            }
    //        }
    //    }
    //}



    private Coord GetLowestFCostNode(List<Coord> pathNodeList)
    {
        Coord lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    //private void OneStepBackinList(Coord currentCell, ref int currentPathIndex)
    //{
    //    Path.RemoveAt(currentPathIndex);
    //    currentPathIndex--;
    //    Path[currentPathIndex].UnavaliableNeighbours.Add(new Coord(currentCell.x, currentCell.y));
    //}

    private bool SelectNextCell(List<Coord> neighbours, out Coord selectedNeighbour)
    {
        if (neighbours.Count > 0)
        {
            var rnd = UnityEngine.Random.Range(0, neighbours.Count);
            selectedNeighbour = neighbours[rnd];
            return true;
        }
        else
        {
            selectedNeighbour = new Coord(0, 0);
            return false;
        }
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

    //public bool IsCellOnPath(Coord neighbours) => Path.Contains(neighbours);

    public bool CellUnavaliableNeighboursGet(Coord neighbours, Coord cell)
    {
        if (allObstacleCoords.Contains(neighbours) || cell.UnavaliableNeighbours.Contains(neighbours))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
