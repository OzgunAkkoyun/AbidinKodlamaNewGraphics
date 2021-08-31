using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public enum ActionPosition { Up,Down }
public enum ActionPositionHorizantal { Left,Middle,Right }
[System.Serializable]
public class TutorialManager : MonoBehaviour,IPointerClickHandler
{
    public TutorialType currentAction;
    public TutorialType[] senarioAction;
    private int actionIndex;
    public TextMeshProUGUI text;
    private bool nextAction;
    public bool forToyCliked;
    public bool tutorialFinished;
    public GameObject textPanel;
    public GameObject toyPanel;
    public GameObject videoPanel;
    public GameObject endPanel;
    public GameObject textPanelButton;
    public GameObject videoPanelButton;

    public Image toyImage;
    public VideoPlayer toyVideo;
    public GameObject tutorialPanel;
    private List<Transform> currentHighLightObjectParent = new List<Transform>();
    private List<Transform> currentHighLightObjects = new List<Transform>();
    public AudioSource audioSource;

    public AllTutorialTypes[] actions = new AllTutorialTypes[5];
    public Commander commander;
    public TutorialCharacterMove character;
    public DeleteCommandTutorial deleteCommand;
    public string comingToyName;
    private float timeRemaining = 20;
    private int whichTutorial;

    public Image abidinImage;

    public void Start()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            whichTutorial = PlayerPrefs.GetInt("whichTutorial");
            tutorialPanel.SetActive(true);
            senarioAction = actions[0].allTutorialTypes;
            commander.OnNewCommand.AddListener(OnNewCommand);
            StartCoroutine(StartTutorial());
        }
        else
        {
            var tutorialSeen = PlayerPrefs.GetString("openingTutorial");

            if (tutorialSeen == "")
            {
                senarioAction = actions[0].allTutorialTypes;
                tutorialPanel.SetActive(true);
                StartCoroutine(StartTutorial());
            }
            else
            {
                tutorialFinished = true;
            }
        }
    }

    void OnDestroy()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial")
            commander.OnNewCommand.RemoveListener(OnNewCommand);
    }

    private void OnNewCommand()
    {
        var newCommand = commander.commands.Last();
        DetectCommand(newCommand);
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !nextAction && !tutorialFinished)
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

        if (!tutorialFinished)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timeRemaining = 20;
                //videoPanel.SetActive(true);
                //PlayActionVideo();
                audioSource.Play();
            }
        }

    }
    public IEnumerator DoApplyCommands()
    {
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(FindObjectOfType<UIInputManagerTutorial>().ShowPath());

        for (var i = 0; i < commander.commands.Count; i++)
        {
            var command = commander.commands[i];
            var type = command.GetType();
            var isLastCommand = i == commander.commands.Count - 1;

            if (type == typeof(MoveCommand))
            {
                var commandMove = command as MoveCommand;
                
                yield return StartCoroutine(character.Move(commandMove.direction));
            }
            //else if (type == typeof(ForCommand))
            //{
            //    var commandFor = command as ForCommand;
            //    var isLastForCommand = false;

            //    for (int j = 0; j < commandFor.loopCount; j++)
            //    {
            //        for (int k = 0; k < commandFor.directions.Count; k++)
            //        {
            //            isLastForCommand = isLastCommand && j == commandFor.loopCount - 1 && k == commandFor.directions.Count - 1;

            //            yield return StartCoroutine(character.Move(commandFor.directions[k]));
            //        }
            //    }
            //}
        }
        tutorialPanel.SetActive(true);
        nextAction = true;
    }
    private IEnumerator StartTutorial()
    {
        foreach (var action in senarioAction)
        {
            currentAction = action;
            nextAction = false;
            currentHighLightObjectParent.Clear();
            currentHighLightObjects.Clear();
            timeRemaining = 20;
            tutorialFinished = false;
            var currentType = currentAction.GetType();

            if (currentType == typeof(TutorialButton))
            {
                textPanel.SetActive(true);
                toyPanel.SetActive(false);
                videoPanel.SetActive(false);
                textPanelButton.SetActive(false);

                SetCurrentHighlightObjects();
                SetTextPanelPosition();
                //yield return StartCoroutine(StartTextWrite("Button"));
                PlayActionVoice();
                yield return StartCoroutine(ShowHighLightObject());

                yield return new WaitUntil(() => nextAction == true);
                yield return new WaitForSeconds(1);
                SetObjectsParentBack();
                ResetButtonBouncingEffect();
                actionIndex++;
            }
            else if (currentType == typeof(TutorialToy))
            {
                toyPanel.SetActive(false);
                yield return new WaitForSeconds(1);
                textPanel.SetActive(false);
                videoPanel.SetActive(false);
                toyPanel.SetActive(true);
                ShowToyPicture();
                yield return new WaitUntil(() => nextAction == true);
                actionIndex++;
            }
            else if (currentType == typeof(OnlyVideo))
            {
                textPanel.SetActive(false);
                toyPanel.SetActive(false);
                videoPanel.SetActive(true);
                videoPanelButton.SetActive(false);
                var actionVideo = currentAction as OnlyVideo;
                toyVideo.clip = actionVideo.showingVideo;
                PlayActionVoice();
                Invoke("OpenVideoButton", 3);
                ShowVideo();
                yield return new WaitUntil(() => nextAction == true);
                actionIndex++;
            }
            else if (currentType == typeof(TutorialOnlyText))
            {
                textPanel.SetActive(true);
                toyPanel.SetActive(false);
                videoPanel.SetActive(false);
                textPanelButton.SetActive(true);

                SetTextPanelPositionForOnlyText();
                SetTextPanelPositionHorizontalForOnlyText();
                yield return StartCoroutine(StartTextWrite("OnlyText"));
                yield return new WaitUntil(() => nextAction == true);
                actionIndex++;
            }
            else if (currentType == typeof(OnlySound))
            {
                textPanel.SetActive(true);
                toyPanel.SetActive(false);
                videoPanel.SetActive(false);
                textPanelButton.SetActive(false);

                SetTextPanelPositionForOnlySound();
                var action1 = currentAction as OnlySound;
                
                if (action1.toyImage != null)
                {
                    toyImage.sprite = action1.toyImage;
                    toyPanel.SetActive(true);
                }

                //yield return StartCoroutine(StartTextWrite("OnlySound"));
                PlayActionVoice();
                yield return StartCoroutine(WaitVoiceEnds());
                yield return new WaitUntil(() => nextAction == true);
                actionIndex++;
            }

            else if (currentType == typeof(ShowSomething))
            {
                textPanel.SetActive(true);
                toyPanel.SetActive(false);
                videoPanel.SetActive(false);
                textPanelButton.SetActive(false);

                SetTextPanelPositionForShowSomething();
                SetTextPanelPositionHorizontalForShowSomething();
                //yield return StartCoroutine(StartTextWrite("ShowSomething"));
                PlayActionVoice();
                yield return StartCoroutine(ShowSomethingInUi());
                HideSomethingInUi();
                yield return new WaitUntil(() => nextAction == true);
                actionIndex++;
            }
            else if (currentType == typeof(TutorialLoopToy))
            {
                textPanel.SetActive(false);
                toyPanel.SetActive(true);
                videoPanel.SetActive(false);
                textPanelButton.SetActive(false);

                ShowToyPictureForLoop();
                yield return new WaitUntil(() => forToyCliked == true);
                toyPanel.SetActive(false);
                PlaySecondSound();

                yield return new WaitUntil(() => nextAction == true);
                actionIndex++;
            }
        }
        OpenTutorialEnd();
        //FinishTutorial();
    }

    void OpenVideoButton()
    {
        videoPanelButton.SetActive(true);
    }
    private void SetTextPanelPositionHorizontalForOnlyText()
    {
        var action = currentAction as TutorialOnlyText;
        if (action.actionPositionHorizantal == ActionPositionHorizantal.Left)
        {
            abidinImage.GetComponent<RectTransform>().SetAnchor(AnchorPresets.MiddleLeft);
            abidinImage.GetComponent<RectTransform>().SetPivot(PivotPresets.MiddleLeft);

        }
        else if (action.actionPositionHorizantal == ActionPositionHorizantal.Middle)
        {
            abidinImage.GetComponent<RectTransform>().SetAnchor(AnchorPresets.MiddleCenter);
            abidinImage.GetComponent<RectTransform>().SetPivot(PivotPresets.MiddleCenter);
        }
    }

    private void SetTextPanelPositionHorizontalForShowSomething()
    {
        var action = currentAction as ShowSomething;
        if (action.actionPositionHorizantal == ActionPositionHorizantal.Left)
        {
            abidinImage.GetComponent<RectTransform>().SetAnchor(AnchorPresets.MiddleLeft);
            abidinImage.GetComponent<RectTransform>().SetPivot(PivotPresets.MiddleLeft);

        }
        else if(action.actionPositionHorizantal == ActionPositionHorizantal.Middle)
        {
            abidinImage.GetComponent<RectTransform>().SetAnchor(AnchorPresets.MiddleCenter);
            abidinImage.GetComponent<RectTransform>().SetPivot(PivotPresets.MiddleCenter);
        }
    }

    private void PlaySecondSound()
    {
        var action = currentAction as TutorialLoopToy;
        audioSource.clip = action.secondSound;
        audioSource.Play();
    }

    private IEnumerator WaitVoiceEnds()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        nextAction = true;
    }

    private IEnumerator ShowSomethingInUi()
    {
        var action = currentAction as ShowSomething;
        var showingObject = GameObject.Find(action.showObjects).transform;
        currentHighLightObjectParent.Add(showingObject.transform.parent);
        showingObject.transform.SetParent(tutorialPanel.transform);
        yield return new WaitForSeconds(audioSource.clip.length+1);
    }
    private void HideSomethingInUi()
    {
        var action = currentAction as ShowSomething;
        var showingObject = GameObject.Find(action.showObjects).transform;

        showingObject.transform.SetParent(currentHighLightObjectParent[0]);
        nextAction = true;
    }

    private void ShowVideo()
    {
        toyVideo.Play();
    }

    public void FinishVideoAction()
    {
        nextAction = true;
    }

    public void FinishOnlyText()
    {
        nextAction = true;
    }
    private void OpenTutorialEnd()
    {
        tutorialFinished = true;
        endPanel.SetActive(true);
        textPanel.SetActive(false);
        toyPanel.SetActive(false);
    }

    #region TutorialToy

    private void ShowToyPicture()
    {
        var action = currentAction as TutorialToy;
        toyImage.sprite = action.toyImage;
        PlayActionVoice();
    }

    private void ShowToyPictureForLoop()
    {
        var action = currentAction as TutorialLoopToy;
        toyImage.sprite = action.toyImage;
        PlayActionVoice();
    }

    private void DetectCommand(Command command)
    {
        var type = command.GetType();

        if (forToyCliked == true)
        {
            if (type == typeof(MoveCommand))
            {
                nextAction = true;
            }
            else
            {
                return;
            }
        }

        if (type == typeof(MoveCommand))
        {
            var moveCommand = (MoveCommand)command;
            if (!forToyCliked)
            {
                IsItWaitingToy(moveCommand.direction.ToString());
                //if (IsItWaitingToy(moveCommand.direction.ToString()))
                //{
                //    StartCoroutine(character.Move(moveCommand.direction));
                //}
            }
        }
        else if (type == typeof(ForCommand))
        {
            var forCommand = (ForCommand)command;
            
            //IsItWaitingToy("Loop");
        }
        else if (type == typeof(PickIfAnyObjectExistsCommandTutorial))
        {
            var ifCommand = (PickIfAnyObjectExistsCommandTutorial)command;
            
            IsItWaitingToy("Condition");
        }
        else if (type == typeof(WaitCommand))
        {
            var waitCommand = (WaitCommand)command;
           
            IsItWaitingToy("Wait");
        }
    }

    private bool IsItWaitingToy(string comingName)
    {
        var action = currentAction as TutorialToy;
        if (!action.CheckIsNull())
        {
            if (action.toyName == comingName)
            {
                nextAction = true;
                return true;
            }
            else
            {
                PlayActionVoice();
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    #endregion

    private void SetCurrentHighlightObjects()
    {
        var action = currentAction as TutorialButton;
        for (int i = 0; i < action.highlightObjects.Count; i++)
        {
            currentHighLightObjects.Add(GameObject.Find(action.highlightObjects[i]).transform);
        }
    }

    public void FinishTutorial()
    {
        tutorialPanel.SetActive(false);
       
        if (SceneManager.GetActiveScene().name == "Level")
        {
            PlayerPrefs.SetString("openingTutorial","finished");
            SceneManager.LoadScene("Level");
        }
        else
        {
            PlayerPrefs.SetString("levelTutorial"+ whichTutorial, "finished");
            SceneManager.LoadScene("Game");
        }
    }

    public void RestartTutorial()
    {
        tutorialPanel.SetActive(true);
        endPanel.SetActive(false);
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            //deleteCommand.DeleteAllCommands();
            SceneManager.LoadScene("Tutorial");
        }
        StartCoroutine(StartTutorial());
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
                GiveButtonBouncingEffect(highLight, action.scaleSize);
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

    private void GiveButtonBouncingEffect(GameObject highLight, float actionScaleSize)
    {
        var a = highLight.transform.DOScale(new Vector3(actionScaleSize, actionScaleSize, actionScaleSize), 1).SetLoops(-1, LoopType.Yoyo);
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

    private void SetTextPanelPositionForOnlyText()
    {
        var action = currentAction as TutorialOnlyText;
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
    private void SetTextPanelPositionForOnlySound()
    {
        var action = currentAction as OnlySound;
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

    private void SetTextPanelPositionForShowSomething()
    {
        var action = currentAction as ShowSomething;
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

    IEnumerator StartTextWrite(string buttonorText)
    {
        if (buttonorText == "Button")
        {
            var action = currentAction as TutorialButton ;
            PlayActionVoice();
            text.text = "";
            var story = action.tutorialText;
            foreach (char c in story)
            {
                text.text += c;
                yield return new WaitForSeconds(0.03f);
            }
        }
        else if(buttonorText == "OnlyText")
        {
            var action = currentAction as TutorialOnlyText;
            PlayActionVoice();
            text.text = "";
            var story = action.tutorialText;
            foreach (char c in story)
            {
                text.text += c;
                yield return new WaitForSeconds(0.03f);
            }
        }
        else if (buttonorText == "ShowSomething")
        {
            var action = currentAction as ShowSomething;
            PlayActionVoice();
            text.text = "";
            var story = action.tutorialText;
            foreach (char c in story)
            {
                text.text += c;
                yield return new WaitForSeconds(0.03f);
            }
        }
        else
        {
            var action = currentAction as OnlySound;
            PlayActionVoice();
            text.text = "";
            var story = action.tutorialText;
            foreach (char c in story)
            {
                text.text += c;
                yield return new WaitForSeconds(0.03f);
            }
        }
    }

    private void PlayActionVoice()
    {
        audioSource.clip = currentAction.voice;
        audioSource.Play();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
    }
}