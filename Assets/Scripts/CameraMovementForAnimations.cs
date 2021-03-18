using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementForAnimations : MonoBehaviour
{
    public static CameraMovementForAnimations instance;
    public Transform animTarget;
    public Transform currentCameraPos;
    public Camera mainCamera;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        animTarget = gameObject.transform.Find("AnimTarget");
        currentCameraPos = gameObject.transform.Find("CameraTarget");
        mainCamera = Camera.main;
    }

    public IEnumerator CameraMoveToTargetPosition()
    {
        float t = 0;
        while (Vector3.Distance(mainCamera.transform.position, animTarget.position) > 0.1f)
        {
            t += Time.deltaTime / 30;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, animTarget.position, t);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, animTarget.rotation, t);

            yield return new WaitForSeconds(0f);
        }
    }

}
