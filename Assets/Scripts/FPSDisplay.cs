using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;
    int FPSCounter;
    public TextMeshProUGUI text;

    private void OnEnable()
    {
        InvokeRepeating("RefreshText", 0f, 1f);
    }

    private void OnDisable()
    {
        CancelInvoke("RefreshText");
    }

    void Update()
    {
        FPSCounter++;
    }

    void RefreshText()
    {
        text.text = FPSCounter.ToString();
        FPSCounter = 0;
    }
}
