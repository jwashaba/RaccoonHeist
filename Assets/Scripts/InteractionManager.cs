using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    private int _lastConsumedFrame = -1;

    /*Reference player states, so values within it can
    be adjusted through the functions here*/
    public PlayerStates playerStates;

    public GameObject pawHolder;
    public SpriteRenderer pawRenderer;

    // called within interactions to only allow the first interact() call in a single frame to run
    // made as a counter against vents teleporting you back instantly
    public bool TryConsumeInteractPress()
    {
        if (_lastConsumedFrame == Time.frameCount) return false;
        _lastConsumedFrame = Time.frameCount;
        return true;
    }

    public void EnablePawIcon()
    {
        pawHolder.SetActive(true);
    }

    public void DisablePawIcon()
    {
        pawHolder.SetActive(false);        
    }

    public void SetPawIcon(float n)
    {
        pawRenderer.size = new Vector2(0.53f, n * 0.61f);
        pawRenderer.transform.localPosition = new Vector3(0f, (-0.61f + pawRenderer.size.y) / 2f, 0f);
    }
    
    // Create functions for different types of interactables
    public void bench()
    {
        playerStates.hiddenState = true;
        Debug.Log("You're Hidden");
    }
    public void leaveBench()
    {
        playerStates.hiddenState = false;
        Debug.Log("You Left");
    }
    public void biscuit()
    {
        playerStates.biscuitsAte++;
    }

}
