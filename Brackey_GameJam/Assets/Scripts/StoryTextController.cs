using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryTextController : MonoBehaviour
{
    public TMP_Text uiText;
    public StoryText[] textList;
    

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TextStart());
        AudioManager.instance.StopAllandPlay("BGM_02");
    }

    public IEnumerator TextStart()
    {
        foreach (StoryText text in textList)
        {
            Debug.Log(text.text);
            LeanTween.value(0, 254, text.fadeInTime).setOnUpdate((float val) =>
            {
                uiText.text = text.text;
                uiText.color = new Color32(255, 255, 255, (byte)val);
                //Debug.Log((byte)val);
            }).setOnComplete(()=> 
            {
                LeanTween.pauseAll();
            }
            );
            Debug.Log("Start");
            yield return new WaitForSeconds(text.fadeInTime + text.textTime);
            LeanTween.value(255, 0, text.fadeOutTime).setOnUpdate((float val) =>
            {
                uiText.text = text.text;
                uiText.color = new Color32(255, 255, 255, (byte)val);
                Debug.Log((byte)val);
            });
            LeanTween.resumeAll();
            yield return new WaitForSeconds(text.fadeOutTime);
            Debug.Log("Complete");
        }
        Skip();
    }
    
    public void Skip()
    {
        ButtonNav button = FindFirstObjectByType<ButtonNav>();
        button.toGame();
    }
    
}
