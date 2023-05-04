using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Create tutorial data")]
public class TutorialData : ScriptableObject
{
    public string description;
    public VideoClip video;
}
