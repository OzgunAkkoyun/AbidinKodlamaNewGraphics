using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;
    public TextMeshProUGUI text;

    void Update()
    {
        ShowFps();
    }

    void ShowFps()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text1 = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        text.text = text1;
    }
}
