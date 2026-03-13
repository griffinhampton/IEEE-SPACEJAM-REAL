using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    [Header("Tracking Settings")]
    public Vector3 offset = new Vector3(0, 10, -10); // Direction relative to players
    public float smoothTime = 0.5f;                  // Time for camera to reach target
    
    [Header("Zoom Settings")]
    public float minDistance = 15f;     // Absolute closest the camera can get
    public float maxDistance = 100f;    // Absolute furthest range (can be high)
    public float zoomPadding = 1.2f;    // Multiplier to ensure players aren't hugging the screen edge
    
    // Players falling below this Y height are considered "off-stage" and ignored
    public float onStageYThreshold = -10f; 

    private Vector3 velocity;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = GetComponentInChildren<Camera>(); // Fallback

        // If offset is zero, use the current transform position relative to origin
        if (offset == Vector3.zero)
        {
            offset = transform.position;
        }
    }

    void LateUpdate()
    {
        // Find players dynamically (handles instantiation)
        var players = FindObjectsByType<PlayInputHandler>(FindObjectsSortMode.None);
        
        if (players.Length == 0) return;

        Move(players);
    }

    void Move(PlayInputHandler[] players)
    {
        var bounds = new Bounds();
        bool boundsInitialized = false;
        int onStageCount = 0;

        foreach (var player in players)
        {
            if (player == null) continue;

            // Use hips (ragdoll center) or transform as fallback
            Vector3 targetPos = player.transform.position;
            if (player.hips != null)
            {
                targetPos = player.hips.position;
            }

            // Only consider players who are "on stage" (active area)
            if (targetPos.y < onStageYThreshold) continue;

            onStageCount++;

            if (!boundsInitialized)
            {
                bounds = new Bounds(targetPos, Vector3.zero);
                boundsInitialized = true;
            }
            else
            {
                bounds.Encapsulate(targetPos);
            }
        }

        // If no active players, just stay put or default
        if (!boundsInitialized) return;

        Vector3 centerPoint = bounds.center;

        // --- Calculate Zoom/Distance ---
        
        // Use the bounds diagonal (radius of enclosing sphere) to determine spread size
        // This works better than Width/Height for cameras at arbitrary angles
        float spreadRadius = bounds.extents.magnitude;

        // Ensure a minimum spread so we don't zoom inside a single player
        // (Roughly the size of one character)
        spreadRadius = Mathf.Max(spreadRadius, 4f);

        // Account for Aspect Ratio to handle any resolution
        // We need to fit the spread into the smallest dimension of the FOV
        float aspect = cam.aspect;
        float vFOV = cam.fieldOfView * Mathf.Deg2Rad;
        
        // Calculate Horizontal FOV based on Vertical FOV and Aspect Ratio
        float hFOV = 2f * Mathf.Atan(Mathf.Tan(vFOV / 2f) * aspect);

        // Use the narrower FOV to ensure the object always fits
        float minFOVHalf = Mathf.Min(vFOV, hFOV) / 2f;

        // Calculate required distance from center to fit the spread radius, with padding
        float requiredDistance = (spreadRadius * zoomPadding) / Mathf.Sin(minFOVHalf);

        // Clamp distance
        requiredDistance = Mathf.Clamp(requiredDistance, minDistance, maxDistance);

        // --- Execute Move ---

        // We position the camera along the offset direction, at the calculated distance
        Vector3 dirToCamera = offset.normalized;
        if (dirToCamera == Vector3.zero) dirToCamera = -Vector3.forward + Vector3.up; // Fallback

        Vector3 finalCamPos = centerPoint + dirToCamera * requiredDistance;

        // Smoothly dampen the position
        transform.position = Vector3.SmoothDamp(transform.position, finalCamPos, ref velocity, smoothTime);
    }
}
