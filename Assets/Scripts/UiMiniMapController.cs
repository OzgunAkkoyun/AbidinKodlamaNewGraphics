using System.Collections;
using UnityEngine;

public class UiMiniMapController : MonoBehaviour
{
    public MapGenerator map;
    public GameManager gm;

    public GameObject minimap;
    public GameObject minimapTexture;
    public GameObject minimapPipBoy;
    public GameObject miniMapGraphics;

    public Camera miniMapCamera;
    public Camera miniMapCameraOnlyVehicle;

    private bool mapZoomed = false;
    private float mapSizeMultiplier = 0.15f;

    public RectTransform miniMapRect;
    public RectTransform miniMapGraphicsRect;
    public RectTransform minimapTextureRect;

    private float screenW;
    private float screenH;

    private float textureSize;

    public GameObject miniMapCloseButton;
    void Start()
    {
        screenW = Screen.width;
        screenH = Screen.height;
        
        //MiniMapSizeSet();

        var miniMapZPos = 0f;
        if (map.currentMap.mapSize.y == 5)
        {
            miniMapZPos = -5.8f;
            miniMapCamera.orthographicSize = 7;
            miniMapCameraOnlyVehicle.orthographicSize = 7;
        }
        else if (map.currentMap.mapSize.y == 7)
        {
            miniMapZPos = -3.8f;
            miniMapCamera.orthographicSize = 9;
            miniMapCameraOnlyVehicle.orthographicSize = 9;
        }
        else if (map.currentMap.mapSize.y == 9)
        {
            miniMapZPos = -1.6f;
            miniMapCamera.orthographicSize = 11;
            miniMapCameraOnlyVehicle.orthographicSize = 11;
        }

        miniMapCamera.transform.position = new Vector3(map.currentMap.mapSize.x - 1, miniMapCamera.transform.position.y, miniMapZPos);
        miniMapCameraOnlyVehicle.transform.position = new Vector3(map.currentMap.mapSize.x - 1, miniMapCamera.transform.position.y, miniMapZPos);
        Invoke("CloseCamera",1f);
        
    }

    void CloseCamera()
    {
        miniMapCamera.gameObject.SetActive(false);
    }

    public void MiniMapCloseButton()
    {
        gm.EndGame();
        minimap.SetActive(false);
    }
    void MiniMapSizeSet()
    {
        miniMapGraphicsRect.sizeDelta = new Vector2(screenH, screenH);
        RatiosForMiniMap();
    }

    void RatiosForMiniMap()
    {
        textureSize = (float)(miniMapGraphicsRect.sizeDelta.y - miniMapGraphicsRect.sizeDelta.y * 0.2);
        minimapTextureRect.sizeDelta = new Vector2(textureSize, textureSize);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            MiniMapFullSize();
        }
    }

    public void MiniMapFullSize()
    {
        miniMapCamera.gameObject.SetActive(true);
        miniMapCloseButton.SetActive(true);

        miniMapRect.SetAnchor(AnchorPresets.StretchAll);
        miniMapRect.SetPivot(PivotPresets.MiddleCenter);

        miniMapRect.SetLeft(0);
        miniMapRect.SetRight(0);
        miniMapRect.SetTop(0);
        miniMapRect.SetBottom(0);

        
        miniMapGraphicsRect.SetAnchor(AnchorPresets.MiddleCenter);
        miniMapGraphicsRect.SetPivot(PivotPresets.MiddleCenter);
        miniMapGraphicsRect.sizeDelta = new Vector2(900f, 900f);

        minimapTextureRect.SetLeft(100);
        minimapTextureRect.SetRight(100);
        minimapTextureRect.SetTop(100);
        minimapTextureRect.SetBottom(100);

    }
    public void MiniMapZoom()
    {
        if (gm.is3DStarted)
            StartCoroutine(MiniMapSizeChange());
    }

    public IEnumerator MiniMapSetStartPosition()
    {
        gm.text.text +=" minimap";
        miniMapRect.SetAnchor(AnchorPresets.TopRight);
        miniMapRect.SetPivot(PivotPresets.TopRight);
        miniMapRect.sizeDelta = new Vector2(screenH, screenH);

        minimapTextureRect.SetLeft(30);
        minimapTextureRect.SetRight(30);
        minimapTextureRect.SetTop(30);
        minimapTextureRect.SetBottom(30);
        //minimapPipBoy.SetActive(false);
        float t = 0;

        while (true)
        {
            t += Time.deltaTime / 10;
            miniMapRect.sizeDelta =
                Vector2.Lerp(miniMapRect.sizeDelta, new Vector2(300, 300), t);

            miniMapGraphics.transform.localScale =
                Vector2.Lerp(miniMapGraphics.transform.localScale, new Vector3(1, 1, 1), t * 2);

            miniMapGraphicsRect.sizeDelta = new Vector2(
                (miniMapRect.sizeDelta.y - 20 - 1 * 30),
                (miniMapRect.sizeDelta.y - 20 - 1 * 30));

           // minimapTextureRect.sizeDelta = Vector2.Lerp(minimapTextureRect.sizeDelta, new Vector2(240, 240), t);

            miniMapGraphicsRect.SetLeft(0);
            miniMapGraphicsRect.SetRight(0);
            miniMapGraphicsRect.SetTop(0);
            miniMapGraphicsRect.SetBottom(0);

            miniMapGraphicsRect.SetAnchor(AnchorPresets.StretchAll);

            miniMapGraphicsRect.SetPivot(PivotPresets.MiddleCenter);

            if (Mathf.Round(miniMapRect.sizeDelta.x) == 300)
            {

                yield break;
            }

            yield return new WaitForSeconds(0f);
        }
    }

    public IEnumerator MiniMapSizeChange()
    {
        for (int i = 0; i < 10; i++)
        {
            if (!mapZoomed)
            {
                minimap.transform.localScale = minimap.transform.localScale +
                                               new Vector3(mapSizeMultiplier, mapSizeMultiplier, mapSizeMultiplier);
                yield return new WaitForSeconds(0f);
            }
            else
            {
                minimap.transform.localScale = minimap.transform.localScale -
                                               new Vector3(mapSizeMultiplier, mapSizeMultiplier, mapSizeMultiplier);
                yield return new WaitForSeconds(0f);
            }
        }
        mapZoomed = !mapZoomed;
    }
}
