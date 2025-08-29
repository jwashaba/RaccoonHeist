using UnityEngine;

public class BenchTrigger : MonoBehaviour
{
    // Reference Interaction Manager
     public InteractionManager interactionManager;

    // Make variable if obj has been interacted with
    bool hasInteracted = false;
    // Create a function for when player enters hidden area
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("character"))
        {
            Debug.Log("You are hiding");
            Interact();
        }
    }

       void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("character"))
        {
            Debug.Log("You Left");
        }
    }
    // Define Interaction and call function from interaction manager
    public virtual void Interact()
    {
        interactionManager.bench();
    }
}
