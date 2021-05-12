using UnityEngine;
using UnityEngine.UI;

public class HintButton : MonoBehaviour
{
  public TraceMap traceMap;
  public Button clickedButton;

    public void Hint()
    {
        clickedButton.interactable = false;
        traceMap.TraceCommands();
        Invoke("OpenHintButton",2);
    }

    void OpenHintButton()
    {
        clickedButton.interactable = true;
    }
}
