using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DapperDino.UDCT.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "Remove Command", menuName = "Utilities/DeveloperConsole/Commands/Remove Command")]
    public class RemoveByName : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            string logText = string.Join(" ", args);

            //Debug.Log(logText);
            var allObjects = GameObject.FindObjectsOfType<GameObject>();
            foreach (var objects in allObjects)
            {
                if (objects.name == logText)
                {
                    Destroy(objects);
                }
            }

            return true;
        }
    }
}