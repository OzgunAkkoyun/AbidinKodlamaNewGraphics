using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShowWrongCleaningTile : MonoBehaviour
{
    public static ShowWrongCleaningTile instance;
    public List<MapGenerator.Coord> wrongWaitTiles = new List<MapGenerator.Coord>();
    public List<MapGenerator.Coord> wrongIfTiles = new List<MapGenerator.Coord>();
    public UiMiniMapController miniMapController;
    public MapGenerator mapGenerator;
    void Awake()
    {
        instance = this;
    }

    public void ShowWrongCleaningTiles()
    {
        miniMapController.MiniMapFullSize();

        for (int i = 0; i < wrongWaitTiles.Count; i++)
        {
            var wrongTileIndex = mapGenerator.allTileCoords.FindIndex(v =>
                (v.x == wrongWaitTiles[i].x) && (v.y == wrongWaitTiles[i].y));
            var wrongTile = mapGenerator.allTileGameObject[wrongTileIndex].gameObject;
            //wrongTile.AddComponent<Renderer>();
            wrongTile.SetActive(true);
            wrongTile.transform.position = new Vector3(wrongTile.transform.position.x, wrongTile.transform.position.y+0.2f, wrongTile.transform.position.z);
            wrongTile.GetComponent<Renderer>().material.DOColor(new Color(217/255f,33/255f,57/255f), 2).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void ShowWrongIfTiles()
    {
        miniMapController.MiniMapFullSize();

        for (int i = 0; i < wrongIfTiles.Count; i++)
        {
            var wrongTileIndex = mapGenerator.allTileCoords.FindIndex(v => (v.x == wrongIfTiles[i].x) && (v.y == wrongIfTiles[i].y));

            var wrongTile = mapGenerator.allTileGameObject[wrongTileIndex].gameObject;
            wrongTile.SetActive(true);
            wrongTile.transform.position = new Vector3(wrongTile.transform.position.x, wrongTile.transform.position.y + 0.2f, wrongTile.transform.position.z);

            wrongTile.GetComponent<Renderer>().material.DOColor(new Color(217 / 255f, 33 / 255f, 57 / 255f), 2).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
