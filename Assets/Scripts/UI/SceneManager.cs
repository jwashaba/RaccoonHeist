using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Unity.Cinemachine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject playerHUD;
    [SerializeField] public BiscuitHudController BiscuitsHUD;
    [SerializeField] private GameObject loadingMask;
    [SerializeField] private GameObject cameraScreen;

    public bool pauseScreenAccessable = false;
    public bool gameIsPaused = false;

    // HUD refs
    [SerializeField] private RectTransform detectionIndicator;
    [SerializeField] private Image detectionProgress;
    [SerializeField] private RectTransform hiddenIndicator;

    // Player state ref (exists only in Museum)
    public PlayerStates playerStates;

    // UI animation speed (higher = snappier)
    [SerializeField] private float hudLerpSpeed = 12f;

    // ===== Photo Placement UI =====
    [SerializeField] private Image darkenedBG;
    [SerializeField] private RectTransform backgroundImageFrame;

    // One Image per enum (drag in Inspector)
    [SerializeField] private Image redPhoto;
    [SerializeField] private Image bluePhoto;
    [SerializeField] private Image tealPhoto;
    [SerializeField] private Image greenPhoto;
    [SerializeField] private Image yellowPhoto;
    [SerializeField] private Image monaPhoto;

    // Speeds
    [SerializeField] private float photoLerpSpeed = 12f;
    [SerializeField] private float frameLerpSpeed = 12f;
    [SerializeField] private float fadeSpeed = 10f;

    // --- Pause Controls Modal ---
    [SerializeField] private GameObject PanelMenuImage;    // the main pause menu panel/image
    [SerializeField] private GameObject controlsScreen;    // the controls modal root

    // One visible Image that swaps sprites (array size = 3)
    [SerializeField] private UnityEngine.UI.Image controlsImage;
    [SerializeField] private Sprite[] controlSprites = new Sprite[3]; // index 0..2

    private int controlIndex = 0;


    public GameObject escapeRope;
    
    [SerializeField] private CinemachineCamera vCam;
    [SerializeField] private Transform vCamTarget;
    public bool IsPhotoOverlayActive => photoState != PhotoState.None;

    // ---- Internal state (do not touch in Inspector)
    private enum PhotoState { None, FadeInAndCenter, MoveToSlotAndBringFrame, SendOffAndFadeOut }
    private PhotoState photoState = PhotoState.None;

    private Image activePhoto;
    private Vector2 targetSlot;           // final anchoredPosition for the photo
    private Color _darkColor;           // working color cache

    // Where the frame ends up when leaving (screen width = 1920)
    private readonly Vector2 frameOffRight = new Vector2(1925f, 0f);

    // Convenience: exponential time-based lerp factor
    private static float T(float speed) => 1f - Mathf.Exp(-speed * Time.unscaledDeltaTime);


    void Update()
    {
        // Photo overlay runs even when pause toggling is disabled
        if (photoState != PhotoState.None)
            PhotoOverlayUpdate();

        if (!pauseScreenAccessable) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetGamePaused(!gameIsPaused);
        }

        // VISUAL LERP UPDATES FOR HUD ELEMENTS
        UpdateHud();
    }

    private void AcquirePlayerStates()
    {
        // Try common patterns: tag, name, or fallback
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) playerObj = GameObject.Find("Player");
        if (playerObj == null)
        {
            playerStates = FindObjectOfType<PlayerStates>(); // last resort
        }
        else
        {
            playerStates = playerObj.GetComponent<PlayerStates>();
        }
    }

    private void UpdateHud()
    {
        // Attempt to reacquire once we’re in Museum and not yet found
        if (playerStates == null && pauseScreenAccessable)
            AcquirePlayerStates();

        // Nothing to do if we still don't have states or UI refs aren't set
        if (playerStates == null) return;

        // Use unscaled time so UI animates even if Time.timeScale = 0
        float t = 1f - Mathf.Exp(-hudLerpSpeed * Time.unscaledDeltaTime); // time-based lerp factor

        // ----- Detection indicator -----
        float detectTargetY = (playerStates.detection > 0f) ? 0f : -555f;

        if (detectionIndicator != null)
        {
            var p = detectionIndicator.anchoredPosition;
            p.y = Mathf.Lerp(p.y, detectTargetY, t);
            detectionIndicator.anchoredPosition = p;
        }

        if (detectionProgress != null)
        {
            detectionProgress.fillAmount = Mathf.Clamp01(playerStates.detection);
        }

        // ----- Hidden indicator -----
        float hiddenTargetY = (playerStates.hiddenState && playerStates.detection <= 0f) ? 0f : -555f;

        if (hiddenIndicator != null)
        {
            var h = hiddenIndicator.anchoredPosition;
            h.y = Mathf.Lerp(h.y, hiddenTargetY, t);
            hiddenIndicator.anchoredPosition = h;
        }
    }

    private void PhotoOverlayUpdate()
    {
        // Safety
        if (activePhoto == null || darkenedBG == null || backgroundImageFrame == null)
            return;

        var pRT = activePhoto.rectTransform;
        var fRT = backgroundImageFrame;

        switch (photoState)
        {
            // STEP 1: Pause + fade BG in + center photo (0,0). Wait for E.
            case PhotoState.FadeInAndCenter:
                {
                    // Fade 0 -> 0.5
                    _darkColor = darkenedBG.color;
                    _darkColor.a = Mathf.Lerp(_darkColor.a, 0.5f, T(fadeSpeed));
                    darkenedBG.color = _darkColor;

                    // Move photo to center
                    var p = pRT.anchoredPosition;
                    p = Vector2.Lerp(p, Vector2.zero, T(photoLerpSpeed));
                    pRT.anchoredPosition = p;

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        photoState = PhotoState.MoveToSlotAndBringFrame;
                    }
                    break;
                }

            // STEP 2: Photo → its slot, Frame → (0,0). Wait for E.
            case PhotoState.MoveToSlotAndBringFrame:
                {
                    // Move photo to its slot
                    pRT.anchoredPosition = Vector2.Lerp(pRT.anchoredPosition, targetSlot, T(photoLerpSpeed));

                    // Bring frame to center
                    fRT.anchoredPosition = Vector2.Lerp(fRT.anchoredPosition, Vector2.zero, T(frameLerpSpeed));

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        // Snap both before reparent/move
                        pRT.anchoredPosition = targetSlot;
                        fRT.anchoredPosition = Vector2.zero;

                        // Make the photo a child of the frame so it travels with it
                        pRT.SetParent(fRT, true); // keep world pos

                        photoState = PhotoState.SendOffAndFadeOut;
                    }
                    break;
                }

            // STEP 3: Frame → (1920,0) while BG fades out 0.5 → 0. Then unpause and restore controls.
            case PhotoState.SendOffAndFadeOut:
                {
                    // Slide frame to the right
                    fRT.anchoredPosition = Vector2.Lerp(fRT.anchoredPosition, frameOffRight, T(frameLerpSpeed));

                    // Fade BG out
                    _darkColor = darkenedBG.color;
                    _darkColor.a = Mathf.Lerp(_darkColor.a, 0f, T(fadeSpeed));
                    darkenedBG.color = _darkColor;

                    bool frameDone = (Vector2.Distance(fRT.anchoredPosition, frameOffRight) < 1f);
                    bool fadeDone = (_darkColor.a <= 0.01f);

                    if (frameDone && fadeDone)
                    {
                        // Clean up
                        darkenedBG.raycastTarget = false;

                        // Resume play + allow pause toggle again
                        gameIsPaused = false;
                        Time.timeScale = 1f;

                        pauseScreenAccessable = true;

                        photoState = PhotoState.None;
                        // (activePhoto remains a child of the frame as requested)
                    }
                    break;
                }
        }
    }

    private void ApplyControlsImage()
    {
        SoundManager.Instance.Play(SoundManager.SoundType.left_click);
        if (controlsImage == null || controlSprites == null || controlSprites.Length == 0) return;
        controlIndex = Mathf.Clamp(controlIndex, 0, controlSprites.Length - 1);
        controlsImage.sprite = controlSprites[controlIndex];
    }
    
    // Open the Controls modal from the Pause menu
    public void PauseMenuOpenControlModal()
    {
        if (PanelMenuImage != null) PanelMenuImage.SetActive(false);
        if (controlsScreen != null) controlsScreen.SetActive(true);

        controlIndex = 0;           // start at first page
        ApplyControlsImage();
    }

    // BACK button / action
    public void PauseControlModalBack()
    {
        if (controlIndex == 0)
        {
            // Close controls, return to pause menu
            if (PanelMenuImage != null) PanelMenuImage.SetActive(true);
            if (controlsScreen != null) controlsScreen.SetActive(false);
            controlIndex = 0; // reset for next open
            return;
        }

        controlIndex = Mathf.Max(0, controlIndex - 1);
        ApplyControlsImage();
    }

    // NEXT button / action
    public void PauseControlModalNext()
    {
        int last = Mathf.Max(0, (controlSprites?.Length ?? 3) - 1); // default to 2 if null
        if (controlIndex >= last) // index == 2 for a 3-item array
        {
            // Close controls, return to pause menu
            if (PanelMenuImage != null) PanelMenuImage.SetActive(true);
            if (controlsScreen != null) controlsScreen.SetActive(false);
            controlIndex = 0; // reset for next open
            return;
        }

        controlIndex = Mathf.Min(last, controlIndex + 1);
        ApplyControlsImage();
    }

    
    public void BeginRoomPhoto(Interactable.RoomColors color)
    {
        // Pick the correct image by enum
        activePhoto = GetPhotoForColor(color);
        if (activePhoto == null) { Debug.LogWarning("No photo bound for color: " + color); return; }

        // Ensure visible
        activePhoto.gameObject.SetActive(true);

        // Compute target slot for this enum
        targetSlot = GetTargetForColor(color);

        // Freeze gameplay & block pause toggling
        gameIsPaused = true;
        Time.timeScale = 0f;

        pauseScreenAccessable = false;

        // Prep dark bg (start transparent)
        if (darkenedBG != null)
        {
            _darkColor = darkenedBG.color;
            _darkColor.a = 0f;
            darkenedBG.color = _darkColor;
            darkenedBG.gameObject.SetActive(true);
            darkenedBG.raycastTarget = true; // block clicks if you want
        }

        // Optional: start frame anywhere; it will lerp to (0,0) in step 2
        // backgroundImageFrame.anchoredPosition = new Vector2(-1920f, 0f);

        photoState = PhotoState.FadeInAndCenter;

        if (color == Interactable.RoomColors.Mona)
        {
            escapeRope.SetActive(true);
            vCamTarget = escapeRope.transform;
        }
    }

    private Image GetPhotoForColor(Interactable.RoomColors c)
    {
        switch (c)
        {
            case Interactable.RoomColors.Red: return redPhoto;
            case Interactable.RoomColors.Blue: return bluePhoto;
            case Interactable.RoomColors.Teal: return tealPhoto;
            case Interactable.RoomColors.Green: return greenPhoto;
            case Interactable.RoomColors.Yellow: return yellowPhoto;
            case Interactable.RoomColors.Mona: return monaPhoto;
        }
        return null;
    }

    // Slots (anchoredPosition) in enum order:
    // -647  274 |   0  274 |  647  274
    // -647 -274 |   0 -274 |  647 -274
    private Vector2 GetTargetForColor(Interactable.RoomColors c)
    {
        switch (c)
        {
            case Interactable.RoomColors.Red: return new Vector2(-647f, 274f);
            case Interactable.RoomColors.Blue: return new Vector2(0f, 274f);
            case Interactable.RoomColors.Teal: return new Vector2(647f, 274f);
            case Interactable.RoomColors.Green: return new Vector2(-647f, -274f);
            case Interactable.RoomColors.Yellow: return new Vector2(0f, -274f);
            case Interactable.RoomColors.Mona: return new Vector2(647f, -274f);
        }
        return Vector2.zero;
    }


    public void SetGamePaused(bool isPaused)
    {
        gameIsPaused = isPaused;

        if (gameIsPaused)
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1;
        }
    }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator ProgressDelay()
    {
        print(Time.time);
        yield return new WaitForSecondsRealtime(5f);
        print(Time.time);
    }

    // very scuffed state-based setter
    private void HandleManagerStates(string sceneName)
    {
        switch (sceneName)
        {
            case "StartMenu":
                pauseScreenAccessable = false;
                SetGamePaused(false);

                pauseScreen.SetActive(false);
                playerHUD.SetActive(false);
                cameraScreen.SetActive(false);
                
                playerStates = null;

                break;
            case "Museum":
                pauseScreenAccessable = true;
                SetGamePaused(false);

                pauseScreen.SetActive(false);
                playerHUD.SetActive(true);
                cameraScreen.SetActive(true);
                AcquirePlayerStates();

                break;
        }
    }

    private async Task CloseSceneToLoad()
    {
        float duration = 0.625f;   // seconds
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 2.5f;
        Vector3 endScale = Vector3.zero;

        loadingMask.transform.localScale = startScale;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // unaffected by pause
            float t = Mathf.Clamp01(elapsed / duration);
            loadingMask.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            await Awaitable.NextFrameAsync();
        }

        loadingMask.transform.localScale = endScale;
    }

    private async Task OpenSceneFromLoad()
    {
        float duration = 0.625f;   // seconds
        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * 2.5f;

        loadingMask.transform.localScale = startScale;

        while (elapsed < duration)
        {
            Debug.Log(Time.unscaledDeltaTime);
            elapsed += Time.unscaledDeltaTime; // unaffected by pause
            float t = Mathf.Clamp01(elapsed / duration);
            loadingMask.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            await Awaitable.NextFrameAsync();
        }

        loadingMask.transform.localScale = endScale;
    }

    // wait helper: checks that frames are stable after completing a scene load before playing UI animations.
    private async Task WaitForStableFrame(float maxDelta = 0.05f, int stableFrames = 2, float timeout = 1f)
    {
        float start = Time.realtimeSinceStartup;
        int ok = 0;
        while (ok < stableFrames && (Time.realtimeSinceStartup - start) < timeout)
        {
            await Awaitable.NextFrameAsync();
            ok = (Time.unscaledDeltaTime <= maxDelta) ? ok + 1 : 0;
        }
    }

    public async void LoadScene(string sceneName)
    {
        // player cannot toggle pause, and non-time-based processes in game remain paused
        pauseScreenAccessable = false;

        AsyncOperation scene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        loadingScreen.SetActive(true);

        await CloseSceneToLoad();

        do
        {
            await Awaitable.NextFrameAsync();
        } while (scene.progress < 0.9f);

        scene.allowSceneActivation = true;

        while (!scene.isDone)
        {
            await Awaitable.NextFrameAsync();
        }

        HandleManagerStates(sceneName);

        await WaitForStableFrame();
        await OpenSceneFromLoad();

        loadingScreen.SetActive(false);
    }
    
    public void FocusCameraForSeconds(Transform target, float seconds)
    {
        StartCoroutine(FocusCameraForSeconds_Co(target, seconds));
    }

    private IEnumerator FocusCameraForSeconds_Co(Transform target, float seconds)
    {
        if (vCam == null || target == null) yield break;

        var original = vCam.Follow;
        vCam.Follow = target;
        yield return new WaitForSecondsRealtime(seconds);
        vCam.Follow = original != null ? original : vCamTarget;
    }
}
