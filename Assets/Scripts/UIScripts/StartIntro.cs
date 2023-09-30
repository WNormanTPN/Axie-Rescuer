using AxieRescuer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartIntro : MonoBehaviour
{
    public Image IntroText;
    public Image IntroBackground;
    public float WaitTime;
    public float FadeRate;
    void Awake()
    {
        StartCoroutine(FadeIn());
        
    }
    IEnumerator FadeIn()
    {
        var targetAlpha = 1.0f;
        Color curColor = IntroText.color;
        while (Mathf.Abs(curColor.a - targetAlpha) > 0.0001f)
        {
            curColor.a = Mathf.Lerp(curColor.a, targetAlpha, FadeRate * Time.deltaTime);
            IntroText.color = curColor;
            yield return null;
        }
    }
    IEnumerator FadeOut()
    {
        var targetAlpha = 0;
        Color curColor = IntroText.color;
        while (curColor.a > targetAlpha)
        {
            curColor.a = Mathf.Lerp(targetAlpha, curColor.a, FadeRate * Time.deltaTime);
            IntroText.color = curColor;
            yield return null;
        }
    }
}
