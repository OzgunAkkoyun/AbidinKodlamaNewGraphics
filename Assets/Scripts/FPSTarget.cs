using UnityEngine;

public class FPSTarget : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 0;
        QualitySettings.vSyncCount = 1;
    }
}
