using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abidin/Tutorial/TutorialButton", order = 1)]
[System.Serializable]
public class TutorialButton : TutorialType
{
    public string tutorialText;
    public List<string> highlightObjects;
    public ActionPosition actionPosition;
    public float scaleSize = 1.5f;
    public AudioClip[] butonPressSound;
}
