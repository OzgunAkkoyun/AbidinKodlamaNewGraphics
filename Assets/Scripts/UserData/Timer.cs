using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public static float StartTime;
    public float Duration;

    public Timer()
    {
        StartTime = Time.realtimeSinceStartup;
        Duration = 0;
    }

    public void Finish()
    {
        var endTime = Time.realtimeSinceStartup;
        Duration = endTime - StartTime;
    }
}
