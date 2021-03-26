using UnityEngine;

public class FPSTarget : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
        Screen.SetResolution(640, 480, true);
    }
}
