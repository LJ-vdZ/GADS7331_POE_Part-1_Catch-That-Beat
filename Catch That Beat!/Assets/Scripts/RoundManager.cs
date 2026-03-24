using System;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    [Header("Rules")]
    [SerializeField] private int catchesToWin = 3;
    [SerializeField] private float roundTimeSeconds = 35f;

    [Header("References")]
    [SerializeField] private DroidAIController droidAI;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform droidSpawnPoint;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject droid;

    public event Action<float> OnTimerUpdated;
    public event Action<int, int> OnCatchesUpdated;
    public event Action<string> OnGameStateChanged;

    private int catches;
    private float timer;
    private bool isGameActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartNewGame();
    }

    private void Update()
    {
        if (!isGameActive)
        {
            return;
        }

        timer -= Time.deltaTime;
        OnTimerUpdated?.Invoke(Mathf.Max(0f, timer));

        if (timer <= 0f)
        {
            LoseGame();
        }
    }

    public void StartNewGame()
    {
        catches = 0;
        timer = roundTimeSeconds;
        isGameActive = true;

        if (droidAI != null)
        {
            droidAI.ResetDifficulty();
        }

        RespawnActors();

        OnCatchesUpdated?.Invoke(catches, catchesToWin);
        OnTimerUpdated?.Invoke(timer);
        OnGameStateChanged?.Invoke("Playing");
    }

    public void RegisterCatch()
    {
        if (!isGameActive)
        {
            return;
        }

        catches++;
        OnCatchesUpdated?.Invoke(catches, catchesToWin);

        if (catches >= catchesToWin)
        {
            WinGame();
            return;
        }

        timer = roundTimeSeconds;
        OnTimerUpdated?.Invoke(timer);

        if (droidAI != null)
        {
            droidAI.OnCaughtAndEscalate();
        }
    }

    private void WinGame()
    {
        isGameActive = false;
        OnGameStateChanged?.Invoke("Win");
    }

    private void LoseGame()
    {
        isGameActive = false;
        OnGameStateChanged?.Invoke("Lose");
    }

    private void RespawnActors()
    {
        if (player != null && playerSpawnPoint != null)
        {
            player.transform.SetPositionAndRotation(playerSpawnPoint.position, playerSpawnPoint.rotation);
        }

        if (droid != null && droidSpawnPoint != null)
        {
            droid.transform.SetPositionAndRotation(droidSpawnPoint.position, droidSpawnPoint.rotation);
        }
    }
}
