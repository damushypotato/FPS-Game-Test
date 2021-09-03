using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    public Slider slider;

    public Color pulseColour;

    private Image fillImg;
    private Coroutine pulseCoroutine;
    private Color pulseStartColour;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        fillImg = slider.fillRect.GetComponent<Image>();
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }

    public void SetMax(float value) 
    {
        slider.maxValue = value;
    }

    public void Pulse(float time)
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            fillImg.color = pulseStartColour;
        }

        pulseCoroutine = StartCoroutine(PulseBar(pulseColour, time));
    }

    private IEnumerator PulseBar(Color colour, float time)
    {
        pulseStartColour = fillImg.color;

        float elapsed = 0f;
        float duration = time / 2;

        while (elapsed < duration)
        {
            fillImg.color = Color.Lerp(pulseStartColour, colour, Mathf.Clamp01(elapsed / duration));

            elapsed += Time.deltaTime;
            yield return null;
        }
        fillImg.color = colour;

        elapsed = 0f;

        while (elapsed < duration)
        {
            fillImg.color = Color.Lerp(colour, pulseStartColour, Mathf.Clamp01(elapsed / duration));

            elapsed += Time.deltaTime;
            yield return null;
        }
        fillImg.color = pulseStartColour;
        pulseCoroutine = null;
    }
}
