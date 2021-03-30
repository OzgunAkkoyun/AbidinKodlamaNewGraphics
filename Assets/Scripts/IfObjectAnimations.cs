using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class IfObjectAnimations : MonoBehaviour
{
    public static IfObjectAnimations instance;
    public PathGenarator pathGenarator;
    private Vector3 smokeScaleVector;
    public int collectedIfObjects;

    void Awake()
    {
        instance = this;
        smokeScaleVector = new Vector3(5,5,5);
    }

    public void RemoveQuestionObjectInAnimal(GameObject currentObject, Vector3 halfVector)
    {
        var questionMark = currentObject.transform.Find("EmtyQuestionMark");

        var character = pathGenarator.gm.character;
        var characterCam = Camera.main;

        var transformPosition = questionMark.transform.position;
        var targetPos = new Vector3(transformPosition.x, transformPosition.y + 15, transformPosition.z);

        if (pathGenarator.selectedAnimals[0].ifName == pathGenarator.currentIfObject.ifName)
        {
            character.transform.DOMoveY(0.3f, .9f).OnComplete(() =>
            {
                questionMark.transform.DOMove(targetPos, 2).OnComplete(() =>
                {
                    questionMark.gameObject.SetActive(false);
                    var lookPos = new Vector3(halfVector.x, currentObject.transform.position.y, halfVector.z);
                    currentObject.transform.DOLookAt(lookPos, 1f);
                });
            });

            //var cameraMovePosDirection = currentObject.transform.position - character.transform.position;

            //var cameraMovePosTemp = characterCam.transform.position + cameraMovePosDirection * 1.3f;

            //var cameraMovePos = new Vector3(cameraMovePosTemp.x, 3, cameraMovePosTemp.z);
            var cameraMovePos = character.gameObject.transform.Find("SSTarget").transform;
            if (pathGenarator.gm.isGameOrLoad !=1)
            {
                //characterCam.transform.DOMove(cameraMovePos, 1);
                StartCoroutine(CameraSmoothMovingToTargetPosition(characterCam, cameraMovePos));
            }
        }
        else
        {
            character.transform.DOMoveY(0.3f, .9f).OnComplete(() =>
            {
                questionMark.transform.DOMove(targetPos, 2).OnComplete(() =>
                {
                    questionMark.gameObject.SetActive(false);
                    var lookPos = new Vector3(halfVector.x, currentObject.transform.position.y, halfVector.z);
                    currentObject.transform.DOLookAt(lookPos, 1f);
                });
            });
        }

    }
    public IEnumerator CameraSmoothMovingToTargetPosition(Camera mainCamera, Transform cameraTarget)
    {
        float t = 0;
        while (Vector3.Distance(mainCamera.transform.position, cameraTarget.position) > 0.1f)
        {
            t += Time.deltaTime / 30;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraTarget.position, t);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, cameraTarget.rotation, t);

            yield return new WaitForSeconds(0f);
        }
    }

    public void RemoveOnlyQuestionMark(GameObject currentQuestionMark)
    {
        var transformPosition = currentQuestionMark.transform.position;
        var targetPos = new Vector3(transformPosition.x, transformPosition.y + 15, transformPosition.z);
        currentQuestionMark.transform.DOMove(targetPos, 2).OnComplete(() =>
        {
            currentQuestionMark.SetActive(false);
        });
    }

    public void AnimalMoveFromPath(GameObject currentAnimal)
    {
        var currentAnimalAnim = currentAnimal.transform.GetChild(0).GetComponentInChildren<Animator>();
        currentAnimalAnim.SetBool("birdFly",true);

        var animLength = currentAnimalAnim.GetCurrentAnimatorStateInfo(0).length;
        StartCoroutine(BirdDeactive(currentAnimal,animLength));
    }

    IEnumerator BirdDeactive(GameObject currentAnimal, float animLength)
    {
        yield return new WaitForSeconds(animLength + 2.1f);
        currentAnimal.SetActive(false);
    }

    public void WrongAnimalMoveFromPath(GameObject currentAnimal)
    {
        var transformPosition = currentAnimal.transform.position;
        var targetPos = new Vector3(transformPosition.x, transformPosition.y + 15, transformPosition.z);
        currentAnimal.transform.DOMove(targetPos, 2).OnComplete(() =>
        {
            currentAnimal.SetActive(false);
        });
    }

    private int selectedIfObjectIndex = 0;
    public IEnumerator SenarioTreeIfCheck(bool isLastCommand, CharacterMovement characterMovement, Vector3 inputVector)
    {
        if (characterMovement.currentPath.whichCoord == AnimalsInIfPath.isAnimalCoord)
        {
            var currentAnimal = pathGenarator.animals.Find(v =>
                (v.transform.position.x == characterMovement.inputVector.Vector3toXZ().x) &&
                (v.transform.position.z == characterMovement.inputVector.Vector3toXZ().z));

            if (pathGenarator.selectedAnimals[selectedIfObjectIndex].ifName == pathGenarator.currentIfObject.ifName)
            {
                if (pathGenarator.gm.isGameOrLoad !=1)
                {
                    characterMovement.cameraMovementForSs.OpenSSLayout();
                    yield return new WaitUntil(() => ScreenShotHandler.instance.isSSTaken);
                }
                else
                {
                    collectedIfObjects++;
                }
                
                yield return new WaitForSeconds(1f);
                AnimalMoveFromPath(currentAnimal);
                yield return characterMovement.CameraPopIn();
                yield return new WaitForSeconds(2f);
                yield return characterMovement.CompleteHalfWay();
            }
            else
            {
                var currentCoord = new MapGenerator.Coord((int)(inputVector.x / pathGenarator.mapGenerator.tileSize), (int)(inputVector.z / pathGenarator.mapGenerator.tileSize));

                var realCoord = pathGenarator.Path.Find(v => v.x == currentCoord.x && v.y == currentCoord.y);

                ShowWrongCleaningTile.instance.wrongIfTiles.Add(realCoord);
                yield return new WaitForSeconds(3f);
                AnimalMoveFromPath(currentAnimal);
                //yield return new WaitForSeconds(1f);
                //characterMovement.isPlayerReachedTarget = false;

                //characterMovement.gm.EndGame();
            }
        }
        else if (characterMovement.currentPath.whichCoord == AnimalsInIfPath.isEmptyAnimalCoord)
        {
            yield return new WaitForSeconds(1f);
            yield return characterMovement.CompleteHalfWay();
        }
        else
        {
            yield return new WaitForSeconds(1f);
            characterMovement.isPlayerReachedTarget = false;

            characterMovement.gm.EndGame();
        }

        selectedIfObjectIndex++;
        characterMovement.checkTargetReached.CheckIfReachedTarget(isLastCommand, characterMovement);
    }

    public IEnumerator SenarioFiveIfCheck(bool isLastCommand, CharacterMovement characterMovement, Vector3 inputVector)
    {
        if (characterMovement.currentPath.whichCoord == AnimalsInIfPath.isAnimalCoord)
        {
            var currentAnimal = pathGenarator.animals.Find(v =>
                (v.transform.position.x == characterMovement.inputVector.Vector3toXZ().x) &&
                (v.transform.position.z == characterMovement.inputVector.Vector3toXZ().z));

            if (pathGenarator.selectedAnimals[selectedIfObjectIndex].ifName == pathGenarator.currentIfObject.ifName)
            {
                collectedIfObjects++;
                yield return new WaitUntil(() => characterMovement.currentAnimal.transform.Find("Fx_PlantSparkle").gameObject.activeSelf == true);
                yield return new WaitForSeconds(2);
                yield return characterMovement.CompleteHalfWay();
            }
            else
            {
                var currentCoord = new MapGenerator.Coord((int)(inputVector.x / pathGenarator.mapGenerator.tileSize), (int)(inputVector.z / pathGenarator.mapGenerator.tileSize));

                var realCoord = pathGenarator.Path.Find(v => v.x == currentCoord.x && v.y == currentCoord.y);

                ShowWrongCleaningTile.instance.wrongIfTiles.Add(realCoord);
                yield return new WaitForSeconds(3f);
                yield return characterMovement.CompleteHalfWay();
                //AnimalMoveFromPath(currentAnimal);
            }
        }
        else if (characterMovement.currentPath.whichCoord == AnimalsInIfPath.isEmptyAnimalCoord)
        {
            yield return new WaitForSeconds(1f);
            yield return characterMovement.CompleteHalfWay();
        }
        else
        {
            yield return new WaitForSeconds(1f);
            characterMovement.isPlayerReachedTarget = false;

            characterMovement.gm.EndGame();
        }

        selectedIfObjectIndex++;
        characterMovement.checkTargetReached.CheckIfReachedTarget(isLastCommand, characterMovement);
    }
    public void ShowIfObjectAnimation(GameObject currentObject, Vector3 halfVector)
    {
        var myAnimator = currentObject.GetComponent<Animator>();

        myAnimator.SetBool("ifAnimStarted",true);
        //AnimatorStateInfo animationState = myAnimator.GetCurrentAnimatorStateInfo(0);
        //AnimatorClipInfo[] myAnimatorClip = myAnimator.GetCurrentAnimatorClipInfo(0);

        //Debug.Log(myAnimatorClip);
        //float myTime = myAnimatorClip[0].clip.length * animationState.normalizedTime;
        
        //var questionMark = currentObject.transform.Find("EmtyQuestionMark");

        //var character = pathGenarator.gm.character;
        //var characterCam = Camera.main;

        //var transformPosition = questionMark.transform.position;
        //var targetPos = new Vector3(transformPosition.x, transformPosition.y + 15, transformPosition.z);

        //questionMark.transform.DOMove(targetPos, 2).OnComplete(() =>
        //{
        //    questionMark.gameObject.SetActive(false);

        //    var transformPosition1 = currentObject.transform.position;
        //    var targetPos1 = new Vector3(transformPosition1.x, transformPosition1.y + 1, transformPosition1.z);

        //    var sparkle = currentObject.transform.Find("GFX/Fx_PlantSparkle/Particle System");
        //    sparkle.GetComponent<ParticleSystem>().Play();
        //    currentObject.transform.DOMove(targetPos1, 4).OnComplete(() =>
        //    {
        //        currentObject.SetActive(false);
        //    });
        //});
    }

}
