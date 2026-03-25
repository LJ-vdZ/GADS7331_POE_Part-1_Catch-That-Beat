using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIButtonWanderer : MonoBehaviour
{
    [Header("Movement Shape")]
    [Tooltip("Maximum anchored offset from the starting position (pixels).")]
    [SerializeField] private Vector2 maxOffset = new Vector2(80f, 40f);

    [Tooltip("Optional: clamp wandering inside this RectTransform (defaults to parent).")]
    [SerializeField] private RectTransform boundary;

    [Header("Timing (Not Too Fast)")]
    [Tooltip("Minimum wait time before choosing the next random position (seconds).")]
    [SerializeField] private float minDelaySeconds = 0.6f;
    [Tooltip("Maximum wait time before choosing the next random position (seconds).")]
    [SerializeField] private float maxDelaySeconds = 1.8f;

    [Tooltip("How long each move takes (seconds).")]
    [SerializeField] private float moveDurationSeconds = 0.35f;

    [Header("Behavior")]
    [SerializeField] private bool useUnscaledTime = false;
    [SerializeField] private bool wanderOnEnable = true;
    [SerializeField] private bool stopWhenDisabled = true;

    private RectTransform rectTransform;
    private Vector2 startAnchoredPosition;
    private Coroutine routine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startAnchoredPosition = rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        if (!wanderOnEnable)
        {
            return;
        }

        if (boundary == null)
        {
            boundary = rectTransform.parent as RectTransform;
        }

        routine = StartCoroutine(WanderRoutine());
    }

    private void OnDisable()
    {
        if (stopWhenDisabled && routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            float delay = Random.Range(minDelaySeconds, maxDelaySeconds);
            if (useUnscaledTime)
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            else
            {
                yield return new WaitForSeconds(delay);
            }

            Vector2 randomOffset = new Vector2(
                Random.Range(-maxOffset.x, maxOffset.x),
                Random.Range(-maxOffset.y, maxOffset.y)
            );

            Vector2 target = startAnchoredPosition + randomOffset;
            if (boundary != null)
            {
                target = ClampToBoundary(target);
            }

            yield return MoveToTarget(target);
        }
    }

    private Vector2 ClampToBoundary(Vector2 candidate)
    {
        // Approximate clamp assuming typical centered pivots.
        float halfWidth = boundary.rect.width * 0.5f;
        float halfHeight = boundary.rect.height * 0.5f;
        float buttonHalfWidth = rectTransform.rect.width * 0.5f;
        float buttonHalfHeight = rectTransform.rect.height * 0.5f;

        float minX = -halfWidth + buttonHalfWidth;
        float maxX = halfWidth - buttonHalfWidth;
        float minY = -halfHeight + buttonHalfHeight;
        float maxY = halfHeight - buttonHalfHeight;

        return new Vector2(
            Mathf.Clamp(candidate.x, minX, maxX),
            Mathf.Clamp(candidate.y, minY, maxY)
        );
    }

    private IEnumerator MoveToTarget(Vector2 target)
    {
        float elapsed = 0f;
        Vector2 from = rectTransform.anchoredPosition;

        while (elapsed < moveDurationSeconds)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float t = moveDurationSeconds <= 0.0001f ? 1f : Mathf.Clamp01(elapsed / moveDurationSeconds);
            rectTransform.anchoredPosition = Vector2.Lerp(from, target, t);
            yield return null;
        }

        rectTransform.anchoredPosition = target;
    }
}

