using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BiscuitHudController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform biscuitHUD;     // the whole window
    [SerializeField] private TMP_Text biscuitTMP;          // counter text
    [SerializeField] private Image titleImage;             // title graphic
    [SerializeField] private Image raccoonImage;           // raccoon graphic

    [Header("Sprites (size = 5)")]
    [SerializeField] private Sprite[] titleSprites = new Sprite[5];    // 0..4 titles
    [SerializeField] private Sprite[] raccoonSprites = new Sprite[5];  // 0..4 raccoons

    [Header("Slide Animation")]
    [SerializeField] private float slideInY  = 0f;         // “bottom at 0”
    [SerializeField] private float slideOutY = -600f;      // “bottom at -600”
    [SerializeField] private float slideLerpSpeed = 12f;   // higher = snappier
    [SerializeField] private float showDuration = 5f;      // hold at 0 for 5s

    [Header("Grow-Size Flicker")]
    [SerializeField] private float growSwapHz = 6f;        // how fast i ⇄ (i-1) swaps
    [SerializeField] private float growSwapDuration = 3f;  // first 3 seconds swap

    // State
    private bool showPopup = false;
    private float popupTimer = 0f;         // unscaled time
    private bool growSize = false;
    private int biscuitCounter = 0;
    private int stageIndex = 0;            // i (0..4)
    private int prevStageIndex = 0;        // i-1 clamped

    // Optional: stage names if you want to show text somewhere
    private static readonly string[] STAGE_NAMES = 
        { "peckish", "snacky", "stuffed", "bloated", "food coma" };

    // Maps 0..3→0, 4..7→1, 8..11→2, 12..15→3, 16+→4
    private int GetStageIndex(int count)
    {
        return Mathf.Clamp(count / 4, 0, 4);
    }

    // Call this to trigger the popup with a new biscuit count.
    public void BiscuitCounterPopUp(int newCount)
    {
        biscuitCounter = newCount;

        // 1) Counter text
        if (biscuitTMP != null)
            biscuitTMP.text = biscuitCounter.ToString();

        // 2) Stage mapping
        stageIndex = GetStageIndex(biscuitCounter);
        prevStageIndex = Mathf.Max(0, stageIndex - 1);

        // 3) Title image (range-based)
        if (titleImage != null && titleSprites != null && titleSprites.Length >= 5)
            titleImage.sprite = titleSprites[stageIndex];

        // 4) Grow-size trigger on exact thresholds except at 0
        //    Only for {4,8,12,16}
        growSize = (biscuitCounter == 4 || biscuitCounter == 8 ||
                    biscuitCounter == 12 || biscuitCounter == 16);

        // 5) Raccoon initial sprite = current stage
        if (raccoonImage != null && raccoonSprites != null && raccoonSprites.Length >= 5)
            raccoonImage.sprite = raccoonSprites[stageIndex];

        // 6) Begin popup animation
        showPopup = true;
        popupTimer = 0f;
    }

    void Update()
    {
        // If popup isn’t active, make sure we’re hidden and bail
        if (!showPopup)
        {
            if (biscuitHUD != null)
            {
                var pos = biscuitHUD.anchoredPosition;
                pos.y = Mathf.Lerp(pos.y, slideOutY, 1f - Mathf.Exp(-slideLerpSpeed * Time.unscaledDeltaTime));
                biscuitHUD.anchoredPosition = pos;
            }
            return;
        }

        // Advance unscaled timer
        popupTimer += Time.unscaledDeltaTime;

        // 1) Slide logic: slide in (to 0), hold 5s, slide out (-600)
        float targetY = (popupTimer <= showDuration) ? slideInY : slideOutY;

        if (biscuitHUD != null)
        {
            float t = 1f - Mathf.Exp(-slideLerpSpeed * Time.unscaledDeltaTime);
            var pos = biscuitHUD.anchoredPosition;
            pos.y = Mathf.Lerp(pos.y, targetY, t);
            biscuitHUD.anchoredPosition = pos;
        }

        // Optionally end the popup after it slides out far enough
        if (popupTimer > showDuration + 2f) // small buffer for slide out
        {
            showPopup = false;
            growSize = false; // reset grow effect
        }

        // 2) Grow-size raccoon swap (first 3s only)
        if (growSize && raccoonImage != null && raccoonSprites != null && raccoonSprites.Length >= 5)
        {
            if (popupTimer <= growSwapDuration)
            {
                bool useCurrent = (Mathf.FloorToInt(Time.unscaledTime * growSwapHz) % 2) == 0;
                raccoonImage.sprite = raccoonSprites[useCurrent ? stageIndex : prevStageIndex];
            }
            else
            {
                // Final 2 seconds: hold at i
                raccoonImage.sprite = raccoonSprites[stageIndex];
            }
        }
    }

    // --- If you truly need to move "bottom" offset instead of anchoredPosition,
    // --- you can use this helper (keeps height constant by shifting both offsets).
    /*
    private void ShiftBottom(RectTransform rt, float targetBottom, float lerpT)
    {
        var offMin = rt.offsetMin; // x = left, y = bottom
        var offMax = rt.offsetMax; // x = right, y = top

        float delta = Mathf.Lerp(offMin.y, targetBottom, lerpT) - offMin.y;
        offMin.y += delta;
        offMax.y += delta;

        rt.offsetMin = offMin;
        rt.offsetMax = offMax;
    }
    */
}
