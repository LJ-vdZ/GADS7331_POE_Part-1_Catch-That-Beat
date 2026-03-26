using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio; // if you use AudioMixer, otherwise use AudioSource

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Round Settings")]
    public int currentRound = 1;
    public int maxRounds = 3;

    [Header("Timers in seconds")]
    public float[] roundTimes = { 90f, 60f, 30f };

    [Header("Music Clips - Assign in order")]
    public AudioClip[] roundMusicClips;

    [Header("Scenes")]
    public string winSceneName = "WinScene";
    public string loseSceneName = "GameOver";

    private CountdownTimer countdownTimer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);        // Keep alive during rounds
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
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
        countdownTimer = FindObjectOfType<CountdownTimer>();

        // Only auto-start round if we're in the gameplay scene
        if (scene.name != winSceneName && scene.name != loseSceneName)
        {
            Invoke(nameof(StartNewRound), 0.2f);
        }
    }

    private void Start()
    {
        countdownTimer = FindObjectOfType<CountdownTimer>();
        if (countdownTimer == null)
            Debug.LogWarning("CountdownTimer not found!");
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

        Debug.Log($"=== ROUND {currentRound} STARTED | Time: {thisRoundTime}s ===");
    }

    private void PlayRoundMusic()
    {
        DroidAIController droid = FindObjectOfType<DroidAIController>();
        if (droid != null && roundMusicClips != null && roundMusicClips.Length >= currentRound)
        {
            AudioSource audioSrc = droid.GetComponent<AudioSource>();
            if (audioSrc != null)
            {
                audioSrc.Stop();
                audioSrc.clip = roundMusicClips[currentRound - 1];
                audioSrc.loop = true;
                audioSrc.Play();
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

        // Clean up everything before going to win scene
        CleanupAudio();

        // DESTROY GameManager so WinScene is completely clean
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }

        SceneManager.LoadScene(winSceneName);
    }

    private void LoseGame()
    {
        Debug.Log("Time ran out ? Game Over");

        CleanupAudio();

        // For Lose scene we can also destroy or keep it - your choice
        // Here we destroy it too for consistency
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }

        SceneManager.LoadScene(loseSceneName);
    }

    private void CleanupAudio()
    {
        AudioSource[] allSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource src in allSources)
        {
            src.Stop();
            src.clip = null;
        }
    }

    public int GetCurrentRound() => currentRound;
}

