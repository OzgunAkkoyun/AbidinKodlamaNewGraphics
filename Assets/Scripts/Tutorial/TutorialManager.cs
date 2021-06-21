using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ActionPosition { Up,Down }
[System.Serializable]
public class TutorialManager : MonoBehaviour,IPointerClickHandler
{
    public TutorialType currentAction;
    private int actionIndex;
    public TextMeshProUGUI text;
    private bool nextAction;
    public GameObject textPanel;
    public GameObject tutorialPanel;
    private List<Transform> currentHighLightObjectParent = new List<Transform>();
    private List<Transform> currentHighLightObjects = new List<Transform>();
    public AudioSource audioSource;

    public AllTutorialTypes actions;

    public void Start()
    {
        PlayerPrefs.DeleteKey("openingTutorial");
        var tutorialSeen = PlayerPrefs.GetString("openingTutorial");
        
        if (tutorialSeen == "")
        {
            tutorialPanel.SetActive(true);
            StartCoroutine(StartTutorial());
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !nextAction)
        {
            var clickName = EventSystem.current.currentSelectedGameObject?.name;
            var currentType = currentAction.GetType();

            if (currentType == typeof(TutorialButton))
            {
                var action = currentAction as TutorialButton;

                for (int i = 0; i < action.highlightObjects.Count; i++)
                {
                    if (clickName == currentHighLightObjects[i].name)
                    {
                        nextAction = true;
                    }
                }
            }
            
        }
    }
    private IEnumerator StartTutorial()
    {
        foreach (var action in actions.allTutorialTypes)
        {
            currentAction = action;
            nextAction = false;
            currentHighLightObjectParent.Clear();
            currentHighLightObjects.Clear();
            var currentType = currentAction.GetType();

            if (currentType == typeof(TutorialButton))
            {
                currentAction = currentAction as TutorialButton;
                SetCurrentHighlightObjects();
                SetTextPanelPosition();
                yield return StartCoroutine(StartTextWrite());
                yield return StartCoroutine(ShowHighLightObject());

                yield return new WaitUntil(() => nextAction == true);
                yield return new WaitForSeconds(1);
                SetObjectsParentBack();
                ResetButtonBouncingEffect();
                actionIndex++;
            }
            else if (currentType == typeof(TutorialToy))
            {
                currentAction = currentAction as TutorialToy;
            }
        }

        FinishTutorial();
        
    }

    private void SetCurrentHighlightObjects()
    {
        var action = currentAction as TutorialButton;
        for (int i = 0; i < action.highlightObjects.Count; i++)
        {
            currentHighLightObjects.Add(GameObject.Find(action.highlightObjects[i]).transform);
        }
    }

    private void FinishTutorial()
    {
        tutorialPanel.SetActive(false);
        PlayerPrefs.SetString("openingTutorial","finished");
    }

    private void SetObjectsParentBack()
    {
        var action = currentAction as TutorialButton;
        if (action.highlightObjects.Count > 0)
        {
            for (int i = 0; i < action.highlightObjects.Count; i++)
            {
                currentHighLightObjects[i].SetParent(currentHighLightObjectParent[i]);
            }
        }
    }

    private IEnumerator ShowHighLightObject()
    {
        var action = currentAction as TutorialButton;
        if (action.highlightObjects.Count>0)
        {
            List<GameObject> highLights = new List<GameObject>();
            for (int i = 0; i < action.highlightObjects.Count; i++)
            {
                highLights.Add(GameObject.Find(action.highlightObjects[i]));
            }
            
            foreach (var highLight in highLights)
            {
                currentHighLightObjectParent.Add(highLight.transform.parent);
                highLight.transform.SetParent(tutorialPanel.transform);
                GiveButtonBouncingEffect(highLight);
            }
            yield return null;
        }
        else
        {
            nextAction = true;
        }
        
    }
    private void ResetButtonBouncingEffect()
    {
        var action = currentAction as TutorialButton;
        for (int i = 0; i < action.highlightObjects.Count; i++)
        {
            currentHighLightObjects[i].transform.DOPause();
            currentHighLightObjects[i].transform.localScale = new Vector3(1,1,1);
        }
        
    }
    private void GiveButtonBouncingEffect(GameObject highLight)
    {
        var a = highLight.transform.DOScale(new Vector3(1.2f,1.2f,1.2f), 1).SetLoops(-1, LoopType.Yoyo);
    }

    private void SetTextPanelPosition()
    {
        var action = currentAction as TutorialButton;
        if (action.actionPosition == ActionPosition.Up)
        {
            textPanel.GetComponent<RectTransform>().SetAnchor(AnchorPresets.HorStretchTop);
            textPanel.GetComponent<RectTransform>().SetPivot(PivotPresets.TopLeft);
            
        }
        else
        {
            textPanel.GetComponent<RectTransform>().SetAnchor(AnchorPresets.HorStretchBottom);
            textPanel.GetComponent<RectTransform>().SetPivot(PivotPresets.BottomLeft);
        }
    }

    IEnumerator StartTextWrite()
    {
        var action = currentAction as TutorialButton;
        audioSource.clip = currentAction.voice;
        audioSource.Play();
        text.text = "";
        var story = action.tutorialText;
        foreach (char c in story)
        {
            text.text += c;
            yield return new WaitForSeconds(0.03f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
    }
}