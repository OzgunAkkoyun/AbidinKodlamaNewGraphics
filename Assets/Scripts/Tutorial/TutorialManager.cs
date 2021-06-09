using System;
using System.Collections;
using System.Collections.Generic;
using DapperDino.UDCT.Utilities.DeveloperConsole.Commands;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ActionPosition { Up,Down }
[System.Serializable]
public class TutorialActions
{
    public string tutorialText;
    public AudioClip voice;
    public List<GameObject> highlightObjects;
    //public GameObject waitingActionObject;
    public ActionPosition actionPosition;
    
}
public class TutorialManager : MonoBehaviour,IPointerClickHandler
{
    public TutorialActions[] actions;
    public TutorialActions currentAction;
    public int actionIndex;
    public TextMeshProUGUI text;
    private bool nextAction;
    public GameObject textPanel;
    public GameObject tutorialPanel;
    private List<Transform> currentHighLightObjectParent = new List<Transform>();
    public AudioSource audioSource;

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
            for (int i = 0; i < currentAction.highlightObjects.Count; i++)
            {
                if (clickName == currentAction.highlightObjects[i].name)
                {
                    nextAction = true;
                }
            }
        }
    }
    private IEnumerator StartTutorial()
    {
        foreach (var action in actions)
        {
            currentAction = action;
            nextAction = false;
            currentHighLightObjectParent.Clear();
            SetTextPanelPosition();
            yield return StartCoroutine(StartTextWrite());
            yield return StartCoroutine(ShowHighLightObject());

            yield return new WaitUntil(() => nextAction==true);
            yield return new WaitForSeconds(1);
            SetObjectsParentBack();
            ResetButtonBouncingEffect();
            actionIndex++;
        }

        FinishTutorial();
        
    }

    private void FinishTutorial()
    {
        tutorialPanel.SetActive(false);
        PlayerPrefs.SetString("openingTutorial","finished");
    }

    private void SetObjectsParentBack()
    {
        if (currentAction.highlightObjects.Count > 0)
        {
            for (int i = 0; i < currentAction.highlightObjects.Count; i++)
            {
                currentAction.highlightObjects[i].transform.SetParent(currentHighLightObjectParent[i]);
            }
        }
    }

    private IEnumerator ShowHighLightObject()
    {
        if (currentAction.highlightObjects.Count>0)
        {
            var highLights = currentAction.highlightObjects;
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
        for (int i = 0; i < currentAction.highlightObjects.Count; i++)
        {
            currentAction.highlightObjects[i].transform.DOPause();
            currentAction.highlightObjects[i].transform.localScale = new Vector3(1,1,1);
        }
        
    }
    private void GiveButtonBouncingEffect(GameObject highLight)
    {
        var a = highLight.transform.DOScale(new Vector3(1.2f,1.2f,1.2f), 1).SetLoops(-1, LoopType.Yoyo);
    }

    private void SetTextPanelPosition()
    {
        if (currentAction.actionPosition == ActionPosition.Up)
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
        audioSource.clip = currentAction.voice;
        audioSource.Play();
        text.text = "";
        var story = currentAction.tutorialText;
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