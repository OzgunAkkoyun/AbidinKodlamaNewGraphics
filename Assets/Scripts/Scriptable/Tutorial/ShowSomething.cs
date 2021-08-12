using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abidin/Tutorial/ShowSomething", order = 1)]
[System.Serializable]

public class ShowSomething : TutorialType
{
    public string tutorialText;
    public string showObjects;
    public ActionPosition actionPosition;
    public ActionPositionHorizantal actionPositionHorizantal;
}
