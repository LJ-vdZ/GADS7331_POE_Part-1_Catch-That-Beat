using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;

    [HideInInspector] public float timeRemaining;   // GameManager will set this

    private bool timerActive = true;

    private void Update()
    {
        if (!timerActive || timeRemaining <= 0) return;

        timeRemaining -= Time.deltaTime;

        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timeRemaining < 0)
        {
            timerActive = false;
            // Let GameManager handle losing the round instead of directly loading GameOver
            GameManager gm = GameManager.Instance;
            if (gm != null)
                gm.TimeRanOut();
            else
                SceneManager.LoadScene("GameOver"); // fallback
        }
    }

    // Called by GameManager at the start of each round
    public void StartTimer(float newTime)
    {
        timeRemaining = newTime;
        timerActive = true;
    }

    public void StopTimer()
    {
        timerActive = false;
    }
}
