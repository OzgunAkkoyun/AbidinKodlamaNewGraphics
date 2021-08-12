using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abidin/Tutorial/Tutorial Loop Toy", order = 1)]
[System.Serializable]
public class TutorialLoopToy : TutorialType
{
    public AudioClip secondSound;
    public Sprite toyImage;
    public string toyName;
}
