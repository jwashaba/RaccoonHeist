using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class IntroCardScene : MonoBehaviour
{
    public Image CardA;
    public Image CardB;
    public Image CardC;

    private Image nextCard;
    private bool isAnimating = false;

    private async void Start()
    {
        // Ensure these Images use Image.type = Filled in the Inspector.
        if (CardA) CardA.fillAmount = 0f;
        if (CardB) CardB.fillAmount = 0f;
        if (CardC) CardC.fillAmount = 0f;

        nextCard = CardA;

        // Small intro delay
        await Task.Delay(500);
        TryStartFill();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            if (nextCard == null)
            {
                SceneManager.Instance.LoadScene("Museum");
                return;
            }

            TryStartFill();
        }
    }

    private void TryStartFill()
    {
        if (isAnimating) return;
        if (nextCard == null) return;
        StartCoroutine(FillCardThenAdvance());
    }

    private IEnumerator FillCardThenAdvance()
    {
        isAnimating = true;

        // Take a snapshot so this coroutine isn't affected by changes to nextCard.
        Image card = nextCard;
        if (card == null)
        {
            isAnimating = false;
            yield break;
        }

        // Safety: if someone forgot to set type to Filled, we avoid NREs and do nothing.
        if (card.type != Image.Type.Filled)
        {
            Debug.LogWarning($"{card.name} Image.type should be 'Filled' for fillAmount animation.");
        }

        float t = 0f;
        const float dur = 0.5f;

        card.fillAmount = 0f;

        while (t < dur)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / dur);
            // Use the snapshot 'card' — NOT 'nextCard'
            card.fillAmount = Mathf.Lerp(0f, 1f, u);
            yield return null;
        }

        card.fillAmount = 1f;

        // Advance pointer AFTER animation completes
        if (nextCard == card) // only advance if we’re still the active one
        {
            if (nextCard == CardA)      nextCard = CardB;
            else if (nextCard == CardB) nextCard = CardC;
            else if (nextCard == CardC) nextCard = null;
        }

        isAnimating = false;
    }
}
