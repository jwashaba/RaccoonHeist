using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    /*Reference player states, so values within it can
    be adjusted through the functions here*/
    public PlayerStates playerStates;

    // Create functions for different types of interactables
    public void door()
    {

    }
    public void biscuit()
    {
        playerStates.biscuitsAte++;
    }

}
