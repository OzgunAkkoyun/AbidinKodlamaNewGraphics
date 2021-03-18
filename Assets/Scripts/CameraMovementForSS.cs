using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraMovementForSS : MonoBehaviour
{
    public UIHandler ui;
    public GameObject ssLayout;
    public float sensitivity;
    public Quaternion cameraRotation;
    public GameObject cameraTarget;
    public Camera camera;
    public GameManager gm;

    void Start()
    {
        cameraTarget = GameObject.Find("CameraTarget");
        camera = Camera.main;
    }

    void FixedUpdate()
    {
        if (gm.currentSenario.senarioIndex != 3)
            return;
        if (Input.GetMouseButton(0) && ssLayout.activeSelf)
        {
            float rotateHorizontal = Input.GetAxis("Mouse X");
            float rotateVertical = Input.GetAxis("Mouse Y");
            camera.transform.RotateAround(camera.transform.position, -Vector3.up,
                rotateHorizontal *
                sensitivity); //use transform.Rotate(-transform.up * rotateHorizontal * sensitivity) instead if you dont want the camera to rotate around the player

            camera.transform.RotateAround(camera.transform.position, camera.transform.right,
                rotateVertical *
                sensitivity); // again, use transform.Rotate(transform.right * rotateVertical * sensitivity) if you don't want the camera to rotate around the player

            if (camera.transform.rotation.eulerAngles.x < 10f)
            {
                var cameraEuler = camera.transform.rotation.eulerAngles;
                //camera.transform.Rotate(new Vector3(10, cameraEuler.y, cameraEuler.z));
                camera.transform.eulerAngles = new Vector3(10, cameraEuler.y, cameraEuler.z);
            }

            if (camera.transform.eulerAngles.x > 30f)
            {
                var cameraEuler = camera.transform.rotation.eulerAngles;
                //camera.transform.Rotate(new Vector3(30, cameraEuler.y, cameraEuler.z));
                camera.transform.eulerAngles = new Vector3(30, cameraEuler.y, cameraEuler.z);
            }

        }
    }

    public void OpenSSLayout()
    {
        ssLayout.SetActive(true);
    }

    public void TakeSS()
    {
        ScreenShotHandler.TakeScreenShot_Static(1080, 720);
        ssLayout.SetActive(false);
        Invoke("PutCameraToTargetPosition",0.2f);
    }

    public void PutCameraToTargetPosition()
    {
        camera.transform.position = cameraTarget.transform.position;
        camera.transform.rotation = cameraTarget.transform.rotation;
    }
}
