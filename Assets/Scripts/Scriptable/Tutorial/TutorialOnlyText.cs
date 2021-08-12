using UnityEngine;

[CreateAssetMenu(menuName = "Abidin/Tutorial/TutorialOnlyText", order = 1)]
[System.Serializable]
public class TutorialOnlyText : TutorialType
{
    public string tutorialText;
    public ActionPosition actionPosition;
    public ActionPositionHorizantal actionPositionHorizantal;
}

