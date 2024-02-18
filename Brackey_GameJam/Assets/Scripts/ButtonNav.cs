using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonNav : MonoBehaviour
{
    public GameObject ExitPanel;

    public enum FloorName
    {
        //isi nama scene nya
        MainMenu,
        StoryStart,
        AHMAD_TES,
        
    };

    public void toGame()
    {
        SceneManager.LoadScene(FloorName.AHMAD_TES.ToString());
    }

    public void toStory()
    {
        SceneManager.LoadScene(FloorName.StoryStart.ToString());
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(FloorName.MainMenu.ToString());
    }
    
    public void GameExit()
    {
        Application.Quit();
    }

    public void OpenExitPanel()
    {
        ExitPanel.transform.GetChild(0).localScale = new Vector3(0, 0, 0);
        ExitPanel.SetActive(true);
        LeanTween.scale(ExitPanel.transform.GetChild(0).gameObject, new Vector3(1, 1, 1), 0.5f);
        
    }

    public void CloseExitPanel()
    {
        LeanTween.scale(ExitPanel.transform.GetChild(0).gameObject, new Vector3(1.1f, 1.1f, 1.1f), 0.15f).setOnComplete(() =>
        {
            LeanTween.scale(ExitPanel.transform.GetChild(0).gameObject, new Vector3(0, 0, 0), 0.50f).setOnComplete(() =>
            {
                ExitPanel.SetActive(false);
            });
        });
        
        
    }
}
