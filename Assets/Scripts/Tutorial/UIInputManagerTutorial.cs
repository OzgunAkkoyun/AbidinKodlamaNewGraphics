using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIInputManagerTutorial : MonoBehaviour
{
    public GameObject codeStringPanel;
    public GameObject codePanelInner;

    public GameObject forInput;
    public GameObject waitInput;
    public GameObject ifInput;

    public GameObject forComplatePanel;
    
    public GameObject codePanel;
    public GameObject codePanelOpenButton;
    private bool codePanelOpened = true;
    private float codePaneleWidth = 0;

    public List<Image> codeInputsObjects = new List<Image>();
    public GameObject codeMoveObject;
    public GameObject codeForObject;
    public GameObject codeForObjectTwoGap;
    public GameObject codeForObjectTreeGap;
    public GameObject codeForObjectFourGap;
    public GameObject codeIfObject;
    public GameObject codeWaitObject;
    public GameObject codeIfObjectChild;
    public GameObject panel;

    public TextMeshProUGUI codeString;
    public Commander commander;

    public int commandIndex = 0;

    private GameObject tempLoopGameObject;
    public GameObject tempLoopGameObjectContainer;
    public GameObject[] pathSprites;

    void Awake()
    {
        codePaneleWidth = Mathf.Abs(codePanel.transform.position.x);

        panel = GameObject.Find("CodePanel/Scroll");

        forInput.gameObject.SetActive(false);
    }

    public IEnumerator ShowPath()
    {
        for (var i=0; i<commander.commands.Count; i++) 
        {
            codeInputsObjects[i].color = new Color(163 / 255, 255 / 255, 131 / 255);
            if (i != 0)
                codeInputsObjects[i - 1].color = Color.white;
            pathSprites[i].SetActive(true);
            pathSprites[i].GetComponent<SpriteRenderer>().material.DOColor(new Color(127 / 255f, 255 / 255f, 202 / 255f,1), 2).SetEase(Ease.Flash);
            yield return new WaitForSeconds(1);
            pathSprites[i].GetComponent<SpriteRenderer>().material.DOColor(new Color(1, 1, 1,0), 2).SetEase(Ease.Flash);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
    }
    void Start()
    {
        commander.OnNewCommand.AddListener(OnNewCommand);
    }

    void OnDestroy()
    {
        commander.OnNewCommand.RemoveListener(OnNewCommand);
    }

    private void OnNewCommand()
    {
        var newCommand = commander.commands.Last();

        ShowCommand(newCommand);
    }

    #region CodePanel

    public void CodePanelOpen()
    {
        StartCoroutine(CodePanel());
    }

    public void CodeStringPanelOpen()
    {
        if (codePanelInner.activeSelf)
        {
            codePanelInner.GetComponent<RectTransform>().DOAnchorPosX(-450f, .4f).OnComplete(() =>
            {
                codePanelInner.SetActive(!codePanelInner.activeSelf);
            });

            codeStringPanel.SetActive(!codeStringPanel.activeSelf);
            codeStringPanel.GetComponent<RectTransform>().DOAnchorPosX(0f, .4f);
        }
        else
        {
            codeStringPanel.GetComponent<RectTransform>().DOAnchorPosX(-450f, .4f).OnComplete(() =>
            {
                codeStringPanel.SetActive(!codeStringPanel.activeSelf);

            });
            codePanelInner.SetActive(!codePanelInner.activeSelf);
            codePanelInner.GetComponent<RectTransform>().DOAnchorPosX(0f, .4f);
        }
    }

    public IEnumerator CodePanel()
    {
        var rectTransform = codePanel.GetComponent<RectTransform>();
        for (int i = 0; i < 10; i++)
        {
            if (codePanelOpened)
            {
                codePanelOpenButton.gameObject.SetActive(false);
                rectTransform.DOMoveX(57, 0.5f);
                yield return new WaitForSeconds(0f);
            }
            else
            {
                codePanelOpenButton.gameObject.SetActive(true);
                rectTransform.DOMoveX(-913f, 1f);
                yield return new WaitForSeconds(0f);
            }
        }

        codePanelOpened = !codePanelOpened;
    }

    public void ShowCommand(Command command)
    {
        var type = command.GetType();
        if (type == typeof(MoveCommand))
        {
            var moveCommand = (MoveCommand)command;
            ShowKey(moveCommand.direction, commandIndex);
            commandIndex++;
        }
        else if (type == typeof(ForCommand))
        {
            var forCommand = (ForCommand)command;
            ShowKeyForLoop(forCommand.directions, forCommand.loopCount, commandIndex);
            commandIndex++;
        }
        else if (type == typeof(PickIfAnyObjectExistsCommand))
        {
            var ifCommand = (PickIfAnyObjectExistsCommand)command;
            ShowKeyForIf(ifCommand.animalName);
            commandIndex++;
        }
        else if (type == typeof(PickIfAnyObjectExistsCommandTutorial))
        {
            var ifCommand = (PickIfAnyObjectExistsCommandTutorial)command;
            ShowKeyForIf(ifCommand.animalName);
            commandIndex++;
        }
        else if (type == typeof(WaitCommand))
        {
            var waitCommand = (WaitCommand)command;
            ShowKeyForWait(waitCommand.seconds);
            commandIndex++;
        }
    }
    
   
    private void ShowKeyForWait(int waitCommandSeconds)
    {
        if (panel == null)
        {
            panel = GameObject.Find("CodePanel/Scroll");
        }

        var codeInputWait = Instantiate(codeWaitObject, codeWaitObject.transform.position, Quaternion.identity);
        
        Destroy(codeInputWait.GetComponent<DeleteCommand>());
        codeInputWait.AddComponent<DeleteCommandTutorial>();
        codeInputWait.transform.Find("CodeInputArea/Wait/seconds").gameObject.GetComponent<TextMeshProUGUI>().text =
            waitCommandSeconds.ToString();

        codeInputWait.transform.parent = panel.transform;
        codeInputWait.transform.name = commandIndex.ToString();

        codeInputWait.transform.localScale = new Vector3(1f, 1f, 1f);

        codeInputsObjects.Add(codeInputWait.transform.Find("CodeInputArea/Wait").GetComponent<Image>());
    }

    public Sprite collectAndPhotoImages;
    private void ShowKeyForIf(string ifCommandAnimalName)
    {
        if (panel == null)
        {
            panel = GameObject.Find("CodePanel/Scroll");
        }

        var codeInputIf = Instantiate(codeIfObject, codeIfObject.transform.position, Quaternion.identity);
        Destroy(codeInputIf.GetComponent<DeleteCommand>());
        codeInputIf.AddComponent<DeleteCommandTutorial>();
        codeInputIf.transform.Find("ifObjectName").gameObject.GetComponent<TextMeshProUGUI>().text =
            ifCommandAnimalName;
        codeInputIf.transform.parent = panel.transform;
        codeInputIf.transform.name = commandIndex.ToString();

        codeInputIf.transform.localScale = new Vector3(1, 1, 1);

        var codeInput = Instantiate(codeIfObjectChild, codeIfObjectChild.transform.position, Quaternion.identity);

        codeInput.transform.parent = codeInputIf.transform.Find("CodeInputArea").transform;
        Destroy(codeInput.GetComponent<DeleteCommand>());

        codeInput.transform.localScale = new Vector3(1f, 1f, 1f);

        codeInputsObjects.Add(codeInputIf.transform.Find("CodeInputArea").GetComponentInChildren<Image>());

        var image = codeInput.transform.Find("Image").GetComponent<Image>();

        image.sprite = collectAndPhotoImages;
      
        GameObject.Find("CodePanel").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
    }

    private void ShowKeyForLoop(List<Direction> direction, int loopCount, int commandIndex)
    {
        if (panel == null)
        {
            panel = GameObject.Find("CodePanel/Scroll");
        }
        Destroy(tempLoopGameObject);

        GameObject codeInputFor = null;
        if (direction.Count == 1)
        {
            codeInputFor = Instantiate(codeForObject, codeForObject.transform.position, Quaternion.identity);
        }
        else if (direction.Count == 2)
        {
            codeInputFor = Instantiate(codeForObjectTwoGap, codeForObjectTwoGap.transform.position, Quaternion.identity);
        }
        else if (direction.Count == 3)
        {
            codeInputFor = Instantiate(codeForObjectTreeGap, codeForObjectTreeGap.transform.position, Quaternion.identity);
        }
        else if (direction.Count == 4)
        {
            codeInputFor = Instantiate(codeForObjectFourGap, codeForObjectFourGap.transform.position, Quaternion.identity);
        }
        Destroy(codeInputFor.GetComponent<DeleteCommand>());
        codeInputFor.AddComponent<DeleteCommandTutorial>();
        codeInputFor.transform.Find("LoopCountText").gameObject.GetComponent<TextMeshProUGUI>().text =
            loopCount.ToString();
        codeInputFor.transform.parent = panel.transform;
        codeInputFor.transform.name = commandIndex.ToString();
        codeInputFor.transform.localScale = new Vector3(1, 1, 1);

        for (int i = 0; i < direction.Count; i++)
        {
            int keyRotate = SetDirectionRotate(direction[i]);

            var codeInput = Instantiate(codeMoveObject, codeMoveObject.transform.position, Quaternion.identity);
            codeInput.transform.parent = codeInputFor.transform.Find("CodeWhole/CodeInputArea").transform;
            Destroy(codeInput.GetComponent<DeleteCommand>());

            var codeInputForRect = codeInputFor.transform.GetComponent<RectTransform>();

            codeInputForRect.sizeDelta = new Vector2(codeInputForRect.sizeDelta.x, codeInputForRect.sizeDelta.y);
            codeInput.transform.localScale = new Vector3(1f, 1f, 1f);

            codeInputsObjects.Add(codeInputFor.transform.Find("CodeWhole/CodeInputArea/CodeInputImage(Clone)/Image").GetComponentInChildren<Image>());

            var arrow = codeInput.transform.Find("Image/Arrow");
            arrow.gameObject.transform.Rotate(new Vector3(0, 0, keyRotate));
            GameObject.Find("CodePanel").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
        }

    }

    public void ShowKeyForLoopTemp(int loopCount)
    {
        Debug.Log("show for loop temp");
        if (panel == null)
        {
            panel = GameObject.Find("CodePanel/Scroll");
        }

        tempLoopGameObject = Instantiate(codeForObjectFourGap, codeForObject.transform.position, Quaternion.identity);
        tempLoopGameObject.transform.Find("LoopCountText").gameObject.GetComponent<TextMeshProUGUI>().text =
            loopCount.ToString();
        tempLoopGameObject.transform.parent = tempLoopGameObjectContainer.transform;
        tempLoopGameObject.transform.localScale = new Vector3(1, 1, 1);
        tempLoopGameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(150, 150, 1);

    }

    public void AddKeyForLoopTemp(Direction direction)
    {
        int keyRotate = SetDirectionRotate(direction);

        var codeInput = Instantiate(codeMoveObject, codeMoveObject.transform.position, Quaternion.identity);
        codeInput.transform.parent = tempLoopGameObject.transform.Find("CodeWhole/CodeInputArea").transform;
        Destroy(codeInput.GetComponent<DeleteCommand>());

        var codeInputForRect = tempLoopGameObject.transform.GetComponent<RectTransform>();

        codeInputForRect.sizeDelta = new Vector2(codeInputForRect.sizeDelta.x, codeInputForRect.sizeDelta.y);
        codeInput.transform.localScale = new Vector3(1f, 1f, 1f);

        codeInputsObjects.Add(tempLoopGameObject.transform.Find("CodeWhole/CodeInputArea/CodeInputImage(Clone)/Image").GetComponentInChildren<Image>());

        var arrow = codeInput.transform.Find("Image/Arrow");
        arrow.gameObject.transform.Rotate(new Vector3(0, 0, keyRotate));
        GameObject.Find("CodePanel").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
    }

    private void ShowKey(Direction direction, int commandIndex)
    {
        if (panel == null)
        {
            panel = GameObject.Find("CodePanel/Scroll");
        }
        int keyRotate = SetDirectionRotate(direction);

        var codeInput = Instantiate(codeMoveObject, codeMoveObject.transform.position, Quaternion.identity);
        Destroy(codeInput.GetComponent<DeleteCommand>());
        codeInput.AddComponent<DeleteCommandTutorial>();
        codeInputsObjects.Add(codeInput.transform.Find("Image").GetComponent<Image>());

        codeInput.transform.parent = panel.transform;
        codeInput.transform.localScale = new Vector3(1f, 1, 1);
        codeInput.transform.name = commandIndex.ToString();
        var arrow = codeInput.transform.Find("Image/Arrow");
        arrow.gameObject.transform.Rotate(new Vector3(0, 0, keyRotate));
        GameObject.Find("CodePanel").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
    }

    private int SetDirectionRotate(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return 180;
                break;
            case Direction.Right:
                return 0;
                break;
            case Direction.Forward:
                return 90;
                break;
            case Direction.Backward:
                return 270;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    #endregion

    public void HomeButton()
    {
        SceneManager.LoadScene(0);
    }

    public void ShowWrongCommand(int wrongCommandIndex)
    {
        codeInputsObjects[wrongCommandIndex].color = Color.red;
        if (!codePanelOpened)
        {
            CodePanelOpen();
        }
    }

    private GameObject hintObject;
    public GameObject codeReachedObject;
    
    private void DeleteHintCommandStarter()
    {
        Destroy(hintObject);
    }

    public void MarkCurrentCommand(int i)
    {
        codeInputsObjects[i].color = new Color(163 / 255, 255 / 255, 131 / 255);
        if (i != 0)
            codeInputsObjects[i - 1].color = Color.white;
    }

    public void ShowHintCommand(Direction direction)
    {
        int keyRotate = SetDirectionRotate(direction);

        hintObject = Instantiate(codeMoveObject, codeMoveObject.transform.position, Quaternion.identity);

        hintObject.transform.localScale = new Vector3(1, 1, 1);
        hintObject.transform.parent = GameObject.Find("Canvas").transform;


        Destroy(hintObject.GetComponent<DeleteCommand>());

        var arrow = hintObject.transform.Find("Image/Arrow");
        arrow.gameObject.transform.Rotate(new Vector3(0, 0, keyRotate));

        hintObject.GetComponent<RectTransform>().SetAnchor(AnchorPresets.MiddleCenter);

        hintObject.GetComponent<RectTransform>().SetPivot(PivotPresets.MiddleCenter);
        hintObject.transform.Translate(150, 0, 0);
        Invoke("DeleteHintCommandStarter", 2f);
    }
    public void ShowHintIfCommand(string ifCommandAnimalName)
    {
        hintObject = Instantiate(codeIfObject, codeIfObject.transform.position, Quaternion.identity);

        hintObject.transform.Find("ifObjectName").gameObject.GetComponent<TextMeshProUGUI>().text = ifCommandAnimalName;
        hintObject.transform.localScale = new Vector3(1, 1, 1);
        hintObject.transform.parent = GameObject.Find("Canvas").transform;

        hintObject.transform.localScale = new Vector3(1, 1, 1);

        var codeInput = Instantiate(codeIfObjectChild, codeIfObjectChild.transform.position, Quaternion.identity);

        codeInput.transform.parent = hintObject.transform.Find("CodeInputArea").transform;
        Destroy(codeInput.GetComponent<DeleteCommand>());

        codeInput.transform.localScale = new Vector3(1f, 1f, 1f);

        var image = codeInput.transform.Find("Image").GetComponent<Image>();

        hintObject.GetComponent<RectTransform>().SetAnchor(AnchorPresets.MiddleCenter);

        hintObject.GetComponent<RectTransform>().SetPivot(PivotPresets.MiddleCenter);
        Invoke("DeleteHintCommandStarter", 2f);
    }

    public void ShowHintWaitCommand(int waitCommandSeconds)
    {
        hintObject = Instantiate(codeWaitObject, codeWaitObject.transform.position, Quaternion.identity);
        hintObject.transform.Find("CodeInputArea/Wait/seconds").gameObject.GetComponent<TextMeshProUGUI>().text =
            waitCommandSeconds.ToString();
        hintObject.transform.localScale = new Vector3(1, 1, 1);
        hintObject.transform.parent = GameObject.Find("Canvas").transform;

        hintObject.GetComponent<RectTransform>().SetAnchor(AnchorPresets.MiddleCenter);
        hintObject.GetComponent<RectTransform>().SetPivot(PivotPresets.MiddleCenter);
        Invoke("DeleteHintCommandStarter", 2f);
    }

    public void ShowHintReachedTarget()
    {
        hintObject = Instantiate(codeReachedObject, codeReachedObject.transform.position, Quaternion.identity);

        hintObject.transform.localScale = new Vector3(1, 1, 1);
        hintObject.transform.parent = GameObject.Find("Canvas").transform;

        hintObject.GetComponent<RectTransform>().SetAnchor(AnchorPresets.MiddleCenter);
        hintObject.GetComponent<RectTransform>().SetPivot(PivotPresets.MiddleCenter);
        Invoke("DeleteHintCommandStarter", 2f);
    }
}