using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioTrackerFeedback : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float maxHearingDistance = 30f;
    [SerializeField] private float minPitch = 0.92f;
    [SerializeField] private float maxPitch = 1.22f;
    [SerializeField] private bool useOcclusionRaycast = true;
    [SerializeField] private LayerMask occlusionLayers;
    [SerializeField] private float occludedLowPass = 900f;
    [SerializeField] private float clearLowPass = 22000f;

    private AudioSource audioSource;
    private AudioLowPassFilter lowPassFilter;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        lowPassFilter = GetComponent<AudioLowPassFilter>();
        if (lowPassFilter == null)
        {
            lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
        }
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
        }
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        float t = 1f - Mathf.Clamp01(distance / maxHearingDistance);

        audioSource.volume = Mathf.Lerp(0.15f, 1f, t);
        audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);

        if (!useOcclusionRaycast)
        {
            lowPassFilter.cutoffFrequency = clearLowPass;
            return;
        }

        Vector3 dir = (player.position - transform.position).normalized;
        bool blocked = Physics.Raycast(transform.position, dir, distance, occlusionLayers);
        lowPassFilter.cutoffFrequency = blocked ? occludedLowPass : clearLowPass;
    }
}
