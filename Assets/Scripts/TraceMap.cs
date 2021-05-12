using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PossibleVisitPoints
{
    public MapGenerator.Coord coord;
    public bool isVisitedInTheCommands;

    public PossibleVisitPoints(MapGenerator.Coord _coord, bool _isVisited)
    {
        this.coord = _coord;
        this.isVisitedInTheCommands = _isVisited;
    }
}
public class TraceMap : MonoBehaviour
{
    public MapGenerator map;
    public PathGenarator pathGenarator;
    public AStarPathFinding pathFinder;
    public Commander commander;
    public CharacterMovement character;
    public UIHandler uh;
    private bool hasTracingStoped;
    public Vector3 inputVector;
    public List<PossibleVisitPoints> posibleVisitNodes = new List<PossibleVisitPoints>();
    private int scaleFactor = 1;
    public List<MapGenerator.Coord> shortestPath = new List<MapGenerator.Coord>();

    public void SetVisitPoints()
    {
        posibleVisitNodes.Clear();
        var ifandWaitCoords = pathGenarator.Path.FindAll(v =>v.whichCoord == AnimalsInIfPath.isAnimalCoord || v.whichDirt != null);

        for (int i = 0; i < ifandWaitCoords.Count; i++)
        {
            posibleVisitNodes.Add(new PossibleVisitPoints(ifandWaitCoords[i],false));
        }
    }

    public void TraceCommands()
    {
        SetVisitPoints();
        if (character == null)
        {
            character = FindObjectOfType<CharacterMovement>();
        }

        inputVector = character.gameObject.transform.position / 2;
        hasTracingStoped = false;

        for (var i = 0; i < commander.commands.Count; i++)
        {
            var command = commander.commands[i];
            var type = command.GetType();
            var isLastCommand = i == commander.commands.Count - 1;

            if (type == typeof(MoveCommand))
            {
                var commandMove = command as MoveCommand;
                DirectionToVector(commandMove.direction);
                if (!CheckInputInBoundsOrInPath())
                {
                    uh.ShowWrongCommand(i);
                    hasTracingStoped = true;
                    break;
                }
            }
            else if (type == typeof(ForCommand))
            {
                var commandFor = command as ForCommand;

                for (int j = 0; j < commandFor.loopCount; j++)
                {
                    for (int k = 0; k < commandFor.directions.Count; k++)
                    {
                        DirectionToVector(commandFor.directions[k]);
                        if (!CheckInputInBoundsOrInPath())
                        {
                            uh.ShowWrongCommand(i);
                            hasTracingStoped = true;
                            break;
                        }
                    }
                }
            }
            else if (type == typeof(PickIfAnyObjectExistsCommand))
            {
                var commandIf = command as PickIfAnyObjectExistsCommand;
                if (!CheckIfObjectLocation(commandIf, i))
                {
                    uh.ShowWrongCommand(i);
                    hasTracingStoped = true;
                    break;
                }
                else
                {
                    CurrentVisitPointAlreadtVisited();
                }
            }
            else if (type == typeof(WaitCommand))
            {
                var commandWait = command as WaitCommand;
                if (!CheckWaitObjectLocation(commandWait, i))
                {
                    uh.ShowWrongCommand(i);
                    hasTracingStoped = true;
                    break;
                }
                else
                {
                    CurrentVisitPointAlreadtVisited();
                }
            }
        }

        if (!hasTracingStoped)
        {
            ShowNextCommand();
        }
    }

    private void CurrentVisitPointAlreadtVisited()
    {
        var currentCoord = GetCurrentCoord();
        var currentPossibleVisistPoint = FindInCurrentVisitPoints(currentCoord);
        currentPossibleVisistPoint.isVisitedInTheCommands = true;
    }

    private PossibleVisitPoints FindInCurrentVisitPoints(MapGenerator.Coord currentCoord) =>
        posibleVisitNodes.Find(v => v.coord.x == currentCoord.x && v.coord.y == currentCoord.y);
    
    private bool CheckWaitObjectLocation(WaitCommand commandWait, int i)
    {
        var currentPath = GetCurrentCoord();
        if (!currentPath.CheckIsNull())
        {
            if (currentPath.whichDirt != null)
            {
                if (commandWait.seconds != currentPath.whichDirt.seconds)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else return false;
        }
        else return false;
    }
    private bool CheckIfObjectLocation(PickIfAnyObjectExistsCommand commandIf, int i)
    {
        var currentPath = GetCurrentCoord();
        if (!currentPath.CheckIsNull())
        {
            if (currentPath.whichCoord != AnimalsInIfPath.Empty)
            {
                if (commandIf.animalName != pathGenarator.currentIfObject.ifName)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else return false;
        }
        else return false;
    }
    private void ShowNextCommand()
    {
        var startPos = new MapGenerator.Coord((int)inputVector.x, (int)inputVector.z);
        if (FindIforWaitCoord())
        {
            var endPos = FindEndPoint();
            shortestPath.Clear();
            pathFinder.FindPath(startPos, endPos);
            if (shortestPath.Count > 0)
            {
                uh.ShowHintCommand(SetDirection());
            }
            else
            {
                uh.ShowHintReachedTarget();
            }
        }
    }

    private MapGenerator.Coord FindEndPoint()
    {
        var notVisitedPossibleVisitPoints = posibleVisitNodes.FindAll(v => !v.isVisitedInTheCommands);
        if (notVisitedPossibleVisitPoints.Count > 0)
        {
            MapGenerator.Coord findNearestPosibleVisitPoints = FindNearestPosibleVisitPoints(notVisitedPossibleVisitPoints);
            return findNearestPosibleVisitPoints;
        }
        else
        {
            return map.currentMap.targetPoint;
        }
    }

    private MapGenerator.Coord FindNearestPosibleVisitPoints(List<PossibleVisitPoints> notVisitedPossibleVisitPoints)
    {
        return notVisitedPossibleVisitPoints.OrderBy(v => Vector2.Distance(new Vector2(v.coord.x,v.coord.y), new Vector2(inputVector.x, inputVector.z))).First().coord;
    }

    private bool FindIforWaitCoord()
    {
        var currentCoord = GetCurrentCoord();
        if (!currentCoord.CheckIsNull())
        {

            if (currentCoord.whichCoord != AnimalsInIfPath.Empty)
            {
                if (!FindInCurrentVisitPoints(currentCoord).isVisitedInTheCommands)
                {
                    uh.ShowHintIfCommand(pathGenarator.currentIfObject.ifName);
                    return false;
                }
                else return true;


            }
            else if (currentCoord.whichDirt != null)
            {
                if (!FindInCurrentVisitPoints(currentCoord).isVisitedInTheCommands)
                {
                    uh.ShowHintWaitCommand(currentCoord.whichDirt.seconds);
                    return false;
                }
                else return true;
            }
            else return true;
            
        }
        else return false;
    }

    private bool CheckIfObjectAlreadyCoded()
    {
       throw new NotImplementedException();
    }

    private Direction SetDirection()
    {
        var goingCell = shortestPath[0];

        if (inputVector.x < goingCell.x)
        {
            return Direction.Right;
        }
        else if (inputVector.x > goingCell.x)
        {
            return Direction.Left;
        }
        else if (inputVector.z < goingCell.y)
        {
            return Direction.Forward;
        }
        else if (inputVector.z > goingCell.y)
        {
            return Direction.Backward;
        }

        return Direction.Empty;
    }

    private bool CheckInputInBoundsOrInPath()
    {
        var currentPath = GetCurrentCoord();
        if (!currentPath.CheckIsNull())
        {
            if (pathGenarator.CellinBounds(currentPath))
            {
                if (pathGenarator.Path.Contains(currentPath))
                    return true;
                else
                    return false;
            }
            else return false;
        }
        else return false;
    }
    
  

    private MapGenerator.Coord GetCurrentCoord() => pathGenarator.Path.Find(v =>
        (v.x == inputVector.Vector3toXZ().x) && (v.y == inputVector.Vector3toXZ().z));

    private void DirectionToVector(Direction moveCommand)
    {
        if (moveCommand == Direction.Left)
        {
            inputVector.x -= scaleFactor;
        }
        else if (moveCommand == Direction.Right)
        {
            inputVector.x += scaleFactor;
        }
        else if (moveCommand == Direction.Forward)
        {
            inputVector.z += scaleFactor;
        }
        else if (moveCommand == Direction.Backward)
        {
            inputVector.z -= scaleFactor;
        }
    }
}
