using UnityEngine;

public class MenuCursorHandler : MonoBehaviour
{
    void Start()
    {
        // Force cursor to be visible and usable in win/lose scenes
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Also make sure the game is not paused
        Time.timeScale = 1f;

        Debug.Log("MenuCursorHandler: Cursor unlocked for menu scene");
    }
}
