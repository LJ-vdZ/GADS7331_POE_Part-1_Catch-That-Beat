using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DroidAIController : MonoBehaviour
{
    private enum DroidState
    {
        Roaming,
        Dashing
    }

    [Header("Targets")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] roamPoints;

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 3.2f;
    [SerializeField] private float speedGainPerCatch = 0.55f;
    [SerializeField] private float stopDistanceFromPoint = 1.0f;
    [SerializeField] private float dashDuration = 1.5f;
    [SerializeField] private float dashSpeedMultiplier = 1.8f;

    private NavMeshAgent agent;
    private DroidState currentState = DroidState.Roaming;
    private float dashTimer;
    private int catchesAgainstDroid;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = baseSpeed;
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

        PickNewRoamPoint();
    }

    private void Update()
    {
        if (roamPoints == null || roamPoints.Length == 0)
        {
            return;
        }

        switch (currentState)
        {
            case DroidState.Roaming:
                HandleRoam();
                break;
            case DroidState.Dashing:
                HandleDash();
                break;
        }
    }

    public void OnCaughtAndEscalate()
    {
        catchesAgainstDroid++;
        agent.speed = baseSpeed + catchesAgainstDroid * speedGainPerCatch;
        StartDash();
    }

    public void ResetDifficulty()
    {
        catchesAgainstDroid = 0;
        agent.speed = baseSpeed;
    }

    private void HandleRoam()
    {
        if (!agent.pathPending && agent.remainingDistance <= stopDistanceFromPoint)
        {
            PickNewRoamPoint();
        }
    }

    private void StartDash()
    {
        currentState = DroidState.Dashing;
        dashTimer = dashDuration;
        agent.speed = (baseSpeed + catchesAgainstDroid * speedGainPerCatch) * dashSpeedMultiplier;

        if (player != null)
        {
            Vector3 awayDir = (transform.position - player.position).normalized;
            Vector3 target = transform.position + awayDir * 6f;
            agent.SetDestination(target);
        }
        else
        {
            PickNewRoamPoint();
        }
    }

    private void HandleDash()
    {
        dashTimer -= Time.deltaTime;
        if (dashTimer <= 0f)
        {
            currentState = DroidState.Roaming;
            agent.speed = baseSpeed + catchesAgainstDroid * speedGainPerCatch;
            PickNewRoamPoint();
        }
    }

    private void PickNewRoamPoint()
    {
        int index = Random.Range(0, roamPoints.Length);
        agent.SetDestination(roamPoints[index].position);
    }
}
