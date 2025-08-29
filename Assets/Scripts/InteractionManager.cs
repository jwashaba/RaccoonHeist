using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    private int _lastConsumedFrame = -1;
    
    /*Reference player states, so values within it can
    be adjusted through the functions here*/
    public PlayerStates playerStates;

    // called within interactions to only allow the first interact() call in a single frame to run
    // made as a counter against vents teleporting you back instantly
    public bool TryConsumeInteractPress()
    {
        if (_lastConsumedFrame == Time.frameCount) return false;
        _lastConsumedFrame = Time.frameCount;
        return true;
    }
    
    // Create functions for different types of interactables
    public void door()
    {
        return;
    }
    public void biscuit()
    {
        playerStates.biscuitsAte++;
    }

}
