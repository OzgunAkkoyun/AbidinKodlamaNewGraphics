using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCharacterMove : MonoBehaviour
{
    private Vector3 inputVector;
    public float scaleFactor = 2f;
    public float animationSpeed = 1f;

    void Start()
    {
        inputVector = transform.position;
    }
    public IEnumerator Move(Direction moveCommand)
    {
        DirectionToVector(moveCommand);
        for (float t = 0f; t < 1f; t += Time.deltaTime * animationSpeed)
        {
            transform.position = Vector3.Lerp(transform.position, inputVector, t);
            yield return new WaitForSeconds(0.04f);
        }
        transform.position = inputVector;
    }
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
