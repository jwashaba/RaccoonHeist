using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject playerHUD;
    
    [SerializeField] private GameObject loadingMask;

    public bool pauseScreenAccessable = false;
    public bool gameIsPaused = false;

    // HUD refs
    [SerializeField] private RectTransform detectionIndicator;
    [SerializeField] private Image         detectionProgress;
    [SerializeField] private RectTransform hiddenIndicator;

    // Player state ref (exists only in Museum)
    public PlayerStates playerStates;

    // UI animation speed (higher = snappier)
    [SerializeField] private float hudLerpSpeed = 12f;
    
    void Update()
    {
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
    
    void SetGamePaused(bool isPaused)
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
                playerStates = null;

                break;
            case "Museum":
                pauseScreenAccessable = true;
                SetGamePaused(false);
                
                pauseScreen.SetActive(false);
                playerHUD.SetActive(true);
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
}
