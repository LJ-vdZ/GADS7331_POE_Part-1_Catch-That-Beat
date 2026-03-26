using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio; // if you use AudioMixer, otherwise use AudioSource

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Round Settings")]
    public int currentRound = 1;           // This will now persist correctly
    public int maxRounds = 3;

    [Header("Timers in seconds")]
    public float[] roundTimes = { 90f, 60f, 30f }; // Round 1: 90s, Round 2: 60s, Round 3: 30s

    [Header("Music Clips - Assign in order (0 = Round 1)")]
    public AudioClip[] roundMusicClips;

    [Header("Scenes")]
    public string winSceneName = "WinScene";
    public string loseSceneName = "GameOver";     // Change if your lose scene has a different name

    private CountdownTimer countdownTimer;
    private static bool isFirstLoad = true;       // Helps handle first scene load

    private void Awake()
    {
        // Proper singleton for scene reloading
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);        // ? This is the key fix
        }
        else if (Instance != this)
        {
            Destroy(gameObject);                  // Destroy duplicate
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-find essential objects after scene reload
        countdownTimer = FindObjectOfType<CountdownTimer>();

        if (isFirstLoad)
        {
            isFirstLoad = false;
            return;
        }

        // Start the new round after reload
        Invoke(nameof(StartNewRound), 0.1f);   // Small delay to let objects initialize
    }

    private void Start()
    {
        if (isFirstLoad)
        {
            countdownTimer = FindObjectOfType<CountdownTimer>();
            StartNewRound();
        }
    }

    public void StartNewRound()
    {
        if (currentRound > maxRounds)
        {
            WinGame();
            return;
        }

        float thisRoundTime = roundTimes[currentRound - 1];

        if (countdownTimer != null)
            countdownTimer.StartTimer(thisRoundTime);

        PlayRoundMusic();

        Debug.Log($"=== ROUND {currentRound} STARTED === Time: {thisRoundTime}s");
    }

    private void PlayRoundMusic()
    {
        DroidAIController droid = FindObjectOfType<DroidAIController>();
        if (droid != null && roundMusicClips != null && roundMusicClips.Length >= currentRound)
        {
            AudioSource audioSrc = droid.GetComponent<AudioSource>();
            if (audioSrc != null)
            {
                audioSrc.Stop();                    // Stop previous music
                audioSrc.clip = roundMusicClips[currentRound - 1];
                audioSrc.loop = true;
                audioSrc.Play();
                Debug.Log($"Playing music for Round {currentRound}");
            }
        }
    }

    public void CatchDroid()
    {
        if (countdownTimer != null)
            countdownTimer.StopTimer();

        DroidAIController droid = FindObjectOfType<DroidAIController>();
        if (droid != null)
            droid.OnCaughtAndEscalate();

        currentRound++;

        if (currentRound > maxRounds)
            WinGame();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TimeRanOut()
    {
        LoseGame();
    }

    private void WinGame()
    {
        Debug.Log("All rounds completed ? Win Scene");
        SceneManager.LoadScene(winSceneName);
    }

    private void LoseGame()
    {
        Debug.Log("Time ran out ? Game Over");
        SceneManager.LoadScene(loseSceneName);
    }

    public int GetCurrentRound() => currentRound;
}
