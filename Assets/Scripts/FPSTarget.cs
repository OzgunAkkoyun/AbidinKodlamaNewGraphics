using UnityEngine;

public class FPSTarget : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 0;
        QualitySettings.vSyncCount = 1;

        
        //Screen.SetResolution(640, 480, true);
    }
}
