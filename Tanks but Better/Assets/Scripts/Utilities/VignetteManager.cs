using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VignetteManager : MonoBehaviour
{
    [SerializeField] private Image vignette;
    [SerializeField] private Image constantVignette;
    public float flashDuration = 0.2f;
    public float fadeDuration = 1.0f;

    void Start()
    {
        SetAlpha(vignette, 0f);
        SetAlpha(constantVignette, 0f);
    }

    private void SetAlpha(Image vig, float alpha)
    {
        Color color = vig.color;
        color.a = alpha;
        vig.color = color;
    }

    public void ActivateConstantVignette(float alpha)
    {
        SetAlpha(constantVignette, alpha);
    }

    public void FlashVignette()
    {
        StopAllCoroutines();
        StartCoroutine(FlashAndFade());
    }

    private IEnumerator FlashAndFade()
    {
        SetAlpha(vignette, 0.5f);
        yield return new WaitForSeconds(flashDuration);

        float elapsedTime = 0f;
        while(elapsedTime < fadeDuration){
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0.5f, 0f, elapsedTime / fadeDuration);
            SetAlpha(vignette, alpha);
            yield return null;
        }

        SetAlpha(vignette, 0f);
    }
}
