using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class MathTools
{
    public static Vector3 ToVector3XZ(this Vector2Int value)
    {
        return new Vector3(value.x, 0f, value.y);
    }

    public static Vector3 ToVector3(this Vector2 value)
    {
        return new Vector3(value.x, value.y, 0f);
    }

    public static Vector3 Vector3toXZ(this Vector3 value)
    {
        return new Vector3(value.x, 0f, value.z);
    }

    public static Vector2 Vector3ToVector2(this Vector3 value)
    {
        return new Vector2(value.x, value.y);
    }

    public static Vector3 Vector2ToVector3(this Vector2 value)
    {
        return new Vector3(value.x, 0, value.y);
    }

    public static string ListPrint<T>(this List<T> list)
    {
        var str = "";

        for (int i = 0; i < list.Count; i++)
        {
            str += list[i].ToString() + " -- ";
        }

        return str.Remove(str.Length - 4, 4);
    }

    public static string ArrayPrint(this Array list)
    {
        var str = "";

        foreach (var l in list)
        {
            str += l.ToString() + " -- ";
        }

        return str.Remove(str.Length - 4, 4);
    }

    public static bool Equal(Vector2 _v1, Vector2 _v2, Vector2 _e)
    {
        return System.Math.Abs(_v1.x - _v2.x) <= _e.x &&
               System.Math.Abs(_v1.y - _v2.y) <= _e.y;
    }
    public static bool CheckIsNull(this object obj)
    {
        bool isNull = obj == null || (obj is UnityEngine.Object && ((obj as UnityEngine.Object) == null));
        return isNull;
    }

    public static void ScrollToBottom(this ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }
    public static Vector2 SizeToParent(this RawImage image, float padding = 0)
    {
        float w = 0, h = 0;
        var parent = image.GetComponentInParent<RectTransform>();
        var imageTransform = image.GetComponent<RectTransform>();

        // check if there is something to do
        if (image.texture != null)
        {
            if (!parent) { return imageTransform.sizeDelta; } //if we don't have a parent, just return our current width;
            padding = 1 - padding;
            float ratio = image.texture.width / (float)image.texture.height;
            var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
            if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
            {
                //Invert the bounds if the image is rotated
                bounds.size = new Vector2(bounds.height, bounds.width);
            }
            //Size by height first
            h = bounds.height * padding;
            w = h * ratio;
            if (w > bounds.width * padding)
            { //If it doesn't fit, fallback to width;
                w = bounds.width * padding;
                h = w / ratio;
            }
        }
        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        return imageTransform.sizeDelta;
    }
}