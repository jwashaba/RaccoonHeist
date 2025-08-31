using System.Collections;
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

    private int _yellowBiscuitsRemaining = 2;
    private int _tealBiscuitsRemaining = 2;
    private int _redBiscuitsRemaining = 5;
    private int _blueBiscuitsRemaining = 5;
    private int _greenBiscuitsRemaining = 3;
    private int _monaBiscuitsRemaining = 1;
    private int _totalBiscuitsRemaining = 17; // not including mona
    [SerializeField] private GameObject electricalDoor;
    [SerializeField] private Transform doorFocusTarget;
    [SerializeField] private float doorFocusSeconds = 2f;



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

    public void biscuit(Interactable.RoomColors roomColor)
    {
        playerStates.biscuitsAte++;
        _totalBiscuitsRemaining--;

        if (_totalBiscuitsRemaining == 0)
        {
            StartCoroutine(OpenElectricalDoor());
        }

        switch (roomColor)
        {
            case Interactable.RoomColors.Blue:
                _blueBiscuitsRemaining--;

                if (_blueBiscuitsRemaining == 0)
                {
                    SceneManager.Instance.BeginRoomPhoto(roomColor);
                    return;
                }

                break;
            case Interactable.RoomColors.Green:
                _greenBiscuitsRemaining--;

                if (_greenBiscuitsRemaining == 0)
                {
                    SceneManager.Instance.BeginRoomPhoto(roomColor);
                    return;
                }

                break;
            case Interactable.RoomColors.Red:
                _redBiscuitsRemaining--;

                if (_redBiscuitsRemaining == 0)
                {
                    SceneManager.Instance.BeginRoomPhoto(roomColor);
                    return;
                }

                break;
            case Interactable.RoomColors.Yellow:
                _yellowBiscuitsRemaining--;

                if (_yellowBiscuitsRemaining == 0)
                {
                    SceneManager.Instance.BeginRoomPhoto(roomColor);
                    return;
                }

                break;
            case Interactable.RoomColors.Teal:
                _tealBiscuitsRemaining--;

                if (_tealBiscuitsRemaining == 0)
                {
                    SceneManager.Instance.BeginRoomPhoto(roomColor);
                    return;
                }

                break;
            case Interactable.RoomColors.Mona:
                _monaBiscuitsRemaining--;

                if (_monaBiscuitsRemaining == 0)
                {
                    SceneManager.Instance.BeginRoomPhoto(roomColor);
                    return;
                }

                break;
        }

        SceneManager.Instance.BiscuitsHUD.BiscuitCounterPopUp(playerStates.biscuitsAte);
    }

    private IEnumerator OpenElectricalDoor()
    {
        var sm = SceneManager.Instance;
        while (sm != null && sm.IsPhotoOverlayActive)
            yield return null;
        electricalDoor.SetActive(false);
        sm?.FocusCameraForSeconds(doorFocusTarget, doorFocusSeconds);
    }
}
