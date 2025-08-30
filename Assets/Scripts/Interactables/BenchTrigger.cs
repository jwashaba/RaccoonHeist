using UnityEngine;

public class BenchTrigger : MonoBehaviour
{
    // Reference Interaction Manager
    public InteractionManager interactionManager;

    // Create a function for when player enters hidden area
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("character"))
        {
            Debug.Log("You are hiding");
            Interact();
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("character"))
        {
            Interact();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("character"))
        {
            InteractTwo();
        }
    }
    // Define Interaction and call function from interaction manager
    public virtual void Interact()
    {
        interactionManager.bench();
    }
       public virtual void InteractTwo()
    {
        interactionManager.leaveBench();
    }
}
