using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class TutorialType : ScriptableObject
{
    public AudioClip voice;
}

[CreateAssetMenu(menuName = "Abidin/Tutorial/TutorialButton", order = 1)]
[System.Serializable]
public class TutorialButton: TutorialType
{
    public string tutorialText;
    public List<string> highlightObjects;
    public ActionPosition actionPosition;
}

[CreateAssetMenu(menuName = "Abidin/Tutorial/TutorialToy", order = 1)]
[System.Serializable]
public class TutorialToy: TutorialType
{
    public VideoClip showingVideo;
    public Sprite toyImage;
}
