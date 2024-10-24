using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    Canvas canvas;
    Image fadeImage;
    public bool isFading = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = true;


        if (canvas == null)
        {
            Debug.LogError("UIManager: Canvas component not found!");
            return;
        }

        // Assuming you have an Image component as a child of the canvas for fading
        fadeImage = canvas.GetComponentInChildren<Image>();
        if (fadeImage == null)
        {
            Debug.LogError("ScreenEffectManager: Image component not found in children!");
            return;
        }

        // Initialize the fade image color
        StartCoroutine(Initialize());
        //fadeImage.color = Color.clear;
    }

    public IEnumerator Initialize()
    {
        Unfade(2f);
        yield return new WaitForSeconds(2f);

        DisableCanvas();
    }

    public void DisableCanvas()
    {
        canvas.enabled = false;
    }

    public void EnableCanvas()
    {
        canvas.enabled = true;
    }

    public void FadeToColor(Color targetColor, float fadeDuration)
    {
        if (!isFading)
        {
            StartCoroutine(FadeTo(targetColor, fadeDuration));
        }
        else
        {
            Debug.LogWarning("Already fading. Wait until the current fade completes.");
        }
    }

    public void FadeToBlack(float fadeDuration)
    {
        FadeToColor(Color.black, fadeDuration);
    }

    public void FadeToWhite(float fadeDuration)
    {
        FadeToColor(Color.white, fadeDuration);
    }

    public void Unfade(float fadeDuration)
    {
        if (!isFading)
        {
            StartCoroutine(FadeTo(Color.clear, fadeDuration));
        }
        else
        {
            Debug.LogWarning("Already fading. Wait until the current fade completes.");
        }
    }

    IEnumerator FadeTo(Color targetColor, float fadeDuration)
    {
        isFading = true;
        canvas.enabled = true;
        float timer = 0;
        Color currentColor = fadeImage.color;
        while (timer < fadeDuration)
        {
            fadeImage.color = Color.Lerp(currentColor, targetColor, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = targetColor;
        isFading = false;
    }

    public void Flash(Material spriteRenderer, Color flashColor){
        StartCoroutine(FlashCoroutine(spriteRenderer, flashColor));
    }

    public IEnumerator FlashCoroutine(Material spriteRenderer, Color flashColor)
    {
        Color originalColor = spriteRenderer.color;
        float flashDuration = 0.2f;

        if (spriteRenderer == null || flashColor == null)
        {
            Debug.Log("Something null");
            yield break;
        }

        float elapsedTime = 0;

        while (elapsedTime < flashDuration)
        {
            Debug.Log("Something working");
            elapsedTime += Time.deltaTime;
            float lerpAmount = Mathf.PingPong(elapsedTime * 2, 1); // Multiply by 2 to flash back and forth
            spriteRenderer.color = Color.Lerp(originalColor, flashColor, lerpAmount);
            yield return null;
        }

        // Ensure the final color is set back to default
        spriteRenderer.color = originalColor;
    }
}
