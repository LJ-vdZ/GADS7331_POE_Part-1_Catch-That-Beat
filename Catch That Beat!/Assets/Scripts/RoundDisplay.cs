using UnityEngine;
using TMPro;

public class RoundDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text roundText;

    private void Start()
    {
        UpdateRoundDisplay();
    }

    private void Update()
    {
        // Optional: Update every frame in case you want live changes
        // UpdateRoundDisplay();
    }

    public void UpdateRoundDisplay()
    {
        if (roundText != null && GameManager.Instance != null)
        {
            int current = GameManager.Instance.GetCurrentRound();
            roundText.text = $"Round {current} / 3";
        }
    }
}
