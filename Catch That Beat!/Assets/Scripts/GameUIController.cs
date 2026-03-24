using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private Text timerText;
    [SerializeField] private Text catchesText;
    [SerializeField] private Text stateText;
    [SerializeField] private GameObject endPanel;

    private void OnEnable()
    {
        if (RoundManager.Instance == null)
        {
            return;
        }

        RoundManager.Instance.OnTimerUpdated += HandleTimerUpdated;
        RoundManager.Instance.OnCatchesUpdated += HandleCatchesUpdated;
        RoundManager.Instance.OnGameStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        if (RoundManager.Instance == null)
        {
            return;
        }

        RoundManager.Instance.OnTimerUpdated -= HandleTimerUpdated;
        RoundManager.Instance.OnCatchesUpdated -= HandleCatchesUpdated;
        RoundManager.Instance.OnGameStateChanged -= HandleStateChanged;
    }

    private void HandleTimerUpdated(float seconds)
    {
        if (timerText != null)
        {
            timerText.text = $"Time: {seconds:0.0}s";
        }
    }

    private void HandleCatchesUpdated(int catches, int target)
    {
        if (catchesText != null)
        {
            catchesText.text = $"Catches: {catches}/{target}";
        }
    }

    private void HandleStateChanged(string state)
    {
        if (stateText != null)
        {
            stateText.text = state;
        }

        if (endPanel != null)
        {
            bool showEnd = state == "Win" || state == "Lose";
            endPanel.SetActive(showEnd);
        }
    }

    public void RestartGame()
    {
        RoundManager.Instance?.StartNewGame();
    }
}
