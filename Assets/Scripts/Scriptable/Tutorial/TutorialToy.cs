using UnityEngine;

[CreateAssetMenu(menuName = "Abidin/Tutorial/Tutorial Toy", order = 1)]
[System.Serializable]
public class TutorialToy : TutorialType
{
    public Sprite toyImage;
    public string toyName;
    public AudioClip wrongToySound;
}
