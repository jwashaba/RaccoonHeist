using UnityEngine;

public class Interactable : MonoBehaviour
{
    // Note: This is just gonna be a basis for the interactables made later
    // Make a default area value for interactables that can be adjusted 
    // Make a reference to the pos of the player
    // Make a reference to the interaction manager
    public float area = 3f; // Declare default Area (Area of Interaction)
    public Transform player;
    public InteractionManager interactionManager;

    // Make variable if obj has been interacted with
    bool hasInteracted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        return;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= area)
        {
            if(Input.GetKeyUp(KeyCode.E)) 
            {
                hasInteracted = true;
                Interact();
            }
        }
    }

    // Define Interaction and call function from interaction manager
    // In this case, this obj will be a biscuit
    public virtual void Interact()
    {
        Debug.Log("You ate a biscuit");
        interactionManager.biscuit();
    }

     // Make function for color of area 
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, area);
    }
}
