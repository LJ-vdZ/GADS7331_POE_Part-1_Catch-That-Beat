using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame() 
    {
        SceneManager.LoadScene("SampleScene");


    }

    public void Retry()
    {
        // Reset time scale BEFORE loading the new scene
        Time.timeScale = 1f;

        // Reload the current active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        Debug.Log("Retry button clicked - Scene reloading with Time.timeScale = 1");
    }

    public void MainMenuButton() 
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        // Find the PlayerController in the scene
        PlayerController player = FindObjectOfType<PlayerController>();

        if (player != null)
        {
            player.ResumeGame();           // This calls your existing method in PlayerController
            Debug.Log("Resume button clicked - Game resumed");
        }
        else
        {
            Debug.LogError("PlayerController not found in the scene! Cannot resume game.");
        }
    }


}
