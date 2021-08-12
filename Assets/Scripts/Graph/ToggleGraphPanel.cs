using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGraphPanel : MonoBehaviour
{
    [SerializeField] private GameObject graphPanel;
    public TutorialManager tm;

    public void TogglePanel()
    {
        if (tm.tutorialFinished)
        {
            graphPanel.SetActive(!graphPanel.activeSelf);
        }
    }
}
