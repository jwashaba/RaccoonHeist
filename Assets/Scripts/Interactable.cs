using System.Runtime.CompilerServices;
using NUnit.Framework;
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

    // Reference biscuit case
    public GameObject bisCase;
    // Set Values for interact hold timer
    public float holdDuration = 0;

    public bool isInteractableOnlyOnce;
    
    // Make variable if obj has been interacted with
    bool hasInteracted = false;

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= area && ((!hasInteracted && isInteractableOnlyOnce) || !isInteractableOnlyOnce))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("get it");
                interactionManager.EnablePawIcon();
            }
             if (Input.GetKey(KeyCode.E) && interactionManager.TryConsumeInteractPress())
            {
                Debug.Log("work");
                if (holdDuration < 3f)
                {
                    holdDuration += Time.deltaTime;
                    interactionManager.SetPawIcon(holdDuration / 3f);
                    Debug.Log("Holding"); 
                }
                else
                {
                    hasInteracted = true;
                    Interact();
                    holdDuration = 0f;
                    interactionManager.DisablePawIcon();
                }
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                interactionManager.DisablePawIcon();
            }
        }
        if (hasInteracted == true)
        {
            if (bisCase) Destroy(bisCase);
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
