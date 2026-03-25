using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] TMP_Text countdownText;
    [SerializeField] float timeRemaining;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;

        int minutes= Mathf.FloorToInt(timeRemaining/60);

        int seconds = Mathf.FloorToInt(timeRemaining%60);

        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
