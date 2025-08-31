using UnityEngine;

public class EscapeRope : Interactable
{
    public override void Interact()
    {
        SceneManager.Instance.LoadScene("WinScene");
    }
}
