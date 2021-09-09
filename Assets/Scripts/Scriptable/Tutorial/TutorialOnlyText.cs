using UnityEngine;

[CreateAssetMenu(menuName = "Abidin/Tutorial/TutorialOnlyText", order = 1)]
[System.Serializable]
public class TutorialOnlyText : TutorialType
{
    public string tutorialText;
    public float whichSecondObjectShow;
    public ActionPosition actionPosition;
    public ActionPositionHorizantal actionPositionHorizantal;
}

