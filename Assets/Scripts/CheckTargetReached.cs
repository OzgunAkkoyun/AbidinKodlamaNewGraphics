using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

public class CheckTargetReached : MonoBehaviour
{
    public void CheckIfReachedTarget(bool isLastCommand, CharacterMovement characterMovement)
    {
        var currentCoord = new Coord((int) (characterMovement.inputVector.x / characterMovement.mapGenerate.tileSize),
            (int) (characterMovement.inputVector.z / characterMovement.mapGenerate.tileSize));

        if (characterMovement.pathGenarator.Path.Contains(currentCoord))
        {
            var targetVec3 = characterMovement.mapGenerate.CoordToPosition(characterMovement.mapGenerate.currentMap.targetPoint.x,
                characterMovement.mapGenerate.currentMap.targetPoint.y);
            
            if (targetVec3 == characterMovement.inputVector.Vector3toXZ() && isLastCommand)
            {
                if (characterMovement.gm.currentSenario.senarioIndex == 1 ||
                    characterMovement.gm.currentSenario.senarioIndex == 2)
                {
                    characterMovement.isPlayerReachedTarget = true;
                    characterMovement.CharacterEndAnimationPlay();
                }
                else if (characterMovement.gm.currentSenario.senarioIndex == 3)
                {
                    CheckIfObjectCount(characterMovement);
                }
                else if (characterMovement.gm.currentSenario.senarioIndex == 4)
                {
                    CheckWaitObjectsCount(characterMovement);
                }
                else if (characterMovement.gm.currentSenario.senarioIndex == 5)
                {
                    CheckIfAndWaitObjectsCount(characterMovement);
                }
            }
            else
            {
                if (isLastCommand)
                {
                    characterMovement.isPlayerReachedTarget = false;
                    characterMovement.gm.EndGame();
                }
            }
        }
        else
        {
            characterMovement.isPlayerReachedTarget = false;
            characterMovement.gm.EndGame();
        }
    }

    public void CheckIfAndWaitObjectsCount(CharacterMovement characterMovement)
    {
        var allObjectsOk = 0;
        if (IfObjectAnimations.instance.collectedIfObjects == characterMovement.gm.currentSubLevel.ifObjectCount)
        {
            allObjectsOk++;
        }
        else
        {
            if (ShowWrongCleaningTile.instance.wrongIfTiles.Count > 0)
            {
                ShowWrongCleaningTile.instance.ShowWrongIfTiles();
                characterMovement.isPlayerReachedTarget = false;
            }
        }

        if (characterMovement.waitObjectsAnimation.howManyDirtCleaned == characterMovement.gm.currentSubLevel.dirtCount)
        {
            allObjectsOk++;
        }
        else
        {
            if (ShowWrongCleaningTile.instance.wrongWaitTiles.Count > 0)
            {
                ShowWrongCleaningTile.instance.ShowWrongCleaningTiles();
                characterMovement.isPlayerReachedTarget = false;
            }
        }

        if (allObjectsOk == 2)
        {
            characterMovement.isPlayerReachedTarget = true;
            characterMovement.gm.EndGame();
        }
        else
        {
            characterMovement.isPlayerReachedTarget = false;
            //characterMovement.gm.EndGame();
        }

    }
    public void CheckIfObjectCount(CharacterMovement characterMovement)
    {
        if (IfObjectAnimations.instance.collectedIfObjects == characterMovement.gm.currentSubLevel.ifObjectCount)
        {
            characterMovement.isPlayerReachedTarget = true;
            characterMovement.CharacterEndAnimationPlay();
        }
        else
        {
            if (ShowWrongCleaningTile.instance.wrongIfTiles.Count > 0)
            {
                ShowWrongCleaningTile.instance.ShowWrongIfTiles();
                characterMovement.isPlayerReachedTarget = false;
            }
            else
            {
                characterMovement.isPlayerReachedTarget = false;
                characterMovement.gm.EndGame();
            }
           
        }
    }
    public void CheckWaitObjectsCount(CharacterMovement characterMovement)
   {
       Debug.Log(characterMovement.waitObjectsAnimation.howManyDirtCleaned +" " + characterMovement.gm.currentSubLevel.dirtCount);
       if (characterMovement.waitObjectsAnimation.howManyDirtCleaned == characterMovement.gm.currentSubLevel.dirtCount )
       {
           characterMovement.isPlayerReachedTarget = true;
           characterMovement.CharacterEndAnimationPlay();
           Debug.Log("eşit");
       }
       else
       {
           Debug.Log("eşit değil");
            if (ShowWrongCleaningTile.instance.wrongWaitTiles.Count > 0)
           {
               ShowWrongCleaningTile.instance.ShowWrongCleaningTiles();
               characterMovement.isPlayerReachedTarget = false;
               Debug.Log("eşit değil 0dan büyük");
            }
           else
           {
               characterMovement.isPlayerReachedTarget = false;
               characterMovement.gm.EndGame();
               Debug.Log("eşit değil 0");
            }
       }
   }
}