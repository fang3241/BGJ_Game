using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryText
{
    [TextArea]
    public string text;
    public float textTime;
    public float fadeInTime;
    public float fadeOutTime;
}
