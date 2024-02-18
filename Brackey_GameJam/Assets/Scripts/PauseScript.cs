using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public GameObject PausePanel;
    public bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseOrUnpause();
        }
    }

    public void PauseOrUnpause()
    {
        if (!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0;
        }
        else
        {
            isPaused = false;
            Time.timeScale = 1;
        }
        PausePanel.SetActive(isPaused);
    }

    public void Restart()
    {
        ButtonNav btn = FindAnyObjectByType<ButtonNav>();
        Time.timeScale = 1;
        btn.toGame();
    }

    public void BacktoMenu()
    {
        ButtonNav btn = FindAnyObjectByType<ButtonNav>();
        Time.timeScale = 1;
        btn.ToMenu();
    }
}
