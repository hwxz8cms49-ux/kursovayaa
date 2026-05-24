using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour 
{
   public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void StopGame()
    {
        Application.Quit();
    }
}

