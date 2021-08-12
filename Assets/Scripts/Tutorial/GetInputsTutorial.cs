using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GetInputsTutorial : MonoBehaviour
{
    public UIInputManagerTutorial uh;

    public bool waitingMoveCommand;

    public int forLoopCount;
    public List<int> seconds = new List<int>();

    public Commander commander;
    public TutorialManager tm;
    public DeleteCommandTutorial deleteCommand;

    public List<Direction> forDirections;

    private bool isFirstCommand;
    
    private List<Vector2> touchList = new List<Vector2>();
    public AllToys toolList;

    public bool canUseToy { get; private set; }

    bool buttonPressed;
    int touchCountCorrect = 3;
    public GameObject toyPanel;

    private float rotationSensivity = 5;
    public float maxAngle;
    public float rotationAngle;
    public float singleObjectAngle = 72;
    public bool canUseRotate = false;
    private List<Touch> touchListforRotate = new List<Touch>();
    public List<GameObject> rotateObjects = new List<GameObject>();
    public GameObject[] rotatableObjectsContainers = new GameObject[3];
    private int selectedObjectIndex = 0;

    public UnityEvent NewCommand = new UnityEvent();

    void Start()
    {
        SoundController.instance.PrepareSounds();
    }
    public void SetRotateObjects(int index)
    {
        selectedObjectIndex = 0;
        rotationAngle = 0;
        rotateObjects.Clear();
        var rotateObjectsTemp = rotatableObjectsContainers[index].transform.Find("Container").transform;
        for (int i = 0; i < rotateObjectsTemp.childCount; i++)
        {
            rotateObjects.Add(rotateObjectsTemp.GetChild(i).gameObject);
        }

        maxAngle = singleObjectAngle * rotateObjects.Count;
    }
    public void TryToy()
    {
        touchList.Clear();
        touchList.Add(new Vector2(1019.2f, 333.6f));
        touchList.Add(new Vector2(957.3f, 558.3f));
        touchList.Add(new Vector2(796.4f, 393.5f));

        var p1 = new Vector2(touchList[0].x, touchList[0].y);
        var p2 = new Vector2(touchList[1].x, touchList[1].y);
        var p3 = new Vector2(touchList[2].x, touchList[2].y);

        string toolName = toolList.IdentifyTool(p1, p2, p3);

        Vector2 center = (p1 + p2 + p3) / 3.0f;
        UseTool(toolName, center);
    }
    void Update()
    {
#if UNITY_EDITOR
        GetKeys();
#endif
#if UNITY_STANDALONE_WIN
        GetKeys();
#endif
#if UNITY_IOS
        HandleUserInput();
#endif
#if UNITY_ANDROID
      HandleUserInput();
#endif

    }

    private void HandleUserInput()
    {
        float scwidth = Screen.width;
        float scheight = Screen.height;
        int ct = Input.touches.Length;

        if (ct == 0 || ct == 1)
        {
            if (canUseRotate == true)
            {
                rotateObjects[selectedObjectIndex].GetComponent<Button>().onClick.Invoke();
            }
            canUseToy = true;
            canUseRotate = false;
        }
        if (ct == touchCountCorrect)
        {
            touchList.Clear();

            for (int i = touchCountCorrect - 3; i < Input.touches.Length; i++)
            {
                TouchPhase phase = Input.touches[i].phase;

                if (phase != TouchPhase.Ended)
                {
                    touchList.Add(new Vector2(Input.touches[i].position.x, Input.touches[i].position.y));
                }
            }
        }

        if (ct == touchCountCorrect && canUseToy)
        {
            canUseToy = false;

            Vector2 p1, p2, p3;

            p1 = new Vector2(touchList[0].x, touchList[0].y);
            p2 = new Vector2(touchList[1].x, touchList[1].y);
            p3 = new Vector2(touchList[2].x, touchList[2].y);

            string toolName = toolList.IdentifyTool(p1, p2, p3);
            
            Vector2 center = (p1 + p2 + p3) / 3.0f;
            UseTool(toolName, center);
        }

        if (canUseRotate)
        {
            ToyRotate();
        }
    }
    void ToyRotate()
    {
        touchListforRotate = Input.touches.ToList();
        var prevPos1 =
            touchListforRotate[0].position -
            touchListforRotate[0].deltaPosition; // Generate previous frame's finger positions
        var prevPos2 = touchListforRotate[1].position - touchListforRotate[1].deltaPosition;

        var prevDir = prevPos2 - prevPos1;
        var currDir = touchListforRotate[1].position - touchListforRotate[0].position;

        var angle = Vector2.Angle(prevDir, currDir);

        var a = Vector3.Cross(prevDir.ToVector3(), currDir.ToVector3());

        rotationAngle += a.z > 0 ? -angle * rotationSensivity : angle * rotationSensivity;

        rotationAngle = rotationAngle > maxAngle ? 0 : rotationAngle;

        rotationAngle = rotationAngle < 0 ? maxAngle : rotationAngle;

        RotationToySelect(rotationAngle);
    }

    private void RotationToySelect(float angle)
    {
        selectedObjectIndex = (int)Mathf.Floor(angle / singleObjectAngle);
        rotateObjects[selectedObjectIndex].GetComponent<Image>().color = Color.red;

        foreach (var item in rotateObjects.Where((v, index) => index != selectedObjectIndex).ToList())
        {
            item.GetComponent<Image>().color = Color.white;
        }

    }

    private void UseTool(string toolName, Vector2 toolPosition)
    {
        SoundController.instance.PlayToySound();
        var action = tm.currentAction as TutorialToy;
        var actionLoop = tm.currentAction as TutorialLoopToy;

        if (!actionLoop.CheckIsNull())
        {
            if (tm.forToyCliked)
            {
                if (toolName == "Forward")
                {
                    commander.AddMoveCommand(Direction.Forward);
                }
                else if (toolName == "Left")
                {
                    commander.AddMoveCommand(Direction.Left);
                }
                else if (toolName == "Right")
                {
                    commander.AddMoveCommand(Direction.Right);
                }
                else if (toolName == "Backward")
                {
                    commander.AddMoveCommand(Direction.Backward);
                }
            }
            else if (toolName == "Loop")
            {
                forDirections = new List<Direction>();
                waitingMoveCommand = true;
                SetRotateObjects(0);
                canUseRotate = true;
                WaitForInput();
            }
        }

        if (!action.CheckIsNull())
        {
            if (toolName == "Forward")
            {
                if (action.toyName == "Forward")
                {
                    if (!waitingMoveCommand)
                    {
                        commander.AddMoveCommand(Direction.Forward);
                    }
                    else
                    {
                        forDirections.Add(Direction.Forward);
                    }
                }
                else
                {
                    tm.audioSource.Play();
                }
            }
            else if (toolName == "Left")
            {
                if (action.toyName == "Left")
                {
                    if (!waitingMoveCommand)
                    {
                        commander.AddMoveCommand(Direction.Left);
                    }
                    else
                    {
                        forDirections.Add(Direction.Left);
                    }
                }
                else
                {
                    tm.audioSource.Play();
                }
            }
            else if (toolName == "Right")
            {
                if (action.toyName == "Right")
                {
                    if (!waitingMoveCommand)
                    {
                        commander.AddMoveCommand(Direction.Right);
                    }
                    else
                    {
                        forDirections.Add(Direction.Right);
                    }
                }
                else
                {
                    tm.audioSource.Play();
                }
            }
            else if (toolName == "Backward")
            {
                if (action.toyName == "Backward")
                {
                    if (!waitingMoveCommand)
                    {
                        commander.AddMoveCommand(Direction.Backward);
                    }
                    else
                    {
                        forDirections.Add(Direction.Backward);
                    }
                }
                else
                {
                    tm.audioSource.Play();
                }
            }

            else if (toolName == "Condition" )
            {
                if (action.toyName == "Condition")
                {
                    SetRotateObjects(1);
                    canUseRotate = true;
                    OpenIfObjectWheel();
                }
                else
                {
                    tm.audioSource.Play();
                }
            }
            else if (toolName == "Wait" )
            {
                if (action.toyName == "Wait")
                {
                    SetRotateObjects(2);
                    canUseRotate = true;
                    WaitWaitInput();
                }
                else
                {
                    tm.audioSource.Play();
                }
            }
            else if (toolName == "Play")
            {
                if (action.toyName == "Play")
                {
                    tm.tutorialPanel.SetActive(false);
                    StartCoroutine(tm.DoApplyCommands());
                }
                else
                {
                    tm.audioSource.Play();
                }
            }
        }
    }

    private void OpenIfObjectWheel()
    {
        uh.ifInput.gameObject.SetActive(true);
    }

    public void CompleteLoopButton()
    {
        if (commander != null)
        {
            commander.AddForCommand(forDirections, forLoopCount);
            uh.forComplatePanel.SetActive(false);
        }
        waitingMoveCommand = false;
    }

    public void GetKeys()
    {
        var action = tm.currentAction as TutorialToy;
        var actionLoop = tm.currentAction as TutorialLoopToy;
        if (!actionLoop.CheckIsNull())
        {
            if (tm.forToyCliked)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    commander.AddMoveCommand(Direction.Forward);
                    deleteCommand.OnBackButton();
                    forDirections.Add(Direction.Forward);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    commander.AddMoveCommand(Direction.Left);
                    deleteCommand.OnBackButton();
                    forDirections.Add(Direction.Left);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    commander.AddMoveCommand(Direction.Right);
                    deleteCommand.OnBackButton();
                    forDirections.Add(Direction.Right);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    commander.AddMoveCommand(Direction.Backward);
                    deleteCommand.OnBackButton();
                    forDirections.Add(Direction.Backward);
                }
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                forDirections = new List<Direction>();
                waitingMoveCommand = true;
                WaitForInput();
            }
        }
        
        if (!action.CheckIsNull())
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (action.toyName == "Forward")
                {
                    if (!waitingMoveCommand)
                    {
                        commander.AddMoveCommand(Direction.Forward);
                    }
                    else
                    {
                        forDirections.Add(Direction.Forward);
                    }
                }
                else
                {
                    tm.audioSource.Play();
                }
                
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (action.toyName == "Left")
                {
                    if (!waitingMoveCommand)
                    {
                        commander.AddMoveCommand(Direction.Left);
                    }
                    else
                    {
                        forDirections.Add(Direction.Left);
                    }
                }
                else
                {
                    tm.audioSource.Play();
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (action.toyName == "Right")
                {
                    if (!waitingMoveCommand)
                    {
                        commander.AddMoveCommand(Direction.Right);
                    }
                    else
                    {
                        forDirections.Add(Direction.Right);
                    }
                }
                else
                {
                    tm.audioSource.Play();
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (action.toyName == "Backward")
                {
                    if (!waitingMoveCommand)
                    {
                        commander.AddMoveCommand(Direction.Backward);
                    }
                    else
                    {
                        forDirections.Add(Direction.Backward);
                    }
                }
                else
                {
                    tm.audioSource.Play();
                }
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                if (commander != null) commander.AddForCommand(forDirections, forLoopCount);
                waitingMoveCommand = false;
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                //rotateToyUi.OpenIfObjectContainer();
                if (action.toyName == "Condition")
                {
                    OpenIfObjectWheel();
                }
                else
                {
                    tm.audioSource.Play();
                }
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                //rotateToyUi.OpenIfObjectContainer();
                if (action.toyName == "Wait")
                {
                    WaitWaitInput();
                }
                else
                {
                    tm.audioSource.Play();
                }
            }
            else if ((Input.GetKeyDown(KeyCode.T)))
            {
                if (action.toyName == "Play")
                {
                    tm.tutorialPanel.SetActive(false);
                    StartCoroutine(tm.DoApplyCommands());
                }
                else
                {
                    tm.audioSource.Play();
                }
            }
        }
       
    }

    private void WaitWaitInput()
    {
        uh.waitInput.gameObject.SetActive(true);
    }

    public void WaitListener(string count)
    {
        var waitCount = int.Parse(count);
        commander.AddWaitCommand(waitCount);
        seconds.Add(waitCount);
        uh.waitInput.gameObject.SetActive(false);
    }

    public void GetIfInput(string animalName)
    {
        commander.AddIfCommandTutorial(animalName);
        uh.ifInput.gameObject.SetActive(false);
    }

    private void WaitForInput()
    {
        uh.forInput.gameObject.SetActive(true);
    }

    public void SetForLoopCount(string count)
    {
        forLoopCount = int.Parse(count);
        uh.forInput.gameObject.SetActive(false);
        uh.forComplatePanel.SetActive(true);
        tm.forToyCliked = true;
    }

    
}
