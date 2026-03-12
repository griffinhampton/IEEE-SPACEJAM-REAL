using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody[] allBones;

    void Start()
    {
        // Grab the Animator and all the Rigidbodies on Gooman's bones
        animator = GetComponentInChildren<Animator>();
        allBones = GetComponentsInChildren<Rigidbody>();

        // Start the game with Gooman awake and animated
        WakeUp();
    }

    // Call this when you want Gooman to walk normally
    public void WakeUp()
    {
        animator.enabled = true; // Turn the animations ON

        foreach (Rigidbody bone in allBones)
        {
            bone.isKinematic = false; // Turn physics OFF
        }
    }

    // Call this when Gooman gets hit!
    public void KnockOut()
    {
        animator.enabled = false; // Turn the animations OFF

        foreach (Rigidbody bone in allBones)
        {
            bone.isKinematic = false; // Turn physics ON
        }
    }
}