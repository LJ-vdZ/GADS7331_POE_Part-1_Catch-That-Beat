using UnityEngine;

public class FixCharacterOrientation : MonoBehaviour
{
    [Header("Rotation Correction")]
    [Tooltip("The axis around which the unwanted rotation happens. Usually X for side-lying issues.")]
    public Vector3 correctionAxis = Vector3.right;   // Most common for 90° side tilt

    [Tooltip("How many degrees to rotate to fix it. +90 or -90 usually works.")]
    public float correctionAngle = 90f;

    [Tooltip("Apply the fix only once at Start, or every frame?")]
    public bool applyOnce = true;

    private bool hasApplied = false;

    void Start()
    {
        ApplyCorrection();
    }

    //private void Update()
    //{
    //    ApplyCorrection();
    //}

    void LateUpdate()
    {
        if (!applyOnce)
        {
            ApplyCorrection();
        }
    }

    private void ApplyCorrection()
    {
        if (applyOnce && hasApplied) return;

        // Option 1: Direct local rotation correction (most reliable)
        Quaternion correction = Quaternion.AngleAxis(correctionAngle, correctionAxis);
        transform.localRotation = correction * transform.localRotation;

        hasApplied = true;

        Debug.Log("Character rotation corrected by " + correctionAngle + "° around " + correctionAxis);
    }

    // Optional: Call this from another script if you need to re-apply the fix later
    public void ReApplyFix()
    {
        hasApplied = false;
        ApplyCorrection();
    }
}
