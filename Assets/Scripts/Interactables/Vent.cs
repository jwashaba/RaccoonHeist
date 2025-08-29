using UnityEngine;

public class Vent : Interactable
{
    public GameObject pairedVent;

    public override void Interact()
    {
        player.position = pairedVent.transform.position;
    }
}
