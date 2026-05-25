using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour 
{
    public GameObject ModeSelection;
    private void Start()
    {
        if (ModeSelection != null) 
        { 
            ModeSelection.SetActive(false); 
        }
    }
    public void OpenSelectionPanel()
    {
        if (ModeSelection != null)
        {
            ModeSelection.SetActive(true);
        }
    }
    public void NormalMode()
    {
        PlayerPrefs.SetString("GameMode", "Normal");
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }
    public void TimerMode()
    {
        PlayerPrefs.SetString("GameMode", "Timer");
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }
    public void StopGame()
    {
        Application.Quit();
    }
}

