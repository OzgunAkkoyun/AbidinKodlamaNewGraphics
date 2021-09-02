using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DapperDino.UDCT.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "CloseGivenTutorial", menuName = "Utilities/DeveloperConsole/Commands/CloseGivenTutorial")]
    public class CloseGivenTutorial : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            string logText = string.Join(" ", args);
            
            PlayerPrefs.SetString("openingTutorial", "finished");
            PlayerPrefs.SetString("levelTutorial" + logText, "finished");
            SceneManager.LoadScene("Level");
            return true;
        }
    }
}