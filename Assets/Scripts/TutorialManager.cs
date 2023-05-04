using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private List<TutorialData> dataList;
    [SerializeField] private TextMeshProUGUI desc;
    [SerializeField] private VideoPlayer videoPlayer;

    private int currentIndex = 0;

    public int CurrentIndex
    {
        get { return currentIndex; }
        set 
        { 
            currentIndex = value;
            OpenPanel(currentIndex);
        }
    }

    public void StartTutorial()
    {
        CurrentIndex = 0;
    }

    private void OpenPanel(int index)
    {
        desc.text = dataList[index].description;
        videoPlayer.clip = dataList[index].video;
    }

    public void OnNextButtonPressed()
    {
        if (currentIndex != dataList.Count - 1)
            CurrentIndex += 1;
        else CloseTutorial();
    }

    public void CloseTutorial()
    {
        gameObject.SetActive(false);
    }
}
