using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VignetteManager : MonoBehaviour
{
    private Image vignette;
    public float flashDuration = 0.2f;
    public float fadeDuration = 1.0f;

    void Start()
    {
        vignette = GetComponent<Image>();
        SetAlpha(0);
    }

    private void SetAlpha(float alpha)
    {
        Color color = vignette.color;
        color.a = alpha;
        vignette.color = color;
    }

    public void FlashVignette()
    {
        StopAllCoroutines();
        StartCoroutine(FlashAndFade());
    }

    private IEnumerator FlashAndFade()
    {
        SetAlpha(0.5f);
        yield return new WaitForSeconds(flashDuration);

        float elapsedTime = 0f;
        while(elapsedTime < fadeDuration){
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0.5f, 0f, elapsedTime / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
    }
}
