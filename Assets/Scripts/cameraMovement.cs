using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    [Header("Tracking Settings")]
    public Vector3 offset = new Vector3(0, 10, -10); 
    public float smoothTime = 0.5f;   
    
    [Header("Zoom Settings")]
    public float minDistance = 15f;     
    public float maxDistance = 100f;    
    public float zoomPadding = 1.2f;    
    
    public float onStageYThreshold = -10f; 

    private Vector3 velocity;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = GetComponentInChildren<Camera>(); 

        if (offset == Vector3.zero)
        {
            offset = transform.position;
        }
    }

    void LateUpdate()
    {
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

            Vector3 targetPos = player.transform.position;
            /*if (player.hips != null)
            {
                targetPos = player.hips.position;
            }*/

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

        if (!boundsInitialized) return;

        Vector3 centerPoint = bounds.center;
        float spreadRadius = bounds.extents.magnitude;

        spreadRadius = Mathf.Max(spreadRadius, 4f);
        float aspect = cam.aspect;
        float vFOV = cam.fieldOfView * Mathf.Deg2Rad;
        
        float hFOV = 2f * Mathf.Atan(Mathf.Tan(vFOV / 2f) * aspect);

        float minFOVHalf = Mathf.Min(vFOV, hFOV) / 2f;

        float requiredDistance = (spreadRadius * zoomPadding) / Mathf.Sin(minFOVHalf);

        requiredDistance = Mathf.Clamp(requiredDistance, minDistance, maxDistance);


        Vector3 dirToCamera = offset.normalized;
        if (dirToCamera == Vector3.zero) dirToCamera = -Vector3.forward + Vector3.up; 

        Vector3 finalCamPos = centerPoint + dirToCamera * requiredDistance;

        transform.position = Vector3.SmoothDamp(transform.position, finalCamPos, ref velocity, smoothTime);
    }
}
