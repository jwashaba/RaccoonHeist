using UnityEngine;
using System.Collections;

public class ElectricalBox : Interactable
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float lockSeconds = 2f;
    private bool hasPeed = false;
    [SerializeField] private GameObject vaultDoor;
    [SerializeField] private Transform doorFocusTarget;
    [SerializeField] private float doorFocusSeconds = 2f;

    public override void Interact()
    {
        if (hasPeed) return;
        StartCoroutine(PeeCoroutine());
    }

    private IEnumerator PeeCoroutine()
    {
        hasPeed = true;
        playerMovement.SetMovementToZero();
        playerMovement.enabled = false;
        if (!playerAnimator.enabled) playerAnimator.enabled = true;
        playerAnimator.Play("playerPee");
        yield return new WaitForSeconds(lockSeconds);
        StartCoroutine(OpenVaultDoor());
        playerMovement.enabled = true;
        playerAnimator.Play("Idle3");
    }

    private IEnumerator OpenVaultDoor()
    {
        var sm = SceneManager.Instance;
        yield return null;
        while (sm != null && sm.IsPhotoOverlayActive)
            yield return null;
        vaultDoor.SetActive(false);
        sm.FocusCameraForSeconds(doorFocusTarget, doorFocusSeconds);
    }
}
