/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using UnityEngine;
using CodeMonkey.Utils;

namespace CodeMonkey {

    /*
     * Debug Class with various helper functions to quickly create buttons, text, etc
     * */
    public static class CMDebug {

        // Creates a Button in the World

        // Creates a World Text object at the world position
        public static void Text(string text, Vector3 localPosition = default(Vector3), Transform parent = null, int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = UtilsClass.sortingOrderDefault) {
            UtilsClass.CreateWorldText(text, parent, localPosition, fontSize, color, textAnchor, textAlignment, sortingOrder);
        }
        
        // World text pop up at mouse position
       

        // World text pop up at mouse position
        public static void TextPopupMouse(object obj, Vector3? offset = null) {
            TextPopupMouse(obj.ToString(), offset);
        }

        // Creates a Text pop up at the world position
        
        // Text Updater in World, (parent == null) = world position
        


        public static GameObject DrawSpriteTimed(Vector3 position, Sprite sprite, float scale, float timer) {
            GameObject gameObject = new GameObject("SpriteTimed", typeof(SpriteRenderer));
            gameObject.transform.position = position;
            gameObject.transform.localScale = Vector3.one * scale;
            gameObject.GetComponent<SpriteRenderer>().sprite = sprite;

            GameObject.Destroy(gameObject, timer);

            return gameObject;
        }

      


    }

}