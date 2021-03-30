using UnityEngine;

public class FPSTarget : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        
        //Screen.SetResolution(640, 480, true);
    }
}
