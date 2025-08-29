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

    private async void Start()
    {
        // Ensure these Images use Image.type = Filled in the Inspector.
        CardA.fillAmount = 0f;
        CardB.fillAmount = 0f;
        CardC.fillAmount = 0f;

        nextCard = CardA;

        // Wait 0.5 seconds before filling the first card
        await Task.Delay(500);
        StartCoroutine(FillCardThenAdvance());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (nextCard == null)
            {
                SceneManager.Instance.LoadScene("Museum");
                return;
            }

            StartCoroutine(FillCardThenAdvance());
        }
    }

    private IEnumerator FillCardThenAdvance()
    {
        float t = 0f;
        float start = 0f;
        float end = 1f;

        nextCard.fillAmount = 0f;

        while (t < 0.5f)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / 0.5f);
            nextCard.fillAmount = Mathf.Lerp(start, end, u);
            yield return null;
        }

        nextCard.fillAmount = 1f;

        // Advance the pointer
        if (nextCard == CardA)
            nextCard = CardB;
        else if (nextCard == CardB)
            nextCard = CardC;
        else if (nextCard == CardC)
            nextCard = null;
    }
}